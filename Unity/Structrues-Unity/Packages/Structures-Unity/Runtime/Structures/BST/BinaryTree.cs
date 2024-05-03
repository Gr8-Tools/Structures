/*using System;

namespace NuclearGames.StructuresUnity.Structures.BST {
    /// <summary>
    /// Двоичное дерево
    /// </summary>
    public class BinaryTree<T> : BaseBinaryTree<T, T> where T : IComparable<T> {
        
        public BinaryTree() : base(SelfSelector) { }
        
        public BinaryTree(T[] sourceBuffer) : base(sourceBuffer, SelfSelector) { }

        private static T SelfSelector(T data) => data;
    }
}*/