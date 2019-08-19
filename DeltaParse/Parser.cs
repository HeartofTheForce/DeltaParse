using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using DeltaParse.Processors;
using DiffMatchPatch;

namespace DeltaParse
{
    public class Parser
    {
        private const string TemplateToken = "{{}}";

        private diff_match_patch DMP { get; }
        private IParseProcessor ParseProcessor { get; }
        private List<string> VariableNames { get; }
        private string Template { get; }

        public Parser(string template, IParseProcessor parseProcessor)
        {
            DMP = new diff_match_patch()
            {
                Diff_Timeout = 0,
            };

            ParseProcessor = parseProcessor;
            VariableNames = new List<string>();

            Template = Regex.Replace(template, "{{(\\w*?)}}", m =>
            {
                VariableNames.Add(m.Groups[1].Value);
                return TemplateToken;
            });

            Template = ParseProcessor.Preprocess(Template, TemplateToken);
        }

        public ParseResult Parse(string input)
        {
            input = ParseProcessor.Preprocess(input, null);

            var diff = DMP.diff_main(Template, input, false);

            return ParseValues(diff, VariableNames);
        }

        private ParseResult ParseValues(List<Diff> diff, List<string> variableNames)
        {
            var output = new ParseResult
            {
                ParsedValues = new Dictionary<string, List<string>>(),
                Difference = 0,
            };

            int variableIndex = -1;
            string value = null;

            int commonLength = 0;
            int diff0 = 0;
            int diff1 = 0;

            for (int i = 0; i < diff.Count; i++)
            {
                var part = diff[i];

                if (value != null)
                {
                    if (part.operation == Operation.INSERT)
                    {
                        value += part.text;
                    }

                    if ((diff.Count - 1) == i || part.operation != Operation.INSERT)
                    {
                        string parsedValue = ParseProcessor.Postprocess(value, null);
                        if (!string.IsNullOrWhiteSpace(parsedValue))
                        {
                            Utilities.AddToDictionaryCollection(output.ParsedValues, variableNames[variableIndex], parsedValue);
                        }
                    }

                    if (part.operation != Operation.INSERT)
                    {
                        value = null;
                    }
                    else
                    {
                        continue;
                    }
                }

                switch (part.operation)
                {
                    case Operation.DELETE:
                        {
                            int cnt = Utilities.SubstringCount(part.text, ParseProcessor.ProcessedToken);
                            variableIndex += cnt;
                            if (part.text.EndsWith(ParseProcessor.ProcessedToken))
                            {
                                value = "";
                            }
                            diff0 += part.text.Length - ParseProcessor.ProcessedToken.Length * cnt;
                        }
                        break;
                    case Operation.INSERT:
                        {
                            diff1 += part.text.Length;
                        }
                        break;
                    case Operation.EQUAL:
                        {
                            commonLength += part.text.Length;
                        }
                        break;
                }
            }

            output.Difference = 1 - commonLength / (double)(commonLength + Math.Max(diff0, diff1));

            return output;
        }
    }
}
