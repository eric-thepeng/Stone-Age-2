using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace Treegen
{
  [ExecuteInEditMode]
  [AddComponentMenu("TREEGEN/TreeGenerator")]
  public class TreegenTreeGenerator : MonoBehaviour
  {
    public enum LeafType
    {
      Normal,
      Palm,
      Needle
    }

    [Tooltip("Surface of trunk and branches")]
    public Material TrunkMaterial;
    [Tooltip("The surface of the leaves")]
    public Material LeavesMaterial;
    [Min(0)]
    [Tooltip("Sets the noise distribution function branches")]
    public int Seed = 0;
    [Tooltip("The impact of noise functions at the direction of the branches")]
    [Range(0f,5f)]
    public float TreeNoiseForce = 0.5f;
    [Min(0)]
    [Tooltip("Noise sets the function direction of the branches")]
    public int TreeNoiseSeed = 0;
    
    [Tooltip("The maximum level of inheritance growing branches")]
    [Range(0, 3)]
    public int TrunkIterations = 2;
    [Tooltip("The number of faces for the segment of the branch")]
    [Range(1,10)]
    public int Parts = 6;
    [Tooltip("The number of stages of the trunc")]
    [Range(1, 25)]
    public int Segments = 15;
    [Tooltip("Period of stages which will grow the branches")]
    public int SkipBranch = 1;

    [HideInInspector]
    [Tooltip("Suppresses the generation of stages on which the leaves grow")]
    public bool HideOnLeaves = false;
    [Tooltip("Long each individual segment of the trunk from 1 to Segments as from 0 to 1")]
    public AnimationCurve TrunkSegmentLength = new AnimationCurve(new Keyframe[2] { new Keyframe(0, 3.0f, 0, 0), new Keyframe(1, 3.0f, 0, 0) });
    [Tooltip("Total length of branches of each level from 1 to TrunkIterations as from 0 to 1")]
    public AnimationCurve TrunkLevelLength = new AnimationCurve(new Keyframe[2] { new Keyframe(0, 1.0f, -0.5f, -0.5f), new Keyframe(1, 0.3f, -0.5f, -0.5f) });
    [Tooltip("The thickness of the trunk for each individual segment from 1 to Segments as from 0 to 1")]
    public AnimationCurve TrunkThickness = new AnimationCurve(new Keyframe[2] { new Keyframe(0, 1, -0.8f, -0.8f), new Keyframe(1, 0.15f, 0, 0) });

    [Tooltip("The maximum number of branches for each segment from 1 to Segments as from 0 to 1, equal Function value * Parts")]
    public AnimationCurve MaxBranch = new AnimationCurve(new Keyframe[2] { new Keyframe(0, 0, 0.5f, 0.5f), new Keyframe(1, 1, 0.5f, 0.5f) });
    [Tooltip("The minimum number of branches for each segment from 1 to Segments as from 0 to 1, equal Function value * Parts")]
    public AnimationCurve MinBranch = new AnimationCurve(new Keyframe[2] { new Keyframe(0, 0, 0.5f, 0.5f), new Keyframe(1, 1, 0.5f, 0.5f) });

    [Tooltip("The slope of the branches for each segment from 1 to Segments as from 0 to 1")]
    public AnimationCurve Twirl = new AnimationCurve(new Keyframe[2] { new Keyframe(0, 1, -0.8f, -0.8f), new Keyframe(1, -0.15f, -0.5f, -0.5f) });

    [Tooltip("The number of stages of the branch")]
    [Range(1, 5)]
    public int BranchSegments = 2;
    [Tooltip("Period of stages which will grow the branches")]
    public int SkipSubBranch = 1;
    [Tooltip("The thickness of the branch for each individual segment from 1 to BranchSegments as from 0 to 1")]
    public AnimationCurve BranchThickness = new AnimationCurve(new Keyframe[2] { new Keyframe(0, 0.5f, -0.5f, -0.5f), new Keyframe(1, 0.1f, -0.5f, -0.5f) });
    [Tooltip("The maximum number of branches for each segment from 1 to Segments as from 0 to 1, equal Function value * Parts")]
    public AnimationCurve MaxSubBranch = new AnimationCurve(new Keyframe[2] { new Keyframe(0, 0, 0.5f, 0.5f), new Keyframe(1, 1, 0.5f, 0.5f) });
    [Tooltip("The minimum number of branches for each segment from 1 to Segments as from 0 to 1, equal Function value * Parts")]
    public AnimationCurve MinSubBranch = new AnimationCurve(new Keyframe[2] { new Keyframe(0, 0, 0.5f, 0.5f), new Keyframe(1, 1, 0.5f, 0.5f) });
    [Tooltip("The slope of the branches for each segment from 1 to Segments as from 0 to 1")]
    public AnimationCurve InnerTwirl = new AnimationCurve(new Keyframe[2] { new Keyframe(0, 0, 0.0f, 0.0f), new Keyframe(1, 0, 0.0f, 0.0f) });

    [Tooltip("The maximum level of inheritance growing branches")]
    [Range(0, 2)]
    public int RootIterations = 1;
    [Tooltip("The number of stages of the branch")]
    [Range(1, 15)]
    public int RootSegments = 6;
    [Tooltip("Period of stages which will grow the branches")]
    public int SkipRootBranch = 1;
    [Tooltip("The number of branches of the root for one segment but not more Parts")]
    [Range(0, 9)]
    public int RootBranchCount = 4;
    [Tooltip("The thickness of the root")]
    public float RootThickness = 1.0f;
    [Tooltip("Long each individual segment of the root from 1 to Segments as from 0 to 1")]
    public AnimationCurve RootSegmentLength = new AnimationCurve(new Keyframe[2] { new Keyframe(0, 2.0f, 0, 0), new Keyframe(1, 2.0f, 0, 0) });
    [Tooltip("Total length of branches of each level from 1 to TrunkIterations as from 0 to 1")]
    public AnimationCurve RootLevelLength = new AnimationCurve(new Keyframe[2] { new Keyframe(0, 0.5f, -0.5f, -0.5f), new Keyframe(1, 0.35f, -0.5f, -0.5f) });
    [Tooltip("The slope of the branches for each segment from 1 to RootSegments as from 0 to 1")]
    public AnimationCurve RootTwirl = new AnimationCurve(new Keyframe[2] { new Keyframe(0, 1, -0.5f, -0.5f), new Keyframe(1, 0, -0.5f, -0.5f) });
    [Tooltip("The impact of noise functions at the direction of the branches")]
    [Min(0f)]
    public float RootNoiseForce = 1.38f;
    [Tooltip("Noise sets the function direction of the branches")]
    [Min(0f)]
    public int RootNoiseSeed = 0;

    [Tooltip("Does a tree have to have leaves?")]
    public bool Leaves = true;
    [Tooltip("Type of leaf shape")]
    public LeafType LeavesType = LeafType.Normal;
    [Tooltip("Custom leaf")]
    public Mesh Mesh = null;
    //[HideInInspector]
    [Tooltip("The initial level branches which will grow leaves")]
    public int StartIteration = 0;
    //[HideInInspector]
    [Tooltip("The final level with leaves")]
    public int EndIteration = 1;
    //[HideInInspector]
    [Tooltip("The initial segment from which will grow the leaves")]
    public int StartSegment = 1;
    //[HideInInspector]
    [Tooltip("The number of segments with leaves")]
    [Range(1, 15)]
    public int CountSegment = 1;
    [Tooltip("The base size of the shape")]
    public Vector3 LeavesScale = new Vector3(5.0f, 5.0f, 5.0f);
    [Tooltip("Scale modifier each level of the branch from 1 to Iterations as from 0 to 1")]
    public AnimationCurve LeavesScaleCurve = new AnimationCurve(new Keyframe[2] { new Keyframe(0, 1, -0.5f, -0.5f), new Keyframe(1, 0.35f, -0.5f, -0.5f) });
    [Tooltip("Scale modifier each individual segment of the branch from 1 to Segments as from 0 to 1")]
    public AnimationCurve LeavesScaleSeg = new AnimationCurve(new Keyframe[2] { new Keyframe(0, 1, 0.5f, 0.5f), new Keyframe(1, 2, 0.5f, 0.5f) });
    [Tooltip("Offset modifier each level of the branch from 1 to Iterations as from 0 to 1")]
    public AnimationCurve LeavesOffsetCurve = new AnimationCurve(new Keyframe[2] { new Keyframe(0, 1, -0.5f, -0.5f), new Keyframe(1, 0, -0.5f, -0.5f) });
    [Range(0, 360)] 
    public float LeavesTurnaroundStrength = 25f;
    public bool LeavesRandomTurnaround = false;
    [Min(0)]
    [Tooltip("Baseline offset")]
    public float LeavesOffset = 9.8f;
    [Tooltip("The number of faces depends on the shape")]
    [Range(0, 30)] 
    public int LeavesDetail = 1;
    [Range(0, 0.35f)]
    [Tooltip("The impact of noise on the values of vertices leaves")]
    public float LeavesNoiseForce = 0.18f;
    [Min(0)]
    [Tooltip("The seed distribution of the noise leaves")]
    public int LeavesNoiseSeed = 0;

    private static MeshTmp trunc_tmp;
    private static MeshTmp leaves_tmp;
    private static int seed_tmp;
    private static AnimationCurve length_tmp;
    private static int startleaves_tmp;
    private static int endleaves_tmp;
    private static float noiseforce_tmp;
    private static int noiseseed_tmp;

    struct BranchProperties
    {
      public int iteratorMax;
      public int iterator;
      public int Segments;
      public int Skip;
      public AnimationCurve Max;
      public AnimationCurve Min;
      public AnimationCurve Thickness;
      public AnimationCurve Twirl;
      public AnimationCurve Length;
      public Vector3 TwirlDir;
    }

    private static BranchProperties InnerBranch;

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
      TrunkIterations = Mathf.Clamp(TrunkIterations, 0, 5);
      RootIterations = Mathf.Clamp(RootIterations, 0, 5);
      Parts = Mathf.Clamp(Parts, 3, 10);
      Segments = Mathf.Clamp(Segments, 1, 25);
      BranchSegments = Mathf.Clamp(BranchSegments, 1, 15);
      RootSegments = Mathf.Clamp(RootSegments, 1, 15);
      SkipBranch = Mathf.Clamp(SkipBranch, 0, 15);
      SkipSubBranch = Mathf.Clamp(SkipSubBranch, 0, 15);
      SkipRootBranch = Mathf.Clamp(SkipRootBranch, 0, 15);

      if (TrunkMaterial == null)
        TrunkMaterial = new Material(Shader.Find("Diffuse"));
      if (LeavesMaterial == null)
        LeavesMaterial = new Material(Shader.Find("Diffuse"));

      if (Leaves)
      {
        if (CountSegment == 0)
          CountSegment = 1;
      }
      else
      {
        CountSegment = 0;
      }

      seed_tmp = Seed;
      Mesh mesh = new Mesh();
      trunc_tmp.Clean();
      leaves_tmp.Clean();

      // Trunc
      startleaves_tmp = StartIteration;
      endleaves_tmp = EndIteration;
      noiseforce_tmp = TreeNoiseForce;
      noiseseed_tmp = TreeNoiseSeed;
      Vector3[] start = StartRing(Parts, TrunkThickness.Evaluate(0));
      BranchProperties Branch;
      Branch.Max = MaxBranch;
      Branch.Min = MinBranch;
      Branch.Thickness = TrunkThickness;
      Branch.iteratorMax = TrunkIterations;
      Branch.iterator = 0;
      Branch.Segments = Segments;
      Branch.Length = TrunkSegmentLength;
      Branch.Twirl = Twirl;
      Branch.TwirlDir = Vector3.zero;
      Branch.Skip = SkipBranch;

      InnerBranch.iteratorMax = TrunkIterations;
      InnerBranch.Segments = BranchSegments;
      InnerBranch.Max = MaxSubBranch;
      InnerBranch.Min = MinSubBranch;
      InnerBranch.Thickness = BranchThickness;
      InnerBranch.Length = TrunkSegmentLength;
      InnerBranch.Twirl = InnerTwirl;
      InnerBranch.Skip = SkipSubBranch;

      length_tmp = TrunkLevelLength;
      GenTrunk(ref Branch, Parts, new Vector3(0, 0, 0), new Vector3(0, 1, 0), start);

      // Root
      startleaves_tmp = 0;
      endleaves_tmp = -1;
      noiseforce_tmp = RootNoiseForce;
      noiseseed_tmp = RootNoiseSeed;

      Branch.iteratorMax = RootIterations;
      Branch.iterator = 0;
      Branch.Segments = 1;
      Branch.Skip = 0;
      Branch.Max = new AnimationCurve(new Keyframe[1] { new Keyframe(0.0f, RootBranchCount * 1.0f / Parts) });
      Branch.Min = new AnimationCurve(new Keyframe[1] { new Keyframe(0.0f, RootBranchCount * 1.0f / Parts) });
      Branch.Thickness = new AnimationCurve(new Keyframe[1] { new Keyframe(0, RootThickness) });
      InnerBranch.Length = RootSegmentLength;
      Branch.Twirl = RootTwirl;

      InnerBranch.iteratorMax = RootIterations;
      InnerBranch.Segments = RootSegments;
      InnerBranch.Skip = SkipRootBranch;
      InnerBranch.Max = new AnimationCurve(new Keyframe[1] { new Keyframe(0.0f, RootBranchCount * 1.0f / Parts) });
      InnerBranch.Min = new AnimationCurve(new Keyframe[1] { new Keyframe(0.0f, RootBranchCount * 1.0f / Parts) });
      //InnerBranch.Thickness = new AnimationCurve(new Keyframe[1] { new Keyframe(0, RootThickness) });
      InnerBranch.Length = RootSegmentLength;
      InnerBranch.Twirl = RootTwirl;
      Array.Reverse(start);

      length_tmp = RootLevelLength;
      GenTrunk(ref Branch, Parts, new Vector3(0, 0, 0), new Vector3(0, -1, 0), start);

      // Create mesh
      mesh.triangles = null;
      MeshTmp.MixToSubMesh(new MeshTmp[2] { trunc_tmp, leaves_tmp }, ref mesh);
      mesh.RecalculateNormals();
      mesh.RecalculateBounds();
      mesh.uv = TREEGEN.CalcUV(mesh.vertices, mesh.normals, mesh.bounds);
      mesh.name = "Tree";

      MeshRenderer mr = gameObject.GetComponent<MeshRenderer>();
      if (mr == null)
        mr = gameObject.AddComponent<MeshRenderer>();

      if (CountSegment > 0)
      {
        mr.sharedMaterials = new Material[2] { TrunkMaterial, LeavesMaterial };
      }
      else
      {
        mr.sharedMaterials = new Material[1] { TrunkMaterial };
      }

      MeshFilter mf = gameObject.GetComponent<MeshFilter>();
      if (mf == null)
        mf = gameObject.AddComponent<MeshFilter>();
      mf.sharedMesh = mesh;

      trunc_tmp.Clean();
      leaves_tmp.Clean();
      return mesh;
    }

    public Mesh NewGenTrunc()
    {
      TrunkIterations = Mathf.Clamp(TrunkIterations, 0, 5);
      RootIterations = Mathf.Clamp(RootIterations, 0, 5);
      Parts = Mathf.Clamp(Parts, 3, 10);
      Segments = Mathf.Clamp(Segments, 1, 25);
      BranchSegments = Mathf.Clamp(BranchSegments, 1, 15);
      RootSegments = Mathf.Clamp(RootSegments, 1, 15);
      SkipBranch = Mathf.Clamp(SkipBranch, 0, 15);
      SkipSubBranch = Mathf.Clamp(SkipSubBranch, 0, 15);
      SkipRootBranch = Mathf.Clamp(SkipRootBranch, 0, 15);

      if (TrunkMaterial == null)
        TrunkMaterial = new Material(Shader.Find("Diffuse"));
      if (LeavesMaterial == null)
        LeavesMaterial = new Material(Shader.Find("Diffuse"));

      seed_tmp = Seed;
      Mesh mesh = new Mesh();
      trunc_tmp.Clean();
      leaves_tmp.Clean();

      // Trunc
      startleaves_tmp = StartIteration;
      endleaves_tmp = EndIteration;
      noiseforce_tmp = TreeNoiseForce;
      noiseseed_tmp = TreeNoiseSeed;
      Vector3[] start = StartRing(Parts, TrunkThickness.Evaluate(0));
      BranchProperties Branch;
      Branch.Max = MaxBranch;
      Branch.Min = MinBranch;
      Branch.Thickness = TrunkThickness;
      Branch.iteratorMax = TrunkIterations;
      Branch.iterator = 0;
      Branch.Segments = Segments;
      Branch.Length = TrunkSegmentLength;
      Branch.Twirl = Twirl;
      Branch.TwirlDir = Vector3.zero;
      Branch.Skip = SkipBranch;

      InnerBranch.iteratorMax = TrunkIterations;
      InnerBranch.Segments = BranchSegments;
      InnerBranch.Max = MaxSubBranch;
      InnerBranch.Min = MinSubBranch;
      InnerBranch.Thickness = BranchThickness;
      InnerBranch.Length = TrunkSegmentLength;
      InnerBranch.Twirl = InnerTwirl;
      InnerBranch.Skip = SkipSubBranch;

      length_tmp = TrunkLevelLength;
      GenTrunk(ref Branch, Parts, new Vector3(0, 0, 0), new Vector3(0, 1, 0), start);

      // Root
      startleaves_tmp = 0;
      endleaves_tmp = -1;
      noiseforce_tmp = RootNoiseForce;
      noiseseed_tmp = RootNoiseSeed;

      Branch.iteratorMax = RootIterations;
      Branch.iterator = 0;
      Branch.Segments = 1;
      Branch.Skip = 0;
      Branch.Max = new AnimationCurve(new Keyframe[1] { new Keyframe(0.0f, RootBranchCount * 1.0f / Parts) });
      Branch.Min = new AnimationCurve(new Keyframe[1] { new Keyframe(0.0f, RootBranchCount * 1.0f / Parts) });
      Branch.Thickness = new AnimationCurve(new Keyframe[1] { new Keyframe(0, RootThickness) });
      InnerBranch.Length = RootSegmentLength;
      Branch.Twirl = RootTwirl;

      InnerBranch.iteratorMax = RootIterations;
      InnerBranch.Segments = RootSegments;
      InnerBranch.Skip = SkipRootBranch;
      InnerBranch.Max = new AnimationCurve(new Keyframe[1] { new Keyframe(0.0f, RootBranchCount * 1.0f / Parts) });
      InnerBranch.Min = new AnimationCurve(new Keyframe[1] { new Keyframe(0.0f, RootBranchCount * 1.0f / Parts) });
      InnerBranch.Thickness = new AnimationCurve(new Keyframe[1] { new Keyframe(0, RootThickness) });
      InnerBranch.Length = RootSegmentLength;
      InnerBranch.Twirl = RootTwirl;
      Array.Reverse(start);

      length_tmp = RootLevelLength;
      GenTrunk(ref Branch, Parts, new Vector3(0, 0, 0), new Vector3(0, -1, 0), start);

      mesh.triangles = null;
      MeshTmp.MixToSubMesh(new MeshTmp[1] {trunc_tmp}, ref mesh);
      mesh.RecalculateNormals();
      mesh.RecalculateBounds();
      mesh.uv = TREEGEN.CalcUV(mesh.vertices, mesh.normals, mesh.bounds);
      mesh.name = "Tree";

      MeshRenderer mr = gameObject.GetComponent<MeshRenderer>();
      if (mr == null)
        mr = gameObject.AddComponent<MeshRenderer>();
      mr.sharedMaterials = new Material[1] { TrunkMaterial };
      MeshFilter mf = gameObject.GetComponent<MeshFilter>();
      if (mf == null)
        mf = gameObject.AddComponent<MeshFilter>();
      mf.sharedMesh = mesh;

      leaves_tmp.Clean();
      return mesh;
    }

    public Mesh NewGenLeaves()
    {
      TrunkIterations = Mathf.Clamp(TrunkIterations, 0, 5);
      RootIterations = Mathf.Clamp(RootIterations, 0, 5);
      Parts = Mathf.Clamp(Parts, 3, 10);
      Segments = Mathf.Clamp(Segments, 1, 25);
      BranchSegments = Mathf.Clamp(BranchSegments, 1, 15);
      RootSegments = Mathf.Clamp(RootSegments, 1, 15);
      SkipBranch = Mathf.Clamp(SkipBranch, 0, 15);
      SkipSubBranch = Mathf.Clamp(SkipSubBranch, 0, 15);
      SkipRootBranch = Mathf.Clamp(SkipRootBranch, 0, 15);

      if (TrunkMaterial == null)
        TrunkMaterial = new Material(Shader.Find("Diffuse"));
      if (LeavesMaterial == null)
        LeavesMaterial = new Material(Shader.Find("Diffuse"));

      seed_tmp = Seed;
      Mesh mesh = new Mesh();
      trunc_tmp.Clean();
      leaves_tmp.Clean();

      // Trunc
      startleaves_tmp = StartIteration;
      endleaves_tmp = EndIteration;
      noiseforce_tmp = TreeNoiseForce;
      noiseseed_tmp = TreeNoiseSeed;
      Vector3[] start = StartRing(Parts, TrunkThickness.Evaluate(0));
      BranchProperties Branch;
      Branch.Max = MaxBranch;
      Branch.Min = MinBranch;
      Branch.Thickness = TrunkThickness;
      Branch.iteratorMax = TrunkIterations;
      Branch.iterator = 0;
      Branch.Segments = Segments;
      Branch.Length = TrunkSegmentLength;
      Branch.Twirl = Twirl;
      Branch.TwirlDir = Vector3.zero;
      Branch.Skip = SkipBranch;

      InnerBranch.iteratorMax = TrunkIterations;
      InnerBranch.Segments = BranchSegments;
      InnerBranch.Max = MaxSubBranch;
      InnerBranch.Min = MinSubBranch;
      InnerBranch.Thickness = BranchThickness;
      InnerBranch.Length = TrunkSegmentLength;
      InnerBranch.Twirl = InnerTwirl;
      InnerBranch.Skip = SkipSubBranch;

      length_tmp = TrunkLevelLength;
      GenTrunk(ref Branch, Parts, new Vector3(0, 0, 0), new Vector3(0, 1, 0), start);

      // Root
      startleaves_tmp = 0;
      endleaves_tmp = -1;
      noiseforce_tmp = RootNoiseForce;
      noiseseed_tmp = RootNoiseSeed;

      Branch.iteratorMax = RootIterations;
      Branch.iterator = 0;
      Branch.Segments = 1;
      Branch.Skip = 0;
      Branch.Max = new AnimationCurve(new Keyframe[1] { new Keyframe(0.0f, RootBranchCount * 1.0f / Parts) });
      Branch.Min = new AnimationCurve(new Keyframe[1] { new Keyframe(0.0f, RootBranchCount * 1.0f / Parts) });
      Branch.Thickness = new AnimationCurve(new Keyframe[1] { new Keyframe(0, RootThickness) });
      InnerBranch.Length = RootSegmentLength;
      Branch.Twirl = RootTwirl;

      InnerBranch.iteratorMax = RootIterations;
      InnerBranch.Segments = RootSegments;
      InnerBranch.Skip = SkipRootBranch;
      InnerBranch.Max = new AnimationCurve(new Keyframe[1] { new Keyframe(0.0f, RootBranchCount * 1.0f / Parts) });
      InnerBranch.Min = new AnimationCurve(new Keyframe[1] { new Keyframe(0.0f, RootBranchCount * 1.0f / Parts) });
      InnerBranch.Thickness = new AnimationCurve(new Keyframe[1] { new Keyframe(0, RootThickness) });
      InnerBranch.Length = RootSegmentLength;
      InnerBranch.Twirl = RootTwirl;
      Array.Reverse(start);

      length_tmp = RootLevelLength;
      GenTrunk(ref Branch, Parts, new Vector3(0, 0, 0), new Vector3(0, -1, 0), start);

      mesh.triangles = null;
      MeshTmp.MixToSubMesh(new MeshTmp[1] {leaves_tmp}, ref mesh);
      mesh.RecalculateNormals();
      mesh.RecalculateBounds();
      mesh.uv = TREEGEN.CalcUV(mesh.vertices, mesh.normals, mesh.bounds);
      mesh.name = "Tree";

      MeshRenderer mr = gameObject.GetComponent<MeshRenderer>();
      if (mr == null)
        mr = gameObject.AddComponent<MeshRenderer>();
      mr.sharedMaterials = new Material[1] { LeavesMaterial };
      MeshFilter mf = gameObject.GetComponent<MeshFilter>();
      if (mf == null)
        mf = gameObject.AddComponent<MeshFilter>();
      mf.sharedMesh = mesh;

      leaves_tmp.Clean();
      return mesh;
    }

    void MinMaxEvaluate(AnimationCurve Max, AnimationCurve Min, int Parts, float i, out int maxb, out int minb)
    {
      maxb = Mathf.Clamp(Mathf.FloorToInt(Max.Evaluate(i) * Parts), 0, Parts);
      minb = Mathf.Clamp(Mathf.FloorToInt(Min.Evaluate(i) * Parts), 0, Parts);
      if (maxb < minb)
      {
        int mmb = maxb;
        maxb = minb;
        minb = mmb;
      }
    }

    void GenTrunk(ref BranchProperties Branch, int Parts, Vector3 From, Vector3 Normal, Vector3[] RootRing)
    {
      int Segs = Branch.Segments;

      Vector3 Fromi = From;

      Vector3 To = Fromi;
      Vector3[] ring = RootRing;
      float Scale = (Branch.iterator == 0) ? 1: RootRing[0].magnitude;
      int[] br = new int[Segs];
      float step = 1.0f / Mathf.Max(1,Segs-1);
      int maxb;
      int minb;
      float s;

      for (int i = 0; i < Segs; i++)
      {
        MinMaxEvaluate(Branch.Max, Branch.Min, Parts, i * step, out maxb, out minb);
        br[i] = (Branch.iterator) < Branch.iteratorMax ? TREEGEN.rnd(seed_tmp, maxb - minb) + minb : 0;
        seed_tmp++;
        if ((Segs - i - 1) % (Branch.Skip + 1) != 0)
          br[i] = 0;
      }

      float iterStep = 1.0f / (Branch.iteratorMax <= 0 ? 1 : Branch.iteratorMax);
      float IterLength = length_tmp.Evaluate(Branch.iterator * iterStep);
      Vector3 N = Vector3.zero;
      Vector3 NF = Branch.TwirlDir;
      bool show = true;
      for (int i = 0; i < Segs - 1; i++)
      {
        s = Scale * Branch.Thickness.Evaluate(i * step);
        N = Vector3.Normalize(Normal + TREEGEN.VertexNoize(Fromi, noiseseed_tmp, noiseforce_tmp) + NF);
        To += N * (br[i] > 0 ? s : Branch.Length.Evaluate(i * step) * IterLength);

        show = !GenLeaves(ref Branch, To, N, ring[0], Segs - i - 1) || !HideOnLeaves;

        ring = GenTrunkSeg(Branch.iterator, br[i], Parts, s, Branch.Twirl.Evaluate(i * step), Fromi, To, N, ref ring, show);
        Fromi = To;
        NF += Branch.TwirlDir * step * (Segs - i - 1);
      }

      s = Scale * Branch.Thickness.Evaluate(1.0f);
      N = Vector3.Normalize(Normal + TREEGEN.VertexNoize(Fromi, noiseseed_tmp, noiseforce_tmp) + NF);
      To += N * (br[Segs - 1] > 0 ? s : Branch.Length.Evaluate(1.0f) * IterLength);
      seed_tmp++;

      show = !GenLeaves(ref Branch, To, N, ring[0], 0) || !HideOnLeaves;
      GenTrunkEnd(Branch.iterator, br[Segs - 1], Parts, Branch.Twirl.Evaluate(1.0f), Fromi, To, N, ring, show);

    }

    Vector3[] StartRing(int Parts, float Scale)
    {
      Vector3[] end = new Vector3[Parts];
      TREEGEN.RingVectors(ref end, Parts, Vector3.one * Scale);
      return end;
    }

    Vector3[] GenTrunkSeg(int Iter, int Branches, int Parts, float Scale, float Twirl, Vector3 From, Vector3 To, Vector3 Normal, ref Vector3[] RootRing,bool show = true)
    {
      int vcount = (Parts - Branches) * 4;
      Vector3[] v = new Vector3[vcount];
      Vector3[] end = new Vector3[Parts];
      bool[] br = new bool[Parts];

      Matrix4x4 u = TREEGEN.Matrix(Normal, RootRing[0].normalized);

      // generate end ring
      TREEGEN.RingVectors(ref end,Parts, Vector3.one * Scale);
      TREEGEN.TransformVectors(ref end, ref u);
      for (int i = 0; i < Parts; i++)
        br[i] = (Branches > i);

      // randomize branches
      if (Branches > 0 && Parts > 1)
      {
        for (int i = 0; i < Parts; i++)
        {
          int idb = TREEGEN.rnd(i + seed_tmp, Parts - 1);
          bool b = br[i];
          br[i] = br[idb];
          br[idb] = b;
        }
        seed_tmp += Parts;
      }

      // generate mesh
      TREEGEN.TubeSegment(ref v, ref br, Parts, From, To, ref RootRing, ref end);
      Vector3[] partv = new Vector3[4];
      for (int i = 0; i < Parts; i++)
      {
        if (br[i])
        {
          partv[0] = RootRing[i] + From;
          partv[1] = RootRing[(i + 1) % Parts] + From;
          partv[2] = end[(i + 1) % Parts] + To;
          partv[3] = end[i] + To;
          Vector3 Fromi = (partv[0] + partv[1] + partv[2] + partv[3]) * 0.25f;
          partv[0] -= Fromi;
          partv[1] -= Fromi;
          partv[2] -= Fromi;
          partv[3] -= Fromi;

          BranchProperties Branch = InnerBranch;
          Branch.iterator = Iter + 1;
          //Branch.Length = TrunkLength.Evaluate(0) * length_tmp.Evaluate(Branch.iterator * 1.0f / Branch.iteratorMax);
          Branch.TwirlDir = Normal * Twirl;

          GenTrunk(ref Branch, 4, Fromi, Vector3.Normalize(Vector3.Cross(partv[0], partv[1]).normalized + Branch.TwirlDir), partv);
        }
      }
      if (show)
        trunc_tmp.AddVertex(v, vcount, 4);

      return end;
    }

    void GenTrunkEnd(int Iter, int Branches, int Parts, float Twirl, Vector3 From, Vector3 To, Vector3 Normal, Vector3[] RootRing, bool show = true)
    {
      int vcount = (Parts - Branches) * 3;
      Vector3[] v = new Vector3[vcount];
      bool[] br = new bool[Parts];

      for (int i = 0; i < Parts; i++)
      {
        br[i] = (Branches > i);
      }

      // randomize branches
      if (Branches > 0 && Parts > 1)
      {
        for (int i = 0; i < Parts; i++)
        {
          int idb = TREEGEN.rnd(i + seed_tmp, Parts - 1);
          bool b = br[i];
          br[i] = br[idb];
          br[idb] = b;
        }
        seed_tmp += Parts;
      }

      // generate mesh
      TREEGEN.TubeEnd(ref v, ref br, Parts, From, To, ref RootRing);
      Vector3[] partv = new Vector3[3];
      for (int i = 0; i < Parts; i++)
      {
        if (br[i])
        {
          partv[0] = RootRing[i] + From;
          partv[1] = RootRing[(i + 1) % Parts] + From;
          partv[2] = To;
          Vector3 Fromi = (partv[0] + partv[1] + partv[2]) * 0.33333f;
          partv[0] -= Fromi;
          partv[1] -= Fromi;
          partv[2] -= Fromi;

          BranchProperties Branch = InnerBranch;
          Branch.iterator = Iter + 1;
          //Branch.Length = TrunkLength.Evaluate(0.0f) * length_tmp.Evaluate(Branch.iterator * 1.0f / Branch.iteratorMax);
          Branch.TwirlDir = Normal * Twirl;

          GenTrunk(ref Branch, 3, Fromi, Vector3.Normalize(Vector3.Cross(partv[0], partv[1]).normalized + Normal * Twirl), partv);
        }
      }
      if (show)
        trunc_tmp.AddVertex(v, vcount, 3);
    }

    bool GenLeaves(ref BranchProperties Branch, Vector3 From, Vector3 Normal, Vector3 Tangent, int Seg)
    {
      bool rez = Branch.iterator >= startleaves_tmp && Branch.iterator <= endleaves_tmp
        && StartSegment <= Seg && CountSegment > Seg - StartSegment;
      if (rez)
      {
        float evali = Branch.iterator * 1.0f / (Branch.iteratorMax <= 0 ? 1 : Branch.iteratorMax);
        float Twirl = Branch.Twirl.Evaluate((Seg - StartSegment) * 1.0f / CountSegment);
        float ls = LeavesScaleCurve.Evaluate(evali) * LeavesScaleSeg.Evaluate((Seg - StartSegment) * 1.0f / CountSegment);
        float offset = LeavesOffsetCurve.Evaluate(evali);
        if (LeavesType == LeafType.Normal)
        {
          if (LeavesRandomTurnaround)
          {
            if(LeavesDetail <= 2)
            GenLeavesNormal(LeavesDetail, ls * LeavesScale, From + ls * Normal * LeavesOffset * offset, Normal,Quaternion.AngleAxis(Seg * (UnityEngine.Random.Range(0f, LeavesTurnaroundStrength) / Parts), Normal) * Tangent);
            else
            GenLeavesNormal(2, ls * LeavesScale, From + ls * Normal * LeavesOffset * offset, Normal,Quaternion.AngleAxis(Seg * (UnityEngine.Random.Range(0f, LeavesTurnaroundStrength) / Parts), Normal) * Tangent);
          }
          else
          {
            if(LeavesDetail <= 2)
            GenLeavesNormal(LeavesDetail, ls * LeavesScale, From + ls * Normal * LeavesOffset * offset, Normal,Quaternion.AngleAxis(Seg * (LeavesTurnaroundStrength / Parts), Normal) * Tangent);
            else
            GenLeavesNormal(2, ls * LeavesScale, From + ls * Normal * LeavesOffset * offset, Normal,Quaternion.AngleAxis(Seg * (LeavesTurnaroundStrength / Parts), Normal) * Tangent);
          }
        }  
        if (LeavesType == LeafType.Needle)
        {
          if (LeavesRandomTurnaround)
          {
            GenLeavesNeedle(LeavesDetail, ls * LeavesScale, From + ls * Normal * LeavesOffset * offset, Normal,Quaternion.AngleAxis(Seg * (UnityEngine.Random.Range(0f, LeavesTurnaroundStrength) / Parts), Normal) * Tangent);
          }
          else
          {
            GenLeavesNeedle(LeavesDetail, ls * LeavesScale, From + ls * Normal * LeavesOffset * offset, Normal,Quaternion.AngleAxis(Seg * (LeavesTurnaroundStrength / Parts), Normal) * Tangent);
          }
        }
        if (LeavesType == LeafType.Palm)
        {
          if (LeavesRandomTurnaround)
          {
            GenLeavesPalm(LeavesDetail, Parts, LeavesScaleCurve.Evaluate(1),ls * LeavesScale, From + ls * Normal * LeavesOffset * offset, Normal, Quaternion.AngleAxis(Seg * (UnityEngine.Random.Range(0f, LeavesTurnaroundStrength) / Parts), Normal) * Tangent, Twirl);
          }
          else
          {
            GenLeavesPalm(LeavesDetail, Parts, LeavesScaleCurve.Evaluate(1),ls * LeavesScale, From + ls * Normal * LeavesOffset * offset, Normal, Quaternion.AngleAxis(Seg * (LeavesTurnaroundStrength / Parts), Normal) * Tangent, Twirl);
          }
        }
      }
      return rez;
    }
    void GenMesh(int Detail, Vector3 Scale, Vector3 From, Vector3 Normal, Vector3 Tangent)
    {
      int[] triangles = Mesh.triangles;
      int vcount = triangles.Length;
      Vector3[] v = new Vector3[vcount];
      Vector3[] vertex = Mesh.vertices;
      Matrix4x4 u = TREEGEN.Matrix(Normal, Tangent);
      for (int i = 0; i < vcount; i++)
      {
        v[i] = u.MultiplyPoint(Vector3.Scale(vertex[triangles[i]], Scale)) + From;
      }
      leaves_tmp.AddVertex(v, vcount, 3);
    }

    void GenLeavesNormal(int Detail, Vector3 Scale, Vector3 From, Vector3 Normal, Vector3 Tangent)
    {
      if (Mesh != null)
      {
        GenMesh(Detail, Scale, From, Normal, Tangent);
        return;
      }

      int vcount = (20) * 3;
      Vector3[] v = new Vector3[vcount];

      Matrix4x4 u = TREEGEN.Matrix(Normal, Tangent);

      TREEGEN.IcosahedronBegin(ref v);
      Detail = Mathf.Min(4, Detail);
      for (int i = 0; i < Detail; i++)
      {
        Vector3[] newv = new Vector3[v.Length * 4];
        TREEGEN.IcosahedronDetailed(ref v, ref newv);
        v = newv;
      }
      vcount = v.Length;

      for (int i = 0; i < vcount; i++)
      {
        v[i] = u.MultiplyPoint(v[i]);
        v[i] = Vector3.Scale((v[i] + TREEGEN.VertexNoize(v[i], LeavesNoiseSeed, LeavesNoiseForce)), Scale) + From;
      }

      leaves_tmp.AddVertex(v, vcount, 3);
    }

    void GenLeavesNeedle(int Detail, Vector3 Scale, Vector3 From, Vector3 Normal, Vector3 Tangent)
    {
      if (Mesh != null)
      {
        GenMesh(Detail, Scale, From, Normal, Tangent);
        return;
      }
      if (Detail < 3)
        Detail = 3;
      Matrix4x4 u = TREEGEN.Matrix(Normal, Tangent);
      Vector3[] v = new Vector3[Detail * 4];
      Vector3[] begin = new Vector3[Detail];
      Vector3[] end = new Vector3[Detail];
      bool[] bt = null;

      // generate cone
      TREEGEN.RingVectors(ref begin, Detail, Vector3.one * Scale.x * 0.9f);
      TREEGEN.TransformVectors(ref begin, ref u);
      TREEGEN.RingVectors(ref end, Detail, Vector3.one * Scale.x);
      for (int i = 0; i < end.Length; i++)
        if ((i & 1) == 0)
        {
          end[i] += Vector3.Normalize(end[i] - Vector3.up * 0.9f * Scale.y) * Scale.z;
          begin[i] += Vector3.Normalize(begin[i] - Vector3.up * Scale.y) * Scale.z*0.5f;
        }
      TREEGEN.TransformVectors(ref end, ref u);

      TREEGEN.TubeSegment(ref v, ref bt, Detail, From, From + Normal * 0.1f * Scale.y, ref begin, ref end);
      for (int i = 0; i < v.Length; i++)
        v[i] += TREEGEN.VertexNoize(v[i], LeavesNoiseSeed, LeavesNoiseForce);
      leaves_tmp.AddVertex(v, Detail * 4, 4);
      v = new Vector3[Detail * 3];
      TREEGEN.TubeEnd(ref v, ref bt, Detail, From + Normal * 0.1f * Scale.y, From + Normal * Scale.y, ref end);
      for (int i = 0; i < v.Length; i++)
        v[i] += TREEGEN.VertexNoize(v[i], LeavesNoiseSeed, LeavesNoiseForce);
      leaves_tmp.AddVertex(v, Detail * 3, 3);
      Array.Reverse(begin);
      TREEGEN.TubeEnd(ref v, ref bt, Detail, From, From, ref begin);
      for (int i = 0; i < v.Length; i++)
        v[i] += TREEGEN.VertexNoize(v[i], LeavesNoiseSeed, LeavesNoiseForce);
      leaves_tmp.AddVertex(v, Detail * 3, 3);
    }

    void GenLeavesPalm(int Detail, int Parts,float Rotate,Vector3 Scale, Vector3 From, Vector3 Normal, Vector3 Tangent, float Twirl)
    {
      Detail = Mathf.Clamp(Detail,1,10);
      float Nso = 1.0f / (Detail+1);
      float Ns=1.0f / Detail;
      Matrix4x4 u = TREEGEN.Matrix(Normal, Tangent);
      Vector3[] normals = new Vector3[Parts];
      Vector3[] end = new Vector3[4];
      Vector3[] tmpb = new Vector3[4];
      Vector3[] tmpe = new Vector3[4];
      TREEGEN.RingVectors(ref normals, Parts, Vector3.one);
      for (int k = 0; k < Parts; k++)
        normals[k] = Vector3.Lerp(normals[k], Vector3.up, Rotate);
      TREEGEN.TransformVectors(ref normals, ref u);
      bool[] bt = null;

      TREEGEN.RingVectors(ref end, 4, Vector3.one);

      Vector3 s = new Vector3(Scale.x, Scale.y, Scale.x*0.5f);

      for (int k = 0; k < Parts; k++)
      {
        if (Mesh != null)
        {
          GenMesh(Detail, Scale, From, normals[k], Normal);
        }
        else
        {
          Matrix4x4 b = TREEGEN.Matrix(normals[k], Normal);
          float sinb = Mathf.Sin(1 * Mathf.PI * Nso);
          Vector3 Pos = From + normals[k] * Scale.y * Nso;
          for (int j = 0; j < end.Length; j++)
            tmpb[j] = Vector3.Scale(end[j], Vector3.Scale(new Vector3(1.2f, 1.0f, 0.3f), s));
          TREEGEN.TransformVectors(ref tmpb, ref b);
          Vector3[] v = new Vector3[4 * 3];
          Array.Reverse(tmpb);
          TREEGEN.TubeEnd(ref v, ref bt, 4, Pos + Vector3.up * sinb * Scale.z, From, ref tmpb);
          for (int j = 0; j < v.Length; j++)
            v[j] += TREEGEN.VertexNoize(v[j], LeavesNoiseSeed, LeavesNoiseForce);
          leaves_tmp.AddVertex(v, 4 * 3, 3);
          Array.Reverse(tmpb);
          v = new Vector3[16];
          for (int i = 0; i < Detail; i++)
          {
            Vector3 To = Pos + normals[k] * Scale.y * Nso + Normal * Twirl;
            float sine = Mathf.Sin((i + 2) * Mathf.PI * Nso);
            float sin = Mathf.Sin((i + 1) * Mathf.PI * Ns);
            for (int j = 0; j < end.Length; j++)
              tmpe[j] = Vector3.Scale(end[j], Vector3.Scale(new Vector3(1.0f + sin, 1.0f, 0.3f), s));
            TREEGEN.TransformVectors(ref tmpe, ref b);
            TREEGEN.TubeSegment(ref v, ref bt, 4, Pos + Vector3.up * sinb * Scale.z, To + Vector3.up * sine * Scale.z, ref tmpb, ref tmpe);
            for (int j = 0; j < v.Length; j++)
              v[j] += TREEGEN.VertexNoize(v[j], LeavesNoiseSeed, LeavesNoiseForce);
            tmpe.CopyTo(tmpb, 0);
            sinb = sine;
            leaves_tmp.AddVertex(v, 16, 4);
            Pos = To;
          }
          v = new Vector3[4 * 3];
          TREEGEN.TubeEnd(ref v, ref bt, 4, Pos + Vector3.up * sinb * Scale.z, Pos + normals[k] * Scale.y * 0.2f - Vector3.up * 0.5f, ref tmpb);
          for (int j = 0; j < v.Length; j++)
            v[j] += TREEGEN.VertexNoize(v[j], LeavesNoiseSeed, LeavesNoiseForce);
          leaves_tmp.AddVertex(v, 4 * 3, 3);
        }
      }
    }

  }
}