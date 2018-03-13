using AutoRest.Core;
using AutoRest.Terraform.Templates;
using Humanizer;
using static AutoRest.Core.Utilities.DependencyInjection;

namespace AutoRest.Terraform
{
    public class SchemaGenerator
        : ITfProviderGenerator
    {
        public SchemaGenerator()
        {
            Singleton<SchemaGenerator>.Instance = this;
        }

        public string FileName { get; } = $"Resource ARM {Singleton<SettingsTf>.Instance.Metadata.ResourceName}".Underscore();

        public ITemplate CreateTempalte() => new SchemaTemplate { Model = this };

        public void Generate(CodeModelTf model)
        {
        }
    }
}
