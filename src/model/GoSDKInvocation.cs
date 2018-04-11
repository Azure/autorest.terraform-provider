using System.Linq;
using static AutoRest.Terraform.TfProviderMetadata;

namespace AutoRest.Terraform
{
    public enum InvocationCategory
    {
        Creation, Read, Update, Deletion
    }

    public class GoSDKInvocation
    {
        public GoSDKInvocation(MethodTf method, SchemaDefinition metadata, InvocationCategory category)
        {
            OriginalMethod = method;
            OriginalMetadata = metadata;
            Category = category;
        }

        public MethodTf OriginalMethod { get; }
        public SchemaDefinition OriginalMetadata { get; }

        public string MethodName => OriginalMethod.Name;
        public InvocationCategory Category { get; }
        public bool IsAsync => OriginalMethod.IsLongRunning;
        public GoSDKTypedData ArgumentsRoot { get; } = new GoSDKTypedData();
        public GoSDKTypedData ResponsesRoot { get; } = new GoSDKTypedData();

        public bool SkipResult => !ResponsesRoot.Traverse(TraverseType.PreOrder).Any();
    }
}
