using System;
using System.Linq.Expressions;

namespace Tide.Ork.Classes.Rules
{
    public class OperChecker
    {
        public static bool IsRelOper(object token) => token.Equals("==")
            || token.Equals("!=") || token.Equals(">=") || token.Equals(">")
            || token.Equals("<=") || token.Equals("<") || token.Equals("in");

        public static bool IsLogOper(object token) => token.Equals("&&")
            || token.Equals("&") || token.Equals("||") || token.Equals("|") || token.Equals("^");

        public static Expression GetRelationalNode(Expression left, object oper, Expression rigth)
        {
            var node = oper.Equals("==") ? Expression.Equal(left, rigth)
                : oper.Equals("!=") ? Expression.NotEqual(left, rigth)
                : oper.Equals(">=") ? Expression.GreaterThanOrEqual(left, rigth)
                : oper.Equals(">") ? Expression.GreaterThan(left, rigth)
                : oper.Equals("<=") ? Expression.LessThanOrEqual(left, rigth)
                : oper.Equals("<") ? Expression.LessThan(left, rigth)
                : oper.Equals("in") ? Expression.Call(rigth, typeof(string)
                    .GetMethod("Contains", new Type[] { typeof(String) }), left) as Expression
                : null;

            if (node == null)
                throw new Exception($"Invalid relational token {oper}");

            return node;
        }

        public static Expression GetLogicalNode(Expression left, object oper, Expression rigth)
        {
            var node = oper.Equals("&&") ? Expression.AndAlso(left, rigth)
                : oper.Equals("&") ? Expression.And(left, rigth)
                : oper.Equals("||") ? Expression.OrElse(left, rigth)
                : oper.Equals("|") ? Expression.Or(left, rigth)
                : oper.Equals("^") ? Expression.ExclusiveOr(left, rigth)
                : null;

            if (node == null)
                throw new Exception($"Invalid Logical token {oper}");

            return node;
        }
    }  
}