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
    /// and <see cref="CodeModelTf.DeleteInvocations"/> based on the filtering information provided in <see cref="SchemaDefinition.Excludes"/>.
    /// </summary>
    internal class NormalizeTransformer
        : ITfProviderTransformer
    {
        public void Transform(CodeModelTf model)
        {
            CodeModel = model;
            ClearComplexWithNoChildrenFields();
        }


        private CodeModelTf CodeModel { get; set; }

        private void ClearComplexWithNoChildrenFields()
        {
            var toRemove = from f in CodeModel.RootField.Traverse(TraverseType.PreOrder)
                           where f.GoType.Terminal == GoSDKTerminalTypes.Complex && !f.SubFields.Any()
                           select f;
            toRemove.ToList().ForEach(f => f.Remove());
        }
    }
}
