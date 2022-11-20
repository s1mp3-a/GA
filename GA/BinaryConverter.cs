using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GA
{
    /// <summary>
    /// Общие настройки для структуры числа с фиксированной точкой в двоичном виде
    /// </summary>
    public static class FloatingSettings
    {
        public static int IntPartSize { get; set; }
        public static int FracPartSize { get; set; }
        public static bool IsUnsigned { get; set; }
    }

    /// <summary>
    /// Структура числа с фиксированной точкой в двоичном виде
    /// </summary>
    public struct Floating
    {
        public int IntPartSize { get; }
        public int FracPartSize { get; }
        public bool IsUnsigned { get; }
        public string Sign { get; }
        public string IntPart { get; }
        public string FracPart { get; }

        public string BitRep => Sign + IntPart + FracPart;
        public double Value
        {
            get
            {
                var decFracPart = 0d;

                for (int i = 0; i < FracPart.Length; i++)
                {
                    decFracPart += (FracPart[i] - '0') * Math.Pow(2, -1 - i);
                }

                double value = Convert.ToInt32(IntPart, 2) + decFracPart;
                return Sign == "0" ? value : -value;
            }
        }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="value">Значение для конвертации</param>
        /// <param name="intPartSize">Размер целой части</param>
        /// <param name="fracPartSize">Размер для дробной части</param>
        /// <param name="isUnsigned">Является ли число беззнаковым</param>
        public Floating(double value, int intPartSize = 0, int fracPartSize = 0, bool? isUnsigned = null)
        {
            IntPartSize = intPartSize == 0 ? FloatingSettings.IntPartSize : intPartSize;
            FracPartSize = fracPartSize == 0 ? FloatingSettings.FracPartSize : fracPartSize;
            IsUnsigned = isUnsigned ?? FloatingSettings.IsUnsigned;

            var bitRep = BinaryConverter.DoubleToBinaryRow(value, IntPartSize, FracPartSize);

            Sign = IsUnsigned ? "0" : bitRep[0].ToString();
            IntPart = bitRep.Substring(1, IntPartSize);
            FracPart = bitRep.Substring(IntPartSize + 1);

        }

        public static implicit operator Floating(double value)
        {
            return new Floating(value);
        }
    }

    /// <summary>
    /// Класс с набором методов для преобразования десятичного числа в двоичную структуру и обратно
    /// </summary>
    public static class BinaryConverter
    {
        /// <summary>
        /// Перевод десятичного числа в двоичную структуру с фиксированной точкой 
        /// </summary>
        /// <param name="value">Значение для конвертации</param>
        /// <param name="intPartSize">Размер целой части</param>
        /// <param name="fracPartSize">Размер для дробной части</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static string DoubleToBinaryRow(double value, int intPartSize = 0, int fracPartSize = 0)
        {
            if (!double.IsFinite(value))
            {
                throw new ArgumentException();
            }

            intPartSize = intPartSize == 0 ? FloatingSettings.IntPartSize : intPartSize;
            fracPartSize = fracPartSize == 0 ? FloatingSettings.FracPartSize : fracPartSize;

            var sign = value > 0 ? "0" : "1";

            var intPart = (int)Math.Truncate(value);
            var fracPart = value - intPart;

            var intPartStr = Convert.ToString(intPart, 2);
            var fracPartStr = GetFracPart(fracPart, fracPartSize);

            intPartStr = TransformPart(intPartStr , intPartSize);
            fracPartStr = TransformPart(fracPartStr, fracPartSize, true);

            string TransformPart(string part, int size, bool padRight = false)
            {
                if (part.Length > size)
                {
                    part = padRight ? part.Substring(0, size) : part.Substring(part.Length - size, size);
                }
                else
                {
                    part = padRight ? part.PadRight(size, '0') : part.PadLeft(size, '0');
                }

                return part;
            }

            string GetFracPart(double value, int size)
            {
                var sb = new StringBuilder(size);
                value = Math.Abs(value);

                for (int i = 0; i < size; i++)
                {
                    value *= 2;
                    var intPart = (int)Math.Truncate(value);
                    sb.Append(intPart.ToString());
                    value -= intPart;

                    if (value == 0)
                        break;
                }

                return sb.ToString();
            }

            return sign + intPartStr + fracPartStr;
        }

        /// <summary>
        /// Перевод битовой строки в десятичное число
        /// </summary>
        /// <param name="bitRep">Битовая строка</param>
        /// <param name="intPartSize">Размер целой части</param>
        /// <returns></returns>
        public static double BinaryRowToDouble(string bitRep, int intPartSize = 0)
        {
            var decFracPart = 0d;

            intPartSize = intPartSize == 0 ? FloatingSettings.IntPartSize : intPartSize;

            var intPart = bitRep.Substring(1, intPartSize);
            var fracPart = bitRep.Substring(intPartSize + 1);

            for (int i = 0; i < fracPart.Length; i++)
            {
                decFracPart += (fracPart[i] - '0') * Math.Pow(2, -1 - i);
            }

            double value = Convert.ToInt32(intPart, 2) + decFracPart;
            return bitRep[0] == '0' ? value : -value;
        }
    }

}
