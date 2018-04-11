using AutoRest.Core;
using System.Collections.Generic;
using System.Linq;
using static AutoRest.Core.Utilities.DependencyInjection;

namespace AutoRest.Terraform
{
    public abstract class TfGeneratorBase
        : ITfProviderGenerator
    {
        protected TfGeneratorBase()
        {
        }

        protected SettingsTf Settings => Singleton<SettingsTf>.Instance;
        protected CodeNamerTf CodeNamer => Singleton<CodeNamerTf>.Instance;
        protected CodeModelTf CodeModel { get; private set; }

        public string ResourceName => Settings.Metadata.ResourceName;
        public string AzureRmResourceName => CodeNamer.GetAzureRmResourceName(ResourceName);

        protected abstract ITemplate CreateTemplateCore();

        public ITemplate CreateTemplate()
        {
            var template = CreateTemplateCore();
            template.Settings = Settings.StandardSettings;
            return template;
        }

        public void Preprocess(CodeModelTf model)
        {
            CodeModel = model;
        }
    }

    public abstract class TfFunctionGeneratorBase
        : TfGeneratorBase
    {
        public string GoSDKClientName => CodeNamer.GetAzureGoSDKClientName(ResourceName);
        public string FunctionName => CodeNamer.GetGoPrivateMethodName(CodeNamer.JoinNonEmpty(AzureRmResourceName, FunctionNamePostfix));

        public virtual IEnumerable<GoSDKInvocation> Invocations => Enumerable.Empty<GoSDKInvocation>();
        protected abstract string FunctionNamePostfix { get; }
    }
}
