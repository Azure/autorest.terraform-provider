using Microsoft.Perks.JsonRPC;
using System.Threading.Tasks;

namespace AutoRest.Terraform
{
    internal class TfProviderPlugin : NewPlugin
    {
        public const string PluginName = "terraform";

        public TfProviderPlugin(Connection connection, string session)
            : base(connection, PluginName, session)
        {
        }

        protected override Task<bool> ProcessInternal()
        {
            Message(new Message { Channel = "information", Text = $"This is a message from {PluginName} plugin." });
            return Task.FromResult(true);
        }
    }
}
