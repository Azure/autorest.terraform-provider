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
        : TfFunctionGeneratorBase<ReadTemplate, ReadGenerator>
    {
        protected override string FunctionNamePostfix => "Read";
        public override IEnumerable<GoSDKInvocation> Invocations => CodeModel.ReadInvocations;
    }
}
