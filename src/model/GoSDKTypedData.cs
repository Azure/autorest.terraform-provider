using System;
using System.Collections.Generic;
using System.Text;

namespace AutoRest.Terraform
{
    internal class GoSDKTypedData
    {
        public string Name { get; }

        public TfProviderField BackingField { get; }

        public GoSDKTypeChain GoType { get; }
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
