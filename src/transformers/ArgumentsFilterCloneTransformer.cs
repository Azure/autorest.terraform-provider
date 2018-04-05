using AutoRest.Core.Model;
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
                           let ex = invn.OriginalMetadata.Excludes.Select(Utilities.ToPropertyPathRegex)
                           let pdata = from p in invn.OriginalMethod.LogicalParameters
                                       let path = p.ToPathString()
                                       where ex.All(x => !x.IsMatch(path))
                                       select Clone(invn, path, p.ModelType, ex, p)
                           let rhdata = from rp in invn.OriginalMethod.Responses
                                        let path = rp.ToPathString(true)
                                        where rp.Value.Headers != null && ex.All(x => !x.IsMatch(path))
                                        select Clone(invn, path, rp.Value.Headers, ex)
                           let rbdata = from rp in invn.OriginalMethod.Responses
                                        let path = rp.ToPathString(false)
                                        where rp.Value.Body != null && ex.All(x => !x.IsMatch(path))
                                        select Clone(invn, path, rp.Value.Body, ex)
                           select (invn.ArgumentsRoot, invn.ResponsesRoot, ArgChildren: pdata, RespChildren: rhdata.Concat(rbdata));
            foreach (var (argRoot, respRoot, argChildren, respChildren) in argsData)
            {
                argRoot.AddProperties(argChildren);
                respRoot.AddProperties(respChildren);
            }
        }

        private GoSDKTypedData Clone(GoSDKInvocation invocation, string path, IModelType type, IEnumerable<Regex> excludes, IVariable variable = null)
        {
            var goData = new GoSDKTypedData(invocation, path, new GoSDKTypeChain(type), variable);
            if (goData.GoType.Terminal == GoSDKTerminalTypes.Complex)
            {
                var children = from p in ((CompositeType)goData.GoType.OriginalTerminalType).ComposedProperties
                               let subpath = p.ToPathString(path)
                               where excludes.All(x => !x.IsMatch(subpath))
                               select Clone(invocation, subpath, p.ModelType, excludes, p);
                goData.AddProperties(children);
            }
            return goData;
        }
    }
}
