using AutoRest.Core;
using AutoRest.Core.Model;
using AutoRest.Extensions;
using System;
using System.Collections.Generic;
using static AutoRest.Core.Utilities.DependencyInjection;

namespace AutoRest.Terraform
{
    internal sealed class TransformerTf
        : CodeModelTransformer<CodeModelTf>
    {
        private IEnumerable<ITransformer<CodeModelTf>> CreateTransformers()
        {
            var metadata = Singleton<SettingsTf>.Instance.Metadata;
            yield return new MethodFilterTransformer
            {
                Acceptances = new Dictionary<string, Action<MethodTf>>
                {
                    { metadata.CreateMethod.Path, m => CodeModel.CreateMethod = m },
                    { metadata.ReadMethod.Path, m => CodeModel.ReadMethod = m },
                    { metadata.UpdateMethod.Path, m => CodeModel.UpdateMethod = m },
                    { metadata.DeleteMethod.Path, m => CodeModel.DeleteMethod = m },
                }
            };
            yield return new ParameterFilterTransformer
            {
                Method = CodeModel.CreateMethod,
                IgnoringPatterns = new HashSet<string>(metadata.CreateMethod.Parameter.Excludes)
            };
            yield return new ParameterFilterTransformer
            {
                Method = CodeModel.ReadMethod,
                IgnoringPatterns = new HashSet<string>(metadata.ReadMethod.Parameter.Excludes)
            };
            yield return new ParameterFilterTransformer
            {
                Method = CodeModel.UpdateMethod,
                IgnoringPatterns = new HashSet<string>(metadata.UpdateMethod.Parameter.Excludes)
            };
            yield return new ParameterFilterTransformer
            {
                Method = CodeModel.DeleteMethod,
                IgnoringPatterns = new HashSet<string>(metadata.DeleteMethod.Parameter.Excludes)
            };
            yield return new ResourceNamesTransformer
            {
                ResourceNamePattern = metadata.CreateMethod.Parameter.ResourceName,
                ResourceGroupNamePattern = metadata.CreateMethod.Parameter.ResourceGroupName
            };
        }

        private CodeModelTf CodeModel { get; set; }

        public override CodeModelTf TransformCodeModel(CodeModel codeModel)
        {
            CodeModel = base.TransformCodeModel(codeModel);
            SwaggerExtensions.NormalizeClientModel(CodeModel);
            foreach (var transformer in CreateTransformers())
            {
                CodeModel = transformer.TransformCodeModel(CodeModel);
            }
            SwaggerExtensions.NormalizeClientModel(CodeModel);
            return CodeModel;
        }
    }
}
