using AutoRest.Terraform.Templates;
using System.Collections.Generic;
using System.Linq;

namespace AutoRest.Terraform
{
    public class ImportsGenerator
        : TfGeneratorBase<ImportsTemplate, ImportsGenerator>
    {
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
