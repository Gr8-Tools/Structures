/*using System;
using NuclearGames.StructuresUnity.Extensions;
using NuclearGames.StructuresUnity.Randoms;
using NuclearGames.StructuresUnity.Structures.BST;
using NUnit.Framework;

namespace Tests.Editor.Extensions {
    internal class BinaryTreeExtensionsUnitTest {
        [Test]
        public void RandomTest() {
            var random = new UniformRandom();

            int[] GetElements() {
                const int size = 1000;
                var array = new int[size];
                for (int i = 0; i < 1000; i++) {
                    int sign = random.Next(0, 1, false) == 1 ? -1 : 1;
                    array[i] = (i * sign);
                }
                
                Array.Sort(array);
                return array;
            }
            
            var tree = new BinaryTree<int>(GetElements());
            
            for (int i = 0; i < 100; i++) {
                var randomElement = tree.GetRandom(random);
                Assert.IsTrue(tree.Contains(randomElement));
            }
        }
    }
}
*/