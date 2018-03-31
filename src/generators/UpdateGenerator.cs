using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoRest.Core;
using AutoRest.Extensions;
using AutoRest.Terraform.Templates;

namespace AutoRest.Terraform
{
    public class UpdateGenerator
        : TfFunctionGeneratorBase<UpdateTemplate, UpdateGenerator>
    {
        protected override string FunctionNamePostfix => "Update";
    }
}
