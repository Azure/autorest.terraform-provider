using AutoRest.Core.Utilities;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using static AutoRest.Terraform.TfProviderMetadata;

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
            RedefineSpecTypes();
        }


        private List<(Regex Pattern, string NewName)> RenameRules { get; } = new List<(Regex, string)>();
        private List<(Regex Pattern, GoSDKTypeChain NewType, GoSDKTypeChain GenType)> TypeRedefineRules { get; } = new List<(Regex, GoSDKTypeChain, GoSDKTypeChain)>();

        private void SetupRules()
        {
            RenameRules.Clear();
            RenameRules.AddRange(from r in Settings.Metadata.SDKTunings?.Renames ?? Enumerable.Empty<SDKRenameDefinition>()
                                 select (r.SourcePath.ToPropertyPathRegex(), r.TargetName));
            TypeRedefineRules.Clear();
            TypeRedefineRules.AddRange(from t in Settings.Metadata.SDKTunings?.TypeDefinitions ?? Enumerable.Empty<SDKTypeDefinition>()
                                       let pattern = t.FieldPath.ToPropertyPathRegex()
                                       select (pattern, GoSDKTypeChain.Parse(t.TargetType), GoSDKTypeChain.Parse(t.GenerateType)));
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

        private void RedefineSpecTypes()
        {
            var propsToRedefine = from t in CodeModel.AllComplexTypes
                                  from p in t.AllComposedProperties
                                  from r in TypeRedefineRules
                                  where r.Pattern.IsMatch(p.ToPathString(t.ToPathString()))
                                  select new { Property = p, Type = r.NewType.ToModelType(), r.GenType };
            foreach (var prop in propsToRedefine)
            {
                prop.Property.ModelType = prop.Type;
                prop.Property.GenerateType = prop.GenType;
            }
        }
    }
}
