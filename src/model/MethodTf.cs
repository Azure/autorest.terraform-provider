using AutoRest.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AutoRest.Terraform
{
    internal class MethodTf
        : Method
    {
        public MethodTf() => InvalidatePath();

        private Lazy<string> path;
        public string Path => path.Value;
        public void InvalidatePath() => path = new Lazy<string>(() => $"{((MethodGroupTf)Parent).Path}/{Name}");

        public IEnumerable<ResponseTf> LogicalResponses => Responses.Select(pair => new ResponseTf(pair.Key, pair.Value));
    }
}
