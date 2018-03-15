using AutoRest.Core;
using AutoRest.Terraform.Templates;
using static AutoRest.Core.Utilities.DependencyInjection;

namespace AutoRest.Terraform
{
    public class SchemaGenerator
        : ITfProviderGenerator
    {
        public SchemaGenerator(DeleteGenerator deleteGenerator)
        {
            DeleteGenerator = deleteGenerator;
        }

        public string FileName { get; } = Singleton<CodeNamerTf>.Instance.GetResourceFileName(Singleton<SettingsTf>.Instance.Metadata.ResourceName);

        public ITemplate CreateTempalte() => new SchemaTemplate { Model = this };

        public void Generate(CodeModelTf model)
        {
        }

        private DeleteGenerator DeleteGenerator { get; }

        public string FunctionName { get; } = Singleton<CodeNamerTf>.Instance.GetResourceDefinitionMethodName(Singleton<SettingsTf>.Instance.Metadata.ResourceName);
        public string DeleteFunctionName => DeleteGenerator.FunctionName;
    }
}
