using AutoRest.Core.Model;
using AutoRest.Extensions;
using System;

namespace AutoRest.Terraform
{
    internal class ParameterTf
        : Parameter
    {
        public bool IsResourceName { get; set; }
        public bool IsResourceGroupName { get; set; }
    }
}
