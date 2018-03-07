using AutoRest.Core;
using AutoRest.Core.Logging;
using AutoRest.Core.Model;
using AutoRest.Extensions;
using System.Collections.Generic;
using System.Linq;

namespace AutoRest.Terraform
{
    internal class TransformerTf
        : CodeModelTransformer<CodeModelTf>
    {
        public TransformerTf()
        {
            Transformers = CreateTransformers();
        }

        private static IEnumerable<ITransformer<CodeModelTf>> CreateTransformers()
        {
            return Enumerable.Empty<ITransformer<CodeModelTf>>();
        }

        private IEnumerable<ITransformer<CodeModelTf>> Transformers { get; }

        public override CodeModelTf TransformCodeModel(CodeModel codeModel)
        {
            Logger.Instance.Log(Category.Debug, "{0} is transforming (using {1} sub-transformers)", nameof(TransformerTf), Transformers.Count());

            SwaggerExtensions.NormalizeClientModel(codeModel);
            foreach (var transformer in Transformers)
            {
                codeModel = transformer.TransformCodeModel(codeModel);
            }
            return base.TransformCodeModel(codeModel);
        }
    }
}
