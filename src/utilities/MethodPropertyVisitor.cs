using AutoRest.Core.Model;
using AutoRest.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AutoRest.Terraform
{
    /// <summary>
    /// The visitor which walks through every (recursive) <see cref="PropertyTf"/> under <see cref="ParameterTf"/> and <see cref="Response"/> of a <see cref="MethodTf"/>.
    /// </summary>
    internal class MethodPropertyVisitor
        : VisitorBase
    {
        public bool SkipParameter { get; set; } = false;

        public bool SkipResponse { get; set; } = false;

        public void Visit(MethodTf root)
        {
            if (!SkipParameter)
            {
                root.LogicalParameters.Cast<ParameterTf>().ForEach(Visit);
            }
            if (!SkipResponse)
            {
                // TODO
            }
        }


        private void Visit(ParameterTf root) => VisitWorker(root, OnParameterVisiting, OnParameterVisited);
        public event EventHandler<VisitingEventArgs<ParameterTf>> ParameterVisiting;
        public event EventHandler<VisitedEventArgs<ParameterTf>> ParameterVisited;
        protected virtual void OnParameterVisiting(ParameterTf parameter, bool isLeaf) => OnVisiting(ParameterVisiting, parameter);
        protected virtual void OnParameterVisited(ParameterTf parameter, bool isLeaf) => OnVisited(ParameterVisited, parameter);


        private void Visit(PropertyTf root) => VisitWorker(root, OnPropertyVisiting, OnPropertyVisited);
        public event EventHandler<VisitingEventArgs<PropertyTf>> PropertyVisiting;
        public event EventHandler<VisitedEventArgs<PropertyTf>> PropertyVisited;
        protected virtual void OnPropertyVisiting(PropertyTf property, bool isLeaf) => OnVisiting(PropertyVisiting, property);
        protected virtual void OnPropertyVisited(PropertyTf property, bool isLeaf) => OnVisited(PropertyVisited, property);


        private ModelTypeVisitor TypeVisitor { get; } = new ModelTypeVisitor();

        private void VisitWorker<TNode, TChild>(TNode node,IEnumerable<TChild> children, Action<TNode, bool> visiting, Action<TChild> visitChild, Action<TNode, bool> visited)
        {
            var isLeaf = !children.Any();
            visiting(node, isLeaf);
            children.ForEach(c => visitChild(c));
            visited(node, isLeaf);
        }

        private void VisitWorker<TNode>(TNode node, Action<TNode, bool> visiting, Action<TNode, bool> visited)
            where TNode: IVariable
        {
            var children = Enumerable.Empty<Property>();

            void TypeVisitor_ComplexVisited(object sender, VisitedEventArgs<CompositeTypeTf> e)
            {
                children = e.Node.ComposedProperties;
                TypeVisitor.ComplexVisited -= TypeVisitor_ComplexVisited;
            }
            TypeVisitor.ComplexVisited += TypeVisitor_ComplexVisited;
            TypeVisitor.Visit(node.ModelType);

            VisitWorker(node, children.Cast<PropertyTf>(), visiting, Visit, visited);
        }
    }
}
