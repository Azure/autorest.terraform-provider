using AutoRest.Core.Model;
using AutoRest.Core.Utilities;
using AutoRest.Extensions;

namespace AutoRest.Terraform
{
    class ParameterTf
        : Parameter
    {
        public void AppendToDisplayString(IndentedStringBuilder builder)
        {
            builder.AppendLine($"{Qualifier} \"{this.GetClientName()}\"; Location: {Location}; Type: {ModelType.ToSummaryString()}");
            builder.Indent();
            ModelType.AppendToDisplayString(builder);
            builder.Outdent();
        }
    }
}
