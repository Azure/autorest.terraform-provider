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
    public class TypeExpandGenerator
        : ResourceGeneratorBase
    {
        public override ITemplate CreateTempalte() => new TypeExpandTemplate { Model = this };

        protected override void GenerateCore()
        {
            var complexTypesToExpand = new List<(string Type, string ChildName, string ChildTfName, string ChildType, string ChildGoType)>();
            var visitor = new MethodPropertyVisitor
            {
                SkipResponse = true
            };
            visitor.Visit(CodeModel.CreateMethod);
            ComplexTypesToExpand = complexTypesToExpand.ToLookup(t => t.Type, t => (t.ChildName, t.ChildTfName, t.ChildType, t.ChildGoType));
        }

        public string GetExpandFunctionName(string typeName) => CodeNamer.GetResourceTypeExpandMethodName(typeName);
        public string GetExpandArrayFunctionName(string typeName) => $"{GetExpandFunctionName(typeName)}Array";

        public ILookup<string, (string Name, string TfName, string Type, string GoType)> ComplexTypesToExpand { get; private set; }
        public IList<(string Name, string Type)> ArrayTypesToExpand { get; } = new List<(string Name, string Type)>();
        
    }
}
