using AutoRest.Core.Model;
using AutoRest.Core.Utilities;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using static AutoRest.Core.Utilities.DependencyInjection;
using static AutoRest.Terraform.Utilities;

namespace AutoRest.Terraform
{
    public class TfProviderField
        : ITreeNode<TfProviderField>
    {
        private const string RootFieldName = "_ROOT_";

        public TfProviderField()
            : this(null, RootFieldName)
        {
            GoType = new GoSDKTypeChain(GoSDKTerminalTypes.Complex);
        }

        private TfProviderField(TfProviderField parent, string name)
        {
            Debug.Assert(!string.IsNullOrWhiteSpace(name));
            Name = name;
            Parent = parent;
        }

        public IVariable OriginalVariable { get; set; }

        public string Name { get; set; }
        public string PropertyPath => IsRoot ? string.Empty : JoinPathStrings(Parent.PropertyPath, Name);
        public GoSDKTypeChain GoType { get; private set; }
        public IEnumerable<TfProviderField> SubFields => from f in subFields.Values
                                                         orderby f.Name
                                                         select f;
        public bool IsRoot => Parent == null;
        public bool IsRequired => OriginalVariable?.IsRequired ?? SubFields.Any(sf => sf.IsRequired);
        public string DefaultValue => OriginalVariable.DefaultValue;
        public bool MightBeEmpty => !IsRequired && string.IsNullOrEmpty(DefaultValue);

        public TfProviderField Parent { get; }
        public IEnumerable<TfProviderField> Children => SubFields;

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
            name = CodeNamer.GetAzureRmSchemaName(name);
            if (!subFields.TryGetValue(name, out var field))
            {
                field = AddField(name);
            }
            return field.LocateOrAdd(paths.Skip(1));
        }

        public TfProviderField Remove() => Parent.RemoveField(Name);

        public void AddUsedBy(GoSDKTypedData data) => usedBy.Add(data);
        public void RemoveUsedBy(GoSDKTypedData data) => usedBy.Remove(data);
        public void AddUpdatedBy(GoSDKTypedData data) => updatedBy.Add(data);
        public void RemoveUpdatedBy(GoSDKTypedData data) => updatedBy.Remove(data);


        public override string ToString() => $"{Name}: [{GoType}]";


        private CodeNamerTf CodeNamer => Singleton<CodeNamerTf>.Instance;

        private TfProviderField AddField(string name)
        {
            Debug.Assert(!string.IsNullOrWhiteSpace(name));
            var field = new TfProviderField(this, name);
            subFields.Add(name, field);
            return field;
        }

        private TfProviderField RemoveField(string fieldName)
        {
            if (subFields.TryGetValue(fieldName, out var toRemove))
            {
                toRemove.usedBy.ToList().ForEach(d => d.UpdateBackingField(null, false));
                toRemove.updatedBy.ToList().ForEach(d => d.UpdateBackingField(null, true));
                subFields.Remove(fieldName);
                return toRemove;
            }
            return null;
        }

        private readonly IDictionary<string, TfProviderField> subFields = new Dictionary<string, TfProviderField>();
        private readonly ISet<GoSDKTypedData> usedBy = new HashSet<GoSDKTypedData>(), updatedBy = new HashSet<GoSDKTypedData>();
    }
}
