using AutoRest.Core;
using System.Collections.Generic;
using System.Linq;
using static AutoRest.Core.Utilities.DependencyInjection;

namespace AutoRest.Terraform
{
    public abstract class TfGeneratorBase<TTemplate, TGenerator>
        : ITfProviderGenerator
        where TTemplate : TfProviderTemplateBase<TGenerator>, new()
        where TGenerator : TfGeneratorBase<TTemplate, TGenerator>
    {
        protected TfGeneratorBase() => Singleton<TGenerator>.Instance = (TGenerator)this;

        protected SettingsTf Settings => Singleton<SettingsTf>.Instance;
        protected CodeNamerTf CodeNamer => Singleton<CodeNamerTf>.Instance;
        protected CodeModelTf CodeModel { get; private set; }

        public string ResourceName => Settings.Metadata.ResourceName;
        public string AzureRmResourceName => CodeNamer.GetAzureRmResourceName(ResourceName);

        public ITemplate CreateTemplate() => new TTemplate
        {
            Model = (TGenerator)this,
            Settings = Settings.StandardSettings
        };

        public void Preprocess(CodeModelTf model)
        {
            CodeModel = model;
        }
    }

    public abstract class TfFunctionGeneratorBase<TTemplate, TGenerator>
        : TfGeneratorBase<TTemplate, TGenerator>
        where TTemplate : TfProviderTemplateBase<TGenerator>, new()
        where TGenerator : TfGeneratorBase<TTemplate, TGenerator>
    {
        public string GoSDKClientName => CodeNamer.GetAzureGoSDKClientName(ResourceName);
        public string FunctionName => CodeNamer.GetGoPrivateMethodName(CodeNamer.JoinNonEmpty(AzureRmResourceName, FunctionNamePostfix));

        public virtual IEnumerable<GoSDKInvocation> Invocations => Enumerable.Empty<GoSDKInvocation>();
        protected abstract string FunctionNamePostfix { get; }
    }
}
