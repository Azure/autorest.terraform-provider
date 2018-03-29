using AutoRest.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoRest.Terraform
{
    internal static partial class Utilities
    {
        public static void AppendSchemaDisplayString(this CodeModelTf model, IndentedStringBuilder builder) => model.RootField.AppendDisplayString(builder);

        private static void AppendDisplayString(this TfProviderField field, IndentedStringBuilder builder)
        {
            builder.AppendLine(field.ToString());
            builder.Indent();
            field.SubFields.ForEach(f => f.AppendDisplayString(builder));
            builder.Outdent();
        }

        public static void AppendInvocationsDisplayString(this CodeModelTf model, IndentedStringBuilder builder)
        {
        }
    }
}
