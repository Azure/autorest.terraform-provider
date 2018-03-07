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
}
