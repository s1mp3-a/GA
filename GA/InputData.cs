using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GA
{
    internal class InputData
    {
        [JsonProperty("e", NullValueHandling = NullValueHandling.Ignore)]
        public E[] E { get; set; }

        [JsonProperty("phi", NullValueHandling = NullValueHandling.Ignore)]
        public double[][][] Phi { get; set; }
    }

    public partial class E
    {
        [JsonProperty("value", NullValueHandling = NullValueHandling.Ignore)]
        public double Value { get; set; }

        [JsonProperty("x", NullValueHandling = NullValueHandling.Ignore)]
        public int X { get; set; }

        [JsonProperty("y", NullValueHandling = NullValueHandling.Ignore)]
        public int Y { get; set; }
    }
}
