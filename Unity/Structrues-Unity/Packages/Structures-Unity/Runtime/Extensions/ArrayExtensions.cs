using System;

namespace NuclearGames.StructuresUnity.Extensions {
    public static class ArrayExtensions {
        /// <summary>
        /// Выполняет <see cref="Array.Resize{T}(ref T[], int)" />, если длина массива меньше требуемой.
        /// </summary>
        public static void ResizeIfSmaller<T>(this T[] array, int requiredLength) {
            if (array.Length < requiredLength) {
                Array.Resize(ref array, requiredLength);
            }
        }
    }
}
