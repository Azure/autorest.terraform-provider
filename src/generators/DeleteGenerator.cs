using AutoRest.Core;
using AutoRest.Terraform.Templates;
using System.Collections.Generic;

namespace AutoRest.Terraform
{
    public class DeleteGenerator
        : TfFunctionGeneratorBase<DeleteTemplate, DeleteGenerator>
    {
        protected override string FunctionNamePostfix => "Delete";
        public override IEnumerable<GoSDKInvocation> Invocations => CodeModel.DeleteInvocations;
    }
}
