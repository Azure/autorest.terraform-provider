using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AutoRest.Terraform
{
    /// <summary>
    /// Set <see cref="CodeModelTf.TfProviderFields"/> from the arguments cloned in <see cref="ArgumentsFilterCloneTransformer"/>.
    /// It also applies the flattening information defined in <see cref="SchemaDefinition.Flattens"/>.
    /// </summary>
    internal class NewFlattenFieldsTransformer
        : ITfProviderTransformer
    {
        public void Transform(CodeModelTf model)
        {
            model.CreateInvocations.ForEach(invn => FlattenFields(invn, model));
            model.ReadInvocations.ForEach(invn => FlattenFields(invn, model));
            model.UpdateInvocations.ForEach(invn => FlattenFields(invn, model));
            model.DeleteInvocations.ForEach(invn => FlattenFields(invn, model));
        }

        private List<(uint Priority, Regex Pattern, TfProviderField Target)> FlattenRules { get; } = new List<(uint, Regex, TfProviderField)>();

        private void FlattenFields(GoSDKInvocation invocation, CodeModelTf model)
        {
            FlattenRules.Clear();
            FlattenRules.AddRange(from r in invocation.OriginalMetadata.Flattens
                                  let p = r.SourcePath.ToPropertyPathRegex()
                                  let path = r.TargetPath.SplitPathStrings()
                                  let t = model.RootField.LocateOrAdd(path)
                                  select ((uint)r.Priority, p, t));
            Walk(invocation.Arguments, model, false);
            Walk(invocation.Responses, model, true);
        }

        private void Walk(IList<GoSDKTypedData> parent, CodeModelTf model, bool isInResponse)
        {
            foreach (var node in parent)
            {
                var (Priority, Target) = (from r in FlattenRules
                                          where r.Pattern.IsMatch(node.PropertyPath)
                                          orderby r.Priority descending
                                          select (r.Priority, r.Target)).FirstOrDefault();
                var target = Target ?? model.RootField.LocateOrAdd(node.PropertyPath.SplitPathStrings().SkipLast(1));
                var field = target.LocateOrAdd(node.Name);
                field.EnsureType(node.GoType);
                field.OriginalVariable = node.OriginalVariable;
                node.UpdateBackingField(field, isInResponse);
                if (node.GoType.Chain.Any() && node.GoType.Terminal == GoSDKTerminalTypes.Complex)
                {
                    FlattenRules.Add((Priority + 1, node.PropertyPath.AppendAnyChildrenPath().ToPropertyPathRegex(), field));
                }
                Walk(node.Properties, model, isInResponse);
            }
        }
    }
}
