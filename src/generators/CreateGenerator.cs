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
        : ResourceGeneratorBase
    {
        internal CreateGenerator(ReadGenerator readGenerator, TypeExpandGenerator expandGenerator)
        {
            ReadGenerator = readGenerator;
            ExpandGenerator = expandGenerator;
        }

        public override ITemplate CreateTempalte() => new CreateTemplate { Model = this };

        protected override void GenerateCore()
        {
            var parameters = CodeModel.CreateMethod.LogicalParameters.Cast<ParameterTf>();
            ResourceNameParameter = CodeNamer.GetResourceSchemaPropertyName(parameters.Single(p => p.IsResourceName).GetClientName());
            ResourceGroupNameParameter = CodeNamer.GetResourceSchemaPropertyName(parameters.Single(p => p.IsResourceGroupName).GetClientName());
            RegularParameters = parameters.Where(p => !p.IsResourceName && !p.IsResourceGroupName).Select(p => (p.GetClientName(), p.ModelTypeName)).ToList();
        }

        private ReadGenerator ReadGenerator { get; }
        public TypeExpandGenerator ExpandGenerator { get; }

        public string FunctionName => CodeNamer.GetResourceCreateMethodName(ResourceName);
        public string ReadFunctionName => ReadGenerator.FunctionName;
        public string ResourceNameParameter { get; private set; }
        public string ResourceGroupNameParameter { get; private set; }
        public IEnumerable<(string Name, string Type)> RegularParameters { get; private set; }
    }
}
