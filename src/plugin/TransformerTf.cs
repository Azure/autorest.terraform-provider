using AutoRest.Core;
using AutoRest.Core.Model;
using AutoRest.Core.Utilities;
using AutoRest.Extensions;
using System.Collections.Generic;
using static AutoRest.Core.Utilities.DependencyInjection;

namespace AutoRest.Terraform
{
    internal interface ITfProviderTransformer
    {
        void Transform(CodeModelTf model);
    }

    internal sealed class TransformerTf
        : CodeModelTransformer<CodeModelTf>
    {
        private IEnumerable<ITfProviderTransformer> CreateTransformers()
        {
            return new ITfProviderTransformer[]
            {
                new NewInvocationsTransformer()
            };
        }

        public override CodeModelTf TransformCodeModel(CodeModel codeModel)
        {
            var model = base.TransformCodeModel(codeModel);
            SwaggerExtensions.NormalizeClientModel(model);
            CreateTransformers().ForEach(tr => tr.Transform(model));
            SwaggerExtensions.NormalizeClientModel(model);
            return model;
        }
    }
}
