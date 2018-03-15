using System;
using System.Collections.Generic;
using System.Text;

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

    internal abstract class VisitorBase
    {
        protected void OnVisiting<T>(EventHandler<VisitingEventArgs<T>> handler, T node) => handler?.Invoke(this, new VisitingEventArgs<T>(node));
        protected void OnVisited<T>(EventHandler<VisitedEventArgs<T>> handler, T node) => handler?.Invoke(this, new VisitedEventArgs<T>(node));
    }
}
