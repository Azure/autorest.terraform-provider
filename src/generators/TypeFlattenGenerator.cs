using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoRest.Core;
using AutoRest.Core.Model;
using AutoRest.Extensions;
using AutoRest.Terraform.Templates;

namespace AutoRest.Terraform
{
    public class TypeFlattenGenerator
        : ResourceGeneratorBase
    {
        public override ITemplate CreateTempalte() => new TypeFlattenTemplate { Model = this };

        protected override void GenerateCore()
        {
            var complexTypesToFlatten = new List<(string Type, string ChildName, string ChildTfName, string ChildType, string ChildGoType)>();
            var visitor = new MethodPropertyVisitor
            {
                SkipParameter = true
            };
            visitor.Visit(CodeModel.ReadMethod);
            ComplexTypesToFlatten = complexTypesToFlatten.ToLookup(t => t.Type, t => (t.ChildName, t.ChildTfName, t.ChildType, t.ChildGoType));
        }

        public string GetFlattenFunctionName(string typeName) => CodeNamer.GetResourceTypeFlattenMethodName(typeName);
        public string GetFlattenArrayFunctionName(string typeName) => $"{GetFlattenFunctionName(typeName)}Array";
        public string GetFlattenMapFunctionName(string typeName) => $"{GetFlattenFunctionName(typeName)}Map";

        public ILookup<string, (string Name, string TfName, string Type, string GoType)> ComplexTypesToFlatten { get; private set; }
        public IList<(string Name, string Type)> ArrayTypesToFlatten { get; } = new List<(string Name, string Type)>();
        public IList<(string Name, string Type)> MapTypesToFlatten { get; } = new List<(string Name, string Type)>();
    }
}
