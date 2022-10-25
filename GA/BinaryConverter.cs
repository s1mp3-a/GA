using System;
using System.Linq;
using System.Text;

namespace GA
{
    public struct Floating
    {
        public string Sign { get; set; }
        public string ExpSign { get; set; }
        public string Exponent { get; set; }
        public string Mantissa { get; set; }

        public string BitRep => Sign + ExpSign + Mantissa + Exponent;
        public double Value 
        { 
            get 
            {
                double mantissaDec = Convert.ToInt32(Mantissa, 2);
                mantissaDec /= Math.Pow(10, mantissaDec.ToString().Length - 1);
                var exponentDec = Convert.ToInt32(Exponent, 2);
                var value = mantissaDec * Math.Pow(10, ExpSign == "0" ? exponentDec : -exponentDec);

                return Sign == "0" ? value : -value ;
            } 
        }
    }


    public struct Fraction
    {
        public string Sign { get; set; }
        public string IntPart { get; set; }
        public string FracPart { get; set; }
    }

    public static class BinaryConverter
    {
        public const int MantissaSize = 16;
        public const int ExponentSize = 4;
        public const int DecFracSize = 3;

        /// <summary>
        /// Конвертирует значение типа decimal в структуру числа с плавающей точкой
        /// </summary>
        /// <param name="value">Конвертируемое значение</param>
        /// <returns>Структура числа с плавающей точкой</returns>
        public static Floating DecimalToBinaryFloating(double value)
        {
            int sign = value > 0 ? 0 : 1;
            double mantissa = value = Math.Abs(value);
            int exponent = 0;

            //Значение должно быть конечно
            if (!double.IsFinite(value))
            {
                throw new ArgumentException();
            }

            if(value == 0)
            {
                return new Floating
                {
                    Mantissa = "0".PadLeft(MantissaSize, '0'),
                    Exponent = "0".PadLeft(ExponentSize, '0'),
                    Sign = "0",
                    ExpSign = "0"
                };
            }

            //Формирование мантиссы и экспоненты
            //Ветвление влияет на знак экспоненты
            if (value >= 1)
            {
                while(true)
                {
                    value /= 10;
                    if (value < 1)
                        break;
                    mantissa = value;
                    exponent++;
                }
            }
            else
            {
                while (true)
                {
                    value *= 10;
                    if (value > 10)
                        break;
                    mantissa = value;
                    exponent--;
                }
            }

            mantissa = Math.Round(mantissa, DecFracSize);

            var mantStr = mantissa.ToString().Replace(",","");
            mantStr = Convert.ToString(Convert.ToInt32(mantStr), 2);
            mantStr = mantStr.PadLeft(MantissaSize, '0').Substring(0,MantissaSize);

            var expStr = Convert.ToString(Math.Abs(exponent), 2);
            expStr = expStr.PadLeft(ExponentSize, '0').Substring(0,ExponentSize);

            return new Floating
            {
                Sign = sign.ToString(),
                Mantissa = mantStr,
                Exponent = expStr,
                ExpSign = exponent >= 0 ? "0" : "1"
            };
        }


        /// <summary>
        /// Преобразует десятичное значение в структуру дробного числа с определенной точностью
        /// </summary>
        /// <param name="value">Конвертируемое значение</param>
        /// <param name="acc">Точность</param>
        /// <returns>Структура дробного числа</returns>
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

        /// <summary>
        /// Преобразует битовую строку в значение типа double
        /// </summary>
        /// <param name="bits">Битовая строка</param>
        /// <returns>Значение типа double</returns>
        public static double RawBinaryToDouble(string bits)
        {
            var value = new Floating
            {
                Sign = $"{bits[0]}",
                ExpSign = $"{bits[1]}",
                Mantissa = bits.Substring(2, MantissaSize),
                Exponent = bits.Substring(MantissaSize + 2, ExponentSize)
            };

            if(double.IsInfinity(value.Value))
            {
                Console.Write("");
            }

            return value.Value;
        }

        /// <summary>
        /// Преобразует структуру дробного числа в значение типа double
        /// </summary>
        /// <param name="value">Структура дробного числа</param>
        /// <returns>Значение типа double</returns>
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