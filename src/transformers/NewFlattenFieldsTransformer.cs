﻿using AutoRest.Core.Utilities;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using static AutoRest.Terraform.Utilities;

namespace AutoRest.Terraform
{
    /// <summary>
    /// Set <see cref="CodeModelTf.TfProviderFields"/> from the arguments cloned in <see cref="ArgumentsFilterCloneTransformer"/>.
    /// It also applies the flattening information defined in <see cref="SchemaDefinition.Flattens"/>.
    /// </summary>
    internal class NewFlattenFieldsTransformer
        : TfProviderTransformerBase
    {
        protected override void TransformCore()
        {
            CodeModel.CreateInvocations.ForEach(FlattenFields);
            CodeModel.ReadInvocations.ForEach(FlattenFields);
            CodeModel.UpdateInvocations.ForEach(FlattenFields);
            CodeModel.DeleteInvocations.ForEach(FlattenFields);
        }

        private Stack<(Regex Pattern, TfProviderField ScopedField)> ScopeRules { get; } = new Stack<(Regex, TfProviderField)>();
        private SortedList<uint, (Regex Pattern, TfProviderField Target, string NewName)> FlattenRules { get; } = new SortedList<uint, (Regex, TfProviderField, string)>();

        private void FlattenFields(GoSDKInvocation invocation)
        {
            ScopeRules.Clear();
            ScopeRules.Push((AnyPathExtension.ToPropertyPathRegex(), CodeModel.RootField));
            FlattenRules.Clear();
            var rulesDefinitions = from r in invocation.OriginalMetadata.Schema.Flattens
                                   let p = r.SourcePath.ToPropertyPathRegex()
                                   let path = r.TargetPath.SplitPathStrings()
                                   let isFolder = r.TargetPath.EndsWith(ModelPathSeparator)
                                   let f = CodeModel.RootField.LocateOrAdd(isFolder ? path : path.SkipLast(1))
                                   select new
                                   {
                                       Priority = (uint)r.Priority,
                                       Pattern = p,
                                       RootField = f,
                                       NewName = isFolder ? null : path.Last()
                                   };
            rulesDefinitions.ForEach(rd => FlattenRules.Add(rd.Priority, (rd.Pattern, rd.RootField, rd.NewName)));
            FlattenTree(invocation.ArgumentsRoot, false);
            FlattenTree(invocation.ResponsesRoot, true);
        }

        private void FlattenTree(GoSDKTypedData root, bool isInResponse)
        {
            foreach (var node in root.Traverse(TraverseType.PreOrder))
            {
                var (target, name) = (from r in FlattenRules
                                      where r.Value.Pattern.IsMatch(node.PropertyPath)
                                      select (r.Value.Target, r.Value.NewName)).FirstOrDefault();
                if (target == null)
                {
                    target = CodeModel.RootField.LocateOrAdd(node.PropertyPath.SplitPathStrings().SkipLast(1));
                }

                var scopedTarget = (from r in ScopeRules
                                    where r.Pattern.IsMatch(node.PropertyPath)
                                    select r.ScopedField).First();
                if (!target.Traverse(TraverseType.Ancestors).Contains(scopedTarget))
                {
                    if (scopedTarget.Traverse(TraverseType.Ancestors).Contains(target))
                    {
                        target = scopedTarget;
                    }
                    else
                    {
                        throw new SchemaFieldOutOfScopeException($"{node.PropertyPath} should be scoped within {scopedTarget.PropertyPath}");
                    }
                }

                var field = target.LocateOrAdd(name ?? node.Name);
                field.EnsureType(node.GoType);
                field.OriginalVariable = node.OriginalVariable;
                node.UpdateBackingField(field, isInResponse);

                if (node.GoType.Chain.Any() && node.GoType.Terminal == GoSDKTerminalTypes.Complex)
                {
                    ScopeRules.Push((node.PropertyPath.AppendAnyChildrenPath().ToPropertyPathRegex(), field));
                }
            }
        }
    }
}
