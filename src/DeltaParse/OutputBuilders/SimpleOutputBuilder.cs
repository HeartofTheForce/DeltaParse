using System.Collections.Generic;
using DiffMatchPatch;

namespace DeltaParse.OutputBuilders
{
    public class SimpleOutputBuilder : IOutputBuilder<SimpleResult>
    {
        public SimpleResult InitializeOutput(IEnumerable<Diff> diffs)
        {
            return new SimpleResult()
            {
                ParsedValues = new Dictionary<string, List<string>>(),
            };
        }

        public SimpleResult TokenParsed(SimpleResult currentOutput, string tokenName, string parsedValue)
        {
            if (!string.IsNullOrWhiteSpace(parsedValue))
            {
                Utilities.AddToDictionaryCollection(currentOutput.ParsedValues, tokenName, parsedValue);
            }

            return currentOutput;
        }

        public SimpleResult FinalizeOutput(SimpleResult currentOutput, double differenceNormalized)
        {
            currentOutput.DifferenceNormalized = differenceNormalized;

            return currentOutput;
        }
    }
}
