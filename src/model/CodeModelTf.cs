using AutoRest.Core.Model;
using System.Collections.Generic;

namespace AutoRest.Terraform
{
    public class CodeModelTf
        : CodeModel
    {
        public TfProviderField RootField { get; } = new TfProviderField();
        internal List<GoSDKInvocation> CreateInvocations { get; } = new List<GoSDKInvocation>();
        internal List<GoSDKInvocation> ReadInvocations { get; } = new List<GoSDKInvocation>();
        internal List<GoSDKInvocation> UpdateInvocations { get; } = new List<GoSDKInvocation>();
        internal List<GoSDKInvocation> DeleteInvocations { get; } = new List<GoSDKInvocation>();
    }
}
