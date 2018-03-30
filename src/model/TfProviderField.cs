using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using static AutoRest.Core.Utilities.DependencyInjection;
using static AutoRest.Terraform.Utilities;

namespace AutoRest.Terraform
{
    internal class TfProviderField
    {
        private const string RootFieldName = "_ROOT_";

        public TfProviderField()
            : this(null, RootFieldName)
        {
        }

        private TfProviderField(TfProviderField parent, string name)
        {
            Debug.Assert(!string.IsNullOrWhiteSpace(name));
            Name = name;
            Parent = parent;
        }

        public string Name { get; set; }
        public string PropertyPath => Parent == null ? string.Empty : JoinPathStrings(Parent.PropertyPath, Name);
        public GoSDKTypeChain GoType { get; private set; }
        public IEnumerable<TfProviderField> SubFields => subFields;

        public void EnsureType(GoSDKTypeChain type)
        {
            Debug.Assert(type != null);
            if (GoType == null)
            {
                GoType = type;
            }
            else if (GoType != type)
            {
                throw new TypeIncompatibleException($"[{type}] is not compatible with the existing type [{GoType}]");
            }
        }

        public TfProviderField LocateOrAdd(params string[] paths) => LocateOrAdd((IEnumerable<string>)paths);

        public TfProviderField LocateOrAdd(IEnumerable<string> paths)
        {
            var name = paths.FirstOrDefault();
            if (name == null)
            {
                return this;
            }
            name = CodeNamer.GetResourceSchemaPropertyName(name);
            if (!subFieldLookup.TryGetValue(name, out int index))
            {
                index = AddField(new TfProviderField(this, name));
            }
            return subFields[index].LocateOrAdd(paths.Skip(1));
        }

        public void AddUsedBy(GoSDKInvocation invocation) => usedBy.Add(invocation);

        public void AddUpdatedBy(GoSDKInvocation invocation) => updatedBy.Add(invocation);


        public override string ToString() => $"{Name}: [{GoType}]";


        private TfProviderField Parent { get; }
        private CodeNamerTf CodeNamer => Singleton<CodeNamerTf>.Instance;

        private int AddField(TfProviderField field)
        {
            var index = subFields.Count;
            subFields.Add(field);
            subFieldLookup.Add(field.Name, index);
            return index;
        }

        private readonly List<TfProviderField> subFields = new List<TfProviderField>();
        private readonly Dictionary<string, int> subFieldLookup = new Dictionary<string, int>();
        private readonly ISet<GoSDKInvocation> usedBy = new HashSet<GoSDKInvocation>(), updatedBy = new HashSet<GoSDKInvocation>();
    }
}
