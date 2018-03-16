using System;
using System.Collections.Generic;
using System.Text;
using AutoRest.Core;
using static AutoRest.Core.Utilities.DependencyInjection;

namespace AutoRest.Terraform
{
    public abstract class ResourceGeneratorBase
        : ITfProviderGenerator
    {
        public string ResourceName => Singleton<SettingsTf>.Instance.Metadata.ResourceName;

        protected CodeNamerTf CodeNamer => Singleton<CodeNamerTf>.Instance;

        protected CodeModelTf CodeModel { get; private set; }

        public string FileName => CodeNamer.GetResourceFileName(ResourceName);

        public abstract ITemplate CreateTempalte();

        public void Generate(CodeModelTf model)
        {
            CodeModel = model;
            GenerateCore();
        }

        protected virtual void GenerateCore()
        {
        }
    }
}
