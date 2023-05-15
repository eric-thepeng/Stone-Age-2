using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Treegen
{
#if UNITY_EDITOR
  public class SwitchNames : PropertyAttribute
  {

    public readonly string byProperty;
    public readonly string[] names;
    public readonly float min;
    public readonly float max;

    public SwitchNames(string[] names, string byProperty, int min, int max)
    {
      this.byProperty = byProperty;
      this.names = names;
      this.min = min;
      this.max = max;
    }
  }

  [CustomPropertyDrawer(typeof(SwitchNames))]
  public class SwitchDrawer : PropertyDrawer
  {
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
      SwitchNames switcAttr = (SwitchNames)attribute;
      SerializedProperty prop = property.serializedObject.FindProperty(switcAttr.byProperty);
      int v = prop.intValue;
      bool isSlider = switcAttr.max != switcAttr.min;
      if (switcAttr.names.Length <= v)
        return;
      GUIContent labelP = new GUIContent(switcAttr.names[v]);
      if (property.propertyType == SerializedPropertyType.Float && isSlider)
        EditorGUI.Slider(position, property, switcAttr.min, switcAttr.max, labelP);
      else if (property.propertyType == SerializedPropertyType.Integer && isSlider)
        EditorGUI.IntSlider(position, property, Mathf.FloorToInt(switcAttr.min), Mathf.FloorToInt(switcAttr.max), labelP);
      else
        EditorGUI.PropertyField(position, property, labelP);
    }
  }
