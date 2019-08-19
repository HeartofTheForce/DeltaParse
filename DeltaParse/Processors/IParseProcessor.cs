namespace DeltaParse.Processors
{
    public interface IParseProcessor
    {
        string ProcessedToken { get; }
        string Preprocess(string input, string templateToken);
        string Postprocess(string input, string templateToken);
    }
}
