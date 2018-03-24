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
        public List<GoSDKTypedData> Arguments { get; } = new List<GoSDKTypedData>();
    }
}
