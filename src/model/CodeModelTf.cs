using AutoRest.Core.Model;
using AutoRest.Core.Utilities;
using AutoRest.Extensions.Azure;
using System.Collections.Generic;
using System.Linq;
using static AutoRest.Terraform.TfProviderMetadata;

namespace AutoRest.Terraform
{
    public class CodeModelTf
        : CodeModel
    {
        public IEnumerable<CompositeTypeTf> AllComplexTypes => AllModelTypes.Cast<CompositeTypeTf>();
        public IEnumerable<EnumTypeTf> AllEnumTypes => EnumTypes.Cast<EnumTypeTf>();

        public TfProviderField RootField { get; } = new TfProviderField();
        internal List<GoSDKInvocation> CreateInvocations { get; } = new List<GoSDKInvocation>();
        internal List<GoSDKInvocation> ReadInvocations { get; } = new List<GoSDKInvocation>();
        internal List<GoSDKInvocation> UpdateInvocations { get; } = new List<GoSDKInvocation>();
        internal List<GoSDKInvocation> DeleteInvocations { get; } = new List<GoSDKInvocation>();
    }

    public class MethodTf
        : Method
    {
        public bool IsLongRunning => Extensions.Get<bool>(AzureExtensions.LongRunningExtension) == true;
    }

    public class CompositeTypeTf
        : CompositeType
    {
        public IEnumerable<PropertyTf> AllComposedProperties => ComposedProperties.Cast<PropertyTf>();

        public TypePackageDefinition OriginalMetadata { get; set; }
        public string DefinedPackage => OriginalMetadata.Package;

        public void Rename(string name) => Name = name;
    }

    public class EnumTypeTf
        : EnumType
    {
        public TypePackageDefinition OriginalMetadata { get; set; }
        public string DefinedPackage => OriginalMetadata.Package;
    }

    public class PropertyTf
        : Property
    {
        public GoSDKTypeChain GenerateType { get; set; }
    }
}
