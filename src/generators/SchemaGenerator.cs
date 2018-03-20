using AutoRest.Core;
using AutoRest.Core.Model;
using AutoRest.Extensions;
using AutoRest.Terraform.Templates;
using System.Collections.Generic;

namespace AutoRest.Terraform
{
    public class SchemaGenerator
        : ResourceGeneratorBase
    {
        public SchemaGenerator(CreateGenerator createGen, ReadGenerator readGen, DeleteGenerator deleteGen)
        {
            CreateGenerator = createGen;
            ReadGenerator = readGen;
            DeleteGenerator = deleteGen;
        }

        public override ITemplate CreateTempalte() => new SchemaTemplate { Model = this };

        protected override void GenerateCore()
        {
            Fields.Clear();
            var visitor = new MethodPropertyVisitor
            {
                SkipResponse = true
            };
            visitor.ParameterVisiting += (s, e) => Fields.Add(new Field(this, true, e.Node));
            visitor.ParameterVisited += (s, e) => Fields.Add(new Field(this, false, e.Node));
            visitor.PropertyVisiting += (s, e) => Fields.Add(new Field(this, true, e.Node));
            visitor.PropertyVisited += (s, e) => Fields.Add(new Field(this, false, e.Node));
            visitor.Visit(CodeModel.CreateMethod);

            var responseVisitor = new MethodPropertyVisitor
            {
                SkipParameter = true
            };
            responseVisitor.ParameterVisiting += (s, e) => Fields.Add(new Field(this, true, e.Node, true));
            responseVisitor.ParameterVisited += (s, e) => Fields.Add(new Field(this, false, e.Node, true));
            responseVisitor.PropertyVisiting += (s, e) => Fields.Add(new Field(this, true, e.Node, true));
            responseVisitor.PropertyVisited += (s, e) => Fields.Add(new Field(this, false, e.Node, true));
            responseVisitor.Visit(CodeModel.ReadMethod);
        }

        private CreateGenerator CreateGenerator { get; }
        private ReadGenerator ReadGenerator { get; }
        private DeleteGenerator DeleteGenerator { get; }

        public string FunctionName => CodeNamer.GetResourceDefinitionMethodName(ResourceName);
        public string CreateFunctionName => CreateGenerator.FunctionName;
        public string ReadFunctionName => ReadGenerator.FunctionName;
        public string DeleteFunctionName => DeleteGenerator.FunctionName;
        public IList<Field> Fields { get; } = new List<Field>();


        public static string GetSchemaTypeFromModelType(IModelType type)
        {
            switch (type)
            {
                case PrimaryType primary:
                    switch (primary.KnownPrimaryType)
                    {
                        case KnownPrimaryType.String:
                            return "schema.TypeString";
                        case KnownPrimaryType.Int:
                            return "schema.TypeInt";
                        case KnownPrimaryType.Boolean:
                            return "schema.TypeBool";
                        default:
                            return "UNSUPPORTED TYPE";
                    }
                case EnumType e:
                    return "schema.TypeString";
                case DictionaryType d:
                    return "schema.TypeMap";
                case SequenceType sequence:
                case CompositeType composite:
                    return "schema.TypeList";
                default:
                    return "UNSUPPORTED TYPE";
            }
        }

        public static string GetGoTypeFromModelType(IModelType type)
        {
            switch (type)
            {
                case PrimaryType primary:
                    switch (primary.KnownPrimaryType)
                    {
                        case KnownPrimaryType.String:
                            return "string";
                        case KnownPrimaryType.Int:
                            return "int32";
                        case KnownPrimaryType.Boolean:
                            return "bool";
                        default:
                            return "UNSUPPORTED TYPE";
                    }
                case EnumType e:
                    return "string";
                case DictionaryType d:
                    return "MAP";
                case SequenceType sequence:
                    return "ARRAY";
                case CompositeType composite:
                    return "COMPLEX";
                default:
                    return "UNSUPPORTED TYPE";
            }
        }

        public sealed class Field
        {
            public Field(SchemaGenerator parent, bool firstOccurrence, IVariable variable, bool isComputed = false)
            {
                IsFirstOccurrence = firstOccurrence;
                Name = parent.CodeNamer.GetResourceSchemaPropertyName(variable.GetClientName());
                Type = GetSchemaTypeFromModelType(variable.ModelType);

                var visitor = new ModelTypeVisitor();
                visitor.ArrayVisited += (s, e) => Subtypes.Add(e.Node.Name);
                visitor.PrimaryVisited += (s, e) => TerminalType = "schema.TypeString";
                visitor.ComplexVisited += (s, e) => TerminalType = "complex";
                visitor.Visit(variable.ModelType);

                IsComputed = isComputed;
                IsRequired = variable.IsRequired;
                MaxItems = GetGoTypeFromModelType(variable.ModelType) == "COMPLEX" ? (int?)1 : null;
            }

            public bool IsFirstOccurrence { get; }
            public string Name { get; }
            public string Type { get; }
            public IList<string> Subtypes { get; } = new List<string>();
            public string TerminalType { get; private set; }

            public bool IsComputed { get; }
            public bool IsRequired { get; }
            public int? MaxItems { get; }
        }
    }
}
