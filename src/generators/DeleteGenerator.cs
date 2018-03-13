using AutoRest.Core;
using AutoRest.Terraform.Templates;
using Humanizer;
using static AutoRest.Core.Utilities.DependencyInjection;

namespace AutoRest.Terraform
{
    public class DeleteGenerator
        : ITfProviderGenerator
    {
        public DeleteGenerator()
        {
            Singleton<DeleteGenerator>.Instance = this;
        }

        public string FileName { get; } = $"Resource ARM {Singleton<SettingsTf>.Instance.Metadata.ResourceName}".Underscore();

        public ITemplate CreateTempalte() => new DeleteTemplate { Model = this };

        public void Generate(CodeModelTf model)
        {
        }
    }
}
