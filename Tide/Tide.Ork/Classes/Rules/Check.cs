using System;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

namespace Tide.Ork.Classes.Rules
{
    public class Check
    {
        public static Expression GetNode(string token) {
            object constant = IsBool(token) ? Cast<bool>(token, bool.TryParse)
                : IsInt(token) ? Cast<int>(token, int.TryParse)
                : IsFloat(token) ? Cast<float>(token, float.TryParse)
                : IsChar(token) ? Cast<char>(token, char.TryParse)
                : IsProp(token) ? PropChecker.GetProp(token)
                : GetString(token);
            
            if (constant == null)
                throw new Exception($"Invalid constant value {token}");

            return ConstantExpression.Constant(constant);
        }

        public static string GetString(string value) => 
            new Regex(@"^(?:""(.*)"")").Matches(value)
            .Select(tkn => tkn.Groups[1].Value).FirstOrDefault();

        public static bool IsBool(string value) => Is<bool>(value, bool.TryParse);

        public static bool IsInt(string value) => Is<int>(value, int.TryParse);

        public static bool IsFloat(string value) => Is<float>(value, float.TryParse);

        public static bool IsChar(string value) => Is<char>(value, char.TryParse);

        public static bool IsProp(string value) => PropChecker.IsProp(value);
        
        private delegate bool TryParseHandler<T>(string value, out T result);

        private static T Cast<T>(string value, TryParseHandler<T> handler) {
            if (string.IsNullOrEmpty(value))
                return default(T);

            T result;
            if (handler(value, out result))
                return result;

            return default(T);
        }

        private static bool Is<T>(string value, TryParseHandler<T> handler) where T : struct
        {
            if (string.IsNullOrEmpty(value))
                return false;

            T result;
            return handler(value, out result);
        }
    }
   
}