using AutoRest.Core;
using AutoRest.Terraform.Templates;
using static AutoRest.Core.Utilities.DependencyInjection;

namespace AutoRest.Terraform
{
    public class ImportsGenerator
        : ResourceGeneratorBase
    {
        public override ITemplate CreateTempalte() => new ImportsTemplate { Model = this };

        public string Header => Singleton<SettingsTf>.Instance.StandardSettings.Header;
        public string PackageName => Singleton<SettingsTf>.Instance.StandardSettings.Namespace;
    }
}
