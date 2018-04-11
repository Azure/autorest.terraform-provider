using AutoRest.Core;
using AutoRest.Terraform.Templates;
using static AutoRest.Core.Utilities.DependencyInjection;

namespace AutoRest.Terraform
{
    public class SchemaGenerator
        : TfFunctionGeneratorBase
    {
        public SchemaGenerator() => Singleton<SchemaGenerator>.Instance = this;
        protected override ITemplate CreateTemplateCore() => new SchemaTemplate { Model = this };

        protected override string FunctionNamePostfix => string.Empty;
        public TfProviderField RootField => CodeModel.RootField;
    }
}
