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
            Singleton<DeleteGenerator>.Instance = this;
        }

        public string FileName => "generation-test.go";

        public ITemplate CreateTempalte() => new DeleteTemplate { Model = this };

        public void Generate(CodeModelTf model)
        {
        }
    }
}
