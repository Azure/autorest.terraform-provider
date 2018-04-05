using AutoRest.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace AutoRest.Terraform
{
    public interface ITreeNode<T>
        where T : ITreeNode<T>
    {
        IEnumerable<T> Children { get; }
        T Parent { get; }
    }

    internal enum TraverseType
    {
        PreOrder, Ancestors
    }

    internal static partial class Utilities
    {
        public static IEnumerable<T> Traverse<T>(this T root, TraverseType traverseType)
            where T : ITreeNode<T>
        {
            switch (traverseType)
            {
                case TraverseType.PreOrder:
                    return root.PreOrderTraverse();
                case TraverseType.Ancestors:
                    return root.AncestorsTraverse();
                default:
                    throw new NotSupportedException(traverseType.ToString());
            }
        }

        private static IEnumerable<T> PreOrderTraverse<T>(this T root)
            where T : ITreeNode<T>
        {
            Debug.Assert(root != null);
            var nodesToVisit = new Stack<T>(root.Children);
            while (nodesToVisit.TryPop(out T node))
            {
                yield return node;
                node.Children.ForEach(n => nodesToVisit.Push(n));
            }
        }

        private static IEnumerable<T> AncestorsTraverse<T>(this T node)
            where T : ITreeNode<T>
        {
            Debug.Assert(node != null);
            while (node != null)
            {
                yield return node;
                node = node.Parent;
            }
        }
    }
}
