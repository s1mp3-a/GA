using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GA
{
    public class Population : IEnumerable<Individual>
    {
        public Individual[] Individuals { get; private set; }
        public Individual this[int i] => Individuals[i];
        public int Size => Individuals.Length;

        public Population(Individual[] individuals)
        {
            Individuals = individuals;
        }

        /// <summary>
        /// Обновление популяции
        /// </summary>
        /// <param name="newIndividuals">Новые особи</param>
        public void UpdatePopulation(IEnumerable<Individual> newIndividuals)
        {
            Individuals = newIndividuals.ToArray();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Individuals.GetEnumerator();
        }

        IEnumerator<Individual> IEnumerable<Individual>.GetEnumerator()
        {
            for (int i = 0; i <= Individuals.Length; i++)
            {
                if(i == Individuals.Length)
                {
                    yield break;
                }
                else
                {
                    yield return Individuals[i];
                }
            }
        }
    }
}
