using System.Collections.Generic;
using System.Linq;
using static AutoRest.Terraform.TfProviderMetadata;
using static AutoRest.Terraform.Utilities;

namespace AutoRest.Terraform
{
    /// <summary>
    /// Set <see cref="CodeModelTf.CreateInvocations"/>, <see cref="CodeModelTf.ReadInvocations"/>, <see cref="CodeModelTf.UpdateInvocations"/>
    /// and <see cref="CodeModelTf.DeleteInvocations"/> based on the filtering information provided in <see cref="SchemaDefinition.Excludes"/>.
    /// </summary>
    internal class NewInvocationsTransformer
        : TfProviderTransformerBase
    {
        protected override void TransformCore()
        {
            var metadata = Settings.Metadata;
            CodeModel.CreateInvocations.AddRange(FilterByPath(metadata.CreateMethods, InvocationCategory.Creation));
            CodeModel.ReadInvocations.AddRange(FilterByPath(metadata.ReadMethods, InvocationCategory.Read));
            CodeModel.UpdateInvocations.AddRange(FilterByPath(metadata.UpdateMethods, InvocationCategory.Update));
            CodeModel.DeleteInvocations.AddRange(FilterByPath(metadata.DeleteMethods, InvocationCategory.Deletion));
        }

        private IEnumerable<GoSDKInvocation> FilterByPath(IEnumerable<MethodDefinition> metadata, InvocationCategory category)
        {
            return from def in metadata
                   let pattern = def.Path.ToPropertyPathRegex()
                   from op in CodeModel.Operations
                   from m in op.Methods.Cast<MethodTf>()
                   let path = JoinPathStrings(CodeModel.Name, op.Name, m.Name)
                   where pattern.IsMatch(path)
                   select new GoSDKInvocation(m, def, category);
        }
    }
}
