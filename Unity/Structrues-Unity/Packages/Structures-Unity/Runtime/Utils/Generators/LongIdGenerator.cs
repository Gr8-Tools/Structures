/*using System.Linq;
using NuclearGames.StructuresUnity.Extension;
using NuclearGames.StructuresUnity.Extensions;

namespace NuclearGames.StructuresUnity.Utils.Generators {
    public class LongIdGenerator: BaseIdGenerator<long> {
        public LongIdGenerator(long startSize) : base(startSize) { }
        
        protected override long Resize(long value) {
            return value + value;
        }

        protected override long[] GenerateNewRange(long start, long count) => GenerateNewRangeInternal(start, count);
        
        protected override long[] GenerateDefaultRange(long count) {
            return GenerateNewRangeInternal(default, count);
        }

        private long[] GenerateNewRangeInternal(long start, long count) {
            return EnumerableExtensions.Range(start, count).ToArray();
        }
    }
}*/