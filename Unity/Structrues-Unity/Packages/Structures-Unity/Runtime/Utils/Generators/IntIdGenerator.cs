/*using System.Linq;

namespace NuclearGames.StructuresUnity.Utils.Generators {
    public class IntIdGenerator : BaseIdGenerator<int> {
        public IntIdGenerator(int startSize) : base(startSize) { }
        
        protected override int Resize(int value) {
            return value + value;
        }

        protected override int[] GenerateNewRange(int start, int count) => GenerateNewRangeInternal(start, count);
        
        protected override int[] GenerateDefaultRange(int count) {
            return GenerateNewRange(default, count);
        }

        private int[] GenerateNewRangeInternal(int start, int count) {
            return Enumerable.Range(start, count).ToArray();
        }
    }
}*/