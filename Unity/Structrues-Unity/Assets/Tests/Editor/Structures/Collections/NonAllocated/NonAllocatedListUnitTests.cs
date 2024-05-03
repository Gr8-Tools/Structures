/*using System;
using NuclearGames.StructuresUnity.Structures.Collections.NonAllocated;
using NUnit.Framework;
using UnityEngine;

namespace NuclearGames.StructuresUnity.Tests.Editor.Structures.Collections.NonAllocated {
    public class NonAllocatedListUnitTests {
        [TestCase(0)]
        [TestCase(10)]
        [TestCase(100)]
        public unsafe void CtorTest(int size) {
            void* n = stackalloc WrapReadValueType<char>*[size];
            var stack = new NonAllocatedList<WrapReadValueType<char>>(n, size);
            
            Assert.AreEqual(0, stack.Count);
        }

        [TestCase(0, new char[0], false)]
        [TestCase(0, new char[] { 'A' }, false)]
        [TestCase(5, new char[] { 'A' }, true)]
        [TestCase(5, new char[] { 'A' }, false)]
        [TestCase(5, new char[] { 'A', 'B', 'C', 'D', 'E' }, true)]
        [TestCase(5, new char[] { 'A', 'B', 'C', 'D', 'E' }, false)]
        [TestCase(5, new char[] { 'A', 'B', 'C', 'D', 'E', 'F' }, true)]
        [TestCase(5, new char[] { 'A', 'B', 'C', 'D', 'E', 'F' }, false)]
        public unsafe void InsertTests(int size, char[] values, bool insertFirst) {
            void* n = stackalloc WrapReadValueType<char>*[size];
            var list = new NonAllocatedList<WrapReadValueType<char>>(n, size);
            
            InsertInternal(in size, values, ref list, in insertFirst);

            var finalSize = Math.Min(size, values.Length);
            Assert.AreEqual(finalSize, list.Count);
        }

        [TestCase(0, new char[0])]
        [TestCase(0, new char[] { 'A' })]
        [TestCase(5, new char[] { 'A', 'B' })]
        [TestCase(5, new char[] { 'A', 'B', 'C', 'D', 'E' })]
        [TestCase(5, new char[] { 'A', 'B', 'C', 'D', 'E', 'F' })]
        public unsafe void AddTests(int size, char[] values) {
            void* n = stackalloc WrapReadValueType<char>*[size];
            var list = new NonAllocatedList<WrapReadValueType<char>>(n, size);
            
            // Span<WrapReadValueType<char>> buffer = stackalloc WrapReadValueType<char>[size];
            // var list = new NonAllocatedList<WrapReadValueType<char>>(ref buffer);

            AddInternal(in size, values, ref list);

            var finalSize = Math.Min(size, values.Length);
            Assert.AreEqual(finalSize, list.Count);
        }

        [TestCase(0, new char[0], '/', false)]
        [TestCase(0, new char[] { 'A' }, 'A', false)]
        [TestCase(5, new char[] { 'A', 'B' }, 'A', true)]
        [TestCase(5, new char[] { 'A', 'B' }, 'C', false)]
        [TestCase(5, new char[] { 'A', 'B', 'C', 'D', 'E' }, 'E', true)]
        [TestCase(5, new char[] { 'A', 'B', 'C', 'D', 'E', 'F' }, 'F', false)]
        public unsafe void ContainsTests(int size, char[] values, char searchValue, bool expectedResult) {
            void* n = stackalloc WrapReadValueType<char>*[size];
            var list = new NonAllocatedList<WrapReadValueType<char>>(n, size);

            AddInternal(in size, values, ref list);

            Assert.AreEqual(expectedResult, list.Contains((WrapReadValueType<char>)searchValue));
        }

        [TestCase(5, new char[] { 'A', 'B', 'C', 'D', 'E', 'F' }, 6, 1)]
        [TestCase(5, new char[] { 'A', 'B', 'C', 'D', 'E', 'F' }, 6, 2)]
        [TestCase(5, new char[] { 'A', 'B', 'C', 'D', 'E', 'F' }, 10, 6)]
        public unsafe void CopyToTests(int size, char[] values, int destSize, int destStartIndex) {
            void* n = stackalloc WrapReadValueType<char>*[size];
            var list = new NonAllocatedList<WrapReadValueType<char>>(n, size);

            AddInternal(in size, values, ref list);

            var destArray = new WrapReadValueType<char>[destSize];

            try {
                list.CopyTo(destArray, in destStartIndex);
            } catch (ArgumentOutOfRangeException e) {
                if (destArray.Length - destStartIndex >= list.Count) {
                    throw;
                }
            }
        }

        [TestCase(0, new char[0], '/', false, new char[0])]
        [TestCase(0, new char[] { 'A' }, 'A', false, new char[0])]
        [TestCase(5, new char[] { 'A', 'B' }, 'A', true, new char[] { 'B' })]
        [TestCase(5, new char[] { 'A', 'B' }, 'C', false, new char[] { 'A', 'B' })]
        [TestCase(5, new char[] { 'A', 'B', 'C', 'D', 'E' }, 'E', true, new char[] { 'A', 'B', 'C', 'D' })]
        [TestCase(5, new char[] { 'A', 'B', 'C', 'D', 'E', 'F' }, 'F', false, new char[] { 'A', 'B', 'C', 'D', 'E' })]
        [TestCase(5, new char[] { 'A', 'B', 'C', 'D', 'E', 'F' }, 'C', true, new char[] { 'A', 'B', 'D', 'E' })]
        public unsafe void RemoveTests(int size, char[] values, char searchValue, bool expectedResult, char[] expectedList) {
            void* n = stackalloc WrapReadValueType<char>*[size];
            var list = new NonAllocatedList<WrapReadValueType<char>>(n, size);

            AddInternal(in size, values, ref list);

            Assert.AreEqual(expectedResult, list.Remove((WrapReadValueType<char>)searchValue));
            for (int i = 0; i < expectedList.Length; i++) {
                Assert.AreEqual(i, list.IndexOf((WrapReadValueType<char>)expectedList[i]));
            }
        }


        [TestCase(0, new char[0])]
        [TestCase(0, new char[] { 'A' })]
        [TestCase(5, new char[] { 'A', 'B' })]
        [TestCase(5, new char[] { 'A', 'B', 'C', 'D', 'E' })]
        [TestCase(5, new char[] { 'A', 'B', 'C', 'D', 'E', 'F' })]
        public unsafe void ClearTests(int size, char[] values) {
            void* n = stackalloc WrapReadValueType<char>*[size];
            var list = new NonAllocatedList<WrapReadValueType<char>>(n, size);

            AddInternal(in size, values, ref list);

            var finalSize = Math.Min(size, values.Length);
            Assert.AreEqual(finalSize, list.Count);

            list.Clear();
            Assert.AreEqual(0, list.Count);
        }

        [TestCase(5, new char[] { 'A', 'B', 'C', 'D', 'E', 'F' })]
        public unsafe void RefGetIndex(int size, char[] values) {
            void* n = stackalloc WrapReadWriteValueType<char>*[size];
            var list = new NonAllocatedList<WrapReadWriteValueType<char>>(n, size);

            AddInternal(in size, values, ref list);

            var finalSize = Math.Min(size, values.Length);
            Assert.AreEqual(finalSize, list.Count);

            ref WrapReadWriteValueType<char> getFirst = ref list[0];
            Assert.AreEqual('A', getFirst.Value);

            getFirst.Value = 'F';

            ref WrapReadWriteValueType<char> getSecond = ref list[0];
            Assert.AreEqual('F', getSecond.Value);
        }

        [TestCase(0, new char[0], '/', -1)]
        [TestCase(0, new char[] { 'A' }, 'A', -1)]
        [TestCase(5, new char[] { 'A', 'B' }, 'A', 0)]
        [TestCase(5, new char[] { 'A', 'B' }, 'C', -1)]
        [TestCase(5, new char[] { 'A', 'B', 'C', 'D', 'E' }, 'E', 4)]
        [TestCase(5, new char[] { 'A', 'B', 'C', 'D', 'E', 'F' }, 'F', -1)]
        public unsafe void IndexOfTests(int size, char[] values, char searchValue, int expectedResult) {
            void* n = stackalloc WrapReadValueType<char>*[size];
            var list = new NonAllocatedList<WrapReadValueType<char>>(n, size);

            AddInternal(in size, values, ref list);

            Assert.AreEqual(expectedResult, list.IndexOf((WrapReadValueType<char>)searchValue));

            var finalSize = Math.Min(size, values.Length);
            for (int i = 0; i < finalSize; i++) {
                Assert.AreEqual(i, list.IndexOf((WrapReadValueType<char>)values[i]));
            }
        }

        [TestCase(0, new char[0], 0, 0)]
        [TestCase(0, new char[] { 'A' }, 1, 0)]
        [TestCase(5, new char[] { 'A', 'B' }, 1, 1)]
        [TestCase(5, new char[] { 'A', 'B' }, 3, 2)]
        [TestCase(5, new char[] { 'A', 'B', 'C', 'D', 'E' }, 2, 4)]
        [TestCase(5, new char[] { 'A', 'B', 'C', 'D', 'E' }, 5, 5)]
        public unsafe void RemoveAtTests(int size, char[] values, int removeAtIndex, int expectedCount) {
            void* n = stackalloc WrapReadValueType<char>*[size];
            var list = new NonAllocatedList<WrapReadValueType<char>>(n, size);

            AddInternal(in size, values, ref list);

            list.RemoveAt(in removeAtIndex);
            Assert.AreEqual(expectedCount, list.Count);
        }

        [TestCase(0, new char[0])]
        [TestCase(0, new char[] { 'A' })]
        [TestCase(5, new char[] { 'A', 'B' })]
        [TestCase(5, new char[] { 'A', 'B', 'C', 'D', 'E' })]
        [TestCase(5, new char[] { 'A', 'B', 'C', 'D', 'E', 'F' })]
        public unsafe void EnumeratorTests(int size, char[] values) {
            void* n = stackalloc WrapReadValueType<char>*[size];
            var list = new NonAllocatedList<WrapReadValueType<char>>(n, size);

            AddInternal(in size, values, ref list);

            var index = 0;
            foreach (var wrapReadValueType in list) {
                Assert.AreEqual(values[index], wrapReadValueType.Value);

                index++;
            }
        }


#region Utils Actions

        private void AddInternal<T>(in int size, T[] values, ref NonAllocatedList<WrapReadWriteValueType<T>> list)
            where T : unmanaged, IComparable<T> {
            for (int i = 0; i < values.Length; i++) {
                try {
                    var value = (WrapReadWriteValueType<T>)values[i];
                    list.Add(in value);
                } catch (Exception ex) {
                    switch (ex) {
                        case OverflowException _:
                        case ArgumentOutOfRangeException _:
                            if (i >= size) {
                                continue;
                            }

                            break;
                    }

                    throw;
                }
            }
        }

        private void AddInternal<T>(in int size, T[] values, ref NonAllocatedList<WrapReadValueType<T>> list)
            where T : unmanaged, IComparable<T> {
            for (int i = 0; i < values.Length; i++) {
                try {
                    var value = (WrapReadValueType<T>)values[i];
                    list.Add(in value);
                } catch (Exception ex) {
                    switch (ex) {
                        case OverflowException _:
                        case ArgumentOutOfRangeException _:
                            if (i >= size) {
                                continue;
                            }

                            break;
                    }

                    throw;
                }
            }
        }

        private void InsertInternal<T>(in int size, T[] values, ref NonAllocatedList<WrapReadValueType<T>> list,
            in bool insertFirst) where T : unmanaged, IComparable<T> {
            for (int i = 0; i < values.Length; i++) {
                try {
                    var value = (WrapReadValueType<T>)values[i];
                    var insertIndex = insertFirst ? 0 : i;
                    list.Insert(in insertIndex, in value);
                } catch (Exception ex) {
                    switch (ex) {
                        case OverflowException _:
                        case ArgumentOutOfRangeException _:
                            if (i >= size) {
                                continue;
                            }

                            break;
                    }

                    throw;
                }
            }
        }

#endregion

#region Utils

        private readonly struct WrapReadValueType<T> : IComparable<WrapReadValueType<T>> where T : unmanaged, IComparable<T> {
            public readonly T Value;

            public WrapReadValueType(T value) {
                Value = value;
            }


            public int CompareTo(WrapReadValueType<T> other) {
                return Value.CompareTo(other.Value);
            }

            public static explicit operator WrapReadValueType<T>(T b) => new WrapReadValueType<T>(b);
        }

        private struct WrapReadWriteValueType<T> : IComparable<WrapReadWriteValueType<T>>
            where T : unmanaged, IComparable<T> {
            public T Value;

            public WrapReadWriteValueType(T value) {
                Value = value;
            }

            public int CompareTo(WrapReadWriteValueType<T> other) {
                return Value.CompareTo(other.Value);
            }

            public static explicit operator WrapReadWriteValueType<T>(T b) => new WrapReadWriteValueType<T>(b);
        }

#endregion
    }
}*/