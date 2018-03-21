using AutoRest.Core.Model;
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
    }
}
