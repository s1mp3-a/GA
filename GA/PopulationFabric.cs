using System;

namespace GA
{
    public static class PopulationFabric
    {
        public static class Settings
        {
            public const int RESOLUTION = 9;
        }

        /// <summary>
        /// Создание популяции с заданными границами
        /// </summary>
        /// <param name="count">Количество особей</param>
        /// <param name="bounds">Границы</param>
        /// <returns></returns>
        public static Population Create(int count, double[] bounds)
        {
            var individuals = new Individual[count];

            //var sqrCnt = Math.Sqrt(count);
            //bool isInt = sqrCnt % 1 == 0;
            //int cnt = (int)Math.Floor(sqrCnt);

            //for (int i = 0; i < cnt; i++)
            //{
            //    for(int j = 0; j < cnt; j++)
            //    {
            //        var x = Map(i, 0, cnt, bounds[0], bounds[1]);
            //        var y = Map(j, 0, cnt, bounds[0], bounds[1]);
            //        individuals[i * cnt + j] = new Individual(x, y);
            //    }
            //}

            //if (!isInt)
            //{
            //    var rnd = new Random();
            //    var diff = bounds[1] - bounds[0];
            //    for(int i = cnt*cnt; i < count; i++)
            //    {
            //        var y = bounds[0] + rnd.NextDouble() * diff;
            //        var x = bounds[0] + rnd.NextDouble() * diff;
            //        individuals[i] = new Individual(x,y);
            //    }
            //}

            var rnd = new Random();

            for(int k = 0; k < count; k++)
            {
                var values = new double[bounds.Length];

                for(int i = 0; i < bounds.Length; i++)
                {
                    values[i] = rnd.NextDouble() * 20;
                }
                individuals[k] = new Individual(values);
            }


            var p = new Population(individuals);
            return p;
        }

        /// <summary>
        /// Распределение
        /// </summary>
        private static double Map(double value, double fromLow, double fromHigh, double toLow, double toHigh)
        {
            return toLow + (value - fromLow) * (toHigh - toLow) / (fromHigh - fromLow);
        }
    }
}