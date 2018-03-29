using System.Collections.Generic;
using System.Diagnostics;

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
        public List<GoSDKTypedData> Properties { get; } = new List<GoSDKTypedData>();
    }
}
