using AutoRest.Core.Extensibility;
using AutoRest.Core.Model;
using AutoRest.Core.Parsing;
using AutoRest.Core.Utilities;
using Microsoft.Perks.JsonRPC;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static AutoRest.Core.Utilities.DependencyInjection;

namespace AutoRest.Terraform
{
    internal class TfProviderPluginHost
        : NewPlugin
    {
        public const string PluginName = "terraform";

        public TfProviderPluginHost(Connection connection, string session)
            : base(connection, PluginName, session)
        {
        }

        protected override Task<bool> ProcessInternal()
        {
            return new TfProviderPlugin(this).ProcessAsync();
        }
    }

    internal class TfProviderPlugin
        : Plugin<SettingsTf, TransformerTf, CodeGeneratorTf, CodeNamerTf, CodeModelTf>
    {
        public TfProviderPlugin(TfProviderPluginHost host)
        {
            Context = new Context
            {
                Context,
                () => Singleton<TfProviderPlugin>.Instance = this,
                new Factory<CodeModel, CodeModelTf>()
            };
            Host = host;
        }

        public TfProviderPluginHost Host { get; }

        public async Task<bool> ProcessAsync()
        {
            await Settings.LoadSettingsAsync(Host);

            var inputs = await Host.ListInputs();
            if (inputs == null || inputs.Length != 1)
            {
                throw new InvalidInputException($"Plugin \"{TfProviderPluginHost.PluginName}\" received incorrect number of inputs: [{inputs.Length} : {string.Join(",", inputs)}]");
            }
            var inputContent = await Host.ReadFile(inputs.Single());
            var inputInJson = inputContent.EnsureYamlIsJson();

            using (Activate())
            {
                var codeModel = Serializer.Load(inputInJson);
                codeModel = Transformer.TransformCodeModel(codeModel);
                await CodeGenerator.Generate(codeModel);
            }

            var outFs = Settings.StandardSettings.FileSystemOutput;
            var outputs = outFs.GetFiles(string.Empty, "*", SearchOption.AllDirectories);
            foreach (var output in outputs)
            {
                Host.WriteFile(output, outFs.ReadAllText(output), null);
            }

            return true;
        }
    }
}
