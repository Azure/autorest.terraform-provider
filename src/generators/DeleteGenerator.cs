using AutoRest.Core;
using AutoRest.Terraform.Templates;
using static AutoRest.Core.Utilities.DependencyInjection;

namespace AutoRest.Terraform
{
    public class DeleteGenerator
        : ITfProviderGenerator
    {
        public DeleteGenerator()
        {
        }

        public string FileName => CodeNamer.GetResourceFileName(ResourceName);

        public ITemplate CreateTempalte() => new DeleteTemplate { Model = this };

        public void Generate(CodeModelTf model)
        {
            CodeModel = model;
        }

        public string ResourceName => Singleton<SettingsTf>.Instance.Metadata.ResourceName;
        private CodeNamerTf CodeNamer => Singleton<CodeNamerTf>.Instance;
        private CodeModelTf CodeModel { get; set; }

        public string FunctionName => CodeNamer.GetResourceDeleteMethodName(ResourceName);
        public string GoSDKClientName => CodeNamer.GetAzureGoSDKClientName(ResourceName);
        public string AzureResourceIdPath => CodeNamer.GetAzureGoSDKIdPathName(CodeModel.DeleteMethod.MethodGroup.Name);
    }
}
