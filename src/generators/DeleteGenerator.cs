using AutoRest.Core;
using AutoRest.Terraform.Templates;

namespace AutoRest.Terraform
{
    public class DeleteGenerator
        : TfGeneratorBase<DeleteTemplate, DeleteGenerator>
    {
        public string FunctionName => CodeNamer.GetResourceDeleteMethodName(ResourceName);
    }
}
