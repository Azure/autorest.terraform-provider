using AutoRest.Core;
using AutoRest.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AutoRest.Terraform
{
    internal class MethodFilterTransformer
        : CodeModelTransformer<CodeModelTf>
    {
        public IDictionary<string, Action<MethodTf>> Acceptances { get; set; }

        public override CodeModelTf TransformCodeModel(CodeModel codeModel)
        {
            var model = base.TransformCodeModel(codeModel);
            var residues = new List<MethodTf>();
            foreach (var m in model.Methods.Cast<MethodTf>())
            {
                if (Acceptances.TryGetValue(m.Path, out var accept))
                {
                    accept?.Invoke(m);
                }
                else
                {
                    residues.Add(m);
                }
            }
            foreach (var m in residues)
            {
                m.MethodGroup.Remove(m);
            }
            return model;
        }
    }
}
