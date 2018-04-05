using System;

namespace AutoRest.Terraform
{
    internal class InvalidInputException
        : ApplicationException
    {
        public InvalidInputException(string message)
            : base(message)
        {
        }
    }

    internal class TypeIncompatibleException
        : ApplicationException
    {
        public TypeIncompatibleException(string message)
            : base(message)
        {
        }
    }

    internal class SchemaFieldOutOfScopeException
        : ApplicationException
    {
        public SchemaFieldOutOfScopeException(string message)
            : base(message)
        {
        }
    }
}
