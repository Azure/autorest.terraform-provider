using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace AutoRest.Terraform
{
    internal class TfProviderField
    {
        private const string RootFieldName = "_ROOT_";

        public TfProviderField()
            : this(RootFieldName)
        {
        }

        private TfProviderField(string name)
        {
            Debug.Assert(!string.IsNullOrWhiteSpace(name));
            Name = name;
        }

        public string Name { get; }
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
            if (!subFieldLookup.TryGetValue(name, out int index))
            {
                index = AddField(new TfProviderField(name));
            }
            return subFields[index].LocateOrAdd(paths.Skip(1));
        }


        public override string ToString() => $"{Name}: [{GoType}]";


        private int AddField(TfProviderField field)
        {
            var index = subFields.Count;
            subFields.Add(field);
            subFieldLookup.Add(field.Name, index);
            return index;
        }

        private List<TfProviderField> subFields = new List<TfProviderField>();
        private Dictionary<string, int> subFieldLookup = new Dictionary<string, int>();
    }
}
