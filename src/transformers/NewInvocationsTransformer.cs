﻿using AutoRest.Core.Model;
using System.Collections.Generic;
using System.Linq;
using static AutoRest.Core.Utilities.DependencyInjection;
using static AutoRest.Terraform.TfProviderMetadata;
using static AutoRest.Terraform.Utilities;

namespace AutoRest.Terraform
{
    /// <summary>
    /// Set <see cref="CodeModelTf.CreateInvocations"/>, <see cref="CodeModelTf.ReadInvocations"/>, <see cref="CodeModelTf.UpdateInvocations"/>
    /// and <see cref="CodeModelTf.DeleteInvocations"/> based on the filtering information provided in <see cref="SchemaDefinition.Excludes"/>.
    /// </summary>
    internal class NewInvocationsTransformer
        : ITfProviderTransformer
    {
        public void Transform(CodeModelTf model)
        {
            var metadata = Singleton<SettingsTf>.Instance.Metadata;
            model.CreateInvocations.AddRange(FilterByPath(model, metadata.CreateMethods, InvocationCategory.Creation));
            model.ReadInvocations.AddRange(FilterByPath(model, metadata.ReadMethods, InvocationCategory.Read));
            model.UpdateInvocations.AddRange(FilterByPath(model, metadata.UpdateMethods, InvocationCategory.Update));
            model.DeleteInvocations.AddRange(FilterByPath(model, metadata.DeleteMethods, InvocationCategory.Deletion));
        }

        private IEnumerable<GoSDKInvocation> FilterByPath(CodeModel model, IEnumerable<MethodDefinition> metadata, InvocationCategory category)
        {
            return from def in metadata
                   let pattern = def.Path.ToPropertyPathRegex()
                   from op in model.Operations
                   from m in op.Methods
                   let path = JoinPathStrings(model.Name, op.Name, m.Name)
                   where pattern.IsMatch(path)
                   select new GoSDKInvocation(m, def.Schema, category);
        }
    }
}
