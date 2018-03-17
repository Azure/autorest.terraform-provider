using AutoRest.Core;
using AutoRest.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace AutoRest.Terraform
{
    internal class ResourceNamesTransformer
        : CodeModelTransformer<CodeModelTf>
    {
        public string ResourceNamePattern { get; set; }
        public string ResourceGroupNamePattern { get; set; }

        public override CodeModelTf TransformCodeModel(CodeModel codeModel)
        {
            var model = base.TransformCodeModel(codeModel);
            var namePattern = new Regex($"^{model.CreateMethod.Path}/{ResourceNamePattern}$", RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.Singleline);
            var rgPattern = new Regex($"^{model.CreateMethod.Path}/{ResourceGroupNamePattern}$", RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.Singleline);
            foreach (var p in model.CreateMethod.LogicalParameters.Cast<ParameterTf>())
            {
                if (namePattern.IsMatch(p.Path))
                {
                    p.IsResourceName = true;
                }
                if (rgPattern.IsMatch(p.Path))
                {
                    p.IsResourceGroupName = true;
                }
            }
            return model;
        }
    }
}
