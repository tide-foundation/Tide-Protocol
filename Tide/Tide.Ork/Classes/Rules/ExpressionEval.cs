using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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

        public Expression<Func<bool>> Eval()
        {
            var rx = new Regex(@"[()]|[^\s()]+", RegexOptions.Compiled | RegexOptions.Multiline);
            var tokens = LoopRel(rx.Matches(query).Select(itm => itm.Value), MapRel).ToList();
            
            var i = 0;
            while (tokens.Count > 1)
            {
                if (tokens.Count < 3)
                    throw new Exception("Unsupported rule expression format");

                if (tokens[i].Equals("(")) {
                    i++;
                    continue;
                }
                else if (tokens[i + 2].Equals("(")) {
                    i += 3;
                    continue;
                }
                else if (i > 0 && tokens[i-1].Equals("(") && tokens[i+1].Equals(")")) {
                    tokens.RemoveAt(i + 1);
                    tokens.RemoveAt(i - 1);

                    i--;
                    if (i >= 2)
                        i -= 2;

                    continue;
                }

                tokens[i] = MapLog(tokens[i] as Expression, tokens[i+1], tokens[i+2] as Expression);
                tokens.RemoveRange(i + 1, 2);
            }

            return Expression.Lambda<Func<bool>>(tokens.Select(itm => itm as Expression).FirstOrDefault());
        }

        private IEnumerable<object> LoopRel(IEnumerable<string> tokens, Func<string, string, string, object> map) {
            var enume = tokens.GetEnumerator();
            
            var window = new List<string>(3);
            var anyMore = true;

            while(anyMore)
            {
                while (window.Count < 3 && (anyMore = enume.MoveNext()))
                {
                    if (IsGroup(enume.Current) || OperChecker.IsLogOper(enume.Current))
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

        private object MapRel(string left, string oper, string right)
        {
            //TODO: Check the expression on the right
            if (!OperChecker.IsRelOper(oper) || !Check.IsProp(left))
                throw new Exception("Invalid relational operation");

            var leftNode = PropChecker.GetNode(left);
            var rightNode = Check.GetNode(right);
            
            return OperChecker.GetRelationalNode(leftNode, oper, rightNode);
        }

        private object MapLog(Expression left, object oper, Expression right)
        {
            if (left == null || left.Type != typeof(Boolean) || right == null || right.Type != typeof(Boolean))
                throw new Exception("Invalid logic operation");

            return OperChecker.GetLogicalNode(left, oper, right);
        }

        private bool IsGroup(string token) => token == "(" || token == ")";
    }
}