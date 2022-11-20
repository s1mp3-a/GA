using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GA
{
    public class OutputData
    {
        public double FunctionValue { get; set; }
        public IEnumerable<ValueWithTag> Values { get; set; }
    }

    public class ValueWithTag
    {
        public string Tag { get; set; }
        public double Value { get; set; }
    }
}
