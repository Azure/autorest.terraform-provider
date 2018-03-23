using AutoRest.Core.Model;
using System.Collections.Generic;

namespace AutoRest.Terraform
{
    public class CodeModelTf
        : CodeModel
    {
        public string Path => Name;


        internal IList<TfProviderField> TfProviderFields { get; } = new List<TfProviderField>();

        internal IList<GoSDKInvocation> CreateInvocations { get; } = new List<GoSDKInvocation>();

        internal IList<GoSDKInvocation> ReadInvocations { get; } = new List<GoSDKInvocation>();

        internal IList<GoSDKInvocation> UpdateInvocations { get; } = new List<GoSDKInvocation>();

        internal IList<GoSDKInvocation> DeleteInvocations { get; } = new List<GoSDKInvocation>();


        internal MethodTf CreateMethod { get; set; }
        internal MethodTf ReadMethod { get; set; }
        internal MethodTf DeleteMethod { get; set; }
    }
}
