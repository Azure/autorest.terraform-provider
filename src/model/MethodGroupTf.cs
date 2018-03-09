using AutoRest.Core.Model;
using AutoRest.Core.Utilities;
using System.Linq;

namespace AutoRest.Terraform
{
    internal class MethodGroupTf
        : MethodGroup
    {
        public void AppendToDisplayString(IndentedStringBuilder builder)
        {
            builder.AppendLine($"{Qualifier} \"{Name}\"; Type: {TypeName}; PropName: {NameForProperty}; Methods [{Methods.Count}]");
            builder.Indent();
            foreach (var method in Methods.Cast<MethodTf>())
            {
                method.AppendToDisplayString(builder);
            }
            builder.Outdent();
        }
    }
}
