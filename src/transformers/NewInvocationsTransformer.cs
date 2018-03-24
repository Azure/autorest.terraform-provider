using AutoRest.Core.Model;
using System.Collections.Generic;
using System.Linq;
using static AutoRest.Core.Utilities.DependencyInjection;
using static AutoRest.Terraform.TfProviderMetadata;
using static AutoRest.Terraform.Utilities;

namespace AutoRest.Terraform
{
    /// <summary>
    /// Set <see cref="CodeModelTf.CreateInvocations"/>, <see cref="CodeModelTf.ReadInvocations"/>, <see cref="CodeModelTf.UpdateInvocations"/>
    /// and <see cref="CodeModelTf.DeleteInvocations"/> based on the filtering information provided in <see cref="MethodDefinition"/>.
    /// </summary>
    internal class NewInvocationsTransformer
        : ITfProviderTransformer
    {
        public void Transform(CodeModelTf model)
        {
            var metadata = Singleton<SettingsTf>.Instance.Metadata;
            model.CreateInvocations.AddRange(FilterByPath(model, metadata.CreateMethods));
            model.ReadInvocations.AddRange(FilterByPath(model, metadata.ReadMethods));
            model.UpdateInvocations.AddRange(FilterByPath(model, metadata.UpdateMethods));
            model.DeleteInvocations.AddRange(FilterByPath(model, metadata.DeleteMethods));
        }

        private IEnumerable<GoSDKInvocation> FilterByPath(CodeModel model, IEnumerable<MethodDefinition> metadata)
        {
            return from def in metadata
                   let pattern = def.Path.AsPropertyPathRegex()
                   from op in model.Operations
                   from m in op.Methods
                   let path = string.Join(ModelPathSeparator, model.Name, op.Name, m.Name)
                   where pattern.IsMatch(path)
                   select new GoSDKInvocation(m, def.Schema);
        }
    }
}
