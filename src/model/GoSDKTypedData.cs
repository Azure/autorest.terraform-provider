using AutoRest.Core.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Text;

namespace AutoRest.Terraform
{
    internal class GoSDKTypedData
    {
        public GoSDKTypedData(string path, GoSDKTypeChain type)
        {
            Debug.Assert(!string.IsNullOrEmpty(path) && type != null);
            PropertyPath = path;
            Name = path.ExtractLastPath();
            GoType = type;
        }

        public string Name { get; }
        public string PropertyPath { get; }
        public TfProviderField BackingField { get; set; }
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
        public GoSDKTypeChain(IModelType type)
        {
        }

        private List<GoSDKNonTerminalTypes> Chain { get; } = new List<GoSDKNonTerminalTypes>();

        private GoSDKTerminalTypes Terminal { get; }

        public List<GoSDKTypedData> Properties { get; } = new List<GoSDKTypedData>();
    }
}
