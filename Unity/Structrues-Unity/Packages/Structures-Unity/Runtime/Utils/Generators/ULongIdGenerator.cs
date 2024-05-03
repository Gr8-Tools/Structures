/*using System.Linq;
using NuclearGames.StructuresUnity.Extension;
using NuclearGames.StructuresUnity.Extensions;

namespace NuclearGames.StructuresUnity.Utils.Generators {
    public class ULongIdGenerator : BaseIdGenerator<ulong> {
        public ULongIdGenerator(ulong startSize) : base(startSize) { }
        protected override ulong Resize(ulong value) {
            return value + value;
        }

        protected override ulong[] GenerateNewRange(ulong start, ulong count) {
            return EnumerableExtensions.Range(start, count).ToArray();
        }

        protected override ulong[] GenerateDefaultRange(ulong count) {
            return GenerateNewRange(default, count);
        }
    }
}*/