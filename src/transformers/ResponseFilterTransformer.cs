using AutoRest.Core;
using AutoRest.Core.Model;
using AutoRest.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace AutoRest.Terraform
{
    internal class ResponseFilterTransformer
        : CodeModelTransformer<CodeModelTf>
    {
        public MethodTf Method { get; set; }

        public ISet<string> IgnoringPatterns { get; set; } = new HashSet<string>();

        public override CodeModelTf TransformCodeModel(CodeModel codeModel)
        {
            var residues = new List<(CompositeType Type, PropertyTf Property)>();
            foreach (var r in Method.Responses.Values)
            {
                if (r.Body is CompositeType composite)
                {
                    foreach (var p in composite.ComposedProperties.Cast<PropertyTf>())
                    {
                        foreach (var pattern in IgnoringPatterns)
                        {
                            if (pattern == p.GetClientName())
                            {
                                residues.Add((composite, p));
                            }
                        }
                    }
                }
            }
            foreach (var props in residues.ToLookup(r => r.Type, r => r.Property))
            {
                foreach (var p in props)
                {
                    props.Key.Remove(p);
                }
            }
            return base.TransformCodeModel(codeModel);
        }
    }
}
