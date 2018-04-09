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

    internal abstract class TfProviderTransformerBase
        : ITfProviderTransformer
    {
        public void Transform(CodeModelTf model)
        {
            CodeModel = model;
            TransformCore();
        }

        protected CodeModelTf CodeModel { get; private set; }
        protected SettingsTf Settings => Singleton<SettingsTf>.Instance;

        protected abstract void TransformCore();
    }

    internal sealed class TransformerTf
        : CodeModelTransformer<CodeModelTf>
    {
        private IEnumerable<ITfProviderTransformer> CreateTransformers()
        {
            return new ITfProviderTransformer[]
            {
                new SDKTuningTransformer(),
                new TypeDefinitionsTransformer(),
                new NewInvocationsTransformer(),
                new ArgumentsFilterCloneTransformer(),
                new NewFlattenFieldsTransformer(),
                new NormalizeTransformer()
            };
        }

        public override CodeModelTf TransformCodeModel(CodeModel codeModel)
        {
            var model = base.TransformCodeModel(codeModel);
            SwaggerExtensions.NormalizeClientModel(model);
            CreateTransformers().ForEach(tr => tr.Transform(model));
            return model;
        }
    }
}
