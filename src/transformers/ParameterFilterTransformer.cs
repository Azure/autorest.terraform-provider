using AutoRest.Core;
using AutoRest.Core.Model;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AutoRest.Terraform
{
    internal class ParameterFilterTransformer
        : CodeModelTransformer<CodeModelTf>
    {
        public MethodTf Method { get; set; }

        public ISet<string> IgnoringPatterns { get; set; } = new HashSet<string>();

        public override CodeModelTf TransformCodeModel(CodeModel codeModel)
        {
            var residues = new List<ParameterTf>();
            var patterns = IgnoringPatterns.Select(p => new Regex($"^{Method.Path}/{p}$", RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.Singleline));
            foreach (var p in Method.Parameters.Cast<ParameterTf>())
            {
                foreach (var pattern in patterns)
                {
                    if (pattern.IsMatch(p.Path))
                    {
                        residues.Add(p);
                    }
                }
            }
            foreach (var p in residues)
            {
                p.Method.Remove(p);
            }
            return base.TransformCodeModel(codeModel);
        }
    }
}
