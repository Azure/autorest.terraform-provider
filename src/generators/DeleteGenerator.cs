using AutoRest.Core;
using AutoRest.Terraform.Templates;

namespace AutoRest.Terraform
{
    public class DeleteGenerator
        : TfFunctionGeneratorBase<DeleteTemplate, DeleteGenerator>
    {
        protected override string FunctionNamePostfix => "Delete";
    }
}
