using AutoRest.Core;
using AutoRest.Terraform.Templates;
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

        public string FileName => "generation-test.go";

        public ITemplate CreateTempalte() => new SchemaTemplate { Model = this };

        public void Generate(CodeModelTf model)
        {
        }
    }
}
