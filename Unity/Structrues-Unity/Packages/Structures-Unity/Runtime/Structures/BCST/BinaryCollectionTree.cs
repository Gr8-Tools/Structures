/*using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using NuclearGames.StructuresUnity.Structures.Collections.NonAllocated;
using NuclearGames.StructuresUnity.Utils.Collections.Interfaces;
using UnityEngine;

namespace NuclearGames.StructuresUnity.Structures.BCST {
    public class BinaryCollectionTree<TData, TComparable, TCollection> : IEnumerable<TData> 
        where TComparable : IComparable<TComparable> 
        where TCollection : IAnyElementCollection<TData>, new() {
        
        /// <summary>
        /// Кол-во узлов в дереве
        /// </summary>
        public int NodesCount { get; private set; }
        
        /// <summary>
        /// Кол-во элементов в дереве
        /// </summary>
        public long DataCount { get; private set; }
        
        /// <summary>
        /// Корневой узел дерева
        /// </summary>
        public CollectionNode<TData, TCollection>? Root { get; private set; }

        /// <summary>
        /// Является ли дерево пустым
        /// </summary>
        public bool IsEmpty() => ReferenceEquals(Root, null);

        private readonly Func<TData, TComparable> _compareFieldSelector;

        public BinaryCollectionTree(Func<TData, TComparable> compareFieldSelector) {
            _compareFieldSelector = compareFieldSelector;
        }

        public BinaryCollectionTree(Func<TData, TComparable> compareFieldSelector, TData[] sourceBuffer) {
            _compareFieldSelector = compareFieldSelector;

            AddRange(sourceBuffer);
        }
        
        /// <summary>
        /// Добавляет данные в дерево
        /// <para>Если подходящий узел с данными уже существует, то данные добавляются в коллекцию этого узла</para>
        /// </summary>
        public void Add(TData data, out CollectionNode<TData, TCollection>? resultNode) {
            if (IsEmpty()) {
                Root = GetNode(data);
                resultNode = Root;
                NodesCount++;
                DataCount++;

                return;
            }

            var node = Root!;
            resultNode = null;
            var dataValue = _compareFieldSelector(data);

            bool found = false;
            while (!found) {
                var compareResult = Compare(dataValue, node);
                if (compareResult < 0) {
                    if (node.Left == null) {
                        node.Left = GetNode(data);
                        resultNode = node.Left!;
                        NodesCount++;
                        found = true;
                    } else {
                        node = node.Left;
                    }
                } else if (compareResult > 0) {
                    if (node.Right == null) {
                        node.Right = GetNode(data);
                        resultNode = node.Right!;
                        NodesCount++;
                        found = true;
                    } else {
                        node = node.Right;
                    }
                } else {
                    node.Add(data);
                    resultNode = node;
                    found = true;
                }
            }
            
            DataCount++;
        }

        /// <summary>
        /// Добавляет массив элементов. Построено на предположении, что <paramref name="sourceBuffer"/> упорядочен по возрастнию. 
        /// </summary>
        /// <returns>Был ли добавлен хотя бы один элемент</returns>
        public void AddRange(TData[] sourceBuffer) {
            
            switch (sourceBuffer.Length) {
                case 0:
                    return;
                case 1:
                    Add(sourceBuffer[0], out _);
                    break;
                default:
                    var stackSize = (int)Mathf.Log(sourceBuffer.Length, 2);
                    
                    unsafe {
                        void* n = stackalloc AddRangeItem*[stackSize];
                        var stackBuffer = new NonAllocatedStack<AddRangeItem>(n, stackSize);

                        stackBuffer.Push(new AddRangeItem(sourceBuffer.Length / 2, sourceBuffer.Length / 2));

                        int maxStackSize = 1;

                        while (stackBuffer.TryPop(out var addRangeItem)) {
                            if (addRangeItem.Index < 0 || addRangeItem.Index >= sourceBuffer.Length) {
                                continue;
                            }

                            Add(sourceBuffer[addRangeItem.Index], out _);

                            if (addRangeItem.Window == 1) {
                                continue;
                            }

                            var newWindow = addRangeItem.Window / 2;
                            if (addRangeItem.Window % 2 == 0) {
                                stackBuffer.Push(new AddRangeItem(addRangeItem.Index + newWindow, newWindow));
                                stackBuffer.Push(new AddRangeItem(addRangeItem.Index - newWindow, newWindow));
                            } else {
                                Add(sourceBuffer[addRangeItem.Index + newWindow * 2], out _);
                                Add(sourceBuffer[addRangeItem.Index - 1], out _);

                                stackBuffer.Push(new AddRangeItem(addRangeItem.Index + newWindow, newWindow));
                                stackBuffer.Push(new AddRangeItem(addRangeItem.Index - newWindow - 1, newWindow));
                            }

                            maxStackSize = Math.Max(maxStackSize, stackBuffer.Count);
                        }

                        Add(sourceBuffer[0], out _);
                        if (sourceBuffer.Length % 2 == 1) {
                            Add(sourceBuffer[sourceBuffer.Length - 1], out _);
                        }

                        break;
                    }
            }
        }

        /// <summary>
        /// Ищет минимальный элемент в дереве
        /// </summary>
        /// <param name="resultNode">Узел дерева с минимальными данными</param>
        /// <returns>Найден такой узел (true) или нет (fasle)</returns>
        public bool TryFindMin(out CollectionNode<TData, TCollection>? resultNode) {
            if (IsEmpty()) {
                resultNode = null;

                return false;
            }

            resultNode = Root!;
            while (resultNode.Left != null) {
                resultNode = resultNode.Left;
            }

            return true;
        }

        /// <summary>
        /// Ищет максимальный элемент в дереве
        /// </summary>
        /// <param name="resultNode">Узел дерева с максимальными данными</param>
        /// <returns>Найден такой узел (true) или нет (fasle)</returns>
        public bool TryFindMax(out CollectionNode<TData, TCollection>? resultNode) {
            if (IsEmpty()) {
                resultNode = null;

                return false;
            }

            resultNode = Root!;
            while (resultNode.Right != null) {
                resultNode = resultNode.Right;
            }

            return true;
        }

        /// <summary>
        /// Пытается найти узел по данным
        /// </summary>
        /// <param name="data">Данные поиска</param>
        /// <param name="resultNode">Узел с данными (если был найден) или null</param>
        /// <returns>True, если узел найден; False - если узел не найден</returns>
        /// <exception cref="ArgumentNullException">Недопустимая ошибка сравнения при обходе дерева</exception>
        public bool TryFind(TData data, out CollectionNode<TData, TCollection>? resultNode) {
            var dataValue = _compareFieldSelector(data);
            return TryFindByKey(dataValue, out resultNode);
        }
        
        /// <summary>
        /// Пытается найти узел по ключу сравнения
        /// </summary>
        /// <param name="comparableKey">Ключ сравнения</param>
        /// <param name="resultNode">Узел с данными (если был найден) или null</param>
        /// <returns>True, если узел найден; False - если узел не найден</returns>
        /// <exception cref="ArgumentNullException">Недопустимая ошибка сравнения при обходе дерева</exception>
        public bool TryFindByKey(TComparable comparableKey, out CollectionNode<TData, TCollection>? resultNode) {
            if (IsEmpty()) {
                resultNode = null;

                return false;
            }

            resultNode = Root!;
            int compareResult;
            while ((compareResult = Compare(comparableKey, resultNode)) != 0) {
                if (compareResult < 0) {
                    resultNode = resultNode.Left;
                } else if (compareResult > 0) {
                    resultNode = resultNode.Right;
                } else {
                    throw new ArgumentNullException(nameof(resultNode));
                }
                
                if (resultNode == null) {
                    return false;
                }
            }

            return true;
        }


        /// <summary>
        /// Пытается удалить данные из дерева.
        /// <para>Если после удаления данных из узла других данных в нем не остается - удаляет узел</para>
        /// <param name="data">Данные поиска</param>
        /// </summary>
        public bool TryRemove(TData data) {
            // Удаляет данные из узла и блокирует удаление узла, если в узле остались другие данные
            bool OnElementFound(CollectionNode<TData, TCollection> node) {
                if (node.Remove(data)) {
                    DataCount--;
                }
                return node.Count == 0;
            }

            return RemoveInternal(_compareFieldSelector(data), OnElementFound, out _);
        }

        /// <summary>
        /// Пытается удалить узел из дерева.
        /// <param name="comparableKey">Ключ поиска по дереву</param>
        /// </summary>
        public bool TryRemoveNode(TComparable comparableKey) {
            bool OnElementFound(CollectionNode<TData, TCollection> node) {
                DataCount -= node.Count;
                node.Clear();

                return true;
            }

            return RemoveInternal(comparableKey, OnElementFound, out _);
        }

        /// <summary>
        /// Пытается удалить узел дерева по данным
        /// </summary>
        /// <param name="actionOnRemove"></param>
        /// <param name="removeNode"></param>
        /// <param name="compareValue"></param>
        /// <returns>True - данные найдены и удалить получилось. False - данные найдены не были </returns>
        private protected virtual bool RemoveInternal(TComparable compareValue, Func<CollectionNode<TData, TCollection>, bool> actionOnRemove, [CanBeNull] out CollectionNode<TData, TCollection> removeNode) {
            bool result = false;
            CollectionNode<TData, TCollection>? temp = Root, parent = null;
            
            // Проверяем, пустое ли дерево
            if (temp == null) {
                removeNode = null;
                return false;
            }

            
            int? prevCompareResult, compareResult = null;
            
            while (temp != null) {
                prevCompareResult = compareResult;
                compareResult = Compare(compareValue, temp);
                if (compareResult > 0) {
                    parent = temp;
                    temp = temp.Right;
                } else if (compareResult < 0) {
                    parent = temp;
                    temp = temp.Left;
                } else {
                    // Выполняем действие при нахождении элемента под удаление
                    bool needRemove = actionOnRemove(temp);
                    if (!needRemove) {
                        removeNode = temp;
                        return true;
                    }
                    
                    CollectionNode<TData, TCollection>? SwitchNull() => null;

                    CollectionNode<TData, TCollection>? SwitchLeaf() {
                        return temp.Left ?? temp.Right;
                    }
                    
                    void ReplaceLeaf(bool withNull) {
                        var newLeaf = withNull ? SwitchNull() : SwitchLeaf();
                        
                        if (prevCompareResult.HasValue) {
                            if (prevCompareResult < 0) {
                                parent!.Left = newLeaf;
                            } else {
                                parent!.Right = newLeaf;
                            }
                        } else {
                            Root = newLeaf;
                        }
                    }
                    
                    if (temp.Left == null && temp.Right == null) {
                        ReplaceLeaf(true);
                        removeNode = temp;
                    } else if (temp.Left == null || temp.Right == null) {
                        ReplaceLeaf(false);
                        removeNode = temp;
                    } else {
                        var parent2 = temp;
                        var temp2 = temp.Right;
                        while (temp2.Left != null) {
                            parent2 = temp2;
                            temp2 = temp2.Left;
                        }

                        // меняем данные в нодах
                        temp.Clear();
                        temp.AddRange(temp2);
                        
                        // удаляем нод
                        if (parent2 == temp) {
                            parent2.Right = null;
                        } else {
                            parent2.Left = null;
                        }

                        removeNode = temp2;
                    }

                    ReleaseNode(removeNode);
                    result = true;

                    break;
                }
            }

            if (result) {
                NodesCount--;
            }

            removeNode = null;
            return result;
        }
        
        /// <summary>
        /// Пытается извлечь нод с минимальным значением.
        /// </summary>
        /// <param name="resultCollection">Извлеченный нод или NULL.</param>
        /// <returns>True - нод был извлечен; False - нет нод.</returns>
        public virtual bool TryDequeue([CanBeNull] out TCollection resultCollection) {
            if (IsEmpty()) {
                resultCollection = default;
                return false;
            }

            // Добегаем до последнего левого нода (минимальное значение).
            CollectionNode<TData, TCollection>? prev = null;
            CollectionNode<TData, TCollection> current = Root!;
            while (current.Left != null) {
                prev = current;
                current = current.Left;
            }

            if (prev == null) {
                // Делаем правый нод корня новым корневым.
                // Если правого нода не было - прекинется NULL.
                Root = current.Right;
            } else {
                // Перекидываем правый нод наименьшего на его родителя.
                // Если правого нода не было - прекинется NULL.
                prev.Left = current.Right;
            }

            NodesCount--;

            resultCollection = new TCollection();
            current.CopyTo(resultCollection);
            
            ReleaseNode(current);

            return true;
        }

        /// <summary>
        /// Очишает все дерево
        /// </summary>
        public void Clear() {
            Root = null;
            NodesCount = 0;
            DataCount = 0;
        }

        /// <summary>
        /// Возвращает минимальную глубину дерева
        /// </summary>
        /// <returns>Минимальная глубина дерева</returns>
        public int FindMinHeight() {
            int FindMinHeightInternal(CollectionNode<TData, TCollection>? node) {
                if (node == null) {
                    return -1;
                }

                var left = FindMinHeightInternal(node.Left);
                var right = FindMinHeightInternal(node.Right);
                if (left < right) {
                    return left + 1;
                }

                return right + 1;
            }

            return FindMinHeightInternal(Root);
        }

        /// <summary>
        /// Возвращает максимальную глубину дерева
        /// </summary>
        /// <returns>максимальную глубина дерева</returns>
        public int FindMaxHeight() {
            int FindMaxHeightInternal(CollectionNode<TData, TCollection>? node) {
                if (node == null) {
                    return -1;
                }

                var left = FindMaxHeightInternal(node.Left);
                var right = FindMaxHeightInternal(node.Right);
                if (left > right) {
                    return left + 1;
                }

                return right + 1;
            }

            return FindMaxHeightInternal(Root);
        }

        /// <summary>
        /// сбалансировано ли дерево?
        /// </summary>
        public bool IsBalanced() => FindMinHeight() >= FindMaxHeight() - 1;
        
        /// <summary>
        /// Возвращает перечисление элементов 
        /// </summary>
        private IEnumerator<TData> TraverseTreeDataInternal() {
            if (Root == null) {
                yield break;
            }

            int stackSize = (int)Math.Log(NodesCount, 2);
            Stack<CollectionNode<TData, TCollection>> stackBuffer = new Stack<CollectionNode<TData, TCollection>>(stackSize);
            CollectionNode<TData, TCollection>? curr = Root;
            while (curr != null || stackBuffer.Count > 0) {
                while (curr != null) {
                    stackBuffer.Push(curr);
                    curr = curr.Left;
                }

                curr = stackBuffer.Pop();

                foreach (var data in curr) {
                    yield return data;
                }
                curr = curr.Right;
            }
        }
        
        /// <summary>
        /// Возвращает перечисление нод элементов 
        /// </summary>
        private IEnumerator<CollectionNode<TData, TCollection>> TraverseTreeInternal() {
            if (Root == null) {
                yield break;
            }

            int stackSize = (int)Math.Log(NodesCount, 2);
            Stack<CollectionNode<TData, TCollection>> stackBuffer = new Stack<CollectionNode<TData, TCollection>>();
            CollectionNode<TData, TCollection> curr = Root;
            while (curr != null || stackBuffer.Count > 0) {
                while (curr != null) {
                    stackBuffer.Push(curr);
                    curr = curr.Left;
                }

                curr = stackBuffer.Pop();
                yield return curr;
                
                curr = curr.Right;
            }
        }
        
#region Overrides

        protected virtual CollectionNode<TData, TCollection> GetNode(TData data) {
            return new CollectionNode<TData, TCollection>(data);
        }

        protected virtual void ReleaseNode(CollectionNode<TData, TCollection> node) {
            node.Clear();
        }

#endregion
        
#region Utils
       
        private int Compare(TData source, IAnyElementCollection<TData> to) {
            if (to.TryGetAny(out var toData)) {
                return _compareFieldSelector(source).CompareTo(_compareFieldSelector(toData!));
            }
            
            throw new InvalidOperationException("Can't compare with empty node: empty node shouldn't exists!");
        }
        
        private int Compare(TComparable comparable, IAnyElementCollection<TData> to) {
            if (to.TryGetAny(out var toData)) {
                return comparable.CompareTo(_compareFieldSelector(toData!));
            }

            throw new InvalidOperationException("Can't compare with empty node: empty node shouldn't exists!");
        }

#endregion

#region Enumerable

        public IEnumerable<CollectionNode<TData, TCollection>> Iterate() {
            var enumerator = TraverseTreeInternal();
            while (enumerator.MoveNext()) {
                yield return enumerator.Current;
            }
        }

        public IEnumerator<TData> GetEnumerator() {
            return TraverseTreeDataInternal();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

#endregion
        
#region Utils

        private readonly struct AddRangeItem {
            public readonly int Index;
            public readonly int Window;
            
            public AddRangeItem(int index, int window) {
                Index = index;
                Window = window;
            }
        }
        

#endregion
    }
}*/