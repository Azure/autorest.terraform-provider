using Microsoft.Perks.JsonRPC;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AutoRest.Terraform
{
    public class Program
    {
        public static int Main(string[] args)
        {
            if (args == null || args.Length <= 0 || args[0] != "--server")
            {
                DisplayUsage();
                return 1;
            }
            new Program().Run();
            return 0;
        }

        internal static void DisplayUsage()
        {
            Console.WriteLine("This is not an entry point.");
            Console.WriteLine("Please invoke this extension through AutoRest:");
            Console.WriteLine($"\tautorest --{TfProviderPlugin.PluginName} ...");
        }

        internal void Run()
        {
            Connection.Dispatch<IEnumerable<string>>(nameof(GetPluginNames), GetPluginNames);
            Connection.Dispatch<string, string, bool>(nameof(Process), Process);
            Connection.DispatchNotification(nameof(Shutdown), Shutdown);
            Connection.GetAwaiter().GetResult();
        }

        private Connection Connection { get; }
            = new Connection(Console.OpenStandardOutput(), Console.OpenStandardInput());

        private IDictionary<string, Func<Connection, string, NewPlugin>> Providers { get; }
            = new Dictionary<string, Func<Connection, string, NewPlugin>>
            {
                { TfProviderPlugin.PluginName, (connection, session) => new TfProviderPlugin(connection, session) }
            };

        public Task<IEnumerable<string>> GetPluginNames() => Task.FromResult((IEnumerable<string>)Providers.Keys);

        public Task<bool> Process(string plugin, string session) => Providers[plugin].Invoke(Connection, session).Process();

        public void Shutdown() => Connection.Stop();
    }
}