#endif

  //[ExecuteInEditMode]
  //[AddComponentMenu("TREEGEN/SimpleGenerator")]
  public class TreegenSimpleGenerator : MonoBehaviour
  {
    public enum SSSType
    {
      Box = 0,
      Noise = 1,
      Crystal = 2,
      Mushroom = 3,
      Grass = 4,
      Grows = 5,
      CurvedWhirl = 6
    }

    [Header("Global")]
    [Tooltip("Main material")]
    public Material Material;
    [Tooltip("Additional material")]
    public Material Sub;

    [Tooltip("The number of faces depends on the shape")]
    [Range(0, 15)]
    public int Detail = 1;
    //public int NoiseDetail = 0;
#if UNITY_EDITOR
    [SwitchNames(new string[] {"Sacle", "Sacle", "Size, Height, Head", "Size, Height, Head", "Length, Width, Twirl", "Root, Head, Twirl", "Root, Head, Height" }, "SurfaceType",0,0)]
    [Tooltip("A set of scaling options")]
#endif
    public Vector3 BoxSize = Vector3.one;
    [Tooltip("The modifier forms of subsidence")]
    public float DownForse = 3.0f;
    [Tooltip("Allows aligning the base form")]
    public Vector3 Center = Vector3.zero;

    [Tooltip("The impact of noise on the values of vertices")]
    public float NoiseForce = 0.5f;
    [Tooltip("The seed distribution of the noise")]
    public int NoiseSeed = 0;

    [Header("Surface")]
    [Tooltip("The form type or modifier specifies the form")]
    public SSSType SurfaceType = SSSType.Box;
#if UNITY_EDITOR
    [Tooltip("The number of modifications or segments of the form")]
    [SwitchNames(new string[] { "Forse", "Modificators Count", "Face Segments", "Face Segments", "Face Segments", "Face Segments", "Face Segments" }, "SurfaceType",0,15)]
#endif
    public int SSSModCount = 2;
#if UNITY_EDITOR
    [Tooltip("Radius of influence")]
    [SwitchNames(new string[] { "Modificator Radius", "Modificator Radius", "Thickness", "Head Radius", "Thickness", "Length", "Thickness" }, "SurfaceType", 0, 0)]
#endif
    public float SSSModRadius = 1.14f;
#if UNITY_EDITOR
    [Tooltip("Distribution function values for the scale segments")]
    [SwitchNames(new string[] { "Modificator Curve", "Modificator Curve", "Thickness Per Segment", "Thickness", "Thickness Per Segment", "Thickness", "Curve" }, "SurfaceType", 0, 0)]
#endif
    public AnimationCurve SSSModCurve = new AnimationCurve(new Keyframe[2] { new Keyframe(0, 1, 0.0f, 0.0f), new Keyframe(1, 1, 0.0f, 0.0f) });

    [Header("Spray")]
    [Tooltip("Show original shape")]
    public bool Original = true;
    [Tooltip("The number of duplicates in the main form")]
    [Range(0, 100)]
    public int SprayCount = 0;
    [Tooltip("Coverage area")]
    public Vector3 SprayBox = Vector3.one;
    [Tooltip("The seed to distribution of forms")]
    public int SpraySeed = 0;
    [Tooltip("The concentration values relative to the distance shapes the center")]
    public AnimationCurve SprayPos = new AnimationCurve(new Keyframe[2] { new Keyframe(0, 0, 0.5f, 0.5f), new Keyframe(1, 1, 0.5f, 0.5f) });
    [Tooltip("Scaling shapes in relation to distance from the center")]
    public AnimationCurve SprayScale = new AnimationCurve(new Keyframe[2] { new Keyframe(0, 1, -0.5f, -0.5f), new Keyframe(1, 0.1f, -0.5f, -0.5f) });
    [Tooltip("The orientation of the shapes relative to the distance from the center")]
    public AnimationCurve SprayNormal = new AnimationCurve(new Keyframe[2] { new Keyframe(0, 0, 0.5f, 0.5f), new Keyframe(1, 1, 0.5f, 0.5f) });

    //private Mesh mesh = null;

    private static MeshTmp rock_tmp;
    private static MeshTmp sub_tmp;
    private static int seed_tmp;

    // Use this for initialization
    void Start()
    {
      NewGen();
    }

    // Update is called once per frame
    void Update()
    {
    }

    void OnValidate()
    {
      NewGen();
    }

    public Mesh NewGen()
    {
      SSSModCount = Mathf.Clamp(SSSModCount, 0, 15);
      if (Material == null)
        Material = new Material(Shader.Find("Diffuse"));
      if (Sub == null)
        Sub = new Material(Shader.Find("Diffuse"));

      //seed_tmp = Seed;
      Mesh mesh = new Mesh();
      rock_tmp.Clean();
      sub_tmp.Clean();

      seed_tmp = NoiseSeed;
      if (Original) GenRock(BoxSize, Center, Vector3.up, Vector3.right);
      Vector3 PlaneScale = new Vector3(SprayBox.x>0.0f ? 1.0f : 0.0f, SprayBox.y > 0.0f ? 1.0f : 0.0f, SprayBox.z > 0.0f ? 1.0f : 0.0f);
      for (int i = 0; i < SprayCount; i++)
      {
        Vector3 pos = Vector3.Scale(TREEGEN.VecrotNoize(i + SpraySeed) * 2.0f, PlaneScale);
        float eval = pos.magnitude;
        Vector3 epos = pos.normalized * SprayPos.Evaluate(eval);
        GenRock(BoxSize * SprayScale.Evaluate(eval), Center + Vector3.Scale(epos, SprayBox), Vector3.Normalize(Vector3.up + pos.normalized * SprayNormal.Evaluate(eval)), Vector3.right);
      }

      if (SurfaceType == SSSType.Box || SurfaceType == SSSType.Noise)
        mesh.name = "Rock";
      if (SurfaceType == SSSType.Crystal)
        mesh.name = "Crystal";
      if (SurfaceType == SSSType.Mushroom)
        mesh.name = "Mushroom";
      if (SurfaceType == SSSType.Grass)
        mesh.name = "Grass";
      if (SurfaceType == SSSType.Grows)
        mesh.name = "Grows";
      if (SurfaceType == SSSType.CurvedWhirl)
        mesh.name = "Curved Whirl";
      
      // Create mesh
      mesh.triangles = null;
      MeshTmp.MixToSubMesh(new MeshTmp[2] { rock_tmp, sub_tmp }, ref mesh);
      mesh.RecalculateNormals();
      mesh.RecalculateBounds();
      mesh.uv = TREEGEN.CalcUV(mesh.vertices, mesh.normals, mesh.bounds);

      MeshRenderer mr = gameObject.GetComponent<MeshRenderer>();
      if (mr == null)
        mr = gameObject.AddComponent<MeshRenderer>();
      if (mesh.subMeshCount == 1)
        mr.sharedMaterials = new Material[1] { Material };
      if (mesh.subMeshCount == 2)
        mr.sharedMaterials = new Material[2] { Material, Sub };
      MeshFilter mf = gameObject.GetComponent<MeshFilter>();
      if (mf == null)
        mf = gameObject.AddComponent<MeshFilter>();
      mf.sharedMesh = mesh;

      rock_tmp.Clean();
      sub_tmp.Clean();

      return mesh;
    }

    void SphereScaleSurfaceMod(ref Vector3[] v, Vector3 pos, float Offset, AnimationCurve Forse, float Radius)
    {
      Vector3 n = pos.normalized;
      for (int i = 0; i < v.Length; i++)
      {
        Vector3 p = v[i] - pos;
        float l = p.magnitude;
        if (l < Radius)
        {
          //v[i] = pos + (p * Forse.Evaluate(p.magnitude / Radius) - n * Vector3.Dot(n,p));
          v[i] = pos + p - n * Vector3.Dot(n, p) * Forse.Evaluate(p.magnitude / Radius);
        }
      }
    }

    void GenRock(Vector3 Scale, Vector3 From, Vector3 Normal, Vector3 Tangent)
    {
      if (SurfaceType == SSSType.Box || SurfaceType == SSSType.Noise)
        GenIcosahedron(Scale, From, Normal, Tangent);
      if (SurfaceType == SSSType.Crystal)
        GenCrystal(Scale, From, Normal, Tangent);
      if (SurfaceType == SSSType.Mushroom)
        GenMushroom(Scale, From, Normal, Tangent);
      if (SurfaceType == SSSType.Grass)
        Grass(Scale, From, Normal, Tangent);
      if (SurfaceType == SSSType.Grows)
        Grows(Scale, From, Normal, Tangent);
      if (SurfaceType == SSSType.CurvedWhirl)
        CurvedWhirl(Scale, From, Normal, Tangent);
    }

    void GenIcosahedron(Vector3 Scale, Vector3 From, Vector3 Normal, Vector3 Tangent)
    {
      if (rock_tmp.vec !=null && rock_tmp.vec.Length > 32768)
        return;
      Detail = Mathf.Clamp(Detail, 0, 5);
      int vcount = (20) * 3;
      Vector3[] v = new Vector3[vcount];

      Matrix4x4 u = TREEGEN.Matrix((Normal + TREEGEN.VecrotNoize(NoiseSeed).normalized * 0.5f).normalized, Tangent);

      TREEGEN.IcosahedronBegin(ref v);
      for (int i = 0; i < Detail; i++)
      {
        Vector3[] newv = new Vector3[v.Length * 4];
        TREEGEN.IcosahedronDetailed(ref v, ref newv);
        v = newv;
      }
      vcount = v.Length;

      for (int i = 0; i < vcount; i++)
      {
        v[i] += TREEGEN.VertexNoize(v[i] + From, seed_tmp, NoiseForce);
        v[i] = u.MultiplyPoint(v[i]);
      }

      if (SurfaceType == SSSType.Noise)
      {
        for (int j = 0; j < 10; j++)
          for (int i = 0; i < SSSModCount; i++)
          {
            SphereScaleSurfaceMod(ref v, TREEGEN.VecrotNoize(seed_tmp + i).normalized * 0.9f, 1.0f, SSSModCurve, SSSModRadius);
          }
        for (int i = 0; i < vcount; i++)
        {
          v[i] = Quaternion.FromToRotation(TREEGEN.VecrotNoize(seed_tmp + SSSModCount - 1).normalized, Vector3.down) * v[i];
        }
      }

      if (SurfaceType == SSSType.Box)
        for (int j = 0; j < SSSModCount; j++)
        {
          SphereScaleSurfaceMod(ref v, Vector3.left * 0.9f, 1.0f, SSSModCurve, SSSModRadius);
          SphereScaleSurfaceMod(ref v, Vector3.right * 0.9f, 1.0f, SSSModCurve, SSSModRadius);
          SphereScaleSurfaceMod(ref v, Vector3.up * 0.9f, 1.0f, SSSModCurve, SSSModRadius);
          SphereScaleSurfaceMod(ref v, Vector3.down * 0.9f, 1.0f, SSSModCurve, SSSModRadius);
          SphereScaleSurfaceMod(ref v, Vector3.forward * 0.9f, 1.0f, SSSModCurve, SSSModRadius);
          SphereScaleSurfaceMod(ref v, Vector3.back * 0.9f, 1.0f, SSSModCurve, SSSModRadius);
        }
      seed_tmp += SSSModCount;

      for (int i = 0; i < vcount; i++)
      {
        v[i] = new Vector3(v[i].x, Mathf.Lerp(-1.0f, v[i].y, Mathf.Pow(Mathf.Clamp(v[i].y * 0.5f + 0.5f, 0.0f, 1.0f), DownForse)), v[i].z);
        v[i].Scale(Scale);
        v[i] += From;
      }

      rock_tmp.AddVertex(v, vcount, 3);
    }

    void GenCrystal(Vector3 Scale, Vector3 From, Vector3 Normal, Vector3 Tangent)
    {
      SSSModCount = Mathf.Clamp(SSSModCount, 3, 15);

      Vector3[] Poss = new Vector3[Detail + 3];
      Vector3[] Scales = new Vector3[Detail + 1];
      Vector3[] Normals = new Vector3[Detail + 1];
      Vector3[] Tangents = new Vector3[Detail + 1];

      Poss[0] = From - Normal * 0.1f * Scale.y * Scale.x;
      Poss[1] = From;
      int all = (Detail <= 0) ? 1 : Detail;
      float std = 1.0f / all;
      for (int i = 0; i < Detail + 1; i++)
      {
        Normals[i] = Vector3.Normalize(Normal + TREEGEN.VertexNoize(Poss[i], NoiseSeed, 1.0f) * DownForse);
        if (i > 0) Poss[i + 1] = Poss[i] + Normals[i] * std * Scale.y * Scale.x;
        Scales[i] = Vector3.one * SSSModRadius * Scale.x * SSSModCurve.Evaluate(i * std);
        Tangents[i] = Tangent;
      }
      Poss[Detail + 2] = Poss[Detail + 1] + Normal * Scale.z * Scale.x;

      int l = rock_tmp.vec == null ? 0 : rock_tmp.vec.Length;
      TREEGEN.Tube(ref rock_tmp, SSSModCount, Poss, Scales, Normals, Tangents);
      for (int i = l; i < rock_tmp.vec.Length; i++)
      {
        rock_tmp.vec[i] += TREEGEN.VertexNoize(rock_tmp.vec[i], NoiseSeed, NoiseForce);
      }
    }

    void GenMushroom(Vector3 Scale, Vector3 From, Vector3 Normal, Vector3 Tangent)
    {
      SSSModCount = Mathf.Clamp(SSSModCount, 3, 15);

      Vector3[] Poss = new Vector3[Detail * 3 + 6];
      Vector3[] Scales = new Vector3[Detail * 3 + 4];
      Vector3[] Normals = new Vector3[Detail * 3 + 4];
      Vector3[] Tangents = new Vector3[Detail * 3 + 4];

      int all = (Detail <= 0) ? 3 : (Detail * 3 + 3);
      float stepH = 1.0f / (Detail + 1);

      for (int i = 0; i < Detail * 3 + 4; i++)
      {
        Vector3 noise = TREEGEN.VertexNoize(Normal * i + From, NoiseSeed, NoiseForce);
        Scales[i] = Vector3.one * Scale.x * SSSModCurve.Evaluate(i * 1.0f / all);
        Normals[i] = Vector3.Normalize(Normal + noise);
        if (i > Detail)
          Normals[i] = Normals[Detail];
        Tangents[i] = Tangent;
      }

      Poss[0] = From - Normals[0] * 0.1f * Scale.y * Scale.x;
      Poss[1] = From;

      Scales[2 + Detail + Detail] *= SSSModRadius;
      Scales[3 + Detail + Detail] *= SSSModRadius;
      for (int i = 0; i < Detail; i++)
      {
        Poss[i + 2] = Poss[i + 1] + Normals[i] * stepH * Scale.y * Scale.x;
      }

      Poss[2 + Detail] = Poss[1 + Detail] + Normals[Detail] * stepH * Scale.y * Scale.x;

      for (int i = 0; i < Detail; i++)
      {
        Scales[i + 2 + Detail] *= SSSModRadius;
        Scales[i + 4 + Detail + Detail] *= SSSModRadius;
      }
      for (int i = 0; i < Detail; i++)
      {
        Poss[i + 3 + Detail] = Poss[i + 2 + Detail] + Normals[Detail] * (stepH * DownForse * Scale.z) * Scale.x;
      }

      Poss[3 + Detail + Detail] = Poss[2 + Detail * 2] + Normals[Detail] * (DownForse * Scale.z) * Scale.x;
      Poss[4 + Detail + Detail] = Poss[2 + Detail] + Normals[Detail] * ((1.0f + DownForse) * Scale.z) * Scale.x;

      for (int i = 0; i < Detail; i++)
      {
        Poss[i + 5 + Detail + Detail] = Poss[2 + Detail] + Normals[Detail] * ((1.0f + stepH * (Detail - i) * DownForse) * Scale.z) * Scale.x;
      }
      Poss[5 + Detail + Detail + Detail] = Poss[2 + Detail] + Normals[Detail] * (Scale.z) * Scale.x;

      TREEGEN.Tube(ref rock_tmp, SSSModCount, Poss, Scales, Normals, Tangents, 0, 3 + (Detail) * 2);
      TREEGEN.Tube(ref sub_tmp, SSSModCount, Poss, Scales, Normals, Tangents, 3 + (Detail) * 2, (Detail + 3));
    }

    void Grass(Vector3 Scale, Vector3 From, Vector3 Normal, Vector3 Tangent)
    {
      SSSModCount = Mathf.Clamp(SSSModCount, 2, 15);

      Vector3[] Poss = new Vector3[Detail + 3];
      Vector3[] Scales = new Vector3[Detail + 1];
      Vector3[] Normals = new Vector3[Detail + 1];
      Vector3[] Tangents = new Vector3[Detail + 1];

      Quaternion r = Quaternion.FromToRotation(Vector3.forward, Vector3.Normalize(new Vector3(Normal.x, 0, Normal.z)));

      Poss[0] = From - Normal * 0.1f;
      Poss[1] = From;
      int all = (Detail <= 0) ? 1 : Detail;
      float std = 1.0f / all;
      Vector3 NF = new Vector3(0, 0, Scale.z);
      for (int i = 0; i < Detail + 1; i++)
      {
        Normals[i] = Vector3.Normalize(Normal + TREEGEN.VertexNoize(Poss[i], NoiseSeed, 1.0f) * DownForse + NF * (i * i * std * std));
        if (i > 0) Poss[i + 1] = Poss[i] + Normals[i] * std * Scale.x;
        Scales[i] = new Vector3(Scale.y, 1, 1) * SSSModRadius * SSSModCurve.Evaluate(i * std);
        Tangents[i] = r * Tangent;
        NF += r * new Vector3(0, 0, Scale.z);
      }
      Poss[Detail + 2] = Poss[Detail + 1] + Vector3.Normalize(Normal + TREEGEN.VertexNoize(Poss[Detail+1], NoiseSeed, 1.0f) * DownForse + NF) * std * Scale.x;

      int l = rock_tmp.vec == null ? 0 : rock_tmp.vec.Length;
      TREEGEN.Tube(ref rock_tmp, SSSModCount, Poss, Scales, Normals, Tangents,0,0x7FFFFFFF,45.0f);
      for (int i = l; i < rock_tmp.vec.Length; i++)
      {
        rock_tmp.vec[i] += TREEGEN.VertexNoize(rock_tmp.vec[i], NoiseSeed, NoiseForce);
      }
    }

    void Grows(Vector3 Scale, Vector3 From, Vector3 Normal, Vector3 Tangent)
    {
      SSSModCount = Mathf.Clamp(SSSModCount, 2, 15);

      Vector3[] Poss = new Vector3[Detail + 3];
      Vector3[] Scales = new Vector3[Detail + 1];
      Vector3[] Normals = new Vector3[Detail + 1];
      Vector3[] Tangents = new Vector3[Detail + 1];

      Quaternion r = Quaternion.FromToRotation(Vector3.forward, Vector3.Normalize(new Vector3(Normal.x, 0, Normal.z)));

      Poss[0] = From - Normal * 0.1f * Scale.x;
      Poss[1] = From;
      int all = (Detail <= 0) ? 1 : Detail;
      float std = 1.0f / all;
      Vector3 NF = new Vector3(0, 0, Scale.z);
      for (int i = 0; i < Detail + 1; i++)
      {
        Normals[i] = Vector3.Normalize(Normal + TREEGEN.VertexNoize(Poss[i], NoiseSeed, 1.0f) * DownForse + NF * (i * i * std * std));
        if (i > 0) Poss[i + 1] = Poss[i] + Normals[i] * std * Mathf.Lerp(Scale.x, SSSModRadius, Mathf.Clamp(i * 2.0f * std, 0, 1)) * Mathf.Lerp(Scale.y, SSSModRadius, Mathf.Clamp((1.0f - i * std) * 2.0f, 0, 1));
        Scales[i] = Vector3.one * SSSModRadius * SSSModCurve.Evaluate(i * std);
        Tangents[i] = r * Tangent;
        NF += r * new Vector3(0, 0, Scale.z);
      }
      Poss[Detail + 2] = Poss[Detail + 1] + Vector3.Normalize(Normal + TREEGEN.VertexNoize(Poss[Detail + 1], NoiseSeed, 1.0f) * DownForse + NF) * std * Scale.y;

      int l = rock_tmp.vec == null ? 0 : rock_tmp.vec.Length;
      TREEGEN.Tube(ref rock_tmp, SSSModCount, Poss, Scales, Normals, Tangents);
      for (int i = l; i < rock_tmp.vec.Length; i++)
      {
        rock_tmp.vec[i] += TREEGEN.VertexNoize(rock_tmp.vec[i], NoiseSeed, NoiseForce);
      }
    }

    void CurvedWhirl(Vector3 Scale, Vector3 From, Vector3 Normal, Vector3 Tangent)
    {
      SSSModCount = Mathf.Clamp(SSSModCount, 2, 15);

      int count = (Detail) * 2;
      Vector3[] Poss = new Vector3[count+2];
      Vector3[] Scales = new Vector3[count];
      Vector3[] Normals = new Vector3[count];
      Vector3[] Tangents = new Vector3[count];
      float r = SSSModRadius * 0.5f;

      Vector3 N = Vector3.up;
      Poss[0] = From - N * r;
      Poss[count+1] = From + N * r;
      int all = (Detail <= 1) ? 1 : Detail-1;
      float std = 1.0f / all;

      for (int i = 0; i < Detail; i++)
      {
        Scales[i] = Vector3.one * (SSSModCurve.Evaluate(i * std) + r);
        Scales[count - i - 1] = Vector3.one * (SSSModCurve.Evaluate(i * std) - r);
        Normals[i] = Vector3.Normalize(Vector3.up + TREEGEN.VertexNoize(Poss[i], NoiseSeed, NoiseForce));
        Normals[count - i - 1] = Normals[i];
        Tangents[i] = Vector3.left;
        Tangents[Detail + i] = Vector3.left;
        Poss[i + 1] = Poss[i] + Normals[i] * std * BoxSize.z;
        Poss[count - i] = Poss[i + 1];
      }
      for (int i = 0; i < Detail; i++)
      {
        float m1 = Mathf.Clamp(((Detail - 1) * 0.5f - i) * 2.0f / (Detail - 1), 0, 1) * BoxSize.x;
        float m2 = Mathf.Clamp((i - (Detail-1) * 0.5f) * 2.0f / (Detail - 1), 0, 1) * BoxSize.y;
        float m3 = Mathf.Clamp((1.0f-Mathf.Abs(i - (Detail-1) * 0.5f) * 2.0f / (Detail - 1)), 0, 1) * DownForse;
        Poss[i + 1] = Poss[i + 1] + Vector3.up * (m1 + m2 + m3);
        Poss[count - i] = Poss[count - i] + Vector3.up * (m1 + m2 + m3);
      }

      TREEGEN.Tube(ref rock_tmp, SSSModCount, Poss, Scales, Normals, Tangents);
    }
  }
}
