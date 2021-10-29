using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using FluentAssertions;
using Tide.Core;
using Tide.Ork.Classes.Rules;
using Xunit;

namespace Tide.Ork.Test
{
    public class RuleConditionEvalTests
    {
        [Theory]
        [InlineData(500)]
        public void EvaluationShouldBeEfficient(int numberOfExpressions)
        {
            var expressions = RandomExpressions(numberOfExpressions).ToList();

            var ini = DateTime.Now;
            expressions.ForEach(exp => exp.Run());
            var diff = DateTime.Now - ini;

            diff.Should().BeLessOrEqualTo(TimeSpan.FromSeconds(1));
        }

        private static IEnumerable<RuleConditionEval> RandomExpressions(int length = 1000)
        {
            return Enumerable.Range(0, length).Select(_ => new RuleConditionEval(new List<RuleCondition> {
                new RuleCondition {
                    Level = 0,
                    Field = "DateInfo.DayOfWeek",
                    Operator = "==",
                    Value = randomText(),
                },
                new RuleCondition {
                    Level = 1,
                    Union = "&&",
                    Field = "ContextInfo.ProcessorCount",
                    Operator = "!=",
                    Value = randomInt(),
                },
                new RuleCondition {
                    Level = 2,
                    Union = "||",
                    Field = "DateInfo.Month",
                    Operator = "!=",
                    Value = randomInt(),
                },
                new RuleCondition {
                    Level = 3,
                    Union = "&&",
                    Field = "ContextInfo.TickCount",
                    Operator = ">=",
                    Value = randomInt(),
                },
            }));
        }

        static byte[] randomData(int size = 8)
        {
            var buffer = new byte[size];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(buffer);
                return buffer;
            }
        }

        static string randomText() => $"\"{Convert.ToBase64String(randomData(9))}\"";

        static string randomInt() => BitConverter.ToInt32(randomData(4), 0).ToString();        
    }
}
