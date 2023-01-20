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

            var rnd = new Random();

            for(int k = 0; k < count; k++)
            {
                var values = new double[bounds.Length];

                for(int i = 0; i < bounds.Length; i++)
                {
                    values[i] = rnd.NextDouble() * 3;
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