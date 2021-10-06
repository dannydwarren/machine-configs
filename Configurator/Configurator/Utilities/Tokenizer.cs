using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Configurator.Utilities
{
    public interface ITokenizer
    {
        string Detokenize(string tokenizedString);
    }

    public class Tokenizer : ITokenizer
    {
        public string Detokenize(string tokenizedString)
        {
            var tokens = GetTokens(tokenizedString);

            FillTokenValues(tokens);

            var detokenizedStringBuilder = new StringBuilder(tokenizedString);
            foreach (var token in tokens)
            {
                detokenizedStringBuilder.Replace(token.Raw, token.Value);
            }

            return detokenizedStringBuilder.ToString();
        }

        private void FillTokenValues(List<Token> tokens)
        {
            foreach (var token in tokens)
            {
                if (token.Source == "env")
                {
                    token.Value = Environment.GetEnvironmentVariable(token.Name) ?? "";
                }
            }
        }

        private List<Token> GetTokens(string tokenizedString)
        {
            var tokens = new List<Token>();

            string tokenPattern = @"(\{\{(.*?)\}\})+";
            foreach (Match? match in Regex.Matches(tokenizedString, tokenPattern))
            {
                if (match == null)
                    continue;

                tokens.Add(CreateToken(match));
            }

            return tokens;
        }

        private Token CreateToken(Match tokenMatch)
        {
            var rawToken = tokenMatch.Value;
            var tokenParts = rawToken.TrimStart('{').TrimEnd('}').Split(":", StringSplitOptions.RemoveEmptyEntries);

            return new Token
            {
                Raw = rawToken,
                Source = tokenParts[0],
                Name = tokenParts[1]
            };
        }

        private class Token
        {
            public string Raw { get; set; }
            public string Source { get; set; }
            public string Name { get; set; }
            public string Value { get; set; }
        }
    }
}
