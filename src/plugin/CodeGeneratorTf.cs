using AutoRest.Core;
using AutoRest.Core.Logging;
using AutoRest.Core.Model;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoRest.Terraform
{
    internal interface ITfProviderGenerator
    {
        string FileName { get; }
        ITemplate Generate();
    }

    internal class CodeGeneratorTf
        : CodeGenerator
    {
        public CodeGeneratorTf()
        {
            Generators = CreateGenerators();
        }

        private static IEnumerable<ITfProviderGenerator> CreateGenerators()
        {
            return Enumerable.Empty<ITfProviderGenerator>();
        }

        private IEnumerable<ITfProviderGenerator> Generators { get; }

        public override string ImplementationFileExtension { get; } = ".go";

        public override string UsageInstructions { get; } = "The runtime package is required to compile the generated code.";

        public override async Task Generate(CodeModel codeModel)
        {
            Logger.Instance.Log(Category.Debug, "{0} is generating (using {1} sub-generators).", nameof(CodeGeneratorTf), Generators.Count());

            await base.Generate(codeModel).ConfigureAwait(false);
            var templateGroups = Generators.ToLookup(gen => gen.FileName, gen => gen.Generate());
            foreach (var templateGroup in templateGroups)
            {
                var content = await ConcatTemplatesAsync(templateGroup).ConfigureAwait(false);
                await Write(content, templateGroup.Key).ConfigureAwait(false);
            }
        }

        private static async Task<string> ConcatTemplatesAsync(IEnumerable<ITemplate> templates)
        {
            const int AverageTemplateSize = 4096;
            using (var writer = new StringWriter(new StringBuilder(AverageTemplateSize)))
            {
                foreach (var template in templates)
                {
                    template.Settings = Settings.Instance;
                    template.TextWriter = writer;
                    await template.ExecuteAsync().ConfigureAwait(false);
                }
                return writer.GetStringBuilder().ToString();
            }
        }
    }
}
