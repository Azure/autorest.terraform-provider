using AutoRest.Core.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace AutoRest.Terraform
{
    public enum GoSDKTerminalTypes
    {
        Boolean, Int32, String, Enum, Complex
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
            { KnownPrimaryType.Object, GoSDKTerminalTypes.String }
        };

        public GoSDKTypeChain(GoSDKTerminalTypes terminal) => Terminal = terminal;

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
                        Chain.Add(GoSDKNonTerminalTypes.Array);
                        break;
                    case DictionaryType dictionary:
                        type = dictionary.ValueType;
                        Chain.Add(GoSDKNonTerminalTypes.StringMap);
                        break;
                }
            } while (type != null);
        }

        public IList<GoSDKNonTerminalTypes> Chain { get; } = new List<GoSDKNonTerminalTypes>();
        public GoSDKTerminalTypes Terminal { get; }
        public IModelType OriginalTerminalType { get; }


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
    }
}
