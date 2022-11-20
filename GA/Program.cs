using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GA
{
    public static class Program
    {
        private static InputData _inputData;
        private static Dictionary<int, string> _tags = new Dictionary<int, string>() // Набор тегов для значений функции
        {
            { 0, "p0" },
            { 1, "l" },
            { 2, "b" },
            { 3, "x0" },
            { 4, "y0" },
            { 5, "m" },
            { 6, "n" },
        };

        /// <summary>
        /// Точка входа программы
        /// </summary>
        /// <param name="args">Массив, где первый элемент - путь к файлу с входными данными</param>
        static void Main(string[] args)
        {
            FloatingSettings.IntPartSize = 3;
            FloatingSettings.FracPartSize = 10;
            FloatingSettings.IsUnsigned = true;

            if (args.Length == 0)
                return;

            //var filePath = @"H:\Projects\VS\GA\GA\input.json"; // для отладки

            var inputJson = File.ReadAllText(args[0]); // считывание входных данных
            _inputData = JsonConvert.DeserializeObject<InputData>(inputJson); // десериализация входных данных

            GeneticAlgorithm algo;

            algo = new GeneticAlgorithm(Function, 0.2, 0.01);
            algo.Run(100);

            var output = new OutputData
            {
                FunctionValue = algo.BestFit,
                Values = algo.Best.Values.Select((x, i) => new ValueWithTag
                {
                    Value = x.Value,
                    Tag = _tags[i]
                })
            };

            // сериализация и запись выходных данных
            using (var sw = new StreamWriter(@"output.json"))
            {
                sw.Write(JsonConvert.SerializeObject(output, Formatting.Indented));
            }

            Console.WriteLine("Готово! Результат находится в output.json");
            Console.ReadLine();
        }

        static double Function(Floating[] fs)
        {
            var xs = fs.Select(f => f.Value).ToArray();

            var p0 = xs[0];
            var l = xs[1];
            var b = xs[2];
            var x0 = xs[3];
            var y0 = xs[4];
            var m = xs[5];
            var n = xs[6];

            return Enumerable.Range(0, _inputData.E.Length)
                .Select(i =>
                {
                    var e = _inputData.E[i];
                    var a = Math.Pow(1 - Math.Pow(Math.Abs(2 * (e.X - x0) / l), n), 1 / n) * Math.Pow(1 - Math.Pow(Math.Abs(2 * (e.Y - y0) / b), m), 1 / m);
                    a = double.IsNaN(a) ? 0 : a;
                    var res = Math.Pow(_inputData.Phi[i][e.Y][e.X] * p0 * a - e.Value, 2);
                    return res;
                })
                .Sum();

        }
    }
}
