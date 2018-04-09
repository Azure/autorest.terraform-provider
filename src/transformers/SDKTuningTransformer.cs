using AutoRest.Core.Utilities;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AutoRest.Terraform
{
    /// <summary>
    /// Adjust the original spec to match the Azure Go SDK.
    /// </summary>
    internal class SDKTuningTransformer
        : TfProviderTransformerBase
    {
        protected override void TransformCore()
        {
            SetupRules();
            RenameSpecTypes();
        }

        private List<(Regex Pattern, string NewName)> RenameRules { get; } = new List<(Regex, string)>();

        private void SetupRules()
        {
            RenameRules.Clear();
            RenameRules.AddRange(from r in Settings.Metadata.SDKTunings.Renames
                                 select (r.SourcePath.ToPropertyPathRegex(), r.TargetName));
        }

        private void RenameSpecTypes()
        {
            var propsToRename = from t in CodeModel.AllComplexTypes
                                from p in t.ComposedProperties
                                from r in RenameRules
                                where r.Pattern.IsMatch(p.ToPathString(t.ToPathString()))
                                select new { Property = p, Name = r.NewName };
            propsToRename.ForEach(p => p.Property.Name = p.Name);
            var typesToRename = from t in CodeModel.AllComplexTypes
                                from r in RenameRules
                                where r.Pattern.IsMatch(t.ToPathString())
                                select new { Type = t, Name = r.NewName };
            typesToRename.ForEach(t => t.Type.Rename(t.Name));
        }
    }
}
