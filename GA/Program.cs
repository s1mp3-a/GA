using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;

namespace GA
{
    public static class Program
    {
        private static InputData _inputData;

        static void Main(string[] args)
        {
            //if (args.Length == 0)
            //    return;

            var filePath = @"H:\Projects\VS\GA\GA\input.json";
            var inputJson = File.ReadAllText(filePath);
            _inputData = JsonConvert.DeserializeObject<InputData>(inputJson);

            GeneticAlgorithm algo;

            do
            {
                algo = new GeneticAlgorithm(Function, 0.2, 0.01);
                algo.Run(100);
            } while (double.IsNaN(algo.BestFit));

            using(var sw = new StreamWriter(@"output.json"))
            {
                sw.Write(JsonConvert.SerializeObject(algo.Best, Formatting.Indented));
            }
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
                    var a1 = Math.Pow(1 - Math.Pow(Math.Abs(2 * (e.X - x0) / l), n), 1 / n);
                    var a2 = Math.Pow(1 - Math.Pow(Math.Abs(2 * (e.Y - y0) / b), m), 1 / m);
                    var res = Math.Pow(_inputData.Phi[i][e.Y][e.X] * p0 * a1 * a2 - e.Value, 2);
                    return res;
                })
                .Sum();

        }
    }
}
