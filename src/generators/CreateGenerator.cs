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
        : TfGeneratorBase<CreateTemplate, CreateGenerator>
    {
        public string FunctionName => CodeNamer.GetResourceCreateMethodName(ResourceName);
    }
}
