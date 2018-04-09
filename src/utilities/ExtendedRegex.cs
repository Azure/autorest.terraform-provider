using AutoRest.Core.Model;
using AutoRest.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;

namespace AutoRest.Terraform
{
    internal static partial class Utilities
    {
        public const string ModelPathSeparator = "/";
        private const string AttributeStart = "[", AttributeEnd = "]", ExtensionStart = "{:", ExtensionEnd = ":}";
        private const string ParameterRootPath = "parameter", ResponseRootPath = "response", TypeRootPath = "type";
        private static readonly string ResponseHeaderAttribute = "Header".ToAttributeString(), ResponseBodyAttribute = "Body".ToAttributeString();

        private const string AnyPathExtName = "**", AnySinglePathExtName = "*", ParameterExtName = "p", ResponseExtName = "r", TypeExtName = "t";
        public static readonly string AnyPathExtension = AnyPathExtName.ToExtensionString();


        private static string ToAttributeString(this object obj) => AttributeStart + obj + AttributeEnd;
        private static string ToExtensionString(this string extension) => ExtensionStart + extension + ExtensionEnd;
        private static string WrapByFormatBraces(this object content) => $"{{{content}}}";
        private static string WrapByEscapedBraces(this object content) => $"{{{{{content}}}}}";

        public static string ExtractLastPath(this string path) => path.Substring(path.LastIndexOf(ModelPathSeparator) + 1);
        public static string AppendChild(this string path, string child) => path + ModelPathSeparator + Regex.Escape(child);
        public static string AppendAnyChildrenPath(this string path) => Regex.Escape(path) + ModelPathSeparator + AnyPathExtension;
        public static string JoinPathStrings(params string[] paths) => string.Join(ModelPathSeparator, paths);
        public static string[] SplitPathStrings(this string path) => path.Split(ModelPathSeparator, StringSplitOptions.RemoveEmptyEntries);

        public static string ToPathString(this Parameter parameter) => (ParameterRootPath + parameter.Location.ToAttributeString()).AppendChild(parameter.GetClientName());
        public static string ToPathString(this KeyValuePair<HttpStatusCode, Response> response, bool isHeader)
            => ResponseRootPath + ((int)response.Key).ToAttributeString() + (isHeader ? ResponseHeaderAttribute : ResponseBodyAttribute);
        public static string ToPathString(this Property property, string parentPath) => parentPath.AppendChild(property.GetClientName());
        public static string ToPathString(this CompositeTypeTf type) => TypeRootPath.AppendChild(type.Name);
        public static string ToPathString(this EnumTypeTf type) => TypeRootPath.AppendChild(type.Name);


        private const string ExtensionParamPattern = "[a-zA-Z0-9*]+", ExtensionNameGroup = "extname", ExtensionParamGroup = "extparam";
        private static readonly Regex ExtensionPattern = new Regex(
            $@"{Regex.Escape(ExtensionStart)}(?<{ExtensionNameGroup}>{ExtensionParamPattern})(:(?<{ExtensionParamGroup}>{ExtensionParamPattern}))*{Regex.Escape(ExtensionEnd)}",
            RegexOptions.ExplicitCapture | RegexOptions.CultureInvariant | RegexOptions.Compiled);

        private static readonly string AnyParamAttributePattern = Regex.Escape(AttributeStart) + ExtensionParamPattern + Regex.Escape(AttributeEnd);
        private static readonly IDictionary<(string Name, int ParamsCount), string> Replacements = new Dictionary<(string, int), string>
        {
            { (AnySinglePathExtName, 0), $@"[^{Regex.Escape(ModelPathSeparator)}]+" },
            { (AnyPathExtName, 0), $@".*" },
            { (ParameterExtName, 0), $@"{Regex.Escape(ParameterRootPath)}{AnyParamAttributePattern}" },
            { (ParameterExtName, 1), $@"{Regex.Escape(ParameterRootPath)}{Regex.Escape(AttributeStart)}{WrapByFormatBraces(0)}{Regex.Escape(AttributeEnd)}" },
            { (ResponseExtName, 0), $@"{Regex.Escape(ResponseRootPath)}({AnyParamAttributePattern}){WrapByEscapedBraces(2)}" },
            { (TypeExtName, 0), $@"{Regex.Escape(TypeRootPath)}" }
        };

        /// <summary>
        /// Treat <paramref name="pattern"/> as a path-specific extended regular expression, and creates an equivalent normal regular expression.
        /// </summary>
        /// <param name="pattern">The path-specific extended regular expression pattern string used to match properties.</param>
        /// <returns>The new instance of a normal regular expression which is equivalent to the extended <paramref name="pattern"/>.</returns>
        /// <remarks>
        /// Based on the normal regular expression, we introduce new extension points wrapped by "{::}".
        ///   {:p:}          matches a single path of "parameter"
        ///   {:p:Body:}     matches a single path of "parameter located in Body"
        ///   {:r:}          matches a single path of any kind of "response"
        ///   {:r:200:}      matches a single path of "HTTP OK response"
        ///   {:r:Header:}   matches a single path of "HTTP response header"
        ///   {:r:200:Body:} matches a single path of "HTTP OK response body"
        ///   {:*:}          matches any single path
        ///   {:**:}         matches zero or more paths and subpaths
        /// </remarks>
        public static Regex ToPropertyPathRegex(this string pattern)
        {
            Debug.Assert(!string.IsNullOrEmpty(pattern));
            pattern = ExtensionPattern.Replace(pattern, match =>
            {
                var name = match.Groups[ExtensionNameGroup].Captures.Single().Value;
                var param = match.Groups[ExtensionParamGroup].Captures.Select(c => c.Value).ToList();
                var replacement = Replacements[(name, param.Count)];
                return string.Format(CultureInfo.InvariantCulture, replacement, param);
            });
            return new Regex($"^{pattern}$", RegexOptions.ExplicitCapture | RegexOptions.CultureInvariant | RegexOptions.Compiled);
        }
    }
}
