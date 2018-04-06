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
        : ITfProviderTransformer
    {
        public void Transform(CodeModelTf model)
        {
            CodeModel = model;
            RefreshRules();
            ResolvePackage();
        }

        private CodeModelTf CodeModel { get; set; }
        private List<(Regex Pattern, TypeDefinition Definition)> Rules { get; } = new List<(Regex Pattern, TypeDefinition Definition)>();

        private void RefreshRules()
        {
            Rules.Clear();
            Rules.AddRange(from def in Singleton<SettingsTf>.Instance.Metadata.TypeDefinitions
                           select (def.Path.ToPropertyPathRegex(), def));
        }

        private void ResolvePackage()
        {
            var matchedTypes = from t in CodeModel.AllModelTypes
                               from r in Rules
                               where r.Pattern.IsMatch(t.ToPathString())
                               select new { Type = t, r.Definition };
            matchedTypes.ForEach(m => m.Type.OriginalMetadata = m.Definition);
        }
    }
}
