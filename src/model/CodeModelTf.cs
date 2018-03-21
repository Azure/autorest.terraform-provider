using AutoRest.Core.Model;

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
    }
}
