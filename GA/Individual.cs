using GA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Individual : ICloneable
{
    public Floating[] Values { get; set; }

    public Individual(params double[] values)
    {
        Values = values.Select(x => (Floating)x).ToArray();
    }

    /// <summary>
    /// Скрещивание особей со случайным выбором алгоритма
    /// </summary>
    /// <param name="i2">Другая особь</param>
    /// <returns>Новая особь</returns>
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
    /// Покомпонентное скрещивание
    /// </summary>
    /// <param name="i2">Другая особь</param>
    /// <returns>Новая особь</returns>
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

        return new Individual(bitValues.Select(x => BinaryConverter.BinaryRowToDouble(x)).ToArray());
    }

    /// <summary>
    /// Скрещивание по битовой строке
    /// </summary>
    /// <param name="i2">Другая особь</param>
    /// <returns>Новая особь</returns>
    public Individual CrossOverFullString(Individual i2, Random random)
    {
        StringBuilder sb = new StringBuilder(Values[0].BitRep.Length * Values.Length);

        string[] bitValues = new string[Values.Length];

        foreach(var value in Values)
            sb.Append(value.BitRep);

        var raw1 = sb.ToString();

        sb.Clear();

        foreach (var value in i2.Values)
            sb.Append(value.BitRep);

        var raw2 = sb.ToString();

        var sliceIdx = random.Next(1, raw1.Length);

        sb.Clear();

        var resultRaw = sb.Append(raw1.Substring(0, sliceIdx))
                          .Append(raw2.Substring(sliceIdx))
                          .ToString();

        var step = resultRaw.Length / Values.Length;

        for(int i = 0; i < Values.Length; i++)
        {
            bitValues[i] = resultRaw.Substring(i * step, step);
        }

        return new Individual(bitValues.Select(x => BinaryConverter.BinaryRowToDouble(x)).ToArray());
    }

    /// <summary>
    /// Диагональное скрещивание
    /// </summary>
    /// <param name="i2">Другая особь</param>
    /// <returns>Новая особь</returns>
    public Individual CrossOverSteps(Individual i2, Random random)
    {
        StringBuilder sb = new StringBuilder(Values[0].BitRep.Length * Values.Length);
        var resultValues = new StringBuilder[Values.Length];

        foreach (var value in Values.Select(x => x.BitRep))
            sb.Append(value);

        foreach (var value in i2.Values.Select(x => x.BitRep))
            sb.Append(value);

        var raw = sb.ToString();

        for (int i = 0; i < Values.Length; i++)
            resultValues[i] = new StringBuilder();

        for(int i = 0; i < raw.Length; i++)
        {
            resultValues[i % Values.Length].Append(raw[i]);
        }

        return new Individual(resultValues
            .Select(x => BinaryConverter.BinaryRowToDouble(x.ToString()))
            .ToArray());
    }

    /// <summary>
    /// Мутация
    /// </summary>
    /// <param name="prob">Порог для мутации</param>
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
            var doubleValue = BinaryConverter.BinaryRowToDouble(bitString);
            newValues[i] = doubleValue;
            start += charCount;
        }

        this.Values = new Individual(newValues).Values;
    }

    /// <summary>
    /// Оценка особи
    /// </summary>
    /// <param name="fit"></param>
    /// <returns></returns>
    public double Score(Func<Floating[], double> fit)
    {
        return fit(Values);
    }

    public object Clone()
    {
        return this.MemberwiseClone();
    }
}