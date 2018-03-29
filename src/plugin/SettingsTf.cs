using AutoRest.Core;
using AutoRest.Core.Extensibility;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using static AutoRest.Core.Utilities.DependencyInjection;

namespace AutoRest.Terraform
{
    internal class SettingsTf
        : IGeneratorSettings
    {
        private const string NamespaceOption = "namespace";
        private static readonly string DisplayModelOption = $"{TfProviderPluginHost.PluginName}.display-model";
        private static readonly string NoProcessOption = $"{TfProviderPluginHost.PluginName}.no-process";
        private static readonly string MetadataFileOption = $"{TfProviderPluginHost.PluginName}.metadata-file";
        private static readonly string MetadataJsonPathOption = $"{TfProviderPluginHost.PluginName}.metadata-jsonpath";

        public SettingsTf()
        {
            new Settings
            {
            };
        }

        public Settings StandardSettings => Settings.Instance;

        public TfProviderMetadata Metadata { get; private set; }

        public bool DisplayModel { get; private set; }

        public bool NoProcess { get; private set; }

        private TfProviderPluginHost Host => Singleton<TfProviderPlugin>.Instance.Host;

        public async Task LoadSettingsAsync()
        {
            StandardSettings.Host = Host;
            StandardSettings.Namespace = await Host.GetValue(NamespaceOption).ConfigureAwait(false);
            StandardSettings.CustomSettings.Add(nameof(DisplayModel), await Host.GetValue<bool>(DisplayModelOption).ConfigureAwait(false));
            StandardSettings.CustomSettings.Add(nameof(NoProcess), await Host.GetValue<bool>(NoProcessOption).ConfigureAwait(false));
            Metadata = await TfProviderMetadata.LoadAsync(await Host.GetValue(MetadataFileOption).ConfigureAwait(false), await Host.GetValue(MetadataJsonPathOption).ConfigureAwait(false));
            Settings.PopulateSettings(this, StandardSettings.CustomSettings);
        }
    }

    internal sealed class TfProviderMetadata
    {
        public static async Task<TfProviderMetadata> LoadAsync(string filename, string jsonpath)
        {
            using (var file = File.OpenText(filename))
            using (var reader = new JsonTextReader(file))
            {
                var rawObject = await JObject.LoadAsync(reader).ConfigureAwait(false);
                var selectedRoot = string.IsNullOrWhiteSpace(jsonpath) ? rawObject : rawObject.SelectToken(jsonpath);
                return selectedRoot.ToObject<TfProviderMetadata>();
            }
        }

        private TfProviderMetadata()
        {
        }

        [JsonProperty("name")]
        public string ResourceName { get; private set; }

        [JsonProperty("create")]
        public IEnumerable<MethodDefinition> CreateMethods { get; private set; }

        [JsonProperty("read")]
        public IEnumerable<MethodDefinition> ReadMethods { get; private set; }

        [JsonProperty("update")]
        public IEnumerable<MethodDefinition> UpdateMethods { get; private set; }

        [JsonProperty("delete")]
        public IEnumerable<MethodDefinition> DeleteMethods { get; private set; }

        internal sealed class MethodDefinition
        {
            [JsonProperty("method")]
            public string Path { get; private set; }

            [JsonProperty("schema")]
            public SchemaDefinition Schema { get; private set; }
        }

        internal sealed class SchemaDefinition
        {
            [JsonProperty("ignore")]
            public IEnumerable<string> Excludes { get; private set; }

            [JsonProperty("flatten")]
            public IEnumerable<FlattenDefinition> Flattens { get; private set; }
        }

        internal enum Priority
            : uint
        {
            Lowest = 0,
            Lower = 15,
            Low = 30,
            Normal = 50,
            High = 70,
            Higher = 85,
            Highest = 100
        }

        internal sealed class FlattenDefinition
        {
            [JsonProperty("source")]
            public string SourcePath { get; private set; }

            [JsonProperty("target")]
            public string TargetPath { get; private set; }

            [JsonProperty("priority")]
            public Priority Priority { get; private set; }
        }
    }
}
