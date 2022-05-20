using System;
using System.Linq;

namespace GA
{
    public static class Program
    {
        static void Main(string[] args)
        {
            GeneticAlgorithm algo;
            algo = new GeneticAlgorithm(Function, 0.2, 0.01);
            algo.Run(100);
        }

        static double Function(Floating[] fs)
        {
            var xs = fs.Select(f => f.Value).ToArray();

            return Math.Cos(xs[0] * xs[1]);
        }
    }
}
