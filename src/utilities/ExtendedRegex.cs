using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace AutoRest.Terraform
{
    internal static partial class Utilities
    {
        public const string ModelPathSeparator = "/", ModelAttributeStart = "[", ModelAttributeEnd = "]";
        public const string ParameterRootPath = "parameter", ResponseRootPath = "response";
        public const string ResponseHeaderAttribute = ModelAttributeStart + "Header" + ModelAttributeEnd, ResponseBodyAttribute = ModelAttributeStart + "Body" + ModelAttributeEnd;


        private const string ExtensionParamPattern = "[a-zA-Z0-9*]+", ExtensionNameGroup = "extname", ExtensionParamGroup = "extparam";
        private static readonly Regex ExtensionPattern = new Regex($@"\{{:(?<{ExtensionNameGroup}>{ExtensionParamPattern})(:(?<{ExtensionParamGroup}>{ExtensionParamPattern}))*:\}}",
            RegexOptions.ExplicitCapture | RegexOptions.CultureInvariant | RegexOptions.Compiled);
        private static readonly IDictionary<(string Name, int ParamsCount), string> Replacements = new Dictionary<(string, int), string>
        {
            { ("*", 0), $@"[^{ModelPathSeparator}]+" },
            { ("p", 0), $@"{ParameterRootPath}" },
            { ("p", 1), $@"{ParameterRootPath}\[\{{0\}}\]" },
            { ("r", 0), $@"{ResponseRootPath}" }
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
        public static Regex AsPropertyPathRegex(this string pattern)
        {
            Debug.Assert(!string.IsNullOrEmpty(pattern));
            pattern = ExtensionPattern.Replace(pattern, match =>
            {
                var name = match.Groups[ExtensionNameGroup].Captures.Single().Value;
                var param = match.Groups[ExtensionParamGroup].Captures.Select(c => c.Value).ToList();
                var replacement = Replacements[(name, param.Count)];
                return string.Format(CultureInfo.InvariantCulture, replacement, param);
            });
            return new Regex($"^{pattern}$", RegexOptions.CultureInvariant | RegexOptions.Compiled);
        }
    }
}
