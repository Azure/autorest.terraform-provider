using AutoRest.Core;
using AutoRest.Core.Model;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using static AutoRest.Terraform.TfProviderMetadata;

namespace AutoRest.Terraform
{
    /// <summary>
    /// Clone the <see cref="Parameter"/>s and <see cref="Response"/>s in a <see cref="Method"/> to the corresponding <see cref="GoSDKInvocation.Arguments"/>.
    /// This transformer also removes all items matching <see cref="SchemaDefinition.Excludes"/>.
    /// </summary>
    internal class ArgumentsFilterCloneTransformer
        : ITfProviderTransformer
    {
        public void Transform(CodeModelTf model)
        {
            throw new System.NotImplementedException();
        }
    }
}
