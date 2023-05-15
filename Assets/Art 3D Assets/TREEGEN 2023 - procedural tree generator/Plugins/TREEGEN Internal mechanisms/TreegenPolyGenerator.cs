using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Treegen
{
  public struct MeshTmp
  {
    public Vector3[] vec;
    public int[] idx;
    public void Clean()
    {
      vec = null;
      idx = null;
    }

    public void AddVertex(Vector3[] v, int vcount, int form)
    {
      int newcount = vcount;
      if (vec != null)
        newcount += vec.Length;
      Vector3[] vec_new = new Vector3[newcount];
      newcount = 0;
      if (vec != null)
      {
        vec.CopyTo(vec_new, 0);
        newcount = vec.Length;
      }
      v.CopyTo(vec_new, newcount);
      vec = vec_new;

      int planes = (vcount / form);
      int vecc = (3 * (form - 2));
      int newcountx = (vcount / form) * (3 * (form - 2));
      if (idx != null)
        newcountx += idx.Length;
      int[] idx_new = new int[newcountx];
      newcountx = 0;
      if (idx != null)
      {
        idx.CopyTo(idx_new, 0);
        newcountx = idx.Length;
      }

      for (int i = 0; i < planes; i++)
      {
        idx_new[i * vecc + newcountx] = newcount + i * form;
        idx_new[i * vecc + newcountx + 1] = newcount + i * form + 1;
        idx_new[i * vecc + newcountx + 2] = newcount + i * form + 2;
        if (form > 3)
        {
          idx_new[i * vecc + newcountx + 3] = newcount + i * form + 1;
          idx_new[i * vecc + newcountx + 4] = newcount + i * form + 3;
          idx_new[i * vecc + newcountx + 5] = newcount + i * form + 2;
        }
      }

      idx = idx_new;
    }

    public static void MixToSubMesh(MeshTmp[] array,ref Mesh mesh)
    {
      mesh.triangles = null;
      int all = 0;
      int allSub = 0;
      for (int i = 0; i < array.Length; i++)
      {
        if (i>0 && array[i].idx != null)
          for (int j = 0; j < array[i].idx.Length; j++)
            array[i].idx[j] += all;
        if (array[i].vec != null)
        {
          allSub++;
          all += array[i].vec.Length;
        }
      }
      Vector3[] vecs = new Vector3[all];
      all = 0;
      for (int i = 0; i < array.Length; i++)
        if (array[i].vec != null)
        {
          array[i].vec.CopyTo(vecs, all);
          all += array[i].vec.Length;
        }
      mesh.vertices = vecs;
      mesh.subMeshCount = allSub;
      all = 0;
      for (int i = 0; i < array.Length; i++)
      {
        if (array[i].idx != null)
        {
          mesh.SetTriangles(array[i].idx, all);
          all++;
        }
      }
    }
  }

  public class TREEGEN
  {
    private static float[] rnd_tmp = null;

    static public float rndf(int id)
    {
      if (rnd_tmp == null || rnd_tmp.Length < 128)
      {
        rnd_tmp = new float[128];
        for (int i = 0; i < 128; i++)
        {
          float v = Mathf.Cos(i * 27.342f) * 345.342f;
          rnd_tmp[i] = v - Mathf.Floor(v);
        }
      }
      return rnd_tmp[(id < 0 ? -id : id) % 128];
    }

    static public int rnd(int id, int avr)
    {
      return Mathf.FloorToInt(rndf(id) * avr);
    }

    static public Vector3 VecrotNoize(int Seed)
    {
      return new Vector3(rndf(Seed) - 0.5f, rndf(Seed + 1) - 0.5f, rndf(Seed + 2) - 0.5f);
    }

    static public Vector3 VertexNoize(Vector3 inV, int Seed, float Force)
    {
      int h = inV.GetHashCode();
      return VecrotNoize(h + Seed) * Force;
    }

    static public Matrix4x4 Matrix(Vector3 Normal, Vector3 Tangent)
    {
      Matrix4x4 u = new Matrix4x4();
      Vector3 n2 = Tangent.normalized;
      n2 = Vector3.Cross(Normal, n2).normalized;
      u.SetColumn(0, new Vector4(n2.x, n2.y, n2.z, 0));
      u.SetColumn(1, new Vector4(Normal.x, Normal.y, Normal.z, 0));
      n2 = Vector3.Cross(n2, Normal).normalized;
      u.SetColumn(2, new Vector4(n2.x, n2.y, n2.z, 0));
      u.SetColumn(3, new Vector4(0, 0, 0, 1));
      return u;
    }

    static public void TransformVectors(ref Vector3[] ring, ref Matrix4x4 mat, int length = -1)
    {
      if (length <= 0)
        length = ring.Length;
      for (int i = 0; i < length; i++)
      {
        ring[i] = mat.MultiplyPoint3x4(ring[i]);
      }
    }

    static public Vector2[] CalcUV(Vector3[] vec, Vector3[] normal, Bounds bounds)
    {
      Vector2[] uv=new Vector2[vec.Length];
      Vector3 scale = new Vector3(1.0f/Mathf.Max(bounds.size.y, bounds.size.z),
        1.0f / Mathf.Max(bounds.size.x, bounds.size.z),
        1.0f / Mathf.Max(bounds.size.x, bounds.size.y));
      for (int i=0; i<vec.Length; i++)
      {
        if (normal[i].x > normal[i].y && normal[i].x > normal[i].z)
        {
          uv[i] = new Vector2(vec[i].z, vec[i].y) * scale.x;
        }
        else if (normal[i].y > normal[i].z)
        {
          uv[i] = new Vector2(vec[i].x, vec[i].z) * scale.y;
        } else
        {
          uv[i] = new Vector2(vec[i].x, vec[i].y) * scale.z;
        }
      }

      return uv;
    }

    static public void RingVectors(ref Vector3[] ring, int Parts, Vector3 Scale)
    {
      for (int i = 0; i < Parts; i++)
      {
        float va = i * 2.0f * Mathf.PI / Parts;
        ring[i] = Vector3.Scale(new Vector3(Mathf.Sin(va), 0, Mathf.Cos(va)),Scale);
      }
    }

    static public void TubeSegment(ref Vector3[] v, ref bool[] skip, int Parts, Vector3 From, Vector3 To,ref Vector3[] FromRing, ref Vector3[] ToRing)
    {
      int id = 0;
      for (int i = 0; i < Parts; i++)
      {
        if (skip==null || !skip[i])
        {
          v[id * 4] = FromRing[i] + From;
          v[id * 4 + 1] = FromRing[(i + 1) % Parts] + From;
          v[id * 4 + 2] = ToRing[i] + To;
          v[id * 4 + 3] = ToRing[(i + 1) % Parts] + To;
          id++;
        }
      }
    }

    static public void TubeEnd(ref Vector3[] v, ref bool[] skip, int Parts, Vector3 From, Vector3 To, ref Vector3[] FromRing)
    {
      int id = 0;
      for (int i = 0; i < Parts; i++)
      {
        if (skip == null || !skip[i])
        {
          v[id * 3] = FromRing[i] + From;
          v[id * 3 + 1] = FromRing[(i + 1) % Parts] + From;
          v[id * 3 + 2] = To;
          id++;
        }
      }
    }

    static public void Tube(ref MeshTmp mesh, int Parts, Vector3[] Pos, Vector3[] Scale, Vector3[] Normal, Vector3[] Tangent, int from=0,int c = 0xFFFFFFF, float facerotate = 0)
    {
      Vector3[] end = new Vector3[Parts];
      Vector3[] tmpb = new Vector3[Parts];
      Vector3[] tmpe = new Vector3[Parts];
      bool[] bt = null;

      RingVectors(ref end, Parts, Vector3.one);

      Matrix4x4 b = Matrix(Normal[0], Tangent[0]);
      Quaternion fr = Quaternion.AngleAxis(facerotate, Vector3.up);
      for (int j = 0; j < end.Length; j++)
      {
        end[j] = fr * end[j];
        tmpb[j] = Vector3.Scale(end[j], Scale[0]);
      }
      TransformVectors(ref tmpb, ref b);
      Vector3[] v = new Vector3[Parts * 3];
      Array.Reverse(tmpb);
      if (from <= 0 && from + c > 0)
        TubeEnd(ref v, ref bt, Parts, Pos[1], Pos[0], ref tmpb);
      //for (int j = 1; j < Pos.Length-1; j++)
      //    v[j] += TREEGEN.VertexNoize(v[j], LeavesNoiseSeed, LeavesNoiseForce);
      mesh.AddVertex(v, Parts * 3, 3);
      Array.Reverse(tmpb);
      v = new Vector3[Parts * 4];
      for (int i = 1; i < Pos.Length-2; i++)
      {
        b = Matrix(Normal[i], Tangent[i]);
        for (int j = 0; j < end.Length; j++)
          tmpe[j] = Vector3.Scale(end[j], Scale[i]);
        TransformVectors(ref tmpe, ref b);
        if (from <= i && from + c > i)
          TubeSegment(ref v, ref bt, Parts, Pos[i], Pos[i+1], ref tmpb, ref tmpe);
        //for (int j = 0; j < v.Length; j++)
        //  v[j] += TREEGEN.VertexNoize(v[j], LeavesNoiseSeed, LeavesNoiseForce);
        tmpe.CopyTo(tmpb, 0);
        mesh.AddVertex(v, Parts * 4, 4);
      }
      v = new Vector3[Parts * 3];
      if (from <= Pos.Length-1 && from + c > Pos.Length - 1)
        TubeEnd(ref v, ref bt, Parts, Pos[Pos.Length-2], Pos[Pos.Length - 1], ref tmpb);
      //for (int j = 0; j < v.Length; j++)
      //  v[j] += TREEGEN.VertexNoize(v[j], LeavesNoiseSeed, LeavesNoiseForce);
      mesh.AddVertex(v, Parts * 3, 3);
    }

    static public void IcosahedronBegin(ref Vector3[] arr)
    {
      float t = (1.0f + Mathf.Sqrt(5.0f)) * 0.5f;
      Vector3[] p ={
        new Vector3( -1.0f, t, 0.0f ), new Vector3( 1.0f, t, 0.0f ), new Vector3( -1.0f, -t, 0.0f ), new Vector3( 1.0f, -t, 0.0f ),
        new Vector3( 0.0f, -1.0f, t ), new Vector3( 0.0f, 1.0f, t ), new Vector3( 0.0f, -1.0f, -t ), new Vector3( 0.0f, 1.0f, -t ),
        new Vector3( t, 0.0f, -1.0f ), new Vector3( t, 0.0f, 1.0f ), new Vector3( -t, 0.0f, -1 ), new Vector3( -t, 0.0f, 1.0f ),
      };

      int[] idxi ={
        0, 11, 5, 0, 5, 1, 0, 1, 7, 0, 7, 10, 0, 10, 11,
        1, 5, 9, 5, 11, 4, 11, 10, 2, 10, 7, 6, 7, 1, 8,
        3, 9, 4, 3, 4, 2, 3, 2, 6, 3, 6, 8, 3, 8, 9,
        4, 9, 5, 2, 4, 11, 6, 2, 10, 8, 6, 7, 9, 8, 1
      };

      for (int i = 0; i < 20; i++)
      {
        arr[i * 3] = p[idxi[i * 3 + 0]].normalized;
        arr[i * 3 + 1] = p[idxi[i * 3 + 1]].normalized;
        arr[i * 3 + 2] = p[idxi[i * 3 + 2]].normalized;
      }
    }

    static public void IcosahedronDetailed(ref Vector3[] vecin,ref Vector3[] vecout)
    {
      Vector3[] edg = new Vector3[6];
      int[] idx = { 0, 3, 5, 5, 3, 1, 4, 3, 5, 4, 2, 4 };

      for (int j = 0; j < (vecin.Length / 3); j++)
      {
        edg[0] = vecin[j * 3];
        edg[1] = vecin[j * 3 + 1];
        edg[2] = vecin[j * 3 + 2];
        edg[3] = Vector3.Normalize(edg[0] + edg[1]);
        edg[4] = Vector3.Normalize(edg[1] + edg[2]);
        edg[5] = Vector3.Normalize(edg[2] + edg[0]);

        for (int i = 0; i < 4; i++)
        {
          int i1 = idx[i + 0], i2 = idx[i + 4], i3 = idx[i + 8];

          vecout[j * 3 * 4 + i * 3] = edg[i1];
          vecout[j * 3 * 4 + i * 3 + 1] = edg[i2];
          vecout[j * 3 * 4 + i * 3 + 2] = edg[i3];
        }
      }
    }

  }
}