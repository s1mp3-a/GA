using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GA
{
    public class GAConfig
    {
        public int Generations { get; set; }
        public double CrossoverProbability { get; set; }
        public double MutationProbability { get; set; }
    }
}
