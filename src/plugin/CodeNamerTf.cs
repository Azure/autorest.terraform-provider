using AutoRest.Core;
using Humanizer;

namespace AutoRest.Terraform
{
    public class CodeNamerTf
        : CodeNamer
    {
        private const string FieldNameEscapedPostfix = " Field";

        private const string ResourcePrefix = "Resource Arm ";
        private const string ResourceTypeExpandMethodPrefix = "Expand Arm ";
        private const string ResourceTypeFlattenMethodPrefix = "Flatten Arm ";
        private const string ResourceCreatePostfix = " Create";
        private const string ResourceReadPostfix = " Read";
        private const string ResourceDeletePostfix = " Delete";

        private const string AzureGoSDKClientPostfix = " Client";


        public override string GetFieldName(string name) => GetEscapedReservedName(TitleUnderscoreCase(name), FieldNameEscapedPostfix).Camelize();

        public virtual string GetAzureGoSDKClientName(string name) => GetFieldName($"{name ?? string.Empty}{AzureGoSDKClientPostfix}");
        public virtual string GetAzureGoSDKIdPathName(string name) => (name ?? string.Empty).ToLowerInvariant();

        public virtual string GetResourceFileName(string name) => TitleUnderscoreCase($"{ResourcePrefix}{name ?? string.Empty}");
        public virtual string GetResourceDefinitionMethodName(string name) => GetGoPrivateMethodName($"{ResourcePrefix}{name ?? string.Empty}");
        public virtual string GetResourceCreateMethodName(string name) => GetGoPrivateMethodName($"{ResourcePrefix}{name ?? string.Empty}{ResourceCreatePostfix}");
        public virtual string GetResourceReadMethodName(string name) => GetGoPrivateMethodName($"{ResourcePrefix}{name ?? string.Empty}{ResourceReadPostfix}");
        public virtual string GetResourceDeleteMethodName(string name) => GetGoPrivateMethodName($"{ResourcePrefix}{name ?? string.Empty}{ResourceDeletePostfix}");
        public virtual string GetResourceSchemaPropertyName(string name) => (name ?? string.Empty).Underscore();
        public virtual string GetResourceTypeExpandMethodName(string name) => GetGoPrivateMethodName($"{ResourceTypeExpandMethodPrefix}{name ?? string.Empty}");
        public virtual string GetResourceTypeFlattenMethodName(string name) => GetGoPrivateMethodName($"{ResourceTypeFlattenMethodPrefix}{name ?? string.Empty}");

        protected virtual string GetGoPrivateMethodName(string name) => GetMethodName(name).Camelize();

        protected virtual string TitleUnderscoreCase(string name) => name.ToLowerInvariant().Titleize().Underscore();
    }
}
