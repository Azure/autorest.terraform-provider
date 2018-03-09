using AutoRest.Core.Model;
using AutoRest.Core.Utilities;
using AutoRest.Extensions;

namespace AutoRest.Terraform
{
    class PropertyTf
        : Property
    {
        public void AppendToDisplayString(IndentedStringBuilder builder, bool isComposed = false)
        {
            builder.AppendLine($"{(isComposed ? "Composed " : string.Empty)}{Qualifier} \"{this.GetClientName()}\"; Type: {ModelType.ToSummaryString()}");
            builder.Indent();
            ModelType.AppendToDisplayString(builder);
            builder.Outdent();
        }
    }
}
