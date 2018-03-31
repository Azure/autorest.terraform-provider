using AutoRest.Core;
using AutoRest.Core.Model;
using AutoRest.Core.Utilities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static AutoRest.Core.Utilities.DependencyInjection;

namespace AutoRest.Terraform
{
    internal interface ITfProviderGenerator
    {
        void Preprocess(CodeModelTf model);
        ITemplate CreateTemplate();
    }

    internal sealed class CodeGeneratorTf
        : CodeGenerator
    {
        /// <summary>
        /// The sequence is critical: the templates will be executed in the order of the returned list.
        /// But it will be put into another position you specified in the target file.
        /// </summary>
        /// <returns>The generators as well as its target file name and position in file.</returns>
        private IEnumerable<(ITfProviderGenerator Generator, string TargetFile, uint PositionInFile)> CreateGeneratorDescriptors()
        {
            var resourceName = Settings.Metadata.ResourceName;
            var resourceFileName = CodeNamer.GetResourceFileName(resourceName) + ImplementationFileExtension;
            return new (ITfProviderGenerator, string, uint)[]
            {
                (new SchemaGenerator(), resourceFileName, 1),
                (new CreateGenerator(), resourceFileName, 2),
                (new ReadGenerator(), resourceFileName, 3),
                (new UpdateGenerator(), resourceFileName, 4),
                (new DeleteGenerator(), resourceFileName, 5),
                (new ImportsGenerator(), resourceFileName, 0),
            };
        }


        private SettingsTf Settings => Singleton<SettingsTf>.Instance;
        private CodeNamerTf CodeNamer => Singleton<CodeNamerTf>.Instance;


        public override string ImplementationFileExtension => ".go";

        public override string UsageInstructions => "The runtime package is required to compile the generated code.";

        public override async Task Generate(CodeModel codeModel)
        {
            var model = (CodeModelTf)codeModel;
            await base.Generate(model).ConfigureAwait(false);

            var descriptors = CreateGeneratorDescriptors();
            descriptors.ForEach(desc => desc.Generator.Preprocess(model));

            var files = from desc in descriptors
                        group desc by desc.TargetFile into fg
                        select new
                        {
                            FileName = fg.Key,
                            Contents = from d in fg
                                       orderby d.PositionInFile
                                       select d.Generator.CreateTemplate().ToString()
                        };

            foreach (var file in files)
            {
                await Write(string.Concat(file.Contents), file.FileName).ConfigureAwait(false);
            }
        }
    }
}
