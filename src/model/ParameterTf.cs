using AutoRest.Core.Model;
using AutoRest.Core.Utilities;
using AutoRest.Extensions;
using System;

namespace AutoRest.Terraform
{
    internal class ParameterTf
        : Parameter
    {
        public ParameterTf() => InvalidatePath();

        private Lazy<string> path;
        public string Path => path.Value;
        public void InvalidatePath() => path = new Lazy<string>(() => $"{((MethodTf)Parent).Path}/{this.GetClientName()}");

        public bool IsResourceName { get; set; }
        public bool IsResourceGroupName { get; set; }

        public void AppendToDisplayString(IndentedStringBuilder builder)
        {
            builder.AppendLine($"{Qualifier}{(IsResourceName ? "[Name]" : string.Empty)}{(IsResourceGroupName ? "[RGName]" : string.Empty)} \"{this.GetClientName()}\"; Location: {Location}; Type: {ModelType.ToSummaryString()}");
            builder.Indent();
            ModelType.AppendToDisplayString(builder);
            builder.Outdent();
        }
    }
}
