/*using System;
using NuclearGames.StructuresUnity.Structures.BST;
using NuclearGames.StructuresUnity.Utils.Generators.Interfaces;

namespace NuclearGames.StructuresUnity.Utils.Generators {
    public abstract class BaseIdGenerator<T> : IIdGenerator<T> where T : IComparable<T> {
        private T _maxId;
        private bool _isInitialized = false;
        private readonly object _locker = new object();
        protected readonly BinaryTree<T> Buffer;

        protected BaseIdGenerator(T startSize) {
            _maxId = startSize;
            Buffer = new BinaryTree<T>();
        }

        protected abstract T Resize(T value);
        protected abstract T[] GenerateNewRange(T start, T count);
        protected abstract T[] GenerateDefaultRange(T count);

        /// <summary>
        /// Генерирует новый Id
        /// </summary>
        public T NewId() {
            lock (_locker) {

                LazyInitialize();
                
                if (Buffer.IsEmpty) {
                    var newMaxId = Resize(_maxId);
                    if (!Buffer.TryAddRange(GenerateNewRange(_maxId, _maxId))) {
                        throw new Exception($"Can't resize id's buffer to '{newMaxId}'");
                    }

                    _maxId = newMaxId;
                }

                if (Buffer.TryFindMin(out var minNode)) {
                    if (Buffer.Remove(minNode!.Data)) {
                        return minNode.Data;
                    }
                    throw new Exception($"Can't remove value '{minNode.Data}' but it was defined as minValue!");
                }   
            }

            throw new Exception("Can't find any min value, but id's buffer isn't empty!");
        }

        private void LazyInitialize() {
            if (_isInitialized) {
                return;
            }
            
            if (!Buffer.TryAddRange(GenerateDefaultRange(_maxId))) {
                throw new Exception($"Can't resize id's buffer to '{_maxId}'");
            }

            _isInitialized = true;
        }
        
        /// <summary>
        /// Возвращает в буффер Id
        /// </summary>
        public void Release(T id) {
            lock (_locker) {
                Buffer.TryAdd(id, out _);
            }
        }
        
    }
}*/