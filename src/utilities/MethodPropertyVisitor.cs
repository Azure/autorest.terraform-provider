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
        public MethodPropertyVisitor()
        {
            ChildrenTypeVisitor = new ModelTypeVisitor();
            ChildrenTypeVisitor.ComplexVisited += ChildrenTypeVisitor_ComplexVisited;
        }

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
                root.LogicalResponses.ForEach(Visit);
            }
        }


        private void Visit(ParameterTf root) => Visit(root, GetTypeChildren(root.ModelType), OnParameterVisiting, Visit, OnParameterVisited);
        public event EventHandler<VisitingEventArgs<ParameterTf>> ParameterVisiting;
        public event EventHandler<VisitedEventArgs<ParameterTf>> ParameterVisited;
        protected virtual void OnParameterVisiting(ParameterTf parameter) => OnVisiting(ParameterVisiting, parameter);
        protected virtual void OnParameterVisited(ParameterTf parameter) => OnVisited(ParameterVisited, parameter);


        private void Visit(ResponseTf root) => Visit(root, GetTypeChildren(root.BodyType), OnResponseVisiting, Visit, OnResponseVisited);
        public event EventHandler<VisitingEventArgs<ResponseTf>> ResponseVisiting;
        public event EventHandler<VisitedEventArgs<ResponseTf>> ResponseVisited;
        protected virtual void OnResponseVisiting(ResponseTf response) => OnVisiting(ResponseVisiting, response);
        protected virtual void OnResponseVisited(ResponseTf response) => OnVisited(ResponseVisited, response);


        private void Visit(Property root) => Visit(root, GetTypeChildren(root.ModelType), OnPropertyVisiting, Visit, OnPropertyVisited);
        public event EventHandler<VisitingEventArgs<Property>> PropertyVisiting;
        public event EventHandler<VisitedEventArgs<Property>> PropertyVisited;
        protected virtual void OnPropertyVisiting(Property property) => OnVisiting(PropertyVisiting, property);
        protected virtual void OnPropertyVisited(Property property) => OnVisited(PropertyVisited, property);


        private ModelTypeVisitor ChildrenTypeVisitor { get; }

        private void ChildrenTypeVisitor_ComplexVisited(object sender, VisitedEventArgs<CompositeType> e)
            => TypeChildren = e.Node.ComposedProperties;

        private IEnumerable<Property> TypeChildren { get; set; }

        private IEnumerable<Property> GetTypeChildren(IModelType type)
        {
            TypeChildren = Enumerable.Empty<Property>();
            ChildrenTypeVisitor.Visit(type);
            return TypeChildren;
        }
    }
}
