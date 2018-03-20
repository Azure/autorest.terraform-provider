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
        : ResourceGeneratorBase
    {
        internal UpdateGenerator(ReadGenerator readGenerator)
        {
            ReadGenerator = readGenerator;
        }

        public override ITemplate CreateTempalte() => new UpdateTemplate { Model = this };

        protected override void GenerateCore()
        {
            var parameters = CodeModel.CreateMethod.LogicalParameters.Cast<ParameterTf>();
            ResourceNameParameter = CodeNamer.GetResourceSchemaPropertyName(parameters.Single(p => p.IsResourceName).GetClientName());
            ResourceGroupNameParameter = CodeNamer.GetResourceSchemaPropertyName(parameters.Single(p => p.IsResourceGroupName).GetClientName());
        }

        private ReadGenerator ReadGenerator { get; }

        public string FunctionName => CodeNamer.GetResourceUpdateMethodName(ResourceName);
        public string ReadFunctionName => ReadGenerator.FunctionName;
        public string ResourceNameParameter { get; private set; }
        public string ResourceGroupNameParameter { get; private set; }
    }
}
