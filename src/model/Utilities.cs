using AutoRest.Core.Model;
using AutoRest.Core.Utilities;
using System.Linq;
using System.Net;

namespace AutoRest.Terraform
{
    internal static partial class Utilities
    {
        public static string ToSummaryString(this IModelType type)
        {
            if (type == null)
            {
                return "NONE";
            }
            switch (type)
            {
                case EnumType @enum:
                    return $"{@enum.Name} [{string.Join(",", @enum.Values.Select(v => v.Name))}]";
                case CompositeTypeTf composite:
                    return $"{composite.Name} {{Properties [{composite.Properties.Count}], ComposedProperties [{composite.ComposedProperties.Count()}]}}";
                case SequenceType sequence:
                    return $"Array<{sequence.ElementType.ToSummaryString()}>";
                case DictionaryType dictionary:
                    return $"StringMap<{dictionary.ValueType.ToSummaryString()}>";
                default:
                    return type.Name;
            }
        }

        public static void AppendToDisplayString(this IModelType type, IndentedStringBuilder builder)
        {
            if (type != null)
            {
                switch (type)
                {
                    case PrimaryType p:
                    case EnumType e:
                        break;
                    case CompositeTypeTf composite:
                        composite.AppendToDisplayString(builder);
                        break;
                    case SequenceType sequence:
                        sequence.ElementType.AppendToDisplayString(builder);
                        break;
                    case DictionaryType dictionary:
                        dictionary.ValueType.AppendToDisplayString(builder);
                        break;
                    default:
                        builder.AppendLine("UNKNOWN TYPE");
                        break;
                }
            }
        }

        public static void AppendToDisplayString(this Response response, IndentedStringBuilder builder, HttpStatusCode status)
        {
            var name = $"HTTP {status} ({(int)status})";
            builder.AppendLine($"Response \"{name}\" Header: {response.Headers.ToSummaryString()}");
            builder.Indent();
            response.Headers.AppendToDisplayString(builder);
            builder.Outdent();

            builder.AppendLine($"Response \"{name}\" Body: {response.Body.ToSummaryString()}");
            builder.Indent();
            response.Body.AppendToDisplayString(builder);
            builder.Outdent();
        }
    }
}
