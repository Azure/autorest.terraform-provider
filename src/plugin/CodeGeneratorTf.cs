using AutoRest.Core;
using AutoRest.Core.Logging;
using AutoRest.Core.Model;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AutoRest.Core.Utilities.DependencyInjection;

namespace AutoRest.Terraform
{
    internal interface ITfProviderGenerator
    {
        string FileName { get; }
        void Generate(CodeModelTf model);
        ITemplate CreateTempalte();
    }

    internal sealed class CodeGeneratorTf
        : CodeGenerator
    {
        public CodeGeneratorTf()
        {
            Generators = CreateGenerators();
        }

        private static IEnumerable<ITfProviderGenerator> CreateGenerators()
        {
            yield return new ImportsGenerator();
            yield return new SchemaGenerator();
            yield return new DeleteGenerator();
        }

        private IEnumerable<ITfProviderGenerator> Generators { get; }

        public override string ImplementationFileExtension => ".go";

        public override string UsageInstructions => "The runtime package is required to compile the generated code.";

        public override async Task Generate(CodeModel codeModel)
        {
            Logger.Instance.Log(Category.Debug, "{0} is generating (using {1} sub-generators).", nameof(CodeGeneratorTf), Generators.Count());

            await base.Generate(codeModel).ConfigureAwait(false);
            foreach (var generator in Generators)
            {
                generator.Generate((CodeModelTf)codeModel);
            }
            var templateGroups = Generators.ToLookup(gen => gen.FileName, gen => gen.CreateTempalte());
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
                    template.Settings = Singleton<SettingsTf>.Instance.StandardSettings;
                    template.TextWriter = writer;
                    await template.ExecuteAsync().ConfigureAwait(false);
                }
                return writer.GetStringBuilder().ToString();
            }
        }
    }
}
