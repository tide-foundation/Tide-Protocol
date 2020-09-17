using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Tide.Ork.Classes.Rules
{
    public class Check
    {
        public static bool IsBool(string value) => Is<bool>(value, bool.TryParse);

        public static bool IsInt(string value) => Is<int>(value, int.TryParse);

        public static bool IsFloat(string value) => Is<float>(value, float.TryParse);

        public static bool IsChar(string value) => Is<char>(value, char.TryParse);

        public static bool IsProp(string value) => PropChecker.IsProp(value);
        
        private delegate bool TryParseHandler<T>(string value, out T result);

        private static bool Is<T>(string value, TryParseHandler<T> handler) where T : struct
        {
            if (string.IsNullOrEmpty(value))
                return false;

            T result;
            return handler(value, out result);
        }
    }

    public class PropChecker
    {
        public static bool IsProp(string value) =>
            string.IsNullOrEmpty(GetPropFormat(value).@class);

        public static object GetProp(string token)
        {
            var (className, prop) = GetPropFormat(token);
            if (className == null)
                return null;

            return Allowed().Where(type => type.Name == className)
                .Select(prp => GetValue(prp, prop)).FirstOrDefault();
        }

        private static IEnumerable<Type> Allowed()
        {
            yield return typeof(DateTime);
            yield return typeof(Environment);
        }

        private static (string @class, string prop) GetPropFormat(string value)
        {
            var rx = new Regex(@"([^\s]+)\.([^\.\s]+)");
            return rx.Matches(value).Select(tkn =>
                (tkn.Groups[1].Value, tkn.Groups[2].Value)).FirstOrDefault();
        }

        private static object GetValue(Type type, string prop) =>
            type.GetProperty(prop, BindingFlags.Public | BindingFlags.Static)?
                .GetValue(null, null);
    }
}