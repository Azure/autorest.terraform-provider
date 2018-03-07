using AutoRest.Core;
using AutoRest.Core.Extensibility;
using System.Threading.Tasks;

namespace AutoRest.Terraform
{
    internal class SettingsTf
        : IGeneratorSettings
    {
        public SettingsTf()
        {
            new Settings
            {
            };
        }

        public Settings StandardSettings => Settings.Instance;

        public Task LoadSettingsAsync(TfProviderPluginHost host)
        {
            StandardSettings.Host = host;
            Settings.PopulateSettings(this, StandardSettings.CustomSettings);
            return Task.FromResult(0);
        }
    }
}
