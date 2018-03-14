using AutoRest.Core.Model;
using AutoRest.Core.Utilities;
using System;
using System.Linq;

namespace AutoRest.Terraform
{
    internal class MethodGroupTf
        : MethodGroup
    {
        public MethodGroupTf() => InvalidatePath();

        private Lazy<string> path;
        public string Path => path.Value;
        public void InvalidatePath() => path = new Lazy<string>(() => $"{((CodeModelTf)Parent).Path}/{Name}");

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
