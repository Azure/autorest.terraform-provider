using AutoRest.Core;
using AutoRest.Extensions;
using AutoRest.Terraform.Templates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoRest.Terraform
{
    public class CreateGenerator
        : TfFunctionGeneratorBase<CreateTemplate, CreateGenerator>
    {
        protected override string FunctionNamePostfix => "Create";
    }
}
