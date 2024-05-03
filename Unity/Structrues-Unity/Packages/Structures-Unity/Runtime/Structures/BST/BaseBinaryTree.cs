/*using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using NuclearGames.StructuresUnity.Structures.BST.Utils;
using NuclearGames.StructuresUnity.Structures.Collections.NonAllocated;
using NuclearGames.StructuresUnity.Utils.Collections.Interfaces;
using UnityEngine;

namespace NuclearGames.StructuresUnity.Structures.BST {
    /// <summary>
    /// Базовая структура Двоичного дерева, где данные типа <typeparamref name="TData"/> отсортированы по ключу <typeparamref name="TComparable"/>
    /// </summary>
    public class BaseBinaryTree<TData, TComparable> : IAnyElementCollection<TData>
        where TComparable : IComparable<TComparable> {
        /// <summary>
        /// Корневой узел дерева
        /// </summary>
        public virtual Node<TData>? Root { get; private protected set; }

        /// <summary>
        /// Кол-во узлов в дереве
        /// </summary>
        public virtual int NodesCount { get; private protected set; }

        /// <summary>
        /// Селектор поля сравнения из объекта данные
        /// </summary>
        public Func<TData, TComparable> CompareFieldSelector { get; }

        public BaseBinaryTree(Func<TData, TComparable> compareFieldSelector) {
            CompareFieldSelector = compareFieldSelector;
        }

        public BaseBinaryTree(TData[] sourceBuffer, Func<TData, TComparable> compareFieldSelector) : this(
            compareFieldSelector) {
            TryAddRange(sourceBuffer);
        }

        /// <summary>
        /// Пытается добавить новый узел в дерево
        /// </summary>
        /// <param name="data">Данные</param>
        /// <param name="resultNode">Узел, добавленный в случае успеха, или существующий, в случае провала</param>
        /// <returns>Удалось создать новый узел (True) или узел уже  существовал (False)</returns>
        public virtual bool TryAdd(TData data, out Node<TData>? resultNode) {
            if (IsEmpty) {
                Root = GetNode(data);
                resultNode = Root;
                NodesCount++;

                return true;
            }

            var node = Root!;
            TComparable dataValue = CompareFieldSelector(data);

            bool? result = null;
            resultNode = null;

            while (!result.HasValue) {
                var compareResult = Compare(dataValue, node.Data);
                if (compareResult < 0) {
                    if (node.Left == null) {
                        node.Left = GetNode(data);
                        resultNode = node.Left!;
                        result = true;
                    } else {
                        node = node.Left;
                    }
                } else if (compareResult > 0) {
                    if (node.Right == null) {
                        node.Right = GetNode(data);
                        resultNode = node.Right!;
                        result = true;
                    } else {
                        node = node.Right;
                    }
                } else {
                    resultNode = node;
                    result = false;
                }
            }

            if (result.Value) {
                NodesCount++;
            }

            return result.Value;
        }

        /// <summary>
        /// Добавляет массив элементов. Построено на предположении, что <paramref name="sourceBuffer"/> упорядочен по возрастнию. 
        /// </summary>
        /// <returns>Был ли добавлен хотя бы один элемент</returns>
        public bool TryAddRange(TData[] sourceBuffer) {
            return TryAddRangeInternal(sourceBuffer);
        }

        /// <summary>
        /// Добавляет массив элементов. Построено на предположении, что <paramref name="sourceBuffer"/> упорядочен по возрастнию. 
        /// </summary>
        /// <returns>Был ли добавлен хотя бы один элемент</returns>
        private bool TryAddRangeInternal(TData[] sourceBuffer) {
            bool anyAdd = false;

            switch (sourceBuffer.Length) {
                case 0:
                    return false;
                case 1:
                    anyAdd |= TryAdd(sourceBuffer[0], out _);

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

                            anyAdd |= TryAdd(sourceBuffer[addRangeItem.Index], out _);

                            if (addRangeItem.Window == 1) {
                                continue;
                            }

                            var newWindow = addRangeItem.Window / 2;
                            if (addRangeItem.Window % 2 == 0) {
                                stackBuffer.Push(new AddRangeItem(addRangeItem.Index + newWindow, newWindow));
                                stackBuffer.Push(new AddRangeItem(addRangeItem.Index - newWindow, newWindow));
                            } else {
                                anyAdd |= TryAdd(sourceBuffer[addRangeItem.Index + newWindow * 2], out _);
                                anyAdd |= TryAdd(sourceBuffer[addRangeItem.Index - 1], out _);

                                stackBuffer.Push(new AddRangeItem(addRangeItem.Index + newWindow, newWindow));
                                stackBuffer.Push(new AddRangeItem(addRangeItem.Index - newWindow - 1, newWindow));
                            }

                            maxStackSize = Math.Max(maxStackSize, stackBuffer.Count);
                        }

                        anyAdd |= TryAdd(sourceBuffer[0], out _);
                        if (sourceBuffer.Length % 2 == 1) {
                            anyAdd |= TryAdd(sourceBuffer[sourceBuffer.Length - 1], out _);
                        }

                        break;   
                    }
            }

            return anyAdd;
        }

        /// <summary>
        /// Ищет минимальный элемент в дереве
        /// </summary>
        /// <param name="resultNode">Узел дерева с минимальными данными</param>
        /// <returns>Найден такой узел (true) или нет (fasle)</returns>
        public virtual bool TryFindMin([CanBeNull] out Node<TData> resultNode) {
            if (IsEmpty) {
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
        public virtual bool TryFindMax([CanBeNull] out Node<TData> resultNode) {
            if (IsEmpty) {
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
        public virtual bool TryFind(TData data, [CanBeNull] out Node<TData> resultNode) {
            if (IsEmpty) {
                resultNode = null;

                return false;
            }

            resultNode = Root!;
            TComparable dataValue = CompareFieldSelector(data);
            int compareResult;
            while ((compareResult = Compare(dataValue, resultNode.Data)) != 0) {
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
        /// Пытается удалить узел дерева по данным
        /// </summary>
        /// <param name="data">Данные, необходмые удалить из дерева</param>
        /// <returns>True - данные найдены и удалить получилось. False - данные найдены не были </returns>
        public virtual bool Remove(TData data) {
            return Remove(data, out _);
        }
        
        /// <summary>
        /// Пытается удалить узел дерева по данным
        /// </summary>
        /// <param name="data">Данные, необходмые удалить из дерева</param>
        /// <param name="removeNode"></param>
        /// <returns>True - данные найдены и удалить получилось. False - данные найдены не были </returns>
        public virtual bool Remove(TData data, [CanBeNull] out Node<TData> removeNode) {
            bool result = false;
            Node<TData>? temp = Root, parent = null;
            removeNode = null;

            // Проверяем, пустое ли дерево
            if (temp == null) {
                return false;
            }

            int? prevCompareResult, compareResult = null; 
            var compareValue = CompareFieldSelector(data);

            while (temp != null) {
                prevCompareResult = compareResult;
                compareResult = Compare(compareValue, temp.Data);
                if (compareResult > 0) {
                    parent = temp;
                    temp = temp.Right;
                } else if (compareResult < 0) {
                    parent = temp;
                    temp = temp.Left;
                } else {
                    Node<TData>? SwitchNull() => null;

                    Node<TData>? SwitchLeaf() {
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
                        (temp.Data, temp2.Data) = (temp2.Data, temp.Data);
                        
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

            return result;
        }


        /// <summary>
        /// Пытается извлечь нод с минимальным значением.
        /// </summary>
        /// <param name="value">Извлеченный нод или NULL.</param>
        /// <returns>True - нод был извлечен; False - нет нод.</returns>
        public virtual bool TryDequeue([CanBeNull] out TData value) {
            if (IsEmpty) {
                value = default;
                return false;
            }

            // Добегаем до последнего левого нода (минимальное значение).
            Node<TData>? prev = null;
            Node<TData> current = Root!;
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

            value = current.Data;
            ReleaseNode(current);

            return true;
        }

        /// <summary>
        /// Очищает все дерево
        /// </summary>
        public virtual void Clear() {
            while (TryDequeue(out _)) { }
            
            Root = null;
            NodesCount = 0;
        }


        /// <summary>
        /// Возвращает минимальную глубину дерева
        /// </summary>
        /// <returns>Минимальная глубина дерева</returns>
        public virtual int FindMinHeight() {
            int FindMinHeightInternal(Node<TData>? node) {
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
        public virtual int FindMaxHeight() {
            int FindMaxHeightInternal(Node<TData>? node) {
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
        public virtual bool IsBalanced() => FindMinHeight() >= FindMaxHeight() - 1;

#region Overrides

        protected virtual Node<TData> GetNode(TData data) {
            return new Node<TData>(data);
        }

        protected virtual void ReleaseNode(Node<TData> node) { }

#endregion

#region IAnyCollection

        /// <summary>
        /// Является ли дерево пустым
        /// </summary>
        public virtual bool IsEmpty => NodesCount == 0;

        /// <summary>
        /// Любой элемент из коллекции
        /// </summary>
        public virtual TData Any {
            get {
                if (IsEmpty) {
                    throw new NullReferenceException("Tree is empty!");
                }

                return Root!.Data;
            }
        }

        /// <summary>
        /// Есть ли в коллекции хотя бы один элемент
        /// </summary>
        /// <param name="value">Любой элемент, если он существует в коллекции</param>
        public virtual bool TryGetAny([CanBeNull] out TData value) {
            if (NodesCount == 0) {
                value = default;

                return false;
            }

            value = Root!.Data;

            return true;
        }

        /// <summary>
        /// Копирует данные в другую коллекцию типа 
        /// </summary>
        public void CopyTo(IAnyElementCollection<TData> destinationCollection) {
            foreach (var data in this) {
                destinationCollection.Add(data);
            }
        }

        /// <summary>
        /// Возвращает перечисление элементов 
        /// </summary>
        IEnumerator<TData> TraverseTreeInternal() {
            if (Root == null) {
                yield break;
            }

            int stackSize = (int)Mathf.Log(Count, 2);
            Stack<Node<TData>> stackBuffer = new Stack<Node<TData>>(stackSize);
            Node<TData> curr = Root;
            while (curr != null || stackBuffer.Count > 0) {
                while (curr != null) {
                    stackBuffer.Push(curr);
                    curr = curr.Left;
                }

                curr = stackBuffer.Pop();

                yield return curr.Data;
                curr = curr.Right;
            }
        }

#endregion

#region Utils

        private int Compare(TComparable value1, TData data2) {
            return value1.CompareTo(CompareFieldSelector(data2));
        }

        private int Compare(TData data1, TData data2) {
            return CompareFieldSelector(data1).CompareTo(CompareFieldSelector(data2));
        }

#endregion

#region ICollection

        public virtual int Count => NodesCount;

        public virtual bool IsReadOnly => false;

        public virtual void Add(TData item) {
            if (!TryAdd(item, out var node)) {
                throw new Exception($"Node with value '{item}' has been already exists!");
            }
        }

        public virtual bool Contains(TData item) {
            return TryFind(item, out var _);
        }

        public virtual void CopyTo(TData[] array, int arrayIndex) {
            if (array.Length < Count + arrayIndex) {
                throw new ArgumentOutOfRangeException(nameof(array),
                                                      $"Invalid array size: it's length should be >= '{Count + arrayIndex}'");
            }

            var index = 0;
            foreach (var node in this) {
                array[index + arrayIndex] = node;
                index++;
            }
        }

#endregion

#region Enumerable

        public virtual IEnumerator<TData> GetEnumerator() {
            return TraverseTreeInternal();
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