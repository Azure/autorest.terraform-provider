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
        public static void AppendSpecDisplayString(this CodeModel model, IndentedStringBuilder builder)
        {
            builder.AppendLine($"{model.Qualifier} \"{model.Name}\"; Operations [{model.Operations.Count}]");
            builder.Indent();
            model.Operations.ForEach(op => op.AppendDisplayString(builder));
            builder.Outdent();
        }

        private static void AppendDisplayString(this MethodGroup group, IndentedStringBuilder builder)
        {
            builder.AppendLine($"{group.Qualifier} \"{group.Name}\"; Type: {group.TypeName}; PropName: {group.NameForProperty}; Methods [{group.Methods.Count}]");
            builder.Indent();
            group.Methods.ForEach(m => m.AppendDisplayString(builder));
            builder.Outdent();
        }

        private static void AppendDisplayString(this Method method, IndentedStringBuilder builder)
        {
            var displayedTypes = new HashSet<string>();

            builder.AppendLine($"{method.Flavor} {method.Qualifier} \"{method.Name}\"; Transformations [{method.InputParameterTransformation.Count}]; Parameters [{method.Parameters.Count}]; Responses [{method.Responses.Count}]");
            builder.Indent();
            method.Parameters.ForEach(p => p.AppendDisplayString(builder, displayedTypes));
            builder.Outdent();

            builder.Indent();
            method.Responses.ForEach(r => r.Value.AppendDisplayString(builder, r.Key, displayedTypes));
            builder.Outdent();
        }

        private static void AppendDisplayString(this Parameter parameter, IndentedStringBuilder builder, ISet<string> displayedTypes)
        {
            builder.AppendLine($"{parameter.Location} {parameter.Qualifier} \"{parameter.GetClientName()}\"; " +
                $"Type: {parameter.ModelType.ToSummaryString()}; {(parameter.IsRequired ? "Required" : "Optional")}");
            builder.Indent();
            parameter.ModelType.AppendDisplayString(builder, displayedTypes);
            builder.Outdent();
        }

        private static void AppendDisplayString(this Response response, IndentedStringBuilder builder, HttpStatusCode status, ISet<string> displayedTypes)
        {
            var name = $"HTTP {status} ({(int)status})";
            builder.AppendLine($"Response \"HTTP {status} ({(int)status})\" Header: {response.Headers?.ToSummaryString() ?? "None"}, Body: {response.Body?.ToSummaryString() ?? "None"}");
            builder.Indent();
            response.Headers?.AppendDisplayString(builder, displayedTypes);
            response.Body?.AppendDisplayString(builder, displayedTypes);
            builder.Outdent();
        }

        private static void AppendDisplayString(this CompositeType composite, IndentedStringBuilder builder, ISet<string> displayedTypes)
        {
            var propertiesSet = new HashSet<Property>(composite.Properties);
            composite.Properties.ForEach(p => p.AppendDisplayString(builder, displayedTypes));
            composite.ComposedProperties.Where(p => !propertiesSet.Contains(p)).ForEach(p => p.AppendDisplayString(builder, displayedTypes, true));
        }

        private static void AppendDisplayString(this Property property, IndentedStringBuilder builder, ISet<string> displayedTypes, bool isComposed = false)
        {
            builder.AppendLine($"{(isComposed ? "Composed " : string.Empty)}{property.Qualifier} \"{property.GetClientName()}\"; " +
                $"Type: {property.ModelType.ToSummaryString()}; {(property.IsRequired ? "Required" : "Optional")}");
            builder.Indent();
            property.ModelType.AppendDisplayString(builder, displayedTypes);
            builder.Outdent();
        }

        private static string ToSummaryString(this IModelType type)
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

        private static void AppendDisplayString(this IModelType type, IndentedStringBuilder builder, ISet<string> displayedTypes)
        {
            if (!displayedTypes.Contains(type.Name))
            {
                displayedTypes.Add(type.Name);
                switch (type)
                {
                    case PrimaryType p:
                    case EnumType e:
                        break;
                    case CompositeType composite:
                        composite.AppendDisplayString(builder, displayedTypes);
                        break;
                    case SequenceType sequence:
                        sequence.ElementType.AppendDisplayString(builder, displayedTypes);
                        break;
                    case DictionaryType dictionary:
                        dictionary.ValueType.AppendDisplayString(builder, displayedTypes);
                        break;
                    default:
                        builder.AppendLine("UNKNOWN TYPE");
                        break;
                }
            }
        }
    }
}
