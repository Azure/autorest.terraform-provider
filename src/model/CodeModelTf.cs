using AutoRest.Core.Model;
using System.Collections.Generic;
using System.Linq;
using static AutoRest.Terraform.TfProviderMetadata;

namespace AutoRest.Terraform
{
    public class CodeModelTf
        : CodeModel
    {
        public new IEnumerable<CompositeTypeTf> AllModelTypes => base.AllModelTypes.Cast<CompositeTypeTf>();

        public TfProviderField RootField { get; } = new TfProviderField();
        internal List<GoSDKInvocation> CreateInvocations { get; } = new List<GoSDKInvocation>();
        internal List<GoSDKInvocation> ReadInvocations { get; } = new List<GoSDKInvocation>();
        internal List<GoSDKInvocation> UpdateInvocations { get; } = new List<GoSDKInvocation>();
        internal List<GoSDKInvocation> DeleteInvocations { get; } = new List<GoSDKInvocation>();
    }

    public class CompositeTypeTf
        : CompositeType
    {
        public TypeDefinition OriginalMetadata { get; set; }
        public string DefinedPackage => OriginalMetadata.Package;
    }
}
