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
        public SchemaGenerator(DeleteGenerator deleteGenerator) => DeleteGenerator = deleteGenerator;

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
        }

        private DeleteGenerator DeleteGenerator { get; }

        public string FunctionName => CodeNamer.GetResourceDefinitionMethodName(ResourceName);
        public string DeleteFunctionName => DeleteGenerator.FunctionName;
        public IList<Field> Fields { get; } = new List<Field>();


        public sealed class Field
        {
            public Field(SchemaGenerator parent, bool firstOccurrence, IVariable variable)
            {
                IsFirstOccurrence = firstOccurrence;
                Name = parent.CodeNamer.GetResourceSchemaPropertyName(variable.GetClientName());
                switch (variable.ModelType)
                {
                    case PrimaryType primary:
                        switch (primary.KnownPrimaryType)
                        {
                            case KnownPrimaryType.String:
                                Type = "schema.TypeString";
                                break;
                            case KnownPrimaryType.Int:
                                Type = "schema.TypeInt";
                                break;
                            case KnownPrimaryType.Boolean:
                                Type = "schema.TypeBool";
                                break;
                            default:
                                Type = "UNSUPPORTED TYPE";
                                break;
                        }
                        break;
                    case EnumType e:
                        Type = "schema.TypeString";
                        break;
                    case DictionaryType d:
                        Type = "schema.TypeMap";
                        break;
                    case SequenceType sequence:
                    case CompositeType composite:
                        Type = "schema.TypeList";
                        break;
                    default:
                        Type = variable.ModelTypeName;
                        break;
                }

                var visitor = new ModelTypeVisitor();
                visitor.ArrayVisited += (s, e) => Subtypes.Add(e.Node.Name);
                visitor.PrimaryVisited += (s, e) => TerminalType = "schema.TypeString";
                visitor.ComplexVisited += (s, e) => TerminalType = "complex";
                visitor.Visit(variable.ModelType);
            }

            public bool IsFirstOccurrence { get; }
            public string Name { get; }
            public string Type { get; }
            public IList<string> Subtypes { get; } = new List<string>();
            public string TerminalType { get; private set; }
        }
    }
}
