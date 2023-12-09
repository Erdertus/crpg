// ReSharper disable InconsistentNaming

using System.Runtime.CompilerServices;

namespace XXHash;

public static partial class xxHash3
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static ulong XXH64_avalanche(ulong hash)
    {
        hash ^= hash >> 33;
        hash *= XXH_PRIME64_2;
        hash ^= hash >> 29;
        hash *= XXH_PRIME64_3;
        hash ^= hash >> 32;
        return hash;
    }
}
