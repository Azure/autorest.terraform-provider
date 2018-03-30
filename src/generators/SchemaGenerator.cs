using AutoRest.Core;
using AutoRest.Core.Model;
using AutoRest.Extensions;
using AutoRest.Terraform.Templates;
using System.Collections.Generic;
using static AutoRest.Core.Utilities.DependencyInjection;

namespace AutoRest.Terraform
{
    public class SchemaGenerator
        : ResourceGeneratorBase
    {
        public SchemaGenerator() => Singleton<SchemaGenerator>.Instance = this;

        public override ITemplate CreateTempalte() => new SchemaTemplate { Model = CodeModel.RootField };
    }
}
