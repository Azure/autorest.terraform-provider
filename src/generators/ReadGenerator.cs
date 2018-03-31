using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoRest.Core;
using AutoRest.Extensions;
using AutoRest.Terraform.Templates;

namespace AutoRest.Terraform
{
    public class ReadGenerator
        : TfGeneratorBase<ReadTemplate, ReadGenerator>
    {
        public string FunctionName => CodeNamer.GetResourceReadMethodName(ResourceName);
    }
}
