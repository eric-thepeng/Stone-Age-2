using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace UnityMeshSimplifier
{
    internal struct BorderVertex
    {
        public int index;
        public int hash;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public BorderVertex(int index, int hash)
        {
            this.index = index;
            this.hash = hash;
        }
    }

    internal class BorderVertexComparer : IComparer<BorderVertex>
    {
        public static readonly BorderVertexComparer instance = new BorderVertexComparer();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Compare(BorderVertex x, BorderVertex y)
        {
            return x.hash.CompareTo(y.hash);
        }
    }
}