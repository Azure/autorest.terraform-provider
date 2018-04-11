using AutoRest.Core.Extensibility;
using AutoRest.Core.Model;
using AutoRest.Core.Parsing;
using AutoRest.Core.Utilities;
using Microsoft.Perks.JsonRPC;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static AutoRest.Core.Utilities.DependencyInjection;

namespace AutoRest.Terraform
{
    internal enum Channel
    {
        Information
    }

    internal class TfProviderPluginHost
        : NewPlugin
    {
        public const string PluginName = "terraform-provider";

        public TfProviderPluginHost(Connection connection, string session)
            : base(connection, PluginName, session)
        {
        }

        protected override Task<bool> ProcessInternal()
        {
            return new TfProviderPlugin(this).ProcessAsync();
        }

        public void ShowMessage(Channel channel, string content)
        {
            var message = new Message
            {
                Channel = channel.ToString().ToLowerInvariant(),
                Text = content
            };
            Message(message);
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
                new Factory<Property, PropertyTf>(),
                new Factory<EnumType, EnumTypeTf>(),
                new Factory<CompositeType, CompositeTypeTf>(),
                new Factory<Method, MethodTf>(),
                new Factory<CodeModel, CodeModelTf>()
            };
            Host = host;
            Singleton<TfProviderPlugin>.Instance = this;
        }

        public TfProviderPluginHost Host { get; }

        public async Task<bool> ProcessAsync()
        {
            await Settings.LoadSettingsAsync().ConfigureAwait(false);

            var inputs = await Host.ListInputs().ConfigureAwait(false);
            if (inputs == null || inputs.Length != 1)
            {
                throw new InvalidInputException($"Plugin \"{TfProviderPluginHost.PluginName}\" received incorrect number of inputs: [{inputs.Length} : {string.Join(",", inputs)}]");
            }
            var inputContent = await Host.ReadFile(inputs.Single()).ConfigureAwait(false);
            var inputInJson = inputContent.EnsureYamlIsJson();

            using (Activate())
            {
                var codeModel = Serializer.Load(inputInJson);
                if (!Settings.NoProcess)
                {
                    codeModel = Transformer.TransformCodeModel(codeModel);
                    await CodeGenerator.Generate(codeModel).ConfigureAwait(false);
                }
                DisplayCodeModel(codeModel);
            }

            var outFs = Settings.StandardSettings.FileSystemOutput;
            var outputs = outFs.GetFiles(string.Empty, "*", SearchOption.AllDirectories);
            foreach (var output in outputs)
            {
                Host.WriteFile(output, outFs.ReadAllText(output), null);
            }

            return true;
        }

        private void DisplayCodeModel(CodeModelTf model)
        {
            void ShowDisplayMessage(string title, Action<CodeModelTf, IndentedStringBuilder> toDisplayString)
            {
                var builder = new IndentedStringBuilder();
                builder.Indent();
                toDisplayString(model, builder);
                builder.Outdent();
                Host.ShowMessage(Channel.Information, title + Environment.NewLine + builder.ToString());
            }

            if (Settings.DisplayModel?.HasFlag(DisplayModelType.Spec) ?? false)
            {
                ShowDisplayMessage("Azure AutoRest Specification", Utilities.AppendSpecDisplayString);
            }
            if (Settings.DisplayModel?.HasFlag(DisplayModelType.Schema) ?? false)
            {
                ShowDisplayMessage("Terraform Provider Schema", Utilities.AppendSchemaDisplayString);
            }
            if (Settings.DisplayModel?.HasFlag(DisplayModelType.SDK) ?? false)
            {
                ShowDisplayMessage("Azure Go SDK Invocations", Utilities.AppendInvocationsDisplayString);
            }
        }
    }
}
