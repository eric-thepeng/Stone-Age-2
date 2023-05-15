using System.Runtime.CompilerServices;

namespace UnityMeshSimplifier
{
    public class Ref
    {
        public int tid;
        public int tvertex;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Set(int tid, int tvertex)
        {
            this.tid = tid;
            this.tvertex = tvertex;
        }

        public override string ToString()
        {
            return string.Format("Ref({0}, {1})", tid, tvertex);
        }
    }
}