using System.Collections.Generic;
using DiffMatchPatch;

namespace DeltaParse.OutputBuilders
{
    public interface IOutputBuilder<T>
    {
        T InitializeOutput(IEnumerable<Diff> diffs);
        T TokenParsed(T currentOutput, string tokenName, string parsedValue);
        T FinalizeOutput(T currentOutput, double differenceNormalized);
    }
}
