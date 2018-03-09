using AutoRest.Core.Model;
using AutoRest.Core.Utilities;
using System.Linq;

namespace AutoRest.Terraform
{
    internal class MethodTf
        : Method
    {
        public void AppendToDisplayString(IndentedStringBuilder builder)
        {
            builder.AppendLine($"{Qualifier} \"{Name}\"; Transformations [{InputParameterTransformation.Count}]; Parameters [{Parameters.Count}]; Responses [{Responses.Count}]");
            builder.Indent();
            foreach (var parameter in Parameters.Cast<ParameterTf>())
            {
                parameter.AppendToDisplayString(builder);
            }
            builder.Outdent();

            builder.Indent();
            foreach (var response in Responses)
            {
                response.Value.AppendToDisplayString(builder, response.Key);
            }
            builder.Outdent();
        }
    }
}
