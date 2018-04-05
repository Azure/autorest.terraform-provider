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
            void DisplayInvocation(string title, List<GoSDKInvocation> invocations)
            {
                builder.AppendLine(title + " [{0}]", invocations.Count);
                builder.Indent();
                invocations.ForEach(invn => invn.AppendDisplayString(builder));
                builder.Outdent();
            }

            DisplayInvocation("Creation Invocations", model.CreateInvocations);
            DisplayInvocation("Read Invocations", model.ReadInvocations);
            DisplayInvocation("Update Invocations", model.UpdateInvocations);
            DisplayInvocation("Deletion Invocations", model.DeleteInvocations);
        }

        private static void AppendDisplayString(this GoSDKInvocation invocation, IndentedStringBuilder builder)
        {
            builder.AppendLine("{0} invocation \"{1}\"", invocation.Category, invocation.MethodName);
            invocation.ArgumentsRoot.AppendDisplayString(builder, false);
            invocation.ResponsesRoot.AppendDisplayString(builder, true);
        }

        private static void AppendDisplayString(this GoSDKTypedData argument, IndentedStringBuilder builder, bool isResponse)
        {
            builder.AppendLine("\"{0}\": [{1}] {3} \"{2}\"", argument.Name, argument.GoType, argument.BackingField?.PropertyPath, isResponse ? "->" : "<-");
            builder.Indent();
            argument.Properties.ForEach(prop => prop.AppendDisplayString(builder, isResponse));
            builder.Outdent();
        }
    }
}
