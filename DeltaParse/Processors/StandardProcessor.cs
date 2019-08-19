using System.Text.RegularExpressions;

namespace DeltaParse.Processors
{
    public class StandardProcessor : IParseProcessor
    {
        private const string Token = "{{}}";
        public string ProcessedToken => Token;

        public string Preprocess(string input, string templateToken)
        {
            if (templateToken != null)
            {
                input = Regex.Replace(input, Regex.Escape(templateToken), m =>
                {
                    return ProcessedToken;
                });
            }

            return input;
        }

        public string Postprocess(string input, string templateToken)
        {
            input = Regex.Replace(input, Regex.Escape(ProcessedToken), m =>
            {
                return templateToken;
            });

            return input;
        }
    }
}
