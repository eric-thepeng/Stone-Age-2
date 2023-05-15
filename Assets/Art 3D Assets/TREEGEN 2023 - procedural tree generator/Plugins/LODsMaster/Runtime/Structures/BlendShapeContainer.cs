using System.Runtime.CompilerServices;
using UnityEngine;

namespace UnityMeshSimplifier
{
    public class BlendShapeContainer
    {
        private readonly string shapeName;
        private readonly BlendShapeFrameContainer[] frames;

        public BlendShapeContainer(BlendShape blendShape)
        {
            shapeName = blendShape.ShapeName;
            frames = new BlendShapeFrameContainer[blendShape.Frames.Length];
            for (int i = 0; i < frames.Length; i++)
            {
                frames[i] = new BlendShapeFrameContainer(blendShape.Frames[i]);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void MoveVertexElement(int dst, int src)
        {
            for (int i = 0; i < frames.Length; i++)
            {
                frames[i].MoveVertexElement(dst, src);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void InterpolateVertexAttributes(int dst, int i0, int i1, int i2, ref Vector3 barycentricCoord)
        {
            for (int i = 0; i < frames.Length; i++)
            {
                frames[i].InterpolateVertexAttributes(dst, i0, i1, i2, ref barycentricCoord);
            }
        }

        public void Resize(int length, bool trimExess = false)
        {
            for (int i = 0; i < frames.Length; i++)
            {
                frames[i].Resize(length, trimExess);
            }
        }

        public BlendShape ToBlendShape()
        {
            var shapeFrames = new BlendShapeFrame[frames.Length];
            for (int i = 0; i < shapeFrames.Length; i++)
            {
                shapeFrames[i] = frames[i].ToBlendShapeFrame();
            }
            return new BlendShape(shapeName, shapeFrames);
        }
    }
}