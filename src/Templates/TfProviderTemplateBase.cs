using AutoRest.Core;
using System;
using System.Linq;
using static AutoRest.Core.Utilities.DependencyInjection;

namespace AutoRest.Terraform
{
    public sealed class TemplateIndentation
    {
        public TemplateIndentation(string indentation = "\t") => IndentationWord = indentation;

        private string IndentationWord { get; }
        private int IndentationLevel { get; set; }

        public void Indent() => IndentationLevel++;
        public void Outdent() => IndentationLevel = Math.Max(IndentationLevel - 1, 0);

        public override string ToString() => string.Join(string.Empty, Enumerable.Repeat(IndentationWord, IndentationLevel));
    }

    public class TfProviderTemplateBase<T>
        : Template<T>
    {
        public TfProviderTemplateBase()
            : base() => Indentation = new TemplateIndentation();

        protected new TemplateIndentation Indentation { get; set; }

        private protected V OtherModel<V>()
            where V : ITfProviderGenerator
            => Singleton<V>.Instance;

        protected string Include<TTemplate, TModel>(TModel model, bool inheritIndentation = true)
            where TTemplate : TfProviderTemplateBase<TModel>, new()
        {
            var template = new TTemplate();
            if (inheritIndentation)
            {
                template.Indentation = Indentation;
            }
            return Include(template, model);
        }
    }
}
