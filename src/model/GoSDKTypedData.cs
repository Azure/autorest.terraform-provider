using AutoRest.Core.Model;
using AutoRest.Core.Utilities;
using System.Collections.Generic;
using System.Diagnostics;

namespace AutoRest.Terraform
{
    public class GoSDKTypedData
        : ITreeNode<GoSDKTypedData>
    {
        private const string RootFieldName = "_ROOT_";

        public GoSDKTypedData()
        {
            Name = RootFieldName;
            PropertyPath = string.Empty;
        }

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
        public GoSDKTypeChain GenerateType => (OriginalVariable as PropertyTf)?.GenerateType;
        public IEnumerable<GoSDKTypedData> Properties => properties;

        public GoSDKTypedData Parent { get; private set; }
        public IEnumerable<GoSDKTypedData> Children => Properties;


        public GoSDKInvocation Invocation { get; }

        public void AddProperties(IEnumerable<GoSDKTypedData> props)
        {
            props.ForEach(p => p.Parent = this);
            properties.AddRange(props);
        }

        public void UpdateBackingField(TfProviderField field, bool isSet)
        {
            if (isSet)
            {
                BackingField?.RemoveUpdatedBy(this);
            }
            else
            {
                BackingField?.RemoveUsedBy(this);
            }
            BackingField = field;
            if (isSet)
            {
                BackingField?.AddUpdatedBy(this);
            }
            else
            {
                BackingField?.AddUsedBy(this);
            }
        }

        private readonly List<GoSDKTypedData> properties = new List<GoSDKTypedData>();
    }
}
