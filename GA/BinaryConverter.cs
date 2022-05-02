using System;
using System.Text;

namespace GA
{
    public struct Floating
    {
        public string Sign { get; set; }
        public string Exponent { get; set; }
        public string Mantissa { get; set; }

        public string BitRep => Sign + Exponent + Mantissa;
        public double Value => BinaryConverter.RawBinaryToDouble(BitRep);
    }

    public struct Fraction
    {
        public string Sign { get; set; }
        public string IntPart { get; set; }
        public string FracPart { get; set; }
    }

    public static class BinaryConverter
    {
        public static Floating DecimalToBinaryFloating(double value)
        {
            var raw = BitConverter.DoubleToInt64Bits(value);
            var sign = raw >> 63 & 0x01;
            var exponent = raw >> 52 & 0x7FF;
            var mantissa = (raw & 0xFFFFFFFFFFFFF);

            return new Floating
            {
                Sign = Convert.ToString(sign, 2),
                Exponent = Convert.ToString(exponent, 2).PadLeft(11, '0'),
                Mantissa = Convert.ToString(mantissa, 2).PadLeft(52, '0')
            };
        }

        public static Fraction DecimalToBinaryFraction(double value, int acc)
        {
            var intPart = (int)Math.Truncate(value);
            var fracPart = value - intPart;

            intPart = Math.Abs(intPart);
            fracPart = Math.Abs(fracPart);

            int fracInt;
            StringBuilder binaryFracPart = new StringBuilder();


            for (int i = 0; i < acc; i++)
            {
                fracPart *= 2;
                fracInt = (int)fracPart;

                if (fracInt == 1)
                {
                    fracPart -= fracInt;
                    binaryFracPart.Append('1');
                }
                else
                {
                    binaryFracPart.Append('0');
                }
            }

            return new Fraction
            {
                Sign = Math.Sign(value) == -1 ? "1" : "0",
                IntPart = Convert.ToString(intPart, 2),
                FracPart = binaryFracPart.ToString()
            };
        }

        public static double BinaryToDouble(Floating value)
        {
            var raw = value.Sign + value.Exponent + value.Mantissa;

            return BitConverter.Int64BitsToDouble(Convert.ToInt64(raw, 2));
        }
        
        public static double RawBinaryToDouble(string bits)
        {
            return BitConverter.Int64BitsToDouble(Convert.ToInt64(bits, 2));
        }

        public static double BinaryToDouble(Fraction value)
        {
            double intPart = 0;
            double fracPart = 0;

            int twos = 1;

            for (int i = value.IntPart.Length - 1; i >= 0; i--)
            {
                intPart += Char.GetNumericValue(value.IntPart[i]) * twos;
                twos *= 2;
            }

            twos = 2;

            for (int i = 0; i < value.FracPart.Length; i++)
            {
                fracPart += Char.GetNumericValue(value.FracPart[i]) / twos;
                twos *= 2;
            }

            var converted = intPart + fracPart;

            converted *= value.Sign == "1" ? -1d : 1d;

            return converted;
        }
    }
}