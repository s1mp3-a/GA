using GA;
using System;
using System.Linq;
using System.Text;

public class Individual : ICloneable
{
    public Floating[] Values { get; set; }

    public Individual(params double[] values)
    {
        Values = values.Select(v => BinaryConverter.DecimalToBinaryFloating(v)).ToArray();
    }

    /// <summary>
    /// Crossing of two individuals
    /// </summary>
    /// <param name="i2">Another individual</param>
    /// <returns>New child individual</returns>
    public Individual CrossOver(Individual i2, Random random)
    {
        var methods = new Func<Individual, Random, Individual>[]
        {
            CrossOverComponent,
            CrossOverFullString,
            CrossOverSteps
        };

        var index = random.Next(0, 3);

        return methods[index](i2, random);
    }

    /// <summary>
    /// Crossing component by component
    /// </summary>
    /// <param name="i2">Another individual</param>
    /// <returns>New child individual</returns>
    public Individual CrossOverComponent(Individual i2, Random random)
    {
        StringBuilder sb = new StringBuilder(Values[0].BitRep.Length * Values.Length);

        string[] bitValues = new string[Values.Length];

        for(int i = 0; i < Values.Length; i++)
        {
            var slice = random.Next(1, Values[i].BitRep.Length);
            sb.Append(Values[i].BitRep.Substring(0, slice));
            sb.Append(i2.Values[i].BitRep.Substring(slice));
            bitValues[i] = sb.ToString();
            sb.Clear();
        }

        return new Individual(bitValues.Select(x => BinaryConverter.RawBinaryToDouble(x)).ToArray());
    }

    /// <summary>
    /// Crossing bitrep by bitrep
    /// </summary>
    /// <param name="i2">Another individual</param>
    /// <returns>New child individual</returns>
    public Individual CrossOverFullString(Individual i2, Random random)
    {
        StringBuilder sb = new StringBuilder(Values[0].BitRep.Length * Values.Length);

        string[] bitValues = new string[Values.Length];

        foreach(var value in Values)
        {
            sb.Append(value.BitRep);
        }

        bitValues[0] = sb.ToString();
        sb.Clear();

        foreach (var value in i2.Values)
        {
            sb.Append(value.BitRep);
        }

        bitValues[1] = sb.ToString();
        
        var slice = random.Next(1, Values[0].BitRep.Length * 2);

        sb.Clear();
        sb.Append(bitValues[0].Substring(0, slice));
        sb.Append(bitValues[1].Substring(slice));

        return new Individual(bitValues.Select(x => BinaryConverter.RawBinaryToDouble(x)).ToArray());
    }

    /// <summary>
    /// Diagonal steps crossing
    /// </summary>
    /// <param name="i2">Another individual</param>
    /// <returns>New child individual</returns>
    public Individual CrossOverSteps(Individual i2, Random random)
    {
        StringBuilder sb = new StringBuilder(Values[0].BitRep.Length * Values.Length);

        string[] bitValues = new string[Values.Length];

        foreach (var value in Values)
        {
            sb.Append(value.BitRep);
        }

        bitValues[0] = sb.ToString();
        sb.Clear();

        foreach (var value in i2.Values)
        {
            sb.Append(value.BitRep);
        }

        bitValues[1] = sb.ToString();
        sb.Clear();

        for(int i = 0; i < bitValues[0].Length; i++)
        {
            sb.Append(bitValues[i % 2][i]);
        }

        var resBits = sb.ToString();

        return new Individual(new double[]
        {
            BinaryConverter.RawBinaryToDouble(resBits.Substring(0,resBits.Length / 2)),
            BinaryConverter.RawBinaryToDouble(resBits.Substring(resBits.Length / 2))
        });

    }

    public void Mutate(double prob, Random random)
    {
        StringBuilder sb = new StringBuilder();
        foreach (var value in Values)
        {
            sb.Append(value.BitRep);
        }

        var bitRep = sb.ToString().ToCharArray();
        var bitSpan = new Span<char>(bitRep);

        for (int i = 0; i < bitSpan.Length; i++)
        {
            if (random.NextDouble() < prob)
            {
                if (bitSpan[i] == '1')
                    bitSpan[i] = '0';
                else
                    bitSpan[i] = '1';
            }
        }

        var charCount = Values[0].BitRep.Length;
        int start = 0;
        var newValues = new double[this.Values.Length];
        for (int i = 0; i < this.Values.Length; i++)
        {
            var modifiedSpan = bitSpan.Slice(start, charCount);
            var bitString = new string(modifiedSpan);
            var doubleValue = BinaryConverter.RawBinaryToDouble(bitString);
            newValues[i] = doubleValue;
            start += charCount;
        }

        this.Values = new Individual(newValues).Values;
    }

    public double Score(Func<Floating[], double> fit)
    {
        return fit(Values);
    }

    public object Clone()
    {
        return this.MemberwiseClone();
    }
}