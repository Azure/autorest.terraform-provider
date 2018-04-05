using AutoRest.Core.Model;
using System.Collections.Generic;
using static AutoRest.Terraform.TfProviderMetadata;

namespace AutoRest.Terraform
{
    public enum InvocationCategory
    {
        Creation, Read, Update, Deletion
    }

    public class GoSDKInvocation
    {
        public GoSDKInvocation(Method method, SchemaDefinition metadata, InvocationCategory category)
        {
            OriginalMethod = method;
            OriginalMetadata = metadata;
            Category = category;
        }

        public Method OriginalMethod { get; }
        public SchemaDefinition OriginalMetadata { get; }

        public string MethodName => OriginalMethod.Name;
        public InvocationCategory Category { get; }
        public GoSDKTypedData ArgumentsRoot { get; } = new GoSDKTypedData();
        public GoSDKTypedData ResponsesRoot { get; } = new GoSDKTypedData();
    }
}
