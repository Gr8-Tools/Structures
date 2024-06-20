using NuclearGames.StructuresUnity.Utils.Enumerators;
using NUnit.Framework;
using System.Diagnostics;

namespace Tests.Editor.Utils.Enumerators {
    internal class UnsafeRefArrayEnumeratorUnitTests {
        [Test]
        public void Enumerator() {
            var array = new int[] { 1, 2, 3, 4, 5 };
            var enumerator = new UnsafeRefArrayEnumerator<int>(array);

            int index = 0;
            while(index < array.Length) {
                Assert.That(enumerator.MoveNext(), Is.True);
                Assert.That(enumerator.Current, Is.EqualTo(array[index]));
                UnityEngine.Debug.Log($"{enumerator.Current} | {array[index]}");
                index++;
            }

            Assert.That(enumerator.MoveNext(), Is.False);
        }

        [TestCase(10000000, 1000)]
        public void Bench(int count, int retries) {
            long totalMs = 0;

            for (int r = 0; r < retries; r++) {

                int[] array = new int[count];
                for (int i = 0; i < array.Length; i++) {
                    array[i] = i;
                }

                var enumerator = new UnsafeRefArrayEnumerator<int>(array);

                var watch = new Stopwatch();
                watch.Start();

                // По енумератору.
                foreach(ref int p in enumerator) {
                    p *= 2;
                }

                // For.
                /*for(int i = 0; i < count; i++) {
                    array[i] *= 2;
                }*/

                watch.Stop();

                totalMs += watch.ElapsedMilliseconds;
            }

            UnityEngine.Debug.Log($"Avg: {totalMs / retries}ms. Total: {totalMs}");
        }
    }
}
