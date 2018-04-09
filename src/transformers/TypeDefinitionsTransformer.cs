using AutoRest.Core.Utilities;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using static AutoRest.Core.Utilities.DependencyInjection;
using static AutoRest.Terraform.TfProviderMetadata;

namespace AutoRest.Terraform
{
    /// <summary>
    /// Set <see cref="CompositeTypeTf.OriginalMetadata"/> property according to the user definitions from <see cref="TypeDefinition"/>.
    /// </summary>
    internal class TypeDefinitionsTransformer
        : TfProviderTransformerBase
    {
        protected override void TransformCore()
        {
            SetupRules();
            ResolvePackage();
        }

        private List<(Regex Pattern, TypeDefinition Definition)> Rules { get; } = new List<(Regex, TypeDefinition)>();

        private void SetupRules()
        {
            Rules.Clear();
            Rules.AddRange(from def in Singleton<SettingsTf>.Instance.Metadata.TypeDefinitions
                           select (def.Path.ToPropertyPathRegex(), def));
        }

        private void ResolvePackage()
        {
            var complexTypes = from t in CodeModel.AllComplexTypes
                               from r in Rules
                               where r.Pattern.IsMatch(t.ToPathString())
                               select new { Type = t, r.Definition };
            complexTypes.ForEach(m => m.Type.OriginalMetadata = m.Definition);
            var enumTypes = from t in CodeModel.AllEnumTypes
                            from r in Rules
                            where r.Pattern.IsMatch(t.ToPathString())
                            select new { Type = t, r.Definition };
            enumTypes.ForEach(m => m.Type.OriginalMetadata = m.Definition);
        }
    }
}
