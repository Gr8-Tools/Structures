using System;
using System.Runtime.CompilerServices;

namespace NuclearGames.StructuresUnity.Utils.Enumerators {
    public unsafe struct UnsafeRefArrayEnumerator<T> where T : unmanaged {
        private readonly T[] _array;
        private readonly T* _endPtr;
        private T* _ptr;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public UnsafeRefArrayEnumerator(T[] array) : this(array, 0, array.Length) { }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public UnsafeRefArrayEnumerator(T[] array, int offset, int length) {
            if (offset + length > array.Length) {
                throw new IndexOutOfRangeException($"[CustomArrayEnumerator ctor] Failed offsetting : [#{array.Length}]: '{offset}' -> '{length}'");
            }

            _array = array;

            fixed (T* firstPtr = _array) {
                _ptr = firstPtr - 1;
                _endPtr = _ptr + length - offset;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool MoveNext() {
            return _ptr++ < _endPtr;
        }

        public readonly ref T Current {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ref *_ptr;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly UnsafeRefArrayEnumerator<T> GetEnumerator() => this;
    }
}
