using AutoRest.Core.Model;
using AutoRest.Core.Utilities;
using System.Linq;

namespace AutoRest.Terraform
{
    public class CodeModelTf
        : CodeModel
    {
        public string Path => Name;

        internal MethodTf CreateMethod { get; set; }
        internal MethodTf ReadMethod { get; set; }
        internal MethodTf UpdateMethod { get; set; }
        internal MethodTf DeleteMethod { get; set; }

        public void AppendToDisplayString(IndentedStringBuilder builder)
        {
            builder.AppendLine($"{Qualifier} \"{Name}\"; Operations [{Operations.Count}]");
            builder.Indent();
            foreach (var methodGroup in Operations.Cast<MethodGroupTf>())
            {
                methodGroup.AppendToDisplayString(builder);
            }
            builder.Outdent();

            builder.AppendLine($"Methods: [{Methods.Count()}]");
            builder.Indent();
            foreach (var method in Methods)
            {
                builder.AppendLine($"Serialized Name: {method.SerializedName}");
            }
            builder.Outdent();

            builder.AppendLine($"All Types: [{AllModelTypes.Count()}]");
            builder.Indent();
            foreach (var type in AllModelTypes)
            {
                builder.AppendLine($"Serialized Name: {type.SerializedName}");
            }
            builder.Outdent();
        }
    }
}
