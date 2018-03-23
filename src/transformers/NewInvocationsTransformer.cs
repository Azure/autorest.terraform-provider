using AutoRest.Core.Model;
using System.Collections.Generic;
using System.Linq;
using static AutoRest.Core.Utilities.DependencyInjection;
using static AutoRest.Terraform.TfProviderMetadata;
using static AutoRest.Terraform.Utilities;

namespace AutoRest.Terraform
{
    /// <summary>
    /// Set <see cref="CodeModelTf.CreateInvocations"/>, <see cref="CodeModelTf.ReadInvocations"/>, <see cref="CodeModelTf.UpdateInvocations"/>
    /// and <see cref="CodeModelTf.DeleteInvocations"/> based on the filtering information provided in <see cref="MethodDefinition"/>.
    /// </summary>
    internal class NewInvocationsTransformer
        : ITfProviderTransformer
    {
        public void Transform(CodeModelTf model)
        {
            var metadata = Singleton<SettingsTf>.Instance.Metadata;
            model.CreateInvocations.AddRange(FilterByPath(model, metadata.CreateMethods));
            model.ReadInvocations.AddRange(FilterByPath(model, metadata.ReadMethods));
            model.UpdateInvocations.AddRange(FilterByPath(model, metadata.UpdateMethods));
            model.DeleteInvocations.AddRange(FilterByPath(model, metadata.DeleteMethods));
        }

        private IEnumerable<GoSDKInvocation> FilterByPath(CodeModel model, IEnumerable<MethodDefinition> metadata)
        {
            return from def in metadata
                   let pattern = def.Path.AsPropertyPathRegex()
                   from op in model.Operations
                   from m in op.Methods
                   let path = string.Join(ModelPathSeparator, model.Name, op.Name, m.Name)
                   where pattern.IsMatch(path)
                   select new GoSDKInvocation(m, def.Schema);
        }

        /*
        private void Traverse(Parameter parameter, IList<GoSDKTypedData> arguments, string subPath)
        {
            Debug.Assert(parameter != null && arguments != null && !string.IsNullOrEmpty(subPath));
            subPath += ModelAttributeStart + parameter.Location + ModelAttributeEnd;
            subPath += ModelPathSeparator + parameter.GetClientName();
            Traverse(parameter.ModelType, invocation, subPath);
        }

        private void Traverse(Response response, HttpStatusCode status, GoSDKInvocation invocation, string subPath)
        {
            Debug.Assert(response != null && invocation != null && !string.IsNullOrEmpty(subPath));
            response.Headers?.Apply(t => Traverse(t, invocation, subPath + ResponseHeaderAttribute));
            response.Body?.Apply(t => Traverse(t, invocation, subPath + ResponseBodyAttribute));
        }

        private void Traverse(Property property, GoSDKInvocation invocation, string subPath)
        {
            Debug.Assert(property != null && invocation != null && !string.IsNullOrEmpty(subPath));
            subPath += ModelPathSeparator + property.GetClientName();
        }

        private void Traverse(CompositeType composite, GoSDKInvocation invocation, string subPath)
        {
            Debug.Assert(composite != null && invocation != null && !string.IsNullOrEmpty(subPath));
            composite.ComposedProperties.ForEach(p => Traverse(p, invocation, subPath));
        }

        private void Traverse(IModelType type, GoSDKInvocation invocation, string subPath)
        {
            Debug.Assert(type != null && invocation != null && !string.IsNullOrEmpty(subPath));
            switch (type)
            {
                case PrimaryType p:
                case EnumType e:
                    break;
                case CompositeType composite:
                    Traverse(composite, invocation, subPath);
                    break;
                case SequenceType sequence:
                    Traverse(sequence.ElementType, invocation, subPath);
                    break;
                case DictionaryType dictionary:
                    Traverse(dictionary.ValueType, invocation, subPath);
                    break;
                default:
                    throw new NotSupportedException($"Model type \"{type}\" is not supported");
            }
        }
        */
    }
}
