using AutoRest.Core;
using AutoRest.Terraform.Templates;
using static AutoRest.Core.Utilities.DependencyInjection;

namespace AutoRest.Terraform
{
    public class ImportsGenerator
        : TfGeneratorBase<ImportsTemplate, ImportsGenerator>
    {
        public string PackageName => Settings.StandardSettings.Namespace;
    }
}
