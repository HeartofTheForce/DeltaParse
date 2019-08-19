using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace DeltaParse.Processors
{
    public class MungeProcessor : IParseProcessor
    {
        private const char TokenChar = (char)0;

        public string ProcessedToken { get; }

        private int CharCount { get; set; }
        private List<char> ReverseMap { get; set; }
        private Dictionary<char, int> CharacterMap { get; set; }
        private StringBuilder Builder { get; set; }

        public MungeProcessor()
        {
            ProcessedToken = new string(new char[] { TokenChar, TokenChar, TokenChar, TokenChar });
            CharCount = 1;

            ReverseMap = new List<char>();
            CharacterMap = new Dictionary<char, int>();
            Builder = new StringBuilder();
        }

        public string Preprocess(string input, string templateToken)
        {
            Builder.Clear();

            List<int> startIndexes = new List<int>();
            if (templateToken != null)
            {
                var matches = Regex.Matches(input, templateToken);

                foreach (Match m in matches)
                {
                    startIndexes.Add(m.Index);
                }
            }

            int tokens = 0;
            for (int i = 0; i < input.Length; i++)
            {
                if (templateToken != null && tokens < startIndexes.Count && startIndexes[tokens] == i)
                {
                    Builder.Append(ProcessedToken);
                    i += ProcessedToken.Length - 1;
                    tokens++;
                    continue;
                }

                char c = input[i];
                if (CharacterMap.TryGetValue(c, out int charIndex))
                {
                    Builder.Append((char)charIndex);
                }
                else
                {
                    if (CharCount == 65535)
                        throw new Exception("Character limited reached, cannot Munge");

                    CharacterMap.Add(c, CharCount);
                    Builder.Append((char)CharCount);

                    CharCount++;

                    ReverseMap.Add(c);
                }
            }

            return Builder.ToString();
        }

        public string Postprocess(string input, string templateToken)
        {
            Builder.Clear();

            for (int i = 0; i < input.Length; i++)
            {
                if (input[i] == TokenChar)
                {
                    i += ProcessedToken.Length - 1;
                    Builder.Append(templateToken);
                }
                else
                {
                    Builder.Append(ReverseMap[input[i] - 1]);
                }
            }

            return Builder.ToString();
        }
    }
}
