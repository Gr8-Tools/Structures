/*using System.Collections.Generic;
using System.Linq;
using NuclearGames.StructuresUnity.Structures.BCST;
using NuclearGames.StructuresUnity.Utils.Collections;
using NUnit.Framework;

namespace NuclearGames.StructuresUnity.Tests.Editor.Structures.BCST {
    public class BinaryCollectionTreeUnitTests {
        private readonly int[] _dataCollection = new int[] {
            9, 4, 17, 3, 6, 22, 5 ,7, 20
        };

        private static int ComparablePredicate(int value) => value;

        private BinaryCollectionTree<int, int, AnyElementList<int>> _tree;

        [SetUp]
        public void SetUp() {
            _tree = new ArrayBinaryCollectionTree<int, int>(ComparablePredicate);
        }
        
        [TestCase(17)]
        public void InitialTests(int data) {
            Assert.AreEqual(true, _tree.IsEmpty());
            Assert.AreEqual(false, _tree.TryFind(data, out _));
            Assert.AreEqual(false, _tree.TryFindMin(out _));
            Assert.AreEqual(false, _tree.TryFindMax(out _));
            Assert.AreEqual(false, _tree.TryRemove(data));
            
            Assert.AreEqual(0, _tree.ToArray().Length);
            Assert.AreEqual(0, _tree.DataCount);
            Assert.AreEqual(0, _tree.NodesCount);
        }
        
        [Test]
        public void ConstructorTest([Values(10, 11, 1000, 1001)]int count) {
            var initial = Enumerable.Range(0, count).ToArray();
            var newTree = new LinkedBinaryCollectionTree<int, int>(ComparablePredicate, initial);
            
            Assert.AreEqual(initial.Length, newTree.ToArray().Length);
            Assert.AreEqual(initial.Length, newTree.DataCount);
            Assert.AreEqual(initial.Length, newTree.NodesCount);
        }
        
        [TestCase]
        public void SuccessfulAdd() {
            FillTree(true);
            FillTree(true, 2);
        }
        
        [Test]
        public void SuccessfulAddRange([Values(10, 11, 1000, 1001)]int count) {
            var sourceBuffer = Enumerable.Range(0, count).ToArray();
            var newTree = new LinkedBinaryCollectionTree<int, int>(ComparablePredicate, sourceBuffer);
            
            Assert.AreEqual(sourceBuffer.Length, newTree.ToArray().Length);
            Assert.AreEqual(sourceBuffer.Length, newTree.DataCount);
            Assert.AreEqual(sourceBuffer.Length, newTree.NodesCount);

            var newSourceBuffer = Enumerable.Range(count, count).ToArray();
            newTree.AddRange(newSourceBuffer);
            
            Assert.AreEqual(count + count, newTree.ToArray().Length);
            Assert.AreEqual(count + count, newTree.DataCount);
            Assert.AreEqual(count + count, newTree.NodesCount);
        }
        
        [TestCase]
        public void FoundAfter() {
            FillTree();
            void AssertResults(bool result, CollectionNode<int, AnyElementList<int>>? internalNode, int? value) {
                Assert.AreEqual(value.HasValue, result);
                if (value.HasValue) {
                    Assert.IsNotNull(internalNode);
                    Assert.AreEqual(true, internalNode!.Contains(value.Value));
                } else {
                    Assert.IsNull(internalNode);
                }
            }

            var findResult = _tree.TryFind(4, out var searchNode);
            AssertResults(findResult, searchNode, 4);
            findResult = _tree.TryFindByKey(4, out searchNode);
            AssertResults(findResult, searchNode, 4);
            
            findResult = _tree.TryFind(19, out searchNode);
            AssertResults(findResult, searchNode, null);
            findResult = _tree.TryFindByKey(19, out searchNode);
            AssertResults(findResult, searchNode, null);
            
            var minResult = _tree.TryFindMin(out var minNode);
            AssertResults(minResult, minNode, 3);
            minResult = _tree.TryFindByKey(3, out minNode);
            AssertResults(minResult, minNode, 3);
            
            var maxResult = _tree.TryFindMax(out var maxNode);
            AssertResults(maxResult, maxNode, 22);
            maxResult = _tree.TryFindByKey(3, out maxNode);
            AssertResults(maxResult, maxNode, 3);
        }
        
        [TestCase]
        public void Children() {
            FillTree();
            CheckChild(6, 5, 7);
            CheckChild(3, null, null);
            CheckChild(17, null, 22);
            CheckChild(22, 20, null);
        }
        
        [TestCase]
        public void IterationDataIsOrdered() {
            FillTree();
            FillTree();

            var values = _tree.ToArray();
            
            Assert.AreEqual(_dataCollection.Length * 2, values.Length);
            
            for (int i = 0; i < values.Length - 1; i++) {
                Assert.LessOrEqual(values[i], values[i+1]);
            }
        }
        
        [TestCase]
        public void IterationNodeIsOrdered() {
            FillTree();
            FillTree();

            var nodes = _tree.Iterate().ToArray();
            
            Assert.AreEqual(_dataCollection.Length, nodes.Length);
            
            for (int i = 0; i < nodes.Length - 1; i++) {
                Assert.AreEqual(2, nodes[i].Count);
                Assert.LessOrEqual(nodes[i].Any, nodes[i+1].First());
            }
        }
        
        [TestCase]
        public void Clear() {
            FillTree();
            Assert.AreEqual(false, _tree.IsEmpty());
            _tree.Clear();
            Assert.AreEqual(true, _tree.IsEmpty());
        }
        
        [TestCase()]
        public void FindHeight() {
            FillTree();
            Assert.AreEqual(1, _tree.FindMinHeight());
            Assert.AreEqual(3, _tree.FindMaxHeight());
            Assert.AreEqual(false, _tree.IsBalanced());
            
            _tree.Clear();
            Assert.AreEqual(-1, _tree.FindMinHeight());
            Assert.AreEqual(-1, _tree.FindMaxHeight());
            Assert.AreEqual(true, _tree.IsBalanced());
        }
        
        [TestCase]
        public void Remove() {
            FillTree();

            var dataCount = _tree.DataCount;
            var nodeCount = _tree.NodesCount;
            
            var removeResult = _tree.TryRemove(3);
            Assert.AreEqual(true, removeResult);
            Assert.AreEqual(--dataCount, _tree.DataCount);
            Assert.AreEqual(--nodeCount, _tree.NodesCount);
            
            removeResult = _tree.TryRemove(3);
            Assert.AreEqual(false, removeResult);
            Assert.AreEqual(dataCount, _tree.DataCount);
            Assert.AreEqual(nodeCount, _tree.NodesCount);

            removeResult = _tree.TryRemove(6);
            Assert.AreEqual(true, removeResult);
            Assert.AreEqual(--dataCount, _tree.DataCount);
            Assert.AreEqual(--nodeCount, _tree.NodesCount);

            removeResult = _tree.TryRemove(17);
            Assert.AreEqual(true, removeResult);
            Assert.AreEqual(--dataCount, _tree.DataCount);
            Assert.AreEqual(--nodeCount, _tree.NodesCount);

            _tree.TryFind(4, out var node);
            Assert.IsNotNull(node);
            Assert.IsNull(node!.Left);
            Assert.IsNotNull(node.Right);
            Assert.AreEqual(true, node.Right!.Contains(7));
            Assert.IsNull(node.Right!.Right);
            Assert.IsNotNull(node.Right!.Left);
            Assert.AreEqual(true, node.Right!.Left!.Contains(5));

            _tree.TryFind(22, out node);
            Assert.IsNotNull(node);
            Assert.IsNull(node!.Right);
            Assert.IsNotNull(node.Left);
            Assert.AreEqual(true, node.Left!.Contains(20));
            Assert.IsNull(node.Left!.Left);
            Assert.IsNull(node.Left!.Right);

            _tree.Add(50, out _);
            _tree.Add(30, out _);
            nodeCount += 2;
            dataCount += 2;
            Assert.AreEqual(dataCount, _tree.DataCount);
            Assert.AreEqual(nodeCount, _tree.NodesCount);
            
            removeResult = _tree.TryRemove(22);
            Assert.AreEqual(true, removeResult);
            Assert.AreEqual(--dataCount, _tree.DataCount);
            Assert.AreEqual(--nodeCount, _tree.NodesCount);

            _tree.TryFind(30, out node);
            Assert.IsNotNull(node);
            Assert.IsNotNull(node!.Left);
            Assert.IsNotNull(node.Right);
            Assert.AreEqual(true, node.Left!.Contains(20));
            Assert.AreEqual(true, node.Right!.Contains(50));
            
            
            _tree.Add(20, out node);
            Assert.AreEqual(++dataCount, _tree.DataCount);
            Assert.AreEqual(nodeCount, _tree.NodesCount);
            Assert.IsNotNull(node);
            Assert.AreEqual(2, node.Count);
            removeResult = _tree.TryRemove(20);
            Assert.AreEqual(true, removeResult);
            Assert.AreEqual(true, _tree.TryFind(20, out _));
            Assert.AreEqual(--dataCount, _tree.DataCount);
            Assert.AreEqual(nodeCount, _tree.NodesCount);
            
            _tree.Add(50, out node);
            Assert.IsNotNull(node);
            Assert.AreEqual(2, node.Count);
            Assert.AreEqual(++dataCount, _tree.DataCount);
            Assert.AreEqual(nodeCount, _tree.NodesCount);
            removeResult = _tree.TryRemoveNode(50);
            Assert.AreEqual(true, removeResult);
            Assert.AreEqual(false, _tree.TryFind(50, out _));
            dataCount -= 2;
            Assert.AreEqual(dataCount, _tree.DataCount);
            Assert.AreEqual(--nodeCount, _tree.NodesCount);
        }

        [Test]
        public void CollectionNodeTest() {
            FillTree();
            var root = _tree.Root;
            
            Assert.AreEqual(false, root!.IsEmpty);
            Assert.AreEqual(false, root.IsReadOnly);
            
        }

        [TestCase(new int[]{ 9, 4, 17, 3, 6, 22, 5 ,7, 20, 7, 17, 3, 22, 5})]
        public void IterationTest(int[] values) {
            _tree.AddRange(values);
            
            var list = new List<int>(values);
            list.Sort();

            var index = 0;
            foreach (var treeValue in _tree) {
                Assert.AreEqual(list[index], treeValue);
                index++;
            }
        }
        
        /// <summary>
        /// Проверяет:
        /// - ноды извлекаются в правильном порядке.
        /// </summary>
        [Test]
        public void TryDequeueTest() {
            _tree.Add(6, out _);
            _tree.Add(10, out _);
            _tree.Add(9, out _);
            _tree.Add(4, out _);
            _tree.Add(6, out _);
            _tree.Add(5, out _);
            _tree.Add(7, out _);
            _tree.Add(4, out _);
            _tree.Add(12, out _);
            _tree.Add(11, out _);

            int[] dequeueOrder = new int[] { 4, 5, 6, 7, 9, 10, 11, 12 };
            int nodesCount = dequeueOrder.Length;

            foreach (int expected in dequeueOrder) {
                Assert.That(_tree.NodesCount, Is.EqualTo(nodesCount--));
                Assert.That(_tree.TryDequeue(out var collection), Is.True);
                collection!.ForEach(v => Assert.That(v, Is.EqualTo(expected)));
            }
            Assert.That(_tree.NodesCount, Is.EqualTo(nodesCount--));
            Assert.That(_tree.TryDequeue(out _), Is.False);
        }

#region Utils

        private void FillTree(in bool checkAssertion = false, in int checkCount = 1) {
            for (int i = 0; i < _dataCollection.Length; i++) {
                var value = _dataCollection[i];
                _tree.Add(value, out var newNode);
                if (checkAssertion) {
                    Assert.IsNotNull(newNode);
                    Assert.AreEqual(true, newNode.Contains(value));
                    Assert.AreEqual(checkCount, newNode.Count);
                }
            }

            if (checkAssertion) {
                Assert.AreEqual(_dataCollection.Length, _tree.NodesCount);
                Assert.AreEqual(_dataCollection.Length * checkCount, _tree.DataCount);
            }
        }
        
        private void CheckChild(int data, int? left, int? right) {
            if (_tree.TryFind(data, out var node)) {
                Assert.IsNotNull(node);

                if (left.HasValue) {
                    Assert.IsNotNull(node!.Left);
                    Assert.AreEqual(true, node.Left!.Contains(left.Value));
                } else {
                    Assert.IsNull(node!.Left);
                }
                
                if (right.HasValue) {
                    Assert.IsNotNull(node.Right);
                    Assert.AreEqual(true, node.Right!.Contains(right.Value));
                } else {
                    Assert.IsNull(node.Right);
                }
            } else {
                Assert.Fail($"Value '{data}' wasn't found in tree!");
            }
        }

#endregion
    }
}*/