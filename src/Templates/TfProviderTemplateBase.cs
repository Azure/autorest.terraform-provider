using AutoRest.Core;
using System;
using System.Linq;

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
    }
}
