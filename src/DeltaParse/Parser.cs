using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using DeltaParse.OutputBuilders;
using DeltaParse.Processors;
using DiffMatchPatch;

namespace DeltaParse
{
    public class Parser<T>
    {
        private const string TemplateToken = "{{}}";

        private diff_match_patch DMP { get; }
        private IParseProcessor ParseProcessor { get; }
        private IOutputBuilder<T> OutputBuilder { get; }
        private List<string> TokenNames { get; }
        private string Template { get; }

        public Parser(
            string template,
            IParseProcessor parseProcessor,
            IOutputBuilder<T> outputBuilder)
        {
            DMP = new diff_match_patch()
            {
                Diff_Timeout = 0,
            };

            ParseProcessor = parseProcessor;
            OutputBuilder = outputBuilder;

            TokenNames = new List<string>();

            Template = Regex.Replace(template, "{{(\\w*?)}}", m =>
            {
                TokenNames.Add(m.Groups[1].Value);
                return TemplateToken;
            });

            Template = ParseProcessor.Preprocess(Template, TemplateToken);
        }

        public T Parse(string input)
        {
            input = ParseProcessor.Preprocess(input, null);

            var diff = DMP.diff_main(Template, input, false);

            return ParseValues(input, diff, TokenNames);
        }

        private T ParseValues(string input, List<Diff> diffs, List<string> tokenNames)
        {
            var output = OutputBuilder.InitializeOutput(diffs);

            int variableIndex = -1;

            int valueIndex = -1;
            int valueLength = 0;

            int commonLength = 0;
            int diffTemplate = 0;
            int diffInput = 0;
            int totalParsedLength = 0;

            for (int i = 0; i <= diffs.Count; i++)
            {
                bool tokenProcessed = valueIndex != -1;

                if (i < diffs.Count)
                {
                    var part = diffs[i];
                    switch (part.operation)
                    {
                        case Operation.DELETE:
                            {
                                int cnt = Utilities.SubstringCount(part.text, ParseProcessor.ProcessedToken);
                                variableIndex += cnt;
                                diffTemplate += part.text.Length - ParseProcessor.ProcessedToken.Length * cnt;

                                if (part.text.EndsWith(ParseProcessor.ProcessedToken))
                                {
                                    valueIndex = commonLength + diffInput;
                                    valueLength = 0;
                                }
                            }
                            break;
                        case Operation.INSERT:
                            {
                                if (tokenProcessed)
                                {
                                    totalParsedLength += part.text.Length;
                                    valueLength += part.text.Length;

                                    tokenProcessed = false;
                                }

                                diffInput += part.text.Length;
                            }
                            break;
                        case Operation.EQUAL:
                            {
                                commonLength += part.text.Length;
                            }
                            break;
                    }
                }

                if (tokenProcessed)
                {
                    string parsedValue = ParseProcessor.Postprocess(input.Substring(valueIndex, valueLength), null);
                    output = OutputBuilder.TokenParsed(output, tokenNames[variableIndex], parsedValue);

                    valueIndex = -1;
                }
            }

            double differenceNormalized = 1 - (double)commonLength / (commonLength + Math.Max(diffTemplate, diffInput - totalParsedLength));
            output = OutputBuilder.FinalizeOutput(output, differenceNormalized);

            return output;
        }
    }
}
