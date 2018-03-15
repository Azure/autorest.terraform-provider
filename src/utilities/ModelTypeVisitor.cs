using AutoRest.Core.Model;
using System;
using System.Collections.Generic;

namespace AutoRest.Terraform
{
    /// <summary>
    /// The visitor which walks through the (probably nested) model types. Note that <see cref="CompositeTypeTf"/> is not considered a nested type.
    /// </summary>
    /// <remarks>Examples of nested types are: <see cref="SequenceType"/> and <see cref="DictionaryType"/>.</remarks>
    internal class ModelTypeVisitor
        : VisitorBase
    {
        public void Visit(IModelType type)
        {
            switch (type)
            {
                case PrimaryType primary:
                    OnPrimaryVisiting(primary.KnownPrimaryType);
                    OnPrimaryVisited(primary.KnownPrimaryType);
                    break;
                case EnumType @enum:
                    OnEnumVisiting(@enum.Values);
                    OnEnumVisited(@enum.Values);
                    break;
                case SequenceType sequence:
                    OnArrayVisiting(sequence);
                    Visit(sequence.ElementType);
                    OnArrayVisited(sequence);
                    break;
                case DictionaryType dictionary:
                    OnStringMapVisiting(dictionary);
                    Visit(dictionary.ValueType);
                    OnStringMapVisited(dictionary);
                    break;
                case CompositeTypeTf composite:
                    OnComplexVisiting(composite);
                    OnComplexVisited(composite);
                    break;
                default:
                    throw new NotSupportedException($"\"{type}\" is not supported");
            }
        }


        public event EventHandler<VisitingEventArgs<KnownPrimaryType>> PrimaryVisiting;
        public event EventHandler<VisitedEventArgs<KnownPrimaryType>> PrimaryVisited;
        protected virtual void OnPrimaryVisiting(KnownPrimaryType primaryType) => OnVisiting(PrimaryVisiting, primaryType);
        protected virtual void OnPrimaryVisited(KnownPrimaryType primaryType) => OnVisited(PrimaryVisited, primaryType);


        public event EventHandler<VisitingEventArgs<IEnumerable<EnumValue>>> EnumVisiting;
        public event EventHandler<VisitedEventArgs<IEnumerable<EnumValue>>> EnumVisited;
        protected virtual void OnEnumVisiting(IEnumerable<EnumValue> enumType) => OnVisiting(EnumVisiting, enumType);
        protected virtual void OnEnumVisited(IEnumerable<EnumValue> enumType) => OnVisited(EnumVisited, enumType);


        public event EventHandler<VisitingEventArgs<SequenceType>> ArrayVisiting;
        public event EventHandler<VisitedEventArgs<SequenceType>> ArrayVisited;
        protected virtual void OnArrayVisiting(SequenceType arrayType) => OnVisiting(ArrayVisiting, arrayType);
        protected virtual void OnArrayVisited(SequenceType arrayType) => OnVisited(ArrayVisited, arrayType);


        public event EventHandler<VisitingEventArgs<DictionaryType>> StringMapVisiting;
        public event EventHandler<VisitedEventArgs<DictionaryType>> StringMapVisited;
        protected virtual void OnStringMapVisiting(DictionaryType stringMapType) => OnVisiting(StringMapVisiting, stringMapType);
        protected virtual void OnStringMapVisited(DictionaryType stringMapType) => OnVisited(StringMapVisited, stringMapType);


        public event EventHandler<VisitingEventArgs<CompositeTypeTf>> ComplexVisiting;
        public event EventHandler<VisitedEventArgs<CompositeTypeTf>> ComplexVisited;
        protected virtual void OnComplexVisiting(CompositeTypeTf complexType) => OnVisiting(ComplexVisiting, complexType);
        protected virtual void OnComplexVisited(CompositeTypeTf complexType) => OnVisited(ComplexVisited, complexType);
    }
}
