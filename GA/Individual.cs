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

    public Individual CrossOver(Individual i2, Random random)
    {
        StringBuilder sb = new StringBuilder(Values[0].BitRep.Length * Values.Length);
        
        foreach (var value in Values)
            sb.Append(value.BitRep);
        var parent1 = sb.ToString().ToCharArray();
        sb.Clear();
        
        foreach (var value in i2.Values)
            sb.Append(value.BitRep);
        var parent2 = sb.ToString().ToCharArray();

        var slice = random.Next(1, parent1.Length - 2);
        
        var part1 = parent1[..slice];
        var part2 = parent2[slice..];

        Span<char> childSpan = stackalloc char[part1.Length + part2.Length];
        var preSlice = childSpan[..slice];
        var postSlice = childSpan[slice..];
        
        part1.CopyTo(preSlice);
        part2.CopyTo(postSlice);

        var charCount = Values[0].BitRep.Length;
        int start = 0;
        var childValues = new double[this.Values.Length];
        for (int i = 0; i < i2.Values.Length; i++)
        {
            var bitString = new string(childSpan.Slice(start, charCount));
            var doubleValue = BinaryConverter.RawBinaryToDouble(bitString);
            childValues[i] = doubleValue;
            start += charCount;
        }

        return new Individual(childValues);
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
            modifiedSpan[2] = '0';
            modifiedSpan[3] = '0';
            modifiedSpan[4] = '0';
            modifiedSpan[5] = '0';
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