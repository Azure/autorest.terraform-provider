using AutoRest.Core.Model;
using AutoRest.Core.Utilities;
using AutoRest.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace AutoRest.Terraform
{
    internal static partial class Utilities
    {
        public static void AppendDisplayString(this CodeModel model, IndentedStringBuilder builder)
        {
            builder.AppendLine($"{model.Qualifier} \"{model.Name}\"; Operations [{model.Operations.Count}]");
            builder.Indent();
            model.Operations.ForEach(op => op.AppendDisplayString(builder));
            builder.Outdent();
        }

        public static void AppendDisplayString(this MethodGroup group, IndentedStringBuilder builder)
        {
            builder.AppendLine($"{group.Qualifier} \"{group.Name}\"; Type: {group.TypeName}; PropName: {group.NameForProperty}; Methods [{group.Methods.Count}]");
            builder.Indent();
            group.Methods.ForEach(m => m.AppendDisplayString(builder));
            builder.Outdent();
        }

        public static void AppendDisplayString(this Method method, IndentedStringBuilder builder)
        {
            builder.AppendLine($"{method.Flavor} {method.Qualifier} \"{method.Name}\"; Transformations [{method.InputParameterTransformation.Count}]; Parameters [{method.Parameters.Count}]; Responses [{method.Responses.Count}]");
            builder.Indent();
            method.Parameters.ForEach(p => p.AppendDisplayString(builder));
            builder.Outdent();

            builder.Indent();
            method.Responses.ForEach(r => r.Value.AppendDisplayString(builder, r.Key));
            builder.Outdent();
        }

        public static void AppendDisplayString(this Parameter parameter, IndentedStringBuilder builder)
        {
            builder.AppendLine($"{parameter.Location} {parameter.Qualifier} \"{parameter.GetClientName()}\"; Type: {parameter.ModelType.ToSummaryString()}");
            builder.Indent();
            parameter.ModelType.AppendDisplayString(builder);
            builder.Outdent();
        }

        public static void AppendDisplayString(this Response response, IndentedStringBuilder builder, HttpStatusCode status)
        {
            var name = $"HTTP {status} ({(int)status})";
            builder.AppendLine($"Response \"HTTP {status} ({(int)status})\" Header: {response.Headers?.ToSummaryString() ?? "None"}, Body: {response.Body?.ToSummaryString() ?? "None"}");
            builder.Indent();
            response.Headers?.AppendDisplayString(builder);
            response.Body?.AppendDisplayString(builder);
            builder.Outdent();
        }

        public static void AppendDisplayString(this CompositeType composite, IndentedStringBuilder builder)
        {
            var propertiesSet = new HashSet<Property>(composite.Properties);
            composite.Properties.ForEach(p => p.AppendDisplayString(builder));
            composite.ComposedProperties.Where(p => !propertiesSet.Contains(p)).ForEach(p => p.AppendDisplayString(builder, true));
        }

        public static void AppendDisplayString(this Property property, IndentedStringBuilder builder, bool isComposed = false)
        {
            builder.AppendLine($"{(isComposed ? "Composed " : string.Empty)}{property.Qualifier} \"{property.GetClientName()}\"; Type: {property.ModelType.ToSummaryString()}");
            builder.Indent();
            property.ModelType.AppendDisplayString(builder);
            builder.Outdent();
        }

        public static string ToSummaryString(this IModelType type)
        {
            switch (type)
            {
                case EnumType @enum:
                    return $"{@enum.Name} [{string.Join(",", @enum.Values.Select(v => v.Name))}]";
                case SequenceType sequence:
                    return $"Array<{sequence.ElementType.ToSummaryString()}>";
                case DictionaryType dictionary:
                    return $"StringMap<{dictionary.ValueType.ToSummaryString()}>";
                default:
                    return type.Name;
            }
        }

        public static void AppendDisplayString(this IModelType type, IndentedStringBuilder builder)
        {
            switch (type)
            {
                case PrimaryType p:
                case EnumType e:
                    break;
                case CompositeType composite:
                    composite.AppendDisplayString(builder);
                    break;
                case SequenceType sequence:
                    sequence.ElementType.AppendDisplayString(builder);
                    break;
                case DictionaryType dictionary:
                    dictionary.ValueType.AppendDisplayString(builder);
                    break;
                default:
                    builder.AppendLine("UNKNOWN TYPE");
                    break;
            }
        }
    }
}
