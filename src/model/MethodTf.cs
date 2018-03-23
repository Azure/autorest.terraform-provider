using AutoRest.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AutoRest.Terraform
{
    internal class MethodTf
        : Method
    {
        public IEnumerable<ResponseTf> LogicalResponses => Responses.Select(pair => new ResponseTf(pair.Key, pair.Value));
    }
}
