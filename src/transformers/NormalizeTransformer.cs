using System.Linq;

namespace AutoRest.Terraform
{
    /// <summary>
    /// Normalize the whole <see cref="CodeModelTf"/> using the following steps:
    ///   1. Remove all complex schema fields with no children
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
