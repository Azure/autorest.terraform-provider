﻿using AutoRest.Core;
using AutoRest.Terraform.Templates;
using static AutoRest.Core.Utilities.DependencyInjection;

namespace AutoRest.Terraform
{
    public class ImportsGenerator
        : ITfProviderGenerator
    {
        public ImportsGenerator()
        {
            Singleton<ImportsGenerator>.Instance = this;
        }

        public string FileName => "generation-test.go";

        public ITemplate CreateTempalte() => new ImportsTemplate { Model = this };

        public void Generate(CodeModelTf model)
        {
        }

        public string Header => Singleton<SettingsTf>.Instance.StandardSettings.Header;
        public string PackageName => Singleton<SettingsTf>.Instance.StandardSettings.Namespace;
    }
}