using AutoRest.Core.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace AutoRest.Terraform
{
    public enum GoSDKTerminalTypes
    {
        Boolean, Int32, String, Enum, Object, Complex
    }

    public enum GoSDKNonTerminalTypes
    {
        Array, StringMap
    }

    public class GoSDKTypeChain
        : IEquatable<GoSDKTypeChain>
    {
        private static readonly IDictionary<KnownPrimaryType, GoSDKTerminalTypes> PrimaryTypeMapping = new Dictionary<KnownPrimaryType, GoSDKTerminalTypes>
        {
            { KnownPrimaryType.Boolean, GoSDKTerminalTypes.Boolean },
            { KnownPrimaryType.Int, GoSDKTerminalTypes.Int32 },
            { KnownPrimaryType.String, GoSDKTerminalTypes.String },
            { KnownPrimaryType.Object, GoSDKTerminalTypes.Object }
        };
        private static readonly IDictionary<GoSDKTerminalTypes, KnownPrimaryType> PrimaryTypeReverseMapping = PrimaryTypeMapping.ToDictionary(pair => pair.Value, pair => pair.Key);

        public GoSDKTypeChain(IModelType type)
        {
            Debug.Assert(type != null);
            do
            {
                OriginalTerminalType = type;
                switch (type)
                {
                    case PrimaryType primary:
                        type = null;
                        Terminal = PrimaryTypeMapping[primary.KnownPrimaryType];
                        break;
                    case EnumType eumeration:
                        type = null;
                        Terminal = GoSDKTerminalTypes.Enum;
                        break;
                    case CompositeType composite:
                        type = null;
                        Terminal = GoSDKTerminalTypes.Complex;
                        break;
                    case SequenceType sequence:
                        type = sequence.ElementType;
                        chain.Add(GoSDKNonTerminalTypes.Array);
                        break;
                    case DictionaryType dictionary:
                        type = dictionary.ValueType;
                        chain.Add(GoSDKNonTerminalTypes.StringMap);
                        break;
                }
            } while (type != null);
        }

        public GoSDKTypeChain(GoSDKTerminalTypes terminal, IEnumerable<GoSDKNonTerminalTypes> nonTerminals = null)
        {
            chain.AddRange(nonTerminals ?? Enumerable.Empty<GoSDKNonTerminalTypes>());
            Terminal = terminal;
        }

        public IEnumerable<GoSDKNonTerminalTypes> Chain => chain;
        public GoSDKTerminalTypes Terminal { get; }
        public IModelType OriginalTerminalType { get; private set; }
        public bool IsSimple => Chain.Count() == 0 && Terminal != GoSDKTerminalTypes.Complex;

        public GoSDKTypeChain StripNonTerminal() => new GoSDKTypeChain(Terminal, Chain.Skip(1))
        {
            OriginalTerminalType = OriginalTerminalType
        };

        public static GoSDKTypeChain Parse(string s)
        {
            var typeStrings = s.Split(',');
            var terminal = (GoSDKTerminalTypes)Enum.Parse(typeof(GoSDKTerminalTypes), typeStrings.Last());
            var nonTerminals = typeStrings.SkipLast(1).Select(nt => (GoSDKNonTerminalTypes)Enum.Parse(typeof(GoSDKNonTerminalTypes), nt));
            return new GoSDKTypeChain(terminal, nonTerminals);
        }

        public IModelType ToModelType()
        {
            IModelType lastType;
            switch (Terminal)
            {
                case GoSDKTerminalTypes.Boolean:
                case GoSDKTerminalTypes.Int32:
                case GoSDKTerminalTypes.String:
                    lastType = new PrimaryTypeSDKImpl(PrimaryTypeReverseMapping[Terminal]);
                    break;
                default:
                    throw new NotSupportedException($"{Terminal} cannot be converted to {nameof(IModelType)}");
            }
            foreach (var nonTerminal in Chain)
            {
                switch (nonTerminal)
                {
                    case GoSDKNonTerminalTypes.Array:
                        lastType = new SequenceTypeSDKImpl(lastType);
                        break;
                    case GoSDKNonTerminalTypes.StringMap:
                        lastType = new DictionaryTypeSDKImpl(lastType);
                        break;
                }
            }
            return lastType;
        }


        private readonly List<GoSDKNonTerminalTypes> chain = new List<GoSDKNonTerminalTypes>();


        public override string ToString() => $"{string.Join(", ", Chain.Select(t => t.ToString()).Concat(Enumerable.Repeat(Terminal.ToString(), 1)))}";

        public override bool Equals(object obj) => Equals(obj as GoSDKTypeChain);

        public static bool operator ==(GoSDKTypeChain left, GoSDKTypeChain right) => Equals(left, right);

        public static bool operator !=(GoSDKTypeChain left, GoSDKTypeChain right) => !Equals(left, right);

        public override int GetHashCode()
        {
            return Terminal.GetHashCode() ^
                Chain.Aggregate(17, (h, t) => h * 31 + t.GetHashCode());
        }

        public bool Equals(GoSDKTypeChain other)
        {
            if (other == null)
            {
                return false;
            }
            if (ReferenceEquals(this, other))
            {
                return true;
            }
            return Terminal == other.Terminal &&
                Enumerable.SequenceEqual(Chain, other.Chain);
        }


        private class PrimaryTypeSDKImpl
            : PrimaryType
        {
            public PrimaryTypeSDKImpl(KnownPrimaryType primary)
                : base(primary)
            {
            }
        }

        private class SequenceTypeSDKImpl
            : SequenceType
        {
            public SequenceTypeSDKImpl(IModelType elementType)
                : base()
                => ElementType = elementType;
        }

        private class DictionaryTypeSDKImpl
            : DictionaryType
        {
            public DictionaryTypeSDKImpl(IModelType valueType)
                : base()
                => ValueType = valueType;
        }
    }
}
