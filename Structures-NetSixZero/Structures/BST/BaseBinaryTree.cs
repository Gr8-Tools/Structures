using System.Collections;
using System.Diagnostics.CodeAnalysis;
using Structures.NetSixZero.Structures.BST.Utils;
using Structures.NetSixZero.Structures.Collections.NonAllocated;
using Structures.NetSixZero.Utils.Collections;
using Structures.NetSixZero.Utils.Collections.Interfaces;


namespace Structures.NetSixZero.Structures.BST {
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
        public virtual bool TryAdd(TData data, out Node<TData> resultNode) {
            if (IsEmpty) {
                Root = GetNode(data);
                resultNode = Root;
                NodesCount++;

                return true;
            }

            var node = Root!;
            TComparable dataValue = CompareFieldSelector(data);

            bool? result = null;
            resultNode = default!;

            while (!result.HasValue) {
                var compareResult = Compare(dataValue, node.Data);
                switch (compareResult) {
                    case < 0:
                        if (node.Left == null) {
                            node.Left = GetNode(data);
                            resultNode = node.Left!;
                            result = true;
                        } else {
                            node = node.Left;
                        }

                        break;

                    case > 0:
                        if (node.Right == null) {
                            node.Right = GetNode(data);
                            resultNode = node.Right!;
                            result = true;
                        } else {
                            node = node.Right;
                        }

                        break;

                    default:
                        resultNode = node;
                        result = false;

                        break;
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
                    Span<AddRangeItem> numbers = stackalloc AddRangeItem[(int)Math.Log2(sourceBuffer.Length)];
                    var stackBuffer = new NonAllocatedStack<AddRangeItem>(ref numbers);

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
                        anyAdd |= TryAdd(sourceBuffer[^1], out _);
                    }

                    break;
            }

            return anyAdd;
        }

        /// <summary>
        /// Ищет минимальный элемент в дереве
        /// </summary>
        /// <param name="resultNode">Узел дерева с минимальными данными</param>
        /// <returns>Найден такой узел (true) или нет (fasle)</returns>
        public virtual bool TryFindMin(out Node<TData>? resultNode) {
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
        public virtual bool TryFindMax(out Node<TData>? resultNode) {
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
        public virtual bool TryFind(TData data, [MaybeNullWhen(false)] out Node<TData> resultNode) {
            if (IsEmpty) {
                resultNode = null;

                return false;
            }

            resultNode = Root!;
            TComparable dataValue = CompareFieldSelector(data);
            int compareResult;
            while ((compareResult = Compare(dataValue, resultNode.Data)) != 0) {
                switch (compareResult) {
                    case < 0:
                        resultNode = resultNode.Left;

                        break;
                    case > 0:
                        resultNode = resultNode.Right;

                        break;
                    default:
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
        public virtual bool Remove(TData data, [MaybeNullWhen(false)] out Node<TData> removeNode) {
            bool result = false;
            Node<TData>? temp = Root, parent = null;
            removeNode = null;

            // Проверяем, пустое ли дерево
            if (temp == null) {
                return false;
            }

            var compareValue = CompareFieldSelector(data);

            while (temp != null) {
                int compareResult = Compare(compareValue, temp.Data);
                if (compareResult > 0) {
                    parent = temp;
                    temp = temp.Right;
                } else if (compareResult < 0) {
                    parent = temp;
                    temp = temp.Left;
                } else {
                    if (temp.Left == null && temp.Right == null) {
                        if (parent != null) {
                            if (parent.Left == temp) {
                                parent.Left = null;
                            } else {
                                parent.Right = null;
                            }
                        }

                        removeNode = temp;
                    } else if (temp.Left == null || temp.Right == null) {
                        if (temp.Left != null) {
                            if (parent == null) {
                                Root = temp.Left;
                            } else {
                                parent.Left = temp.Left;
                            }
                        } else {
                            if (parent == null) {
                                Root = temp.Right;
                            } else {
                                parent.Right = temp.Right;
                            }
                        }

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
        /// <param name="resultNode">Извлеченный нод или NULL.</param>
        /// <returns>True - нод был извлечен; False - нет нод.</returns>
        public virtual bool TryDequeue(out Node<TData>? resultNode) {
            if (IsEmpty) {
                resultNode = null;

                return false;
            }

            if (Root == null) {
                Console.WriteLine($"null: {IsEmpty}, {NodesCount}");
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

            ReleaseNode(current);

            resultNode = current;

            return true;
        }

        /// <summary>
        /// Очишает все дерево
        /// </summary>
        public virtual void Clear() {
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
        public virtual bool TryGetAny(out TData? value) {
            if (NodesCount == 0) {
                value = default;

                return false;
            }

            value = Root!.Data;

            return true;
        }

        /// <summary>
        /// Возвращает перечисление элементов 
        /// </summary>
        IEnumerator<TData> TraverseTreeInternal() {
            if (Root == null) {
                yield break;
            }

            Stack<Node<TData>> stackBuffer = new Stack<Node<TData>>((int)Math.Log2(Count));
            Node<TData>? curr = Root;
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

        private readonly record struct AddRangeItem(int Index, int Window);

#endregion
    }
}