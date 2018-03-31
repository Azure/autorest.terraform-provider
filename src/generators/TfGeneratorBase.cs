using System;
using System.Collections.Generic;
using System.Text;
using AutoRest.Core;
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
        public string GoSDKClientName => CodeNamer.GetAzureGoSDKClientName(ResourceName);

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
}
