using System;
using System.Runtime.CompilerServices;

namespace UnityMeshSimplifier
{

    
    public class Vertex : IEquatable<Vertex>
    {
        public int index;
        public Vector3d p;
        public int tstart;
        public int tcount;
        public int estart;
        public int ecount;
        public SymmetricMatrix q;
        public SymmetricMatrix qPenaltyEdge; // quadric error matrix for preserving all edges type (border, seams, etc)
        public bool borderEdge;
        public bool uvSeamEdge;
        public bool uvFoldoverEdge;

        public ToleranceSphere enclosingSphere;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vertex(int index, Vector3d p)
        {
            this.index = index;
            this.p = p;
            this.tstart = 0;
            this.tcount = 0;
            this.estart = 0;
            this.ecount = 0;
            this.q = new SymmetricMatrix(0);
            this.qPenaltyEdge = new SymmetricMatrix(0);
            this.borderEdge = true;
            this.uvSeamEdge = false;
            this.uvFoldoverEdge = false;
            enclosingSphere = null;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
            //return index;
        }

        public override bool Equals(object obj)
        {
            if (obj is Vertex)
            {
                var other = (Vertex)obj;
                return index == other.index;
            }

            return false;
        }

        public bool Equals(Vertex other)
        {
            return index == other.index;
        }

        public override string ToString()
        {
            return string.Format("v{0} ({1:F3}, {2:F3}, {3:F3}) {4} tris", index, p.x, p.y, p.z, tcount);
        }

    }
    
}