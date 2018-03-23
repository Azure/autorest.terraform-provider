using AutoRest.Core.Model;
using System;
using System.Collections.Generic;
using System.Text;
using static AutoRest.Terraform.TfProviderMetadata;

namespace AutoRest.Terraform
{
    internal class GoSDKInvocation
    {
        public GoSDKInvocation(Method method, SchemaDefinition metadata)
        {
            OriginalMethod = method;
            OriginalMetadata = metadata;
        }

        public Method OriginalMethod { get; }
        public SchemaDefinition OriginalMetadata { get; }

        public string MethodName => OriginalMethod.Name;
        public IList<GoSDKTypedData> Arguments { get; } = new List<GoSDKTypedData>();
    }

    internal class GoSDKTypedData
    {
        public string Name { get; }

        public TfProviderField BackingField { get; }

        public GoSDKTypeChain TypeChain { get; }
    }

    internal enum GoSDKTerminalTypes
    {
        Boolean, Int32, String, Enum, Complex
    }

    internal enum GoSDKNonTerminalTypes
    {
        Array, StringMap
    }

    internal class GoSDKTypeChain
    {
        private IList<GoSDKNonTerminalTypes> Chain { get; }

        private GoSDKTerminalTypes Terminal { get; }

        public IList<GoSDKTypedData> Properties { get; }
    }
}
