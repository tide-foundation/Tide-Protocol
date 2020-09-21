using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Tide.Ork.Classes.Rules
{
    public class ExpressionEval
    {
        private readonly string query;

        public ExpressionEval(string query)
        {
            this.query = query;
        }

        public string Eval()
        {
            var rx = new Regex(@"[()]|[^\s()]+", RegexOptions.Compiled | RegexOptions.Multiline);
            var tokens = LoopRel(rx.Matches(query).Select(itm => itm.Value), MapOper).ToList();
            
            var i = 0;
            while (tokens.Count > 1)
            {
                if (tokens.Count < 3)
                    throw new Exception("Unsupported rule expression format");

                if (tokens[i] == "(") {
                    i++;
                    continue;
                }
                else if (tokens[i + 2] == "(") {
                    i += 3;
                    continue;
                }
                else if (i > 0 && tokens[i-1] == "(" && tokens[i+1] == ")") {
                    tokens[i] = $"({tokens[i]})";
                    tokens.RemoveAt(i + 1);
                    tokens.RemoveAt(i - 1);

                    i--;
                    if (i >= 2)
                        i -= 2;

                    continue;
                }

                tokens[i] = $"{tokens[i]} {tokens[i+1]} {tokens[i+2]}";
                tokens.RemoveRange(i + 1, 2);
            }

            return tokens.FirstOrDefault();
        }

        private string MapOper(string left, string oper, string right) {
            return $"{left} {oper} {right}";
        }

        private IEnumerable<string> LoopRel(IEnumerable<string> tokens, Func<string, string, string, string> map) {
            var enume = tokens.GetEnumerator();
            
            var window = new List<string>(3);
            var anyMore = true;

            while(anyMore)
            {
                while (window.Count < 3 && (anyMore = enume.MoveNext()))
                {
                    if (IsGroup(enume.Current) || IsLogOper(enume.Current))
                    {
                        if (window.Count != 0)
                            throw new Exception("Invalid logic expression");

                        yield return enume.Current;
                        continue;
                    }

                    window.Add(enume.Current);
                }
                
                if (window.Count == 3)
                {
                    yield return map(window[0], window[1], window[2]);
                }
                else
                {
                    foreach (var token in window)
                    {
                        yield return token;
                    }
                }

                window.Clear();
            }
        }

        private bool IsGroup(string token) => token == "(" || token == ")";

        private bool IsRelOper(string token) => Regex.IsMatch(token, @"^(==|!=|>=|>|<=|<|in)$");

        private bool IsLogOper(string token) => Regex.IsMatch(token, @"^(&&|&|\|\||\||\^)$");
    }
}