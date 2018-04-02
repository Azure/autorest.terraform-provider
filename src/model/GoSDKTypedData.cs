using AutoRest.Core.Model;
using System.Collections.Generic;
using System.Diagnostics;

namespace AutoRest.Terraform
{
    public class GoSDKTypedData
    {
        public GoSDKTypedData(GoSDKInvocation invocation, string path, GoSDKTypeChain type, IVariable variable = null)
        {
            Debug.Assert(invocation != null);
            Debug.Assert(!string.IsNullOrEmpty(path) && type != null);
            Invocation = invocation;
            PropertyPath = path;
            Name = path.ExtractLastPath();
            GoType = type;
            OriginalVariable = variable;
        }

        public IVariable OriginalVariable { get; }

        public string Name { get; }
        public string PropertyPath { get; }
        public TfProviderField BackingField { get; private set; }
        public GoSDKTypeChain GoType { get; }
        public List<GoSDKTypedData> Properties { get; } = new List<GoSDKTypedData>();

        private GoSDKInvocation Invocation { get; }

        public void UpdateBackingField(TfProviderField field, bool isSet)
        {
            BackingField = field;
            if (isSet)
            {
                BackingField.AddUpdatedBy(Invocation);
            }
            else
            {
                BackingField.AddUsedBy(Invocation);
            }
        }
    }
}
