using System.Collections.Generic;

namespace DeltaParse
{
    public class SimpleResult
    {
        public Dictionary<string, List<string>> ParsedValues { get; set; }
        public double DifferenceNormalized { get; set; }
    }
}
