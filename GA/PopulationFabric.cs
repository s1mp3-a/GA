using System;

namespace GA
{
    public static class PopulationFabric
    {
        public static class Settings
        {
            public const int RESOLUTION = 9;
        }

        public static Population Create(int count, double[] bounds)
        {
            var individuals = new Individual[count];

            var sqrCnt = Math.Sqrt(count);
            bool isInt = sqrCnt % 1 == 0;
            int cnt = (int)Math.Floor(sqrCnt);

            for (int i = 0; i < cnt; i++)
            {
                for(int j = 0; j < cnt; j++)
                {
                    var x = Map(i, 0, cnt, bounds[0], bounds[1]);
                    var y = Map(j, 0, cnt, bounds[0], bounds[1]);
                    individuals[i * cnt + j] = new Individual(x, y);
                }
            }

            if (!isInt)
            {
                var rnd = new Random();
                var diff = bounds[1] - bounds[0];
                for(int i = cnt*cnt; i < count; i++)
                {
                    var y = bounds[0] + rnd.NextDouble() * diff;
                    var x = bounds[0] + rnd.NextDouble() * diff;
                    individuals[i] = new Individual(x,y);
                }
            }

            var p = new Population(individuals);
            return p;
        }

        private static double Map(double value, double fromLow, double fromHigh, double toLow, double toHigh)
        {
            return toLow + (value - fromLow) * (toHigh - toLow) / (fromHigh - fromLow);
        }
    }
}