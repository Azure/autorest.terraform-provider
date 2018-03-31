using AutoRest.Terraform.Templates;

namespace AutoRest.Terraform
{
    public class SchemaGenerator
        : TfFunctionGeneratorBase<SchemaTemplate, SchemaGenerator>
    {
        protected override string FunctionNamePostfix => string.Empty;
        public TfProviderField RootField => CodeModel.RootField;
    }
}
