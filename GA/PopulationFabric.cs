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
            
            // foreach (var individual in individuals)
            // {
            //     individual.Values = new Floating[2];
            // }

            for (int i = 0; i < count; i++)
            {
                for (int j = 0; j < count; j++)
                {
                    var x = Map(i, 0, count/2d, bounds[0], bounds[1]);
                    var y = Map(j, 0, count/2d, bounds[0], bounds[1]);
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