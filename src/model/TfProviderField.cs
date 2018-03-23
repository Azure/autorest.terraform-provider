using System;
using System.Collections.Generic;
using System.Text;

namespace AutoRest.Terraform
{
    internal class TfProviderField
    {
        public string Name { get; }

        public GoSDKTypeChain GoType { get; }

        public IList<TfProviderField> SubFields { get; } = new List<TfProviderField>();
    }
}
