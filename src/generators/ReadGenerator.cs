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
        : ResourceGeneratorBase
    {
        public override ITemplate CreateTempalte() => new ReadTemplate { Model = this };

        protected override void GenerateCore()
        {
            var createParameters = CodeModel.CreateMethod.LogicalParameters.Cast<ParameterTf>();
            ResourceNameParameter = CodeNamer.GetResourceSchemaPropertyName(createParameters.Single(p => p.IsResourceName).GetClientName());
            ResourceGroupNameParameter = CodeNamer.GetResourceSchemaPropertyName(createParameters.Single(p => p.IsResourceGroupName).GetClientName());
        }

        public string FunctionName => CodeNamer.GetResourceReadMethodName(ResourceName);
        public string AzureResourceIdPath => CodeNamer.GetAzureGoSDKIdPathName(CodeModel.DeleteMethod.MethodGroup.Name);
        public string ResourceNameParameter { get; private set; }
        public string ResourceGroupNameParameter { get; private set; }
    }
}
