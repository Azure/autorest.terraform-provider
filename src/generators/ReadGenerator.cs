using System;
using System.Collections.Generic;
using System.Text;
using AutoRest.Core;
using AutoRest.Terraform.Templates;

namespace AutoRest.Terraform
{
    public class ReadGenerator
        : ResourceGeneratorBase
    {
        public override ITemplate CreateTempalte() => new ReadTemplate { Model = this };

        public string FunctionName => CodeNamer.GetResourceReadMethodName(ResourceName);
    }
}
