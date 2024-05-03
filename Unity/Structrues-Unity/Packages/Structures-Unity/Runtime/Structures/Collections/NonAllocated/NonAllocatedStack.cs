/*using System;
using BinarySerialization.Extensions;
using JetBrains.Annotations;

namespace NuclearGames.StructuresUnity.Structures.Collections.NonAllocated {
    public ref struct NonAllocatedStack<T> where T : unmanaged {
        public int Count => _count;
        
        private readonly IntPtr _ptr;
        private readonly int _size;
        private readonly int _sizeOfType;
        
        private int _count;

        public unsafe NonAllocatedStack(void* ptr, in int size, int? sizeOfType = null) {
            _ptr = (IntPtr)ptr;
            _size = size;
            _sizeOfType = sizeOfType ?? sizeof(T);
            _count = 0;
        }

        /// <summary>Returns the object at the top of the <see cref="T:System.Collections.Generic.Stack`1" /> without removing it.</summary>
        /// <exception cref="T:System.InvalidOperationException">The <see cref="T:System.Collections.Generic.Stack`1" /> is empty.</exception>
        /// <returns>The object at the top of the <see cref="T:System.Collections.Generic.Stack`1" />.</returns>
        public ref T Peek() {
            AssertCanPop();

            return ref UnsafeUtilityExtensions.OffsetRef<T>(_ptr, _count - 1, _sizeOfType);
            // return ref _buffer[_count - 1];
        }

        /// <summary>Removes and returns the object at the top of the <see cref="T:System.Collections.Generic.Stack`1" />.</summary>
        /// <exception cref="T:System.ArgumentOutOfRangeException">The <see cref="T:System.Collections.Generic.Stack`1" /> is empty.</exception>
        /// <returns>The object removed from the top of the <see cref="T:System.Collections.Generic.Stack`1" />.</returns>
        public ref T Pop() {
            AssertCanPop();
            return ref UnsafeUtilityExtensions.OffsetRef<T>(_ptr, --_count, _sizeOfType);
            // return ref _buffer[--_count];
        }

        /// <summary>Returns a value that indicates whether there is an object at the top of the <see cref="T:System.Collections.Generic.Stack`1" />, and if one is present, copies it to the <paramref name="result" /> parameter. The object is not removed from the <see cref="T:System.Collections.Generic.Stack`1" />.</summary>
        /// <param name="result">If present, the object at the top of the <see cref="T:System.Collections.Generic.Stack`1" />; otherwise, the default value of <typeparamref name="T" />.</param>
        /// <returns>
        /// <see langword="true" /> if there is an object at the top of the <see cref="T:System.Collections.Generic.Stack`1" />; <see langword="false" /> if the <see cref="T:System.Collections.Generic.Stack`1" /> is empty.</returns>
        public bool TryPeek([CanBeNull] out T result) {
            try {
                result = Peek();
                return true;
            } catch (ArgumentOutOfRangeException _) {
                result = default;
                return false;
            }
        }

        /// <summary>Returns a value that indicates whether there is an object at the top of the <see cref="T:System.Collections.Generic.Stack`1" />, and if one is present, copies it to the <paramref name="result" /> parameter, and removes it from the <see cref="T:System.Collections.Generic.Stack`1" />.</summary>
        /// <param name="result">If present, the object at the top of the <see cref="T:System.Collections.Generic.Stack`1" />; otherwise, the default value of <typeparamref name="T" />.</param>
        /// <returns>
        /// <see langword="true" /> if there is an object at the top of the <see cref="T:System.Collections.Generic.Stack`1" />; <see langword="false" /> if the <see cref="T:System.Collections.Generic.Stack`1" /> is empty.</returns>
        public bool TryPop([CanBeNull] out T result) {
            try {
                result = Pop();
                return true;
            } catch (ArgumentOutOfRangeException _) {
                result = default;
                return false;
            }
        }

        /// <summary>Inserts an object at the top of the <see cref="T:System.Collections.Generic.Stack`1" />.</summary>
        /// <param name="item">The object to push onto the <see cref="T:System.Collections.Generic.Stack`1" />. The value can be <see langword="null" /> for reference types.</param>
        public void Push(T item) {
            AssertCanPush();
            unsafe {
                void* writePtr = UnsafeUtilityExtensions.OffsetPtr(_ptr, _count++, _sizeOfType);
                *(T*)writePtr = item;
            }
            // _buffer[_count++] = item;
        }
        
        /// <summary>Inserts an object at the top of the <see cref="T:System.Collections.Generic.Stack`1" />.</summary>
        /// <param name="item">The object to push onto the <see cref="T:System.Collections.Generic.Stack`1" />. The value can be <see langword="null" /> for reference types.</param>
        public void Push(ref T item) {
            AssertCanPush();
            
            unsafe {
                void* writePtr = UnsafeUtilityExtensions.OffsetPtr(_ptr, _count++, _sizeOfType);
                *(T*)writePtr = item;
            }
            
            // _buffer[_count++] = item;
        }

#region MyRegion

        private void AssertCanPush() {
            if (_count >= _size) {
                throw new ArgumentOutOfRangeException(nameof(_size), "Buffer is overfilled!");
            }
        }

        private void AssertCanPop() {
            if (_count == 0) {
                throw new ArgumentOutOfRangeException(nameof(_size), "Buffer is empty!");
            }
        }

#endregion
    }
}*/