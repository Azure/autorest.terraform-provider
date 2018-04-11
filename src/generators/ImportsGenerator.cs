using AutoRest.Core;
using AutoRest.Terraform.Templates;
using System.Collections.Generic;
using System.Linq;
using static AutoRest.Core.Utilities.DependencyInjection;

namespace AutoRest.Terraform
{
    public class ImportsGenerator
        : TfGeneratorBase
    {
        public ImportsGenerator() => Singleton<ImportsGenerator>.Instance = this;
        protected override ITemplate CreateTemplateCore() => new ImportsTemplate { Model = this };

        public string PackageName => Settings.StandardSettings.Namespace;
        public IEnumerable<string> SystemReferences
            => from r in references
               let pkg = Settings.Metadata.ImportCandidates[r]
               where r == pkg
               select pkg;

        public IEnumerable<(string Alias, string Package)> LibraryReferences
            => from r in references
               let pkg = Settings.Metadata.ImportCandidates[r]
               where r != pkg
               let alias = CodeNamer.ExtractAliasFromGoPackage(pkg)
               select (alias == r ? string.Empty : r, pkg);

        public string Reference(string package)
        {
            references.Add(package);
            return package;
        }

        private readonly ISet<string> references = new HashSet<string>();
    }
}
