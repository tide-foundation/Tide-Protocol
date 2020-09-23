using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using Tide.Core;

namespace Tide.Ork.Classes.Rules
{
    public class RuleConditionEval
    {
        private readonly string query;

        public RuleConditionEval(string query)
        {
            this.query = query;
        }

        public RuleConditionEval(List<RuleCondition> conditions)
        {
            this.query = GetQuery(conditions);
        }

        public RuleConditionEval(RuleVault rule)
        {
            if (string.IsNullOrWhiteSpace(rule.Condition))
                this.query = "false";
            else if (rule.Condition.ToLower() == "true" || rule.Condition.ToLower() == "false")
                this.query = rule.Condition;
            else
                this.query = GetQuery(JsonSerializer.Deserialize<List<RuleCondition>>(rule.Condition, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }));
        }

        private static string GetQuery(List<RuleCondition> conditions)
        {
            var str = new StringBuilder();
            var level = 0;
            foreach (var item in conditions)
            {
                var diff = item.Level - level;
                if (diff < 0)
                    Enumerable.Range(0, Math.Abs(diff)).ToList().ForEach(i => str.Append(')'));

                if (item.HashUnion)
                    str.Append(" ").Append(item.Union);

                if (diff > 0)
                    Enumerable.Range(0, diff).ToList().ForEach(i => str.Append('('));

                str.Append(" ").Append(item.Field).Append(" ").Append(item.Operator).Append(" ").Append(item.Value);

                level += diff;
            }

            return str.ToString();
        }

        public bool Run() => Eval().Compile()();

        public Expression<Func<bool>> Eval()
        {
            if (query == "true" || query == "false")
                return () => query == "true";
            
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