using AutoRest.Core.Model;
using System;

namespace AutoRest.Terraform
{
    internal class MethodGroupTf
        : MethodGroup
    {
        public MethodGroupTf() => InvalidatePath();

        private Lazy<string> path;
        public string Path => path.Value;
        public void InvalidatePath() => path = new Lazy<string>(() => $"{((CodeModelTf)Parent).Path}/{Name}");
    }
}
