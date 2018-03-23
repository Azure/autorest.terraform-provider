using AutoRest.Core.Model;
using AutoRest.Core.Utilities;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
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

            MethodMatchPatterns.Clear();
            AddMethodPattern(metadata.CreateMethods, model.CreateInvocations);
            AddMethodPattern(metadata.ReadMethods, model.ReadInvocations);
            AddMethodPattern(metadata.UpdateMethods, model.UpdateInvocations);
            AddMethodPattern(metadata.DeleteMethods, model.DeleteInvocations);

            Traverse(model);
        }

        private void AddMethodPattern(IEnumerable<MethodDefinition> metadata, IList<GoSDKInvocation> target)
            => metadata.ForEach(m => MethodMatchPatterns.Add((m.Path.AsPropertyPathRegex(), m.Schema, target)));


        private IList<(Regex Pattern, SchemaDefinition Metadata, IList<GoSDKInvocation> Target)> MethodMatchPatterns { get; } = new List<(Regex, SchemaDefinition, IList<GoSDKInvocation>)>();

        private void Traverse(CodeModel model) => model.Operations.ForEach(op => Traverse(op, model.Name));

        private void Traverse(MethodGroup group, string path)
        {
            path += ModelPathSeparator + group.Name;
            group.Methods.ForEach(m => Traverse(m, path));
        }

        private void Traverse(Method method, string path)
        {
            path += ModelPathSeparator + method.Name;
            (from mm in MethodMatchPatterns
             where mm.Pattern.IsMatch(path)
             select (mm.Metadata, mm.Target)).ForEach(mt => mt.Target.Add(new GoSDKInvocation(method, mt.Metadata)));
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
