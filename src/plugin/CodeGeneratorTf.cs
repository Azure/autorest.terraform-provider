using AutoRest.Core;
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
        private IEnumerable<ITfProviderGenerator> CreateGenerators()
        {
            var expandGenerator = new TypeExpandGenerator();
            var flattenGenerator = new TypeFlattenGenerator();
            var readGenerator = new ReadGenerator();
            var createGenerator = new CreateGenerator(readGenerator, expandGenerator);
            var updateGenerator = new UpdateGenerator(readGenerator);
            var deleteGenerator = new DeleteGenerator();

            yield return new ImportsGenerator();
            yield return new SchemaGenerator(createGenerator, readGenerator, updateGenerator, deleteGenerator);
            yield return createGenerator;
            yield return readGenerator;
            yield return updateGenerator;
            yield return deleteGenerator;
            yield return expandGenerator;
            yield return flattenGenerator;
        }

        public override string ImplementationFileExtension => ".go";

        public override string UsageInstructions => "The runtime package is required to compile the generated code.";

        public override async Task Generate(CodeModel codeModel)
        {
            await base.Generate(codeModel).ConfigureAwait(false);
            var generators = new List<ITfProviderGenerator>();
            foreach (var generator in CreateGenerators())
            {
                generator.Generate((CodeModelTf)codeModel);
                generators.Add(generator);
            }
            var templateGroups = generators.ToLookup(gen => gen.FileName, gen => gen.CreateTempalte());
            foreach (var templateGroup in templateGroups)
            {
                var content = await ConcatTemplatesAsync(templateGroup).ConfigureAwait(false);
                await Write(content, $"{templateGroup.Key}{ImplementationFileExtension}").ConfigureAwait(false);
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
