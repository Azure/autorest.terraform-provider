using AutoRest.Core;
using Humanizer;
using System.Collections.Generic;
using System.Linq;
using static AutoRest.Terraform.Utilities;

namespace AutoRest.Terraform
{
    public class CodeNamerTf
        : CodeNamer
    {
        public override string GetFieldName(string name) => GetEscapedReservedName(TitleUnderscoreCase(name), "_field").Camelize();

        public virtual string ExtractAliasFromGoPackage(string package) => package.Substring(package.LastIndexOf('/') + 1);
        public virtual string GetGoPrivateMethodName(string name) => GetMethodName(name).Camelize();
        public virtual string GetGoLocalVariableName(string name) => GetVariableName(name).Camelize();

        public virtual string GetAzureGoSDKClientName(string name) => GetFieldName(JoinNonEmpty(name, "Client"));
        public virtual string GetAzureGoSDKIdPathName(string name) => name.ToLowerInvariant();

        public virtual string GetAzureRmResourceName(string name) => JoinNonEmpty("Resource Arm", name);
        public virtual string GetAzureRmSchemaName(string name) => name.Underscore();

        public virtual string GetAzureRmPropPathLocalVarName(TfProviderField field) => GetAzureRmPropPathLocalVarName(field.PropertyPath.SplitPathStrings());
        public virtual string GetAzureRmPropPathLocalVarName(GoSDKTypedData data) => GetAzureRmPropPathLocalVarName(data.PropertyPath.SplitPathStrings().Skip(1));
        protected virtual string GetAzureRmPropPathLocalVarName(IEnumerable<string> paths) => GetGoLocalVariableName(string.Join(' ', paths));
        public virtual string GetAzureRmPropPathLocalVarBaseName(TfProviderField field) => GetAzureRmPropPathLocalVarBaseName(field.PropertyPath.SplitPathStrings());
        public virtual string GetAzureRmPropPathLocalVarBaseName(GoSDKTypedData data) => GetAzureRmPropPathLocalVarBaseName(data.PropertyPath.SplitPathStrings().Skip(1));
        protected virtual string GetAzureRmPropPathLocalVarBaseName(IEnumerable<string> paths) => GetAzureRmPropPathLocalVarName(paths).Pascalize();

        public virtual string GetResourceFileName(string name) => TitleUnderscoreCase(GetAzureRmResourceName(name));


        public string JoinNonEmpty(params string[] items) => JoinNonEmpty((IEnumerable<string>)items);
        public virtual string JoinNonEmpty(IEnumerable<string> items) => string.Join(" ", items.Where(i => !string.IsNullOrEmpty(i)));
        protected virtual string TitleUnderscoreCase(string name) => name.ToLowerInvariant().Titleize().Underscore();
    }
}
