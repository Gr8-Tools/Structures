/*using System;
using NuclearGames.StructuresUnity.Structures.Collections.NonAllocated;
using NUnit.Framework;

namespace NuclearGames.StructuresUnity.Tests.Editor.Structures.Collections.NonAllocated {
    public class NonAllocatedStackUnitTests {
    [TestCase(0)]
    [TestCase(10)]
    [TestCase(100)]
    public unsafe void CtorTest(int size) {
        void* n = stackalloc WrapValueType<char>*[size];
        var stack = new NonAllocatedStack<WrapValueType<char>>(n, size);
        
        Assert.AreEqual(0, stack.Count);
    }

    [TestCase(0, new char[0])]
    [TestCase(0, new char[] { 'A' })]
    [TestCase(5, new char[] { 'A' })]
    [TestCase(5, new char[] { 'A', 'B', 'C', 'D', 'E' })]
    [TestCase(5, new char[] { 'A', 'B', 'C', 'D', 'E', 'F' })]
    public unsafe void PushTest(int size, char[] values) {
        void* n = stackalloc WrapValueType<char>*[size];
        var stack = new NonAllocatedStack<WrapValueType<char>>(n, size);

        PushInternal(in size, values, ref stack);

        Assert.AreEqual(Math.Min(size, values.Length), stack.Count);
    }

    [TestCase(0, new char[0])]
    [TestCase(0, new char[] { 'A' })]
    [TestCase(5, new char[] { 'A' })]
    [TestCase(5, new char[] { 'A', 'B', 'C', 'D', 'E' })]
    [TestCase(5, new char[] { 'A', 'B', 'C', 'D', 'E', 'F' })]
    public unsafe void PeakTest(int size, char[] values) {
        void* n = stackalloc WrapValueType<char>*[size];
        var stack = new NonAllocatedStack<WrapValueType<char>>(n, size);

        PushInternal(in size, values, ref stack);

        Assert.AreEqual(Math.Min(size, values.Length), stack.Count);

        var isEmpty = stack.Count == 0;
        int lastIndex = Math.Min(size, values.Length) - 1;

        Assert.AreEqual(!isEmpty, stack.TryPeek(out var peakedValue));
        if (!isEmpty) {
            Assert.AreEqual(values[lastIndex], peakedValue.Value);
        }

        try {
            peakedValue = stack.Peek();
            Assert.AreEqual(values[lastIndex], peakedValue.Value);
        } catch (ArgumentOutOfRangeException ex) {
            if (!isEmpty) {
                Assert.Fail(ex.ToString());
            }
        }

        Assert.AreEqual(Math.Min(size, values.Length), stack.Count);
    }

    [TestCase(0, new char[0])]
    [TestCase(0, new char[] { 'A' })]
    [TestCase(5, new char[] { 'A', 'B' })]
    [TestCase(5, new char[] { 'A', 'B', 'C', 'D', 'E' })]
    [TestCase(5, new char[] { 'A', 'B', 'C', 'D', 'E', 'F' })]
    public unsafe void PopTest(int size, char[] values) {
        void* n = stackalloc WrapValueType<char>*[size];
        var stack = new NonAllocatedStack<WrapValueType<char>>(n, size);

        PushInternal(in size, values, ref stack);

        Assert.AreEqual(Math.Min(size, values.Length), stack.Count);

        var isEmpty = stack.Count == 0;
        int lastIndex = Math.Min(size, values.Length) - 1;

        Assert.AreEqual(!isEmpty, stack.TryPop(out var peakedValue));
        if (!isEmpty) {
            Assert.AreEqual(values[lastIndex--], peakedValue.Value);
        }

        try {
            peakedValue = stack.Pop();
            Assert.AreEqual(values[lastIndex--], peakedValue.Value);
        } catch (ArgumentOutOfRangeException ex) {
            if (!isEmpty) {
                Assert.Fail(ex.ToString());
            }
        }

        // var finalSize = Math.Min(size, values.Length);
        // finalSize = Math.Clamp(finalSize - 2, 0, finalSize);
        var finalSize = lastIndex + 1;
        Assert.AreEqual(finalSize, stack.Count);
    }


#region Utils Actions

    private void PushInternal<T>(in int size, T[] values, ref NonAllocatedStack<WrapValueType<T>> stack) where T : unmanaged {
        for (int i = 0; i < values.Length; i++) {
            try {
                var value = (WrapValueType<T>)values[i];

                if (i % 2 == 0) {
                    stack.Push(value);
                } else {
                    stack.Push(ref value);
                }
            } catch (ArgumentOutOfRangeException ex) {
                if (i >= size) {
                    continue;
                }

                Assert.Fail(ex.ToString());
            }
        }
    }

#endregion

#region Utils

    private readonly struct WrapValueType<T> where T : unmanaged {
        public readonly T Value;
        
        public WrapValueType(T value) {
            Value = value;
        }

        public static explicit operator WrapValueType<T>(T b) => new WrapValueType<T>(b);
    }

#endregion
}
}*/