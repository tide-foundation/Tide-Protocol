using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Tide.Ork.Classes.Rules
{
    public class PropChecker
    {
        public static bool IsProp(string value) =>
            !string.IsNullOrEmpty(GetPropFormat(value).ClassName);

        public static object GetProp(string token)
        {
            var (className, propName) = GetPropFormat(token);
            if (className == null)
                return null;

            return Allowed().Where(type => type.Name == className)
                .Select(prp => GetValue(prp, propName)).FirstOrDefault();
        }

        public static MemberExpression GetNode(string token)
        {
            var (className, propName) = GetPropFormat(token);
            if (className == null)
                return null;

            var propInfo = Allowed().Where(type => type.Name == className)
                .Select(type => type.GetProperty(propName, BindingFlags.Public | BindingFlags.Static))
                .FirstOrDefault();
            
            if (propInfo == null)
                throw new Exception($"Invalid property {token}");

            return Expression.Property(null, propInfo);
        }

        private static IEnumerable<Type> Allowed()
        {
            yield return typeof(DateTime);
            yield return typeof(DateInfo);
            yield return typeof(Environment);
            yield return typeof(ContextInfo);
        }

        private static (string ClassName, string PropName) GetPropFormat(string value)
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