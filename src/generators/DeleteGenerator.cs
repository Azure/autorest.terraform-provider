using AutoRest.Core;
using AutoRest.Terraform.Templates;

namespace AutoRest.Terraform
{
    public class DeleteGenerator
        : ResourceGeneratorBase
    {
        public override ITemplate CreateTempalte() => new DeleteTemplate { Model = this };

        public string FunctionName => CodeNamer.GetResourceDeleteMethodName(ResourceName);
        public string GoSDKClientName => CodeNamer.GetAzureGoSDKClientName(ResourceName);
        public string AzureResourceIdPath => CodeNamer.GetAzureGoSDKIdPathName(CodeModel.DeleteMethod.MethodGroup.Name);
    }
}
