﻿using AutoRest.Core.Model;
using AutoRest.Core.Utilities;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AutoRest.Terraform
{
    /// <summary>
    /// Clone the <see cref="Parameter"/>s and <see cref="Response"/>s in a <see cref="Method"/> to the corresponding <see cref="GoSDKInvocation.Arguments"/>.
    /// This transformer also removes all items matching <see cref="SchemaDefinition.Excludes"/>.
    /// </summary>
    internal class ArgumentsFilterCloneTransformer
        : ITfProviderTransformer
    {
        public void Transform(CodeModelTf model)
        {
            FilterCloneParametersAndResponses(model.CreateInvocations);
            FilterCloneParametersAndResponses(model.ReadInvocations);
            FilterCloneParametersAndResponses(model.UpdateInvocations);
            FilterCloneParametersAndResponses(model.DeleteInvocations);
        }

        private void FilterCloneParametersAndResponses(IEnumerable<GoSDKInvocation> invocations)
        {
            var argsData = from invn in invocations
                           let ex = invn.OriginalMetadata.Excludes.Select(x => x.AsPropertyPathRegex())
                           let pdata = from p in invn.OriginalMethod.LogicalParameters
                                       let path = p.ToPathString()
                                       where ex.All(x => !x.IsMatch(path))
                                       select Clone(path, p.ModelType, ex)
                           let rhdata = from rp in invn.OriginalMethod.Responses
                                        let path = rp.ToPathString(true)
                                        where rp.Value.Headers != null && ex.All(x => !x.IsMatch(path))
                                        select Clone(path, rp.Value.Headers, ex)
                           let rbdata = from rp in invn.OriginalMethod.Responses
                                        let path = rp.ToPathString(false)
                                        where rp.Value.Body != null && ex.All(x => !x.IsMatch(path))
                                        select Clone(path, rp.Value.Body, ex)
                           select (invn.Arguments, Children: pdata.Concat(rhdata).Concat(rbdata));
            argsData.ForEach(ac => ac.Arguments.AddRange(ac.Children));
        }

        private GoSDKTypedData Clone(string path, IModelType type, IEnumerable<Regex> excludes)
        {
            var goType = new GoSDKTypeChain(type);
            if (type is CompositeType composite)
            {
                var children = from p in composite.ComposedProperties
                               let subpath = p.ToPathString(path)
                               where excludes.All(x => !x.IsMatch(subpath))
                               select Clone(subpath, p.ModelType, excludes);
                goType.Properties.AddRange(children);
            }
            return new GoSDKTypedData(path, goType);
        }
    }
}
