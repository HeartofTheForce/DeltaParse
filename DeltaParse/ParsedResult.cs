using System.Collections.Generic;
using DiffMatchPatch;

namespace DeltaParse
{
    public class ParseResult
    {
        public Dictionary<string, List<string>> ParsedValues { get; set; }
        public double Difference { get; set; }
    }
}
