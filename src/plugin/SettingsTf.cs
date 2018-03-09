using AutoRest.Core;
using AutoRest.Core.Extensibility;
using System.Threading.Tasks;
using static AutoRest.Core.Utilities.DependencyInjection;

namespace AutoRest.Terraform
{
    internal class SettingsTf
        : IGeneratorSettings
    {
        private static readonly string DisplayModelOption = $"{TfProviderPluginHost.PluginName}.display-model";
        private static readonly string NoProcessOption = $"{TfProviderPluginHost.PluginName}.no-process";

        public SettingsTf()
        {
            new Settings
            {
            };
        }

        public Settings StandardSettings => Settings.Instance;

        public bool DisplayModel { get; private set; }

        public bool NoProcess { get; private set; }

        private TfProviderPluginHost Host => Singleton<TfProviderPlugin>.Instance.Host;

        public async Task LoadSettingsAsync()
        {
            StandardSettings.Host = Host;
            StandardSettings.CustomSettings.Add(nameof(DisplayModel), await Host.GetValue<bool>(DisplayModelOption));
            StandardSettings.CustomSettings.Add(nameof(NoProcess), await Host.GetValue<bool>(NoProcessOption));
            Settings.PopulateSettings(this, StandardSettings.CustomSettings);
        }
    }
}
