using AutoRest.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace AutoRest.Terraform
{
    internal class VisitedEventArgs<T>
        : EventArgs
    {
        public VisitedEventArgs(T node) => Node = node;
        public T Node { get; }
    }

    internal class VisitingEventArgs<T>
        : VisitedEventArgs<T>
    {
        public VisitingEventArgs(T node)
            : base(node)
        {
        }
    }

    /// <summary>
    /// The base class of all kinds of visitors. Make sure all your Visit methods invoke one of the
    /// <see cref="VisitorBase.Visit{TNode, TChild}(TNode, IEnumerable{TChild}, Action{TNode}, Action{TChild}, Action{TNode})"/> method.
    /// </summary>
    internal abstract class VisitorBase
    {
        protected void OnVisiting<T>(EventHandler<VisitingEventArgs<T>> handler, T node) => handler?.Invoke(this, new VisitingEventArgs<T>(node));
        protected void OnVisited<T>(EventHandler<VisitedEventArgs<T>> handler, T node) => handler?.Invoke(this, new VisitedEventArgs<T>(node));

        protected void Visit<TNode>(TNode node, Action<TNode> visiting, Action<TNode> visited)
            => Visit(node, Enumerable.Empty<object>(), visiting, null, visited);

        protected void Visit<TNode, TChild>(TNode node, TChild child, Action<TNode> visiting, Action<TChild> visitChild, Action<TNode> visited)
            => Visit(node, Enumerable.Repeat(child, 1), visiting, visitChild, visited);

        protected void Visit<TNode, TChild>(TNode node, IEnumerable<TChild> children, Action<TNode> visiting, Action<TChild> visitChild, Action<TNode> visited)
        {
            Debug.Assert(children != null);
            Debug.Assert(visiting != null && visited != null);
            Debug.Assert(!children.Any() || visitChild != null);
            visiting(node);
            children.ForEach(visitChild);
            visited(node);
        }
    }
}
