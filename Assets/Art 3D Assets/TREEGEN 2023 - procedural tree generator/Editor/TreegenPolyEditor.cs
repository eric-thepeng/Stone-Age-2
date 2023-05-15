using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Xml;
using System;
using System.IO;

namespace Treegen
{
  public class MeshEditorWindow : EditorWindow
  {
    const string DefaultMaterialName = "Default-Diffuse.mat";
    const string DotShaderName = "TREEGEN/ViewDotMeshEditor";
    const string EdgeShaderName = "TREEGEN/ViewEdgeMeshEditor";
    const float DotSelectionRadius = 0.99995f;

    private Material dotMaterial;
    private Material edgeMaterial;
    private Material faceMaterial;
    private Transform transform = null;
    //Object context = null;
    private Mesh edited = null;
    private Mesh tmp = null;
    private ComputeBuffer vertexBuff = null;
    private int subEdit;
    private GUIContent[] subEditNames = new GUIContent[] { new GUIContent("New", "") };
    private Vector2 mousePos;
    private int[] SelectedVertexId = null;
    private Rect selectionRect;
    private Bounds selectionBounds;
    private Quaternion selectionRotate;
    private Vector3 selectionScale;
    int currentAssetWindow = 0;
    int currentAttachWindow = 0;

    Mode mode = Mode.None;
    CreateMode createmode = CreateMode.None;
    GUIStyle selected;
    GUIStyle box;

    bool openSelection = true;
    bool openEdit = true;
    bool openSurface = true;

    bool ignoreBackfacing = false;
    bool removeDeattach = false;

    //float WeldSelected = 0.1f;
    //float WeldTarget = 4.0f;
    float ExtrudeForce = 0.4f;
    float AngleSmooth = 30.0f;

    //Color VertexColor = Color.white;
    //Color SelectVertexColor = Color.white;

    /*Mesh[] stack = new Mesh[10];
    int stackPoint = 0;
    int stackSize = 0;*/

    enum Mode
    {
      Vertex,
      Edge,
      Face,
      Polygon,
      Element,
      None
    }

    enum CreateMode
    {
      None,
      FaceV0,
      FaceV1,
      FaceV2,
    }

    [MenuItem("Window/TREEGEN/Mesh Editor Tool")]
    static void ShowEditor()
    {
      GetEditor();
    }

    public static MeshEditorWindow GetEditor()
    {
      MeshEditorWindow editor = (MeshEditorWindow)EditorWindow.GetWindow(typeof(MeshEditorWindow), false, "Mesh Editor Tool");
      editor.Show();
      return editor;
    }

    private void OnEnable()
    {
      //dotMaterial = new Material(Shader.Find("TREEGEN/ViewMeshEditor"));
      SceneView.onSceneGUIDelegate -= this.OnSceneGUI;
      SceneView.onSceneGUIDelegate += this.OnSceneGUI;
      Undo.undoRedoPerformed -= this.UndoRedoCallback;
      Undo.undoRedoPerformed += this.UndoRedoCallback;
    }

    void OnDestroy()
    {
      if (vertexBuff != null)
        vertexBuff.Release();
      SceneView.onSceneGUIDelegate -= this.OnSceneGUI;
    }

    void OnGUI()
    {
      if (selected == null)
      {
        selected = new GUIStyle(EditorStyles.toolbarButton);// EditorStyles.toolbarButton;
        selected.normal = selected.active;
        selected.onNormal = selected.onActive;
      }
      if (box == null)
      {
        box = new GUIStyle(EditorStyles.inspectorDefaultMargins);
        //box.margin = new RectOffset(4, 10, 10, 10);
        box.padding = new RectOffset(10, 10, 10, 10);
      }

      EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
      if (GUILayout.Button(new GUIContent("Create Empy", "Create new game object"), EditorStyles.toolbarButton, GUILayout.Width(90)))
      {
        GameObject obj = CreateNew(null);
        Selection.activeTransform = obj.transform;
        OpenSelected();
      }
      if (GUILayout.Button(new GUIContent("Edit selected", "Connect to selected game object for edit mesh"), EditorStyles.toolbarButton, GUILayout.Width(90)))
      {
        OpenSelected();
      }
      if (GUILayout.Button(new GUIContent("Open Asset", "Change mesh in game object mesh filter from asset"), EditorStyles.toolbarButton, GUILayout.Width(80)))
      {
        OpenAsset();
      }
      if (Event.current.commandName == "ObjectSelectorUpdated" && currentAssetWindow > 0)
      {
        SetNew((Mesh)EditorGUIUtility.GetObjectPickerObject());
        UpdateMeshFilter(edited);
      }
      if (Event.current.commandName == "ObjectSelectorUpdated" && currentAttachWindow > 0)
      {
        GameObject go = (GameObject)EditorGUIUtility.GetObjectPickerObject();
        if (go != null)
          Selection.activeTransform = go.transform;
        else
          Selection.activeTransform = null;
      }
      if (Event.current.commandName == "ObjectSelectorClosed" && currentAttachWindow > 0)
      {
        AttachSelectedObject();
        UpdateMeshFilter(edited);
      }
      if (GUILayout.Button(new GUIContent("Save Asset", "Save current edited mesh to asset"), EditorStyles.toolbarButton, GUILayout.Width(80)))
      {
        string path = EditorUtility.SaveFilePanelInProject("Save Asset", edited.name + ".asset", "asset", "");
        if (path.Length != 0)
        {
          Mesh mesh = Instantiate(edited);
          mesh.name = Path.GetFileNameWithoutExtension(path);
          AssetDatabase.CreateAsset(mesh, path);
          AssetDatabase.SaveAssets();
        }
      }
      GUILayout.Label("");
      EditorGUILayout.EndHorizontal();
      EditorGUILayout.BeginVertical(box);
      string nameObject = "None";
      if (transform != null)
        nameObject = transform.name;
      GUILayout.Label("Game Object: " + nameObject);
      EditorGUILayout.BeginHorizontal();
      Mesh changeMesh = edited;
      edited = (Mesh)EditorGUILayout.ObjectField("Object Mesh Filter", edited, typeof(Mesh), false); //???
      if (changeMesh != edited)
      {
        SetNew(edited);
        UpdateMeshFilter(edited);
      }
      EditorGUILayout.EndHorizontal();
      GUILayout.Space(10);
      EditorGUILayout.BeginHorizontal();
      int vertex = tmp == null ? 0 : tmp.vertices.Length;
      GUILayout.Label("vertex : " + vertex);
      int triangles = tmp == null ? 0 : tmp.triangles.Length;
      GUILayout.Label("triangles : " + (triangles / 3) + " (" + triangles + ")");
      int subMesh = tmp == null ? 0 : tmp.subMeshCount;
      GUILayout.Label("sub mesh : " + subMesh);
      EditorGUILayout.EndHorizontal();
      EditorGUILayout.EndVertical();

      //EditorGUILayout.InspectorTitlebar(true,(Object)null);
      EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
      openSelection = EditorGUILayout.Foldout(openSelection, "Selection", true);
      EditorGUILayout.EndHorizontal();
      if (openSelection)
      {
        EditorGUILayout.BeginVertical(box);
        //Mode Editor
        EditorGUILayout.BeginHorizontal();
        Mode newMode = Mode.None;
        GUIStyle style = mode == Mode.Vertex ? selected : EditorStyles.toolbarButton;
        if (GUILayout.Button(new GUIContent("Vertex", "Active edit by vertex"), style, GUILayout.Width(55)))
        {
          newMode = Mode.Vertex;
        }
        GUILayout.Label("");
        style = mode == Mode.Edge ? selected : EditorStyles.toolbarButton;
        if (GUILayout.Button(new GUIContent("Edge", "Active edit by edge"), style, GUILayout.Width(55)))
        {
          newMode = Mode.Edge;
        }
        GUILayout.Label("");
        style = mode == Mode.Face ? selected : EditorStyles.toolbarButton;
        if (GUILayout.Button(new GUIContent("Face", "Active edit by face"), style, GUILayout.Width(55)))
        {
          newMode = Mode.Face;
        }
        GUILayout.Label("");
        style = mode == Mode.Polygon ? selected : EditorStyles.toolbarButton;
        if (GUILayout.Button(new GUIContent("Polygon", "Active edit by polygon"), style, GUILayout.Width(55)))
        {
          newMode = Mode.Polygon;
        }
        GUILayout.Label("");
        style = mode == Mode.Element ? selected : EditorStyles.toolbarButton;
        if (GUILayout.Button(new GUIContent("Element", "Active edit by element"), style, GUILayout.Width(55)))
        {
          newMode = Mode.Element;
        }
        if (newMode != Mode.None)
        {
          if (mode == newMode)
            mode = Mode.None;
          else
            mode = newMode;
          SceneView.RepaintAll();
        }
        EditorGUILayout.EndHorizontal();
        GUILayout.Space(10);
        subEdit = EditorGUILayout.Popup(new GUIContent("Sub mesh", "Destionation sub mesh for add new element or mesh when attached"), subEdit, subEditNames);
        ignoreBackfacing = EditorGUILayout.ToggleLeft(new GUIContent("Ignore Backfacing", "Set to disable select backfacing elements"), ignoreBackfacing);
        removeDeattach = EditorGUILayout.ToggleLeft(new GUIContent("Remove Deattach", "Auto deleting selected elemets when deattached"), removeDeattach);
        GUILayout.Space(10);
        EditorGUILayout.EndVertical();
      }

      if (mode == Mode.Vertex)
        OnVertexGUI();
      if (mode == Mode.Edge)
        OnEdgeGUI();
      if (mode == Mode.Face)
        OnFaceGUI();
      if (mode == Mode.Polygon)
        OnPolygonGUI();
      if (mode == Mode.Element)
        OnElementGUI();
    }

    void OnVertexGUI()
    {
      EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
      openEdit = EditorGUILayout.Foldout(openEdit, "Edit Geometry", true);
      EditorGUILayout.EndHorizontal();

      if (openEdit)
      {
        EditorGUILayout.BeginVertical(box);
        EditorGUILayout.BeginHorizontal();
        GUIStyle style = createmode != CreateMode.None ? selected : EditorStyles.toolbarButton;
        if (GUILayout.Button(new GUIContent("Create", "Create new face"), style, GUILayout.Width(100)))
        {
          if (createmode == CreateMode.None)
            createmode = CreateMode.FaceV0;
          else
            createmode = CreateMode.None;
        }
        GUILayout.Label("");
        if (GUILayout.Button(new GUIContent("Delete", "Remove all selected vertex"), EditorStyles.toolbarButton, GUILayout.Width(100)))
        {
          DeleteSelectedVertex();
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button(new GUIContent("Attach", "Mix meshes different game objects"), EditorStyles.toolbarButton, GUILayout.Width(100)))
        {
          OpenForAttach();
        }
        GUILayout.Label("");
        if (GUILayout.Button(new GUIContent("Collapse", "Merges selected vertices into one"), EditorStyles.toolbarButton, GUILayout.Width(100)))
        {
          CollapseSelectedVertex();
        }
        EditorGUILayout.EndHorizontal();
        /*EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Break", EditorStyles.toolbarButton))
        {

        }
        EditorGUILayout.EndHorizontal();
        GUILayout.Space(10);
        GUILayout.Label("Weld");
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Selected", EditorStyles.toolbarButton, GUILayout.Width(100)))
        {

        }
        GUILayout.Space(45);
        WeldSelected = EditorGUILayout.FloatField(WeldSelected);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Target", EditorStyles.toolbarButton, GUILayout.Width(100)))
        {

        }
        GUILayout.Space(45);
        WeldTarget = EditorGUILayout.FloatField(WeldTarget);
        EditorGUILayout.EndHorizontal();
        GUILayout.Space(10);
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Remove Isolated Vertex", EditorStyles.toolbarButton))
        {

        }
        EditorGUILayout.EndHorizontal();
        GUILayout.Space(10);*/
        /*EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("View Align", EditorStyles.toolbarButton, GUILayout.Width(100)))
        {

        }
        EditorGUILayout.EndHorizontal();*/
        EditorGUILayout.EndVertical();
      }

      EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
      openSurface = EditorGUILayout.Foldout(openSurface, "Surface Properties", true);
      EditorGUILayout.EndHorizontal();

      if (openSurface)
      {
        /*EditorGUILayout.BeginVertical(box);
        VertexColor = EditorGUILayout.ColorField("Vertex Color", VertexColor);
        GUILayout.Space(10);
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Select Color", EditorStyles.toolbarButton, GUILayout.Width(100)))
        {

        }
        GUILayout.Space(45);
        SelectVertexColor = EditorGUILayout.ColorField(SelectVertexColor);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();*/
      }
    }

    void OnEdgeGUI()
    {
      EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
      openEdit = EditorGUILayout.Foldout(openEdit, "Edit Geometry", true);
      EditorGUILayout.EndHorizontal();

      if (openEdit)
      {
        EditorGUILayout.BeginVertical(box);
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button(new GUIContent("Attach", "Mix meshes different game objects"), EditorStyles.toolbarButton, GUILayout.Width(100)))
        {
          OpenForAttach();
        }
        GUILayout.Label("");
        if (GUILayout.Button(new GUIContent("Delete", "Remove all selected edges"), EditorStyles.toolbarButton, GUILayout.Width(100)))
        {
          DeleteSelectedEdge();
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button(new GUIContent("Divide", "Divides the edge in half creating additional faces and edges"), EditorStyles.toolbarButton, GUILayout.Width(100)))
        {
          DivideEdge();
        }
        GUILayout.Label("");
        if (GUILayout.Button(new GUIContent("Turn", "Rotates the edge between adjacent faces"), EditorStyles.toolbarButton, GUILayout.Width(100)))
        {
          TurnEdge();
        }
        EditorGUILayout.EndHorizontal();
        /*GUILayout.Space(10);
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Select Open Edge", EditorStyles.toolbarButton))
        {

        }
        EditorGUILayout.EndHorizontal();*/
        EditorGUILayout.EndVertical();
      }

      EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
      openSurface = EditorGUILayout.Foldout(openSurface, "Surface Properties", true);
      EditorGUILayout.EndHorizontal();

      if (openSurface)
      {
      }
    }

    void OnFaceGUI()
    {
      EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
      openEdit = EditorGUILayout.Foldout(openEdit, "Edit Geometry", true);
      EditorGUILayout.EndHorizontal();

      if (openEdit)
      {
        EditorGUILayout.BeginVertical(box);
        EditorGUILayout.BeginHorizontal();
        GUIStyle style = createmode != CreateMode.None ? selected : EditorStyles.toolbarButton;
        if (GUILayout.Button(new GUIContent("Create", "Create new face"), style, GUILayout.Width(100)))
        {
          if (createmode == CreateMode.None)
            createmode = CreateMode.FaceV0;
          else
            createmode = CreateMode.None;
        }
        GUILayout.Label("");
        if (GUILayout.Button(new GUIContent("Delete", "Remove all selected faces"), EditorStyles.toolbarButton, GUILayout.Width(100)))
        {
          DeleteSelectedFace();
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button(new GUIContent("Attach", "Mix meshes different game objects"), EditorStyles.toolbarButton, GUILayout.Width(100)))
        {
          OpenForAttach();
        }
        GUILayout.Label("");
        if (GUILayout.Button(new GUIContent("Deattach", "Create new object with selected elements"), EditorStyles.toolbarButton, GUILayout.Width(100)))
        {
          CreateNewObjectSelectedFace();
          if (removeDeattach)
            DeleteSelectedFace();
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button(new GUIContent("Break", "Separates the selected face from the surface"), EditorStyles.toolbarButton, GUILayout.Width(100)))
        {
          BreakFace();
        }
        GUILayout.Label("");
        EditorGUILayout.EndHorizontal();
        GUILayout.Space(10);
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button(new GUIContent("Extrude", "Extrudes the selected face at the specified height"), EditorStyles.toolbarButton, GUILayout.Width(100)))
        {
          Extrude(ExtrudeForce);
        }
        GUILayout.Space(45);
        ExtrudeForce = EditorGUILayout.FloatField(ExtrudeForce);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();
      }

      EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
      openSurface = EditorGUILayout.Foldout(openSurface, "Surface Properties", true);
      EditorGUILayout.EndHorizontal();

      if (openSurface)
      {
        EditorGUILayout.BeginVertical(box);
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button(new GUIContent("Smooth", "Merge selected faсe normals"), EditorStyles.toolbarButton, GUILayout.Width(100)))
        {
          SmoothNormal(180);
        }
        GUILayout.Label("");
        if (GUILayout.Button(new GUIContent("Faces", "Faceting selected normals"), EditorStyles.toolbarButton, GUILayout.Width(100)))
        {
          SmoothNormal(0);
        }
        EditorGUILayout.EndHorizontal();
        GUILayout.Space(10);
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button(new GUIContent("By Angle", "Merge the normals of the selected faces, provided that the angle between them is less than specified"), EditorStyles.toolbarButton, GUILayout.Width(100)))
        {
          SmoothNormal(AngleSmooth);
        }
        GUILayout.Space(45);
        AngleSmooth = EditorGUILayout.FloatField(AngleSmooth);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();
      }
    }

    void OnPolygonGUI()
    {
      EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
      openEdit = EditorGUILayout.Foldout(openEdit, "Edit Geometry", true);
      EditorGUILayout.EndHorizontal();

      if (openEdit)
      {
        EditorGUILayout.BeginVertical(box);
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Create", EditorStyles.toolbarButton, GUILayout.Width(100)))
        {
        }
        GUILayout.Label("");
        if (GUILayout.Button(new GUIContent("Delete", "Remove all selected faces"), EditorStyles.toolbarButton, GUILayout.Width(100)))
        {
          DeleteSelectedFace();
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button(new GUIContent("Attach", "Mix meshes different game objects"), EditorStyles.toolbarButton, GUILayout.Width(100)))
        {
          OpenForAttach();
        }
        GUILayout.Label("");
        if (GUILayout.Button(new GUIContent("Deattach", "Create new object with selected elements"), EditorStyles.toolbarButton, GUILayout.Width(100)))
        {
          CreateNewObjectSelectedFace();
          if (removeDeattach)
            DeleteSelectedFace();
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();
      }

      EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
      openSurface = EditorGUILayout.Foldout(openSurface, "Surface Properties", true);
      EditorGUILayout.EndHorizontal();

      if (openSurface)
      {
        EditorGUILayout.BeginVertical(box);
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button(new GUIContent("Smooth", "Merge faсe normals"), EditorStyles.toolbarButton, GUILayout.Width(100)))
        {
          SmoothNormal(180);
        }
        GUILayout.Label("");
        if (GUILayout.Button(new GUIContent("Faces", "Faceting selected normals"), EditorStyles.toolbarButton, GUILayout.Width(100)))
        {
          SmoothNormal(0);
        }
        EditorGUILayout.EndHorizontal();
        GUILayout.Space(10);
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button(new GUIContent("By Angle", "Merge the normals of the selected faces, provided that the angle between them is less than specified"), EditorStyles.toolbarButton, GUILayout.Width(100)))
        {
          SmoothNormal(AngleSmooth);
        }
        GUILayout.Space(45);
        AngleSmooth = EditorGUILayout.FloatField(AngleSmooth);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();
      }
    }

    void OnElementGUI()
    {
      EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
      openEdit = EditorGUILayout.Foldout(openEdit, "Edit Geometry", true);
      EditorGUILayout.EndHorizontal();

      if (openEdit)
      {
        EditorGUILayout.BeginVertical(box);
        EditorGUILayout.BeginHorizontal();
        /*if (GUILayout.Button("Create", EditorStyles.toolbarButton, GUILayout.Width(100)))
        {
        }*/
        GUILayout.Label("");
        if (GUILayout.Button(new GUIContent("Delete", "Remove all selected faces"), EditorStyles.toolbarButton, GUILayout.Width(100)))
        {
          DeleteSelectedFace();
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button(new GUIContent("Attach", "Mix meshes different game objects"), EditorStyles.toolbarButton, GUILayout.Width(100)))
        {
          OpenForAttach();
        }
        GUILayout.Label("");
        if (GUILayout.Button(new GUIContent("Deattach", "Create new object with selected elements"), EditorStyles.toolbarButton, GUILayout.Width(100)))
        {
          CreateNewObjectSelectedFace();
          if (removeDeattach)
            DeleteSelectedFace();
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();
      }

      EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
      openSurface = EditorGUILayout.Foldout(openSurface, "Surface Properties", true);
      EditorGUILayout.EndHorizontal();

      if (openSurface)
      {
        EditorGUILayout.BeginVertical(box);
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button(new GUIContent("Smooth", "Merge selected faсe normals"), EditorStyles.toolbarButton, GUILayout.Width(100)))
        {
          SmoothNormal(180);
        }
        GUILayout.Label("");
        if (GUILayout.Button(new GUIContent("Faces", "Faceting selected normals"), EditorStyles.toolbarButton, GUILayout.Width(100)))
        {
          SmoothNormal(0);
        }
        EditorGUILayout.EndHorizontal();
        GUILayout.Space(10);
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button(new GUIContent("By Angle", "Merge the normals of the selected faces, provided that the angle between them is less than specified"), EditorStyles.toolbarButton, GUILayout.Width(100)))
        {
          SmoothNormal(AngleSmooth);
        }
        GUILayout.Space(45);
        AngleSmooth = EditorGUILayout.FloatField(AngleSmooth);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();
      }
    }

    void OnSceneGUI(SceneView sceneView)
    {
      Tools.hidden = false;
      if (tmp == null || Camera.current == null || transform == null || mode == Mode.None)
        return;
      Tools.hidden = true;

      Event e = Event.current;
      Matrix4x4 matrix = transform.localToWorldMatrix;
      Handles.BeginGUI();
      if (SelectedVertexId != null && SelectedVertexId.Length > 0)
      {
        Handles.SetCamera(Camera.current);
        bool change = false;
        if (Tools.current == Tool.Move)
        {
          EditorGUI.BeginChangeCheck();
          Vector3 vertexoffset = matrix.inverse.MultiplyPoint3x4(Handles.PositionHandle(matrix.MultiplyPoint3x4(selectionBounds.center), Quaternion.identity));
          change = EditorGUI.EndChangeCheck();
          if (change)
          {
            vertexoffset -= selectionBounds.center;
            selectionBounds.center += vertexoffset;
            Vector3[] vertexE = tmp.vertices;
            for (int i = 0; i < SelectedVertexId.Length; i++)
              vertexE[SelectedVertexId[i]] += vertexoffset;
            tmp.vertices = vertexE;
          }
        }
        if (Tools.current == Tool.Rotate)
        {
          EditorGUI.BeginChangeCheck();
          Quaternion newRotate = Handles.RotationHandle(selectionRotate, matrix.MultiplyPoint3x4(selectionBounds.center));
          change = EditorGUI.EndChangeCheck();
          if (change)
          {
            selectionRotate = Quaternion.Inverse(selectionRotate) * newRotate;
            Vector3[] vertexE = tmp.vertices;
            for (int i = 0; i < SelectedVertexId.Length; i++)
              vertexE[SelectedVertexId[i]] = selectionRotate * (vertexE[SelectedVertexId[i]] - selectionBounds.center) + selectionBounds.center;
            selectionRotate = newRotate;
            tmp.vertices = vertexE;
          }
        }
        if (Tools.current == Tool.Scale)
        {
          EditorGUI.BeginChangeCheck();
          Vector3 pos = matrix.MultiplyPoint3x4(selectionBounds.center);
          Vector3 newScale = Handles.ScaleHandle(selectionScale, pos, Quaternion.identity, Vector3.Distance(pos, Camera.current.transform.position) * 0.2f);
          change = EditorGUI.EndChangeCheck();
          if (change)
          {
            selectionScale = Vector3.Scale(newScale, new Vector3(1 / selectionScale.x, 1 / selectionScale.y, 1 / selectionScale.z));
            Vector3[] vertexE = tmp.vertices;
            for (int i = 0; i < SelectedVertexId.Length; i++)
              vertexE[SelectedVertexId[i]] = Vector3.Scale((vertexE[SelectedVertexId[i]] - selectionBounds.center), selectionScale) + selectionBounds.center;
            selectionScale = newScale;
            tmp.vertices = vertexE;
          }
        }
        if (change)
        {
          //if (e.type == EventType.mouseUp)
          Apply();
        }
      }
      Handles.EndGUI();

      //e = Event.current;
      Handles.BeginGUI();
      if (selectionRect.size != Vector2.zero && e.type != EventType.Used)
      {
        GUI.Box(selectionRect, "", GUI.skin.textArea);
        //SelectedVertexId = null;
      }
      Handles.EndGUI();

      mousePos = e.mousePosition;
      if (e.type == EventType.Repaint)
      {
        if (mode == Mode.Vertex || createmode != CreateMode.None)
          RenderDots(matrix);
        if (mode == Mode.Edge)
          RenderEdge(matrix);
        if (mode == Mode.Face || mode == Mode.Polygon || mode == Mode.Element)
          RenderFace(matrix);
        SceneView.RepaintAll();
      }

      if (createmode != CreateMode.None)
      {
        if (e.button == 0 && (e.type == EventType.MouseDown || e.type == EventType.MouseDrag))
        {
          GUIUtility.hotControl = GUIUtility.GetControlID(FocusType.Passive);
          e.Use();
        }
        if (e.button == 0 && (e.type == EventType.MouseUp || e.type == EventType.Ignore))
        {
          if (createmode == CreateMode.FaceV0)
          {
            AddFace(mousePos, 0, !ignoreBackfacing);
            createmode = CreateMode.FaceV1;
          }
          else if (createmode == CreateMode.FaceV1)
          {
            AddFace(mousePos, 1, !ignoreBackfacing);
            createmode = CreateMode.FaceV2;
          }
          else if (createmode == CreateMode.FaceV2)
          {
            AddFace(mousePos, 2, !ignoreBackfacing);
            createmode = CreateMode.FaceV0;
          }
          e.Use();
          UpdateSelectedVertex();
          GUIUtility.hotControl = 0;
        }
      }

      if (e.button == 0 && e.type == EventType.MouseDown)
      {
        selectionRect = new Rect(mousePos, Vector2.zero);
        GUIUtility.hotControl = GUIUtility.GetControlID(FocusType.Passive);
        e.Use();
      }
      if (e.button == 0 && e.type == EventType.MouseDrag)
      {
        selectionRect.size = mousePos - selectionRect.position;
        e.Use();
      }
      if (e.button == 0 && (e.type == EventType.MouseUp || e.type == EventType.Ignore))
      {
        if (mode == Mode.Vertex)
        {
          if (selectionRect.size == Vector2.zero)
            SelectedVertexId = GetSelectionVertex(mousePos, !ignoreBackfacing);
          else
            SelectedVertexId = GetSelectionVertex(selectionRect, !ignoreBackfacing);
        }
        if (mode == Mode.Edge)
        {
          SelectedVertexId = GetSelectionEdge(mousePos, !ignoreBackfacing);
        }
        if (mode == Mode.Face)
        {
          SelectedVertexId = GetSelectionFace(mousePos, !ignoreBackfacing);
        }
        if (mode == Mode.Polygon)
        {
          SelectedVertexId = GetSelectionPolygon(mousePos, !ignoreBackfacing);
        }
        if (mode == Mode.Element)
        {
          SelectedVertexId = GetSelectionElement(mousePos, !ignoreBackfacing);
        }
        selectionRect = Rect.zero;
        e.Use();

        UpdateSelectedVertex();
        GUIUtility.hotControl = 0;
      }
    }

    void RenderDots(Matrix4x4 matrix)
    {
      if (dotMaterial == null)
        dotMaterial = new Material(Shader.Find(DotShaderName));

      dotMaterial.SetVector("MouseRay", Camera.current.ScreenPointToRay(new Vector3(mousePos.x, Camera.current.pixelHeight - mousePos.y, 0)).direction);
      for (int i = 0; i < tmp.subMeshCount; i++)
        Graphics.DrawMesh(tmp, matrix, dotMaterial, 0, Camera.current, i);
    }

    void RenderEdge(Matrix4x4 matrix)
    {
      if (edgeMaterial == null)
        edgeMaterial = new Material(Shader.Find(EdgeShaderName));

      edgeMaterial.SetVector("MouseRay", Camera.current.ScreenPointToRay(new Vector3(mousePos.x, Camera.current.pixelHeight - mousePos.y, 0)).direction);
      for (int i = 0; i < tmp.subMeshCount; i++)
        Graphics.DrawMesh(tmp, matrix, edgeMaterial, 0, Camera.current, i);
    }

    void RenderFace(Matrix4x4 matrix)
    {
      if (faceMaterial == null)
        faceMaterial = new Material(Shader.Find("TREEGEN/ViewFaceMeshEditor"));

      faceMaterial.SetVector("MouseRay", Camera.current.ScreenPointToRay(new Vector3(mousePos.x, Camera.current.pixelHeight - mousePos.y, 0)).direction);
      for (int i = 0; i < tmp.subMeshCount; i++)
        Graphics.DrawMesh(tmp, matrix, faceMaterial, 0, Camera.current, i);
    }

    void SetNew(Mesh mesh)
    {
      if (mesh != edited)
        edited = mesh;
      ChangeSubMeshNames(mesh == null ? 0 : mesh.subMeshCount);
      if (vertexBuff != null)
        vertexBuff.Release();
      vertexBuff = null;
      if (mesh == null)
        return;
      tmp = new Mesh();
      tmp.vertices = mesh.vertices;
      tmp.subMeshCount = mesh.subMeshCount;
      for (int i = 0; i < mesh.subMeshCount; i++)
        tmp.SetTriangles(mesh.GetIndices(i), i);
      tmp.colors = new Color[tmp.vertexCount];
      tmp.RecalculateNormals();
      vertexBuff = new ComputeBuffer(tmp.vertexCount, 4 * 3, ComputeBufferType.Append);
      vertexBuff.SetData(tmp.vertices);
      SelectedVertexId = null;
      UpdateSelectedVertex();
    }

    void UpdateMeshFilter(Mesh mesh)
    {
      if (transform == null)
        return;
      MeshFilter mfs = transform.GetComponent<MeshFilter>();
      if (mfs == null)
        return;
      mfs.sharedMesh = mesh;
      MeshRenderer mr = transform.GetComponent<MeshRenderer>();
      if (mr == null)
        return;
      Material[] materials = new Material[mesh.subMeshCount];
      for (int i = 0; i < Mathf.Min(mr.sharedMaterials.Length, materials.Length); i++)
      {
        materials[i] = mr.sharedMaterials[i];
        if (materials[i] == null)
          materials[i] = AssetDatabase.GetBuiltinExtraResource<Material>(DefaultMaterialName);
      }
      for (int i = mr.sharedMaterials.Length; i < mesh.subMeshCount; i++)
        materials[i] = AssetDatabase.GetBuiltinExtraResource<Material>(DefaultMaterialName);
      mr.sharedMaterials = materials;
    }

    void ChangeSubMeshNames(int count)
    {
      subEditNames = new GUIContent[count + 1];
      for (int i = 0; i < subEditNames.Length - 1; i++)
        subEditNames[i] = new GUIContent(i.ToString(), "");
      subEditNames[subEditNames.Length - 1] = new GUIContent("New", "");
    }

    // Open selected object mesh for edit
    void OpenSelected()
    {
      if (Selection.activeTransform == null)
        return;
      MeshFilter[] mfs = Selection.activeTransform.GetComponentsInChildren<MeshFilter>();
      if (mfs.Length <= 0)
        return;
      edited = mfs[0].sharedMesh;
      transform = Selection.activeTransform;
      SetNew(edited);
    }

    void OpenAsset()
    {
      currentAttachWindow = 0;
      currentAssetWindow = EditorGUIUtility.GetControlID(FocusType.Passive) + 100;

      EditorGUIUtility.ShowObjectPicker<Mesh>(null, false, "", currentAssetWindow);
      //ObjectSelector.get.Show(obj, objType, property, allowSceneObjects);
    }

    void OpenForAttach()
    {
      currentAssetWindow = 0;
      currentAttachWindow = EditorGUIUtility.GetControlID(FocusType.Passive) + 100;

      EditorGUIUtility.ShowObjectPicker<MeshFilter>(null, true, "", currentAttachWindow);
    }

    GameObject CreateNew(Mesh mesh)
    {
      GameObject obj = new GameObject();
      MeshFilter mf = obj.AddComponent<MeshFilter>();
      obj.AddComponent<MeshRenderer>();
      mf.sharedMesh = mesh;
      return obj;
    }

    void Apply(bool undo = true)
    {
      if (tmp == null || edited == null)
        return;
      if (undo)
        Undo.RecordObject(edited, "Modify Vertices");
      //edited.uv
      edited.triangles = new int[0];
      edited.vertices = tmp.vertices;
      bool mfchange = edited.subMeshCount != tmp.subMeshCount;
      if (mfchange)
        ChangeSubMeshNames(tmp.subMeshCount);
      edited.subMeshCount = tmp.subMeshCount;
      for (int i = 0; i < tmp.subMeshCount; i++)
        edited.SetTriangles(tmp.GetIndices(i), i);
      edited.RecalculateNormals();
      edited.RecalculateBounds();
      if (mfchange)
        UpdateMeshFilter(edited);
      SceneView.RepaintAll();
    }

    public void UndoRedoCallback()
    {
      SetNew(edited);
      Apply(false);
      SceneView.RepaintAll();
    }

    /*void ClearStack()
    {
      for (int i = 0; i < stack.Length; i++)
        stack[i] = null;
      stackPoint = 0;
      stackSize = 0;
    }

    void StoreEditedMesh()
    {
      Undo.RecordObject(edited, "Modify Vertices");
      stack[stackPoint] = Instantiate<Mesh>(edited);
      stackPoint++;
      if (stackPoint >= stack.Length) stackPoint = 0;
      if (stackSize < stack.Length) stackSize++;
    }

    void RestoreEditedMesh()
    {
      if (stackSize <= 0)
        return;
      stackPoint--;
      stackSize--;
      if (stackPoint < 0) stackPoint = stack.Length - 1;
      Mesh ed = stack[stackPoint];
      edited.triangles = new int[0];
      edited.vertices = ed.vertices;
      for (int i = 0; i < ed.subMeshCount; i++)
      {
        edited.SetIndices(ed.GetIndices(i), MeshTopology.Triangles, i);
      }
      edited.normals = ed.normals;
      edited.uv = ed.uv;
      edited.colors = ed.colors;
      edited.RecalculateBounds();
      SetNew(edited);
    }*/

    int[] GetSelectionVertex(Vector2 pos, bool backFace = true)
    {
      Vector3[] vertex = tmp.vertices;
      Vector3[] normal = tmp.normals;
      if (normal.Length != vertex.Length)
        return null;
      Matrix4x4 matrix = transform.localToWorldMatrix;
      Ray ray = Camera.current.ScreenPointToRay(new Vector3(pos.x, Camera.current.pixelHeight - pos.y, 0));
      float md = -1;
      int id = -1;
      for (int i = 0; i < tmp.vertexCount; i++)
      {
        Vector3 vert = matrix.MultiplyPoint3x4(vertex[i]) - ray.origin;
        float fd = vert.sqrMagnitude;
        Vector3 r = vert.normalized;
        if (Vector3.Dot(ray.direction, r) > DotSelectionRadius && (fd < md || md < 0)
          && (backFace || Vector3.Dot(matrix.MultiplyVector(normal[i]), r) < 0))
        {
          md = fd;
          id = i;
        }
      }
      List<int> allEnt = new List<int>();
      if (Event.current.shift && SelectedVertexId != null)
        allEnt.AddRange(SelectedVertexId);
      if (id >= 0)
      {
        Vector3 et = vertex[id];
        for (int i = 0; i < tmp.vertexCount; i++)
        {
          if (vertex[i] == et)
            allEnt.Add(i);
        }
      }
      return allEnt.ToArray();
    }

    int[] GetSelectionVertex(Rect rect, bool backFace = true)
    {
      Vector3[] vertex = tmp.vertices;
      Vector3[] normal = tmp.normals;
      if (normal.Length != vertex.Length)
        return null;
      Matrix4x4 matrix = transform.localToWorldMatrix;
      Ray ray1 = Camera.current.ScreenPointToRay(new Vector3(rect.xMin, Camera.current.pixelHeight - rect.center.y, 0));
      Ray ray2 = Camera.current.ScreenPointToRay(new Vector3(rect.xMax, Camera.current.pixelHeight - rect.center.y, 0));
      Ray ray3 = Camera.current.ScreenPointToRay(new Vector3(rect.center.x, Camera.current.pixelHeight - rect.yMin, 0));
      Ray ray4 = Camera.current.ScreenPointToRay(new Vector3(rect.center.x, Camera.current.pixelHeight - rect.yMax, 0));
      Vector3 up = Vector3.Cross(ray1.direction, ray2.direction);
      Vector3 r1 = Vector3.Cross(up, ray1.direction);
      Vector3 r2 = Vector3.Cross(up, ray2.direction);
      Vector3 left = Vector3.Cross(ray3.direction, ray4.direction);
      Vector3 r3 = Vector3.Cross(left, ray3.direction);
      Vector3 r4 = Vector3.Cross(left, ray4.direction);
      List<int> allEnt = new List<int>();
      for (int i = 0; i < tmp.vertexCount; i++)
      {
        Vector3 vert = matrix.MultiplyPoint3x4(vertex[i]) - ray1.origin;
        float fd = vert.sqrMagnitude;
        Vector3 r = vert.normalized;
        if (Vector3.Dot(r1, r) * Vector3.Dot(r2, r) < 0 && Vector3.Dot(r3, r) * Vector3.Dot(r4, r) < 0
          && (backFace || Vector3.Dot(matrix.MultiplyVector(normal[i]), r) < 0))
        {
          allEnt.Add(i);
        }
      }
      if (Event.current.shift && SelectedVertexId != null)
        allEnt.AddRange(SelectedVertexId);
      return allEnt.ToArray();
    }

    int[] GetSelectionEdge(Vector2 pos, bool backFace = true)
    {
      int[] triangles = tmp.triangles;
      Vector3[] vertex = tmp.vertices;
      Ray ray = Camera.current.ScreenPointToRay(new Vector3(pos.x, Camera.current.pixelHeight - pos.y, 0));
      Matrix4x4 matrix = transform.localToWorldMatrix;
      int id1 = -1;
      int id2 = -1;
      float md = -1;
      for (int i = 0; i < triangles.Length / 3; i++)
      {
        int idt = i * 3;
        int vi1 = triangles[idt];
        int vi2 = triangles[idt + 1];
        int vi3 = triangles[idt + 2];
        Vector3 v1 = matrix.MultiplyPoint3x4(vertex[vi1]) - ray.origin;
        Vector3 v2 = matrix.MultiplyPoint3x4(vertex[vi2]) - ray.origin;
        Vector3 v3 = matrix.MultiplyPoint3x4(vertex[vi3]) - ray.origin;
        Vector3 nv1 = Vector3.Cross(v1, v2).normalized;
        Vector3 nv2 = Vector3.Cross(v2, v3).normalized;
        Vector3 nv3 = Vector3.Cross(v3, v1).normalized;
        float fd1 = (v1.sqrMagnitude + v2.sqrMagnitude) * 0.5f;
        float fd2 = (v2.sqrMagnitude + v3.sqrMagnitude) * 0.5f;
        float fd3 = (v3.sqrMagnitude + v1.sqrMagnitude) * 0.5f;
        if (Mathf.Abs(Vector3.Dot(ray.direction, nv1)) < 0.007f && Vector3.Dot(ray.direction, -nv2) > 0 && Vector3.Dot(ray.direction, -nv3) > 0
          && (id1 < 0 || md > fd1))
        {
          id1 = idt;
          id2 = idt + 1;
          md = fd1;
        }
        if (Mathf.Abs(Vector3.Dot(ray.direction, nv2)) < 0.007f && Vector3.Dot(ray.direction, -nv3) > 0 && Vector3.Dot(ray.direction, -nv1) > 0
          && (id1 < 0 || md > fd2))
        {
          id1 = idt + 1;
          id2 = idt + 2;
          md = fd2;
        }
        if (Mathf.Abs(Vector3.Dot(ray.direction, nv3)) < 0.007f && Vector3.Dot(ray.direction, -nv1) > 0 && Vector3.Dot(ray.direction, -nv2) > 0
          && (id1 < 0 || md > fd3))
        {
          id1 = idt + 2;
          id2 = idt;
          md = fd3;
        }
      }
      List<int> allEnt = new List<int>();
      if (Event.current.shift && SelectedVertexId != null)
        allEnt.AddRange(SelectedVertexId);
      if (id1 >= 0)
      {
        Vector3 et1 = vertex[triangles[id1]];

        Vector3 et2 = vertex[triangles[id2]];
        for (int i = 0; i < tmp.vertexCount; i++)
        {
          if (vertex[i] == et1 || vertex[i] == et2)
            allEnt.Add(i);
        }
      }
      return allEnt.ToArray();
    }

    int[] GetSelectionFace(Vector2 pos, bool backFace = true)
    {
      int[] triangles = tmp.triangles;
      Vector3[] vertex = tmp.vertices;
      int id = GetFaceId(pos, ref triangles, ref vertex, backFace);
      List<int> allEnt = new List<int>();
      if (Event.current.shift && SelectedVertexId != null)
        allEnt.AddRange(SelectedVertexId);
      if (id >= 0)
      {
        Vector3 et1 = vertex[triangles[id * 3]];
        Vector3 et2 = vertex[triangles[id * 3 + 1]];
        Vector3 et3 = vertex[triangles[id * 3 + 2]];
        for (int i = 0; i < tmp.vertexCount; i++)
        {
          if (vertex[i] == et1 || vertex[i] == et2 || vertex[i] == et3)
            allEnt.Add(i);
        }
      }
      return allEnt.ToArray();
    }

    int[] GetSelectionPolygon(Vector2 pos, bool backFace = true)
    {
      int[] triangles = tmp.triangles;
      Vector3[] vertex = tmp.vertices;
      int id = GetFaceId(pos, ref triangles, ref vertex, backFace);
      HashSet<int> triangsel = new HashSet<int>();
      HashSet<int> indexsel = new HashSet<int>();
      if (id >= 0)
        RecurentSelectionPolygon(ref triangsel, ref triangles, ref vertex, id);
      foreach (int key in triangsel)
      {
        Vector3 et1 = vertex[triangles[key * 3]];
        Vector3 et2 = vertex[triangles[key * 3 + 1]];
        Vector3 et3 = vertex[triangles[key * 3 + 2]];
        for (int i = 0; i < tmp.vertexCount; i++)
          if (!indexsel.Contains(i) && (vertex[i] == et1 || vertex[i] == et2 || vertex[i] == et3))
            indexsel.Add(i);
      }
      if (Event.current.shift && SelectedVertexId != null)
        indexsel.UnionWith(SelectedVertexId);
      int[] allEnt = new int[indexsel.Count];
      indexsel.CopyTo(allEnt);
      return allEnt;
    }

    int[] GetSelectionElement(Vector2 pos, bool backFace = true)
    {
      int[] triangles = tmp.triangles;
      Vector3[] vertex = tmp.vertices;
      int id = GetFaceId(pos, ref triangles, ref vertex, backFace);
      HashSet<int> indexsel = new HashSet<int>();
      if (id >= 0)
        RecurentSelectionElement(ref indexsel, ref triangles, ref vertex, id);
      if (Event.current.shift && SelectedVertexId != null)
        indexsel.UnionWith(SelectedVertexId);
      int[] allEnt = new int[indexsel.Count];
      indexsel.CopyTo(allEnt);
      return allEnt;
    }

    int GetFaceId(Vector2 pos, ref int[] triangles, ref Vector3[] vertex, bool backFace = true)
    {
      Ray ray = Camera.current.ScreenPointToRay(new Vector3(pos.x, Camera.current.pixelHeight - pos.y, 0));
      Matrix4x4 matrix = transform.localToWorldMatrix;
      int id = -1;
      float md = -1;
      for (int i = 0; i < triangles.Length / 3; i++)
      {
        int idt = i * 3;
        int vi1 = triangles[idt];
        int vi2 = triangles[idt + 1];
        int vi3 = triangles[idt + 2];
        Vector3 v1 = matrix.MultiplyPoint3x4(vertex[vi1]) - ray.origin;
        Vector3 v2 = matrix.MultiplyPoint3x4(vertex[vi2]) - ray.origin;
        Vector3 v3 = matrix.MultiplyPoint3x4(vertex[vi3]) - ray.origin;
        float fd = Mathf.Min(Mathf.Min(v1.sqrMagnitude, v2.sqrMagnitude), v3.sqrMagnitude);
        Vector3 nv1 = Vector3.Cross(v1, v2);
        Vector3 nv2 = Vector3.Cross(v2, v3);
        Vector3 nv3 = Vector3.Cross(v3, v1);
        if (((Vector3.Dot(ray.direction, -nv1) > 0 && Vector3.Dot(ray.direction, -nv2) > 0 && Vector3.Dot(ray.direction, -nv3) > 0)
          || (backFace && Vector3.Dot(ray.direction, nv1) > 0 && Vector3.Dot(ray.direction, nv2) > 0 && Vector3.Dot(ray.direction, nv3) > 0))
          && (id < 0 || fd < md))
        {
          md = fd;
          id = i;
        }
      }
      return id;
    }

    void RecurentSelectionPolygon(ref HashSet<int> triangsel, ref int[] triangles, ref Vector3[] vertex, int trid)
    {
      int vi1 = triangles[trid * 3];
      int vi2 = triangles[trid * 3 + 1];
      int vi3 = triangles[trid * 3 + 2];
      triangsel.Add(trid);
      for (int i = 0; i < triangles.Length; i++)
        if (!triangsel.Contains(i / 3) && (triangles[i] == vi1 || triangles[i] == vi2 || triangles[i] == vi3))
          RecurentSelectionPolygon(ref triangsel, ref triangles, ref vertex, i / 3);
    }

    void RecurentSelectionElement(ref HashSet<int> indexsel, ref int[] triangles, ref Vector3[] vertex, int trid)
    {
      int vi1 = triangles[trid * 3];
      int vi2 = triangles[trid * 3 + 1];
      int vi3 = triangles[trid * 3 + 2];
      indexsel.Add(vi1);
      indexsel.Add(vi2);
      indexsel.Add(vi3);
      Vector3 v1 = vertex[vi1];
      Vector3 v2 = vertex[vi2];
      Vector3 v3 = vertex[vi3];
      for (int i = 0; i < vertex.Length; i++)
        if (!indexsel.Contains(i) && (vertex[i] == v1 || vertex[i] == v2 || vertex[i] == v3))
          for (int j = 0; j < triangles.Length; j++)
            if (triangles[j] == i)
              RecurentSelectionElement(ref indexsel, ref triangles, ref vertex, j / 3);
    }

    Vector3 GetRayColision(Vector2 pos)
    {
      Ray ray = Camera.current.ScreenPointToRay(new Vector3(pos.x, Camera.current.pixelHeight - pos.y, 0));
      float l = Vector3.Magnitude(transform.position - ray.origin);
      return ray.origin + ray.direction * l;
    }

    void UpdateSelectedVertex()
    {
      selectionBounds.size = Vector3.zero;
      Color[] colors = tmp.colors;
      for (int i = 0; i < colors.Length; i++)
        colors[i] = new Color(0.2F, 0.3F, 0.1F, 0.5F);
      if (SelectedVertexId != null && SelectedVertexId.Length > 0)
      {
        Vector3[] vertex = tmp.vertices;
        selectionBounds.center = vertex[SelectedVertexId[0]];
        for (int i = 0; i < SelectedVertexId.Length; i++)
        {
          int id = SelectedVertexId[i];
          colors[id] = Color.white;
          selectionBounds.Encapsulate(vertex[id]);
        }
        if (Selection.activeTransform != transform)
          Selection.activeTransform = transform;
      }
      tmp.colors = colors;
      selectionRotate = Quaternion.identity;
      selectionScale = Vector3.one;
    }

    void AddFace(Vector2 pos, int index, bool backFace = true)
    {
      int[] v = GetSelectionVertex(pos, backFace);
      Vector3[] vertex = tmp.vertices;
      int[] tirangles = subEdit >= tmp.subMeshCount ? new int[0] : tmp.GetIndices(subEdit);
      Vector3[] nvertex;
      if (index == 0)
      {
        nvertex = new Vector3[vertex.Length + 3];
        int[] ntirangles = new int[tirangles.Length + 3];
        for (int i = 0; i < tirangles.Length; i++)
          ntirangles[i] = tirangles[i];
        ntirangles[tirangles.Length] = vertex.Length;
        ntirangles[tirangles.Length + 1] = vertex.Length + 1;
        ntirangles[tirangles.Length + 2] = vertex.Length + 2;
        for (int i = 0; i < vertex.Length; i++)
          nvertex[i] = vertex[i];
        tmp.vertices = nvertex;
        if (subEdit >= tmp.subMeshCount)
          tmp.subMeshCount = subEdit + 1;
        tmp.SetIndices(ntirangles, MeshTopology.Triangles, subEdit);
      }
      else
      {
        nvertex = vertex;
      }
      Vector3 posv;

      if (v.Length == 0)
      {
        posv = GetRayColision(pos);
      }
      else
      {
        posv = nvertex[v[0]];
      }
      if (index == 0)
        nvertex[nvertex.Length - 3] = posv;
      if (index < 2)
        nvertex[nvertex.Length - 2] = posv;
      if (index < 3)
        nvertex[nvertex.Length - 1] = posv;
      tmp.vertices = nvertex;
      Apply();
    }

    void AttachSelectedObject()
    {
      if (Selection.activeTransform == null || transform == null || tmp == null)
        return;
      MeshFilter mf = Selection.activeTransform.GetComponent<MeshFilter>();
      Matrix4x4 from = Selection.activeTransform.localToWorldMatrix;
      Matrix4x4 to = transform.worldToLocalMatrix;
      if (mf == null)
        return;
      if (mf.sharedMesh == null)
        return;

      Mesh attach = mf.sharedMesh;
      int incIndex = tmp.vertices.Length;
      Vector3[] vertex = tmp.vertices;
      ArrayUtility.AddRange(ref vertex, attach.vertices);
      for (int i = incIndex; i < vertex.Length; i++)
        vertex[i] = to.MultiplyPoint3x4(from.MultiplyPoint3x4(vertex[i]));
      tmp.vertices = vertex;
      /*Vector3[] normal = tmp.normals;
      ArrayUtility.AddRange(ref normal, attach.normals);
      for (int i = incIndex; i < normal.Length; i++)
        normal[i] = to.MultiplyVector(from.MultiplyVector(normal[i]));
      tmp.normals = normal;*/
      int incSub = 0;
      int subs = subEdit;
      if (subEdit >= tmp.subMeshCount)
      {
        incSub = 1;
        tmp.subMeshCount = tmp.subMeshCount + attach.subMeshCount;
      }
      for (int i = 0; i < attach.subMeshCount; i++)
      {
        int[] atriangles = attach.GetIndices(i);
        for (int j = 0; j < atriangles.Length; j++)
          atriangles[j] += incIndex;
        int[] triangles = tmp.GetIndices(subs);
        ArrayUtility.AddRange(ref triangles, atriangles);
        tmp.SetIndices(triangles, MeshTopology.Triangles, subs);
        subs += incSub;
      }
      Apply();
    }

    HashSet<int> DeleteSelectedVertex(ref Mesh mesh, int[] selectionsId)
    {
      HashSet<int> selections = new HashSet<int>();
      if (mesh == null)
        return selections;
      Vector3[] vertex = mesh.vertices;
      int[][] triangles = new int[mesh.subMeshCount][];
      int[][] ntriangles = new int[mesh.subMeshCount][];
      if (selectionsId == null || selectionsId.Length == 0)
        return selections;
      selections.UnionWith(selectionsId);
      HashSet<int> alenids = new HashSet<int>();
      int[] put = new int[mesh.subMeshCount];
      for (int i = 0; i < triangles.Length; i++)
      {
        triangles[i] = mesh.GetIndices(i);
        for (int j = 0; j < triangles[i].Length / 3; j++)
        {
          int id = j * 3;
          int v1 = triangles[i][id];
          int v2 = triangles[i][id + 1];
          int v3 = triangles[i][id + 2];
          if (!selections.Contains(v1) && !selections.Contains(v2) && !selections.Contains(v3))
          {
            triangles[i][id - put[i]] = v1;
            triangles[i][id - put[i] + 1] = v2;
            triangles[i][id - put[i] + 2] = v3;
          }
          else
          {
            if (!alenids.Contains(v1))
              alenids.Add(v1);
            if (!alenids.Contains(v2))
              alenids.Add(v2);
            if (!alenids.Contains(v3))
              alenids.Add(v3);
            put[i] += 3;
          }
        }
        ntriangles[i] = new int[triangles[i].Length - put[i]];
        for (int j = 0; j < ntriangles[i].Length; j++)
        {
          ntriangles[i][j] = triangles[i][j];
          int v = ntriangles[i][j];
          if (alenids.Contains(v))
            alenids.Remove(v);
        }
      }
      HashSet<int> ousubs = new HashSet<int>();
      for (int i = 0; i < ntriangles.Length; i++)
        if (ntriangles[i] == null || ntriangles[i].Length == 0)
          ousubs.Add(i);
      int subCount = NormalizeIndex(ref ntriangles, ref triangles, ref alenids);
      Vector3[] nvertex = new Vector3[vertex.Length - alenids.Count];
      int pv = 0;
      for (int i = 0; i < vertex.Length; i++)
      {
        if (!alenids.Contains(i))
          nvertex[i - pv] = vertex[i];
        else
          pv++;
      }
      mesh.subMeshCount = subCount;
      for (int i = 0; i < subCount; i++)
        mesh.SetIndices(ntriangles[i], MeshTopology.Triangles, i);
      mesh.vertices = nvertex;
      return ousubs;
    }

    void DeleteSelectedVertex()
    {
      DeleteSelectedVertex(ref tmp, SelectedVertexId);
      SelectedVertexId = null;
      UpdateSelectedVertex();
      Apply();
    }

    HashSet<int> DeleteSelectedTriangles(ref Mesh mesh, int[] selectionsId)
    {
      Vector3[] vertex = mesh.vertices;
      int[][] triangles = new int[mesh.subMeshCount][];
      int[][] ntriangles = new int[mesh.subMeshCount][];
      HashSet<int> selections = new HashSet<int>(selectionsId);
      HashSet<int> alenids = new HashSet<int>();
      int[] put = new int[mesh.subMeshCount];
      int sti = 0;
      for (int i = 0; i < triangles.Length; i++)
      {
        triangles[i] = mesh.GetIndices(i);
        for (int j = 0; j < triangles[i].Length / 3; j++)
        {
          int id = j * 3;
          int v1 = triangles[i][id];
          int v2 = triangles[i][id + 1];
          int v3 = triangles[i][id + 2];
          if (!selections.Contains(j + sti))
          {
            triangles[i][id - put[i]] = v1;
            triangles[i][id - put[i] + 1] = v2;
            triangles[i][id - put[i] + 2] = v3;
          }
          else
          {
            if (!alenids.Contains(v1))
              alenids.Add(v1);
            if (!alenids.Contains(v2))
              alenids.Add(v2);
            if (!alenids.Contains(v3))
              alenids.Add(v3);
            put[i] += 3;
          }
        }
        ntriangles[i] = new int[triangles[i].Length - put[i]];
        for (int j = 0; j < ntriangles[i].Length; j++)
        {
          ntriangles[i][j] = triangles[i][j];
          int v = ntriangles[i][j];
          if (alenids.Contains(v))
            alenids.Remove(v);
        }
        sti += triangles[i].Length / 3;
      }
      HashSet<int> ousubs = new HashSet<int>();
      for (int i = 0; i < ntriangles.Length; i++)
        if (ntriangles[i] == null || ntriangles[i].Length == 0)
          ousubs.Add(i);
      int subCount = NormalizeIndex(ref ntriangles, ref triangles, ref alenids);
      Vector3[] nvertex = new Vector3[vertex.Length - alenids.Count];
      int pv = 0;
      for (int i = 0; i < vertex.Length; i++)
      {
        if (!alenids.Contains(i))
          nvertex[i - pv] = vertex[i];
        else
          pv++;
      }
      mesh.subMeshCount = subCount;
      for (int i = 0; i < subCount; i++)
        mesh.SetIndices(ntriangles[i], MeshTopology.Triangles, i);
      mesh.vertices = nvertex;
      return ousubs;
    }

    void DeleteSelectedFace()
    {
      DeleteSelectedTriangles(ref tmp, SelectionToFace());
      SelectedVertexId = null;
      UpdateSelectedVertex();
      Apply();
    }

    void DeleteSelectedEdge()
    {
      DeleteSelectedTriangles(ref tmp, SelectionToFace());
      SelectedVertexId = null;
      UpdateSelectedVertex();
      Apply();
    }

    void CreateNewObjectSelectedVertex()
    {
      if (edited == null || SelectedVertexId == null)
        return;
      Mesh mesh = Instantiate(edited);
      int[] invertSelection = new int[edited.vertexCount - SelectedVertexId.Length];
      HashSet<int> selectedIds = new HashSet<int>(SelectedVertexId);
      int j = 0;
      for (int i = 0; i < edited.vertexCount; i++)
      {
        if (!selectedIds.Contains(i))
        {
          invertSelection[j] = i;
          j++;
        }
      }
      HashSet<int> mat = DeleteSelectedVertex(ref mesh, invertSelection);
      mesh.RecalculateNormals();
      mesh.RecalculateBounds();
      GameObject obj = CreateNew(mesh);
      MeshRenderer mr = obj.GetComponent<MeshRenderer>();
      MeshRenderer mrc = transform.GetComponent<MeshRenderer>();
      Material[] materials = new Material[mesh.subMeshCount];
      int mid = 0;
      for (int i = 0; i < edited.subMeshCount; i++)
        if (!mat.Contains(i))
        {
          materials[mid] = mrc.sharedMaterials[i];
          mid++;
        }
      mr.sharedMaterials = materials;
      obj.transform.position = transform.position;
      obj.transform.rotation = transform.rotation;
      obj.transform.localScale = transform.localScale;
    }

    void CreateNewObjectSelectedFace()
    {
      if (edited == null || SelectedVertexId == null)
        return;
      Mesh mesh = Instantiate(edited);
      List<int> invertSelectionFace = new List<int>();
      HashSet<int> selectedIds = new HashSet<int>(SelectedVertexId);
      int[] triangles = tmp.triangles;
      for (int i = 0; i < triangles.Length / 3; i++)
      {
        int v1 = triangles[i * 3];
        int v2 = triangles[i * 3 + 1];
        int v3 = triangles[i * 3 + 2];
        if (!selectedIds.Contains(v1) || !selectedIds.Contains(v2) || !selectedIds.Contains(v3))
        {
          invertSelectionFace.Add(i);
        }
      }
      HashSet<int> mat = DeleteSelectedTriangles(ref mesh, invertSelectionFace.ToArray());
      mesh.RecalculateNormals();
      mesh.RecalculateBounds();
      GameObject obj = CreateNew(mesh);
      MeshRenderer mr = obj.GetComponent<MeshRenderer>();
      MeshRenderer mrc = transform.GetComponent<MeshRenderer>();
      Material[] materials = new Material[mesh.subMeshCount];
      int mid = 0;
      for (int i = 0; i < edited.subMeshCount; i++)
        if (!mat.Contains(i))
        {
          materials[mid] = mrc.sharedMaterials[i];
          mid++;
        }
      mr.sharedMaterials = materials;
      obj.transform.position = transform.position;
      obj.transform.rotation = transform.rotation;
      obj.transform.localScale = transform.localScale;
    }

    void TurnEdge()
    {
      HashSet<int> selections = new HashSet<int>();
      if (SelectedVertexId != null)
        selections.UnionWith(SelectedVertexId);
      Vector3[] vertex = tmp.vertices;
      for (int i = 0; i < tmp.subMeshCount; i++)
      {
        int[] triangles = tmp.GetIndices(i);
        List<int> edges = new List<int>();
        for (int j = 0; j < triangles.Length / 3; j++)
        {
          int id = j * 3;
          int vi1 = triangles[id];
          int vi2 = triangles[id + 1];
          int vi3 = triangles[id + 2];
          if (selections.Contains(vi1) && selections.Contains(vi2))
          {
            edges.Add(id);
            edges.Add(id + 1);
          }
          if (selections.Contains(vi2) && selections.Contains(vi3))
          {
            edges.Add(id + 1);
            edges.Add(id + 2);
          }
          if (selections.Contains(vi3) && selections.Contains(vi1))
          {
            edges.Add(id + 2);
            edges.Add(id);
          }
        }
        for (int j = 0; j < edges.Count / 2; j++)
        {
          Vector3 p1 = vertex[triangles[edges[j * 2]]];
          Vector3 p2 = vertex[triangles[edges[j * 2 + 1]]];
          bool te = false;
          for (int k = 0; k < edges.Count / 2; k++) if (k != j)
            {
              if (p1 == vertex[triangles[edges[k * 2 + 1]]] && p2 == vertex[triangles[edges[k * 2]]])
              {
                TurnEdgeInside(i, edges[k * 2], edges[k * 2 + 1], edges[j * 2], edges[j * 2 + 1], triangles, vertex);
                te = true;
                break;
              }
            }
          if (te)
            break;
        }
        tmp.SetIndices(triangles, MeshTopology.Triangles, i);
      }
      tmp.vertices = vertex;
      SelectedVertexId = null;
      UpdateSelectedVertex();
      Apply();
    }

    void DivideEdge()
    {
      HashSet<int> selections = new HashSet<int>();
      if (SelectedVertexId != null)
        selections.UnionWith(SelectedVertexId);
      Vector3[] vertex = tmp.vertices;
      int[] triangles;
      int vea1 = -1;
      int vea2 = -1;
      int veb1 = -1;
      int veb2 = -1;
      for (int i = 0; i < tmp.subMeshCount; i++)
      {
        triangles = tmp.GetIndices(i);
        List<int> edges = new List<int>();
        for (int j = 0; j < triangles.Length / 3; j++)
        {
          int id = j * 3;
          int vi1 = triangles[id];
          int vi2 = triangles[id + 1];
          int vi3 = triangles[id + 2];
          if (selections.Contains(vi1) && selections.Contains(vi2))
          {
            edges.Add(id);
            edges.Add(id + 1);
          }
          if (selections.Contains(vi2) && selections.Contains(vi3))
          {
            edges.Add(id + 1);
            edges.Add(id + 2);
          }
          if (selections.Contains(vi3) && selections.Contains(vi1))
          {
            edges.Add(id + 2);
            edges.Add(id);
          }
        }
        bool te = false;
        for (int j = 0; j < edges.Count / 2; j++)
        {
          Vector3 p1 = vertex[triangles[edges[j * 2]]];
          Vector3 p2 = vertex[triangles[edges[j * 2 + 1]]];
          for (int k = 0; k < edges.Count / 2; k++) if (k != j)
            {
              if (p1 == vertex[triangles[edges[k * 2 + 1]]] && p2 == vertex[triangles[edges[k * 2]]])
              {
                vea1 = edges[k * 2];
                vea2 = edges[k * 2 + 1];
                veb1 = edges[j * 2];
                veb2 = edges[j * 2 + 1];
                te = true;
                break;
              }
            }
          if (te)
            break;
        }
        if (!te)
          continue;
        int da = (vea1 / 3);
        int db = (veb1 / 3);
        int ida = da * 3;
        int idb = db * 3;
        if (vea1 == ida || vea2 == ida)
          ida++;
        if (vea1 == ida || vea2 == ida)
          ida++;
        if (veb1 == idb || veb2 == idb)
          idb++;
        if (veb1 == idb || veb2 == idb)
          idb++;
        bool smooth = (vea1 == veb2 && vea2 == veb1);
        Vector3[] nvertex = new Vector3[vertex.Length + 1 + (smooth ? 0 : 3)];
        vertex.CopyTo(nvertex, 0);
        int idt = triangles.Length;
        int[] ntriangles = new int[triangles.Length + 6];
        triangles.CopyTo(ntriangles, 0);
        nvertex[vertex.Length] = (vertex[triangles[vea1]] + vertex[triangles[veb1]]) * 0.5f;
        if (!smooth)
        {
          nvertex[vertex.Length + 1] = nvertex[vertex.Length];
          nvertex[vertex.Length + 2] = nvertex[vertex.Length];
          nvertex[vertex.Length + 3] = nvertex[vertex.Length];
        }
        ntriangles[idt] = triangles[vea1];
        ntriangles[idt + 1] = vertex.Length;
        ntriangles[idt + 2] = triangles[ida];
        ntriangles[idt + 3] = triangles[veb1];
        ntriangles[idt + 4] = vertex.Length;
        ntriangles[idt + 5] = triangles[idb];
        ntriangles[vea1] = vertex.Length;
        ntriangles[veb1] = vertex.Length;
        if (!smooth)
        {
          ntriangles[idt + 4]++;
          ntriangles[vea1] += 2;
          ntriangles[veb1] += 3;
        }
        tmp.vertices = nvertex;
        tmp.SetIndices(ntriangles, MeshTopology.Triangles, i);
        break;
      }
      SelectedVertexId = null;
      UpdateSelectedVertex();
      Apply();
    }

    void TurnEdgeInside(int sub, int edga1, int edga2, int edgb1, int edgb2, int[] triangles, Vector3[] vertex)
    {
      int da = (edga1 / 3);
      int db = (edgb1 / 3);
      int ida = da * 3;
      int idb = db * 3;
      if (edga1 == ida || edga2 == ida)
        ida++;
      if (edga1 == ida || edga2 == ida)
        ida++;
      if (edgb1 == idb || edgb2 == idb)
        idb++;
      if (edgb1 == idb || edgb2 == idb)
        idb++;
      int[] trall = tmp.triangles;
      int uniquea = 0;
      int uniqueb = 0;
      int va = triangles[edga1];
      int vb = triangles[edgb1];
      for (int i = 0; i < trall.Length; i++)
      {
        if (trall[i] == va)
          uniquea++;
        if (trall[i] == vb)
          uniqueb++;
      }

      if (uniquea > 1)
        triangles[edga1] = triangles[idb];
      else
        vertex[triangles[edga1]] = vertex[triangles[idb]];
      if (uniqueb > 1)
        triangles[edgb1] = triangles[ida];
      else
        vertex[triangles[edgb1]] = vertex[triangles[ida]];
    }

    void CollapseSelectedVertex()
    {
      if (SelectedVertexId == null)
        return;
      Vector3[] vertex = tmp.vertices;
      for (int i = 0; i < SelectedVertexId.Length; i++)
      {
        int id = SelectedVertexId[i];
        vertex[id] = selectionBounds.center;
      }
      int[][] triangles = new int[tmp.subMeshCount][];// tmp.triangles;
      int[] put = new int[tmp.subMeshCount];
      HashSet<int> alenids = new HashSet<int>();
      int[][] ntriangles = new int[tmp.subMeshCount][];// new int[triangles[i].Length - put[i]];
      for (int i = 0; i < triangles.Length; i++)
      {
        put[i] = 0;
        triangles[i] = tmp.GetIndices(i);
        for (int j = 0; j < triangles[i].Length / 3; j++)
        {
          int id = j * 3;
          int vi1 = triangles[i][id];
          int vi2 = triangles[i][id + 1];
          int vi3 = triangles[i][id + 2];
          if (vertex[vi1] == vertex[vi2] || vertex[vi2] == vertex[vi3] || vertex[vi3] == vertex[vi1])
          {
            if (!alenids.Contains(vi1))
              alenids.Add(vi1);
            if (!alenids.Contains(vi2))
              alenids.Add(vi2);
            if (!alenids.Contains(vi3))
              alenids.Add(vi3);
            put[i] += 3;
          }
          else
          {
            triangles[i][id - put[i]] = vi1;
            triangles[i][id - put[i] + 1] = vi2;
            triangles[i][id - put[i] + 2] = vi3;
          }
        }
        ntriangles[i] = new int[triangles[i].Length - put[i]];
        for (int j = 0; j < ntriangles[i].Length; j++)
        {
          ntriangles[i][j] = triangles[i][j];
          if (alenids.Contains(triangles[i][j]))
            alenids.Remove(triangles[i][j]);
        }
      }
      NormalizeIndex(ref ntriangles, ref triangles, ref alenids);
      Vector3[] nvertex = new Vector3[vertex.Length - alenids.Count];
      int pv = 0;
      for (int i = 0; i < vertex.Length; i++)
        if (alenids.Contains(i))
          pv++;
        else
          nvertex[i - pv] = vertex[i];
      for (int i = 0; i < ntriangles.Length; i++)
        tmp.SetIndices(ntriangles[i], MeshTopology.Triangles, i);
      tmp.vertices = nvertex;
      SelectedVertexId = null;
      UpdateSelectedVertex();
      Apply();
    }

    void WeldSelectedVertex(float distance)
    {
      if (SelectedVertexId == null)
        return;
      Vector3[] vertex = tmp.vertices;
      for (int i = 0; i < SelectedVertexId.Length; i++)
      {
        //int id = SelectedVertexId[i];
        //vertex[id] = selectionBounds.center;
      }
    }

    void SmoothNormal(float Angle)
    {
      if (tmp == null) return;
      HashSet<int> alenids = new HashSet<int>();
      HashSet<int> selectedIds = new HashSet<int>();
      if (SelectedVertexId != null)
        selectedIds.UnionWith(SelectedVertexId);
      int[] triangles = tmp.triangles;
      for (int i = 0; i < triangles.Length / 3; i++)
      {
        int v1 = triangles[i * 3];
        int v2 = triangles[i * 3 + 1];
        int v3 = triangles[i * 3 + 2];
        if (selectedIds.Contains(v1) && selectedIds.Contains(v2) && selectedIds.Contains(v3))
        {
          alenids.Add(v1);
          alenids.Add(v2);
          alenids.Add(v3);
        }
      }
      float cosang = Mathf.Cos(Angle / 180.0f * Mathf.PI);
      Vector3[] vertex = tmp.vertices;
      Vector3[][] vec = new Vector3[tmp.subMeshCount][];
      int veccount = 0;
      tmp.vertices = new Vector3[tmp.triangles.Length];
      for (int k = 0; k < tmp.subMeshCount; k++)
      {
        List<List<int>> group = new List<List<int>>();
        triangles = tmp.GetTriangles(k);
        int faces = triangles.Length / 3;
        Vector3[] normal = new Vector3[faces];
        for (int i = 0; i < normal.Length; i++)
          normal[i] = Vector3.zero;
        for (int i = 0; i < faces; i++)
        {
          int id = i * 3;
          Vector3 v1 = vertex[triangles[id]];
          Vector3 v2 = vertex[triangles[id + 1]];
          Vector3 v3 = vertex[triangles[id + 2]];
          Vector3 norm = Vector3.Cross(v1 - v2, v3 - v2).normalized;
          normal[i] = norm;
        }
        Dictionary<Vector3, List<int>> compareV = new Dictionary<Vector3, List<int>>();
        for (int i = 0; i < triangles.Length; i++)
        {
          Vector3 key = vertex[triangles[i]];
          List<int> list;
          if (!compareV.TryGetValue(key, out list)) compareV.Add(key, list = new List<int>());
          list.Add(i);
        }
        foreach (KeyValuePair<Vector3, List<int>> vecv in compareV)
        {
          int[] separate = new int[vecv.Value.Count];
          for (int i = 0; i < separate.Length; i++) separate[i] = -1;
          for (int i = 0; i < separate.Length; i++)
          {
            int id1 = vecv.Value[i];
            if (separate[i] < 0) separate[i] = i;
            for (int j = i + 1; j < separate.Length; j++)
            {
              int id2 = vecv.Value[j];
              bool check = false;
              if (alenids.Contains(triangles[id1]) && alenids.Contains(triangles[id2]))
              {
                check = Vector3.Dot(normal[id1 / 3], normal[id2 / 3]) > cosang;
              }
              else
              {
                check = triangles[id1] == triangles[id2];
              }
              if (check)
              {
                if (separate[j] >= 0)
                  for (int m = 0; m < separate.Length; m++)
                    if (separate[m] == separate[j]) separate[m] = separate[i];
                separate[j] = separate[i];
              }
            }
          }
          for (int i = 0; i < separate.Length; i++)
          {
            List<int> v = new List<int>();
            for (int j = 0; j < separate.Length; j++) if (i == separate[j]) v.Add(vecv.Value[j]);
            if (v.Count > 0) group.Add(v);
          }
        }
        vec[k] = new Vector3[group.Count];
        for (int i = 0; i < vec[k].Length; i++)
        {
          vec[k][i] = vertex[triangles[group[i][0]]];
          for (int j = 0; j < group[i].Count; j++) triangles[group[i][j]] = i;
        }
        for (int i = 0; i < triangles.Length; i++) // check index
        {
          if (triangles[i] >= vec[k].Length)
          {
            Debug.Log("Error");
            return;
          }
          triangles[i] += veccount;
        }
        veccount += group.Count;
        tmp.SetIndices(triangles, MeshTopology.Triangles, k);
      }
      Vector3[] nvec = new Vector3[veccount];
      veccount = 0;
      for (int k = 0; k < tmp.subMeshCount; k++)
      {
        vec[k].CopyTo(nvec, veccount);
        veccount += vec[k].Length;
      }
      tmp.vertices = nvec;
      tmp.RecalculateBounds();
      SelectedVertexId = null;
      UpdateSelectedVertex();
      Apply();
    }

    int[] BreakFace()
    {
      if (tmp == null)
        return null;
      int[] selectionsId = SelectionToFace();
      HashSet<int> allfase = new HashSet<int>();
      if (selectionsId == null || selectionsId.Length == 0)
        return null;
      allfase.UnionWith(selectionsId);
      int troffset = 0;
      int[][] triangles = new int[tmp.subMeshCount][];
      Dictionary<int, int> triVertexClone = new Dictionary<int, int>();
      Vector3[] vertex = tmp.vertices;
      int nvid = vertex.Length;
      for (int k = 0; k < tmp.subMeshCount; k++)
      {
        int[] trianglesSub = tmp.GetIndices(k);
        for (int i = 0; i < trianglesSub.Length; i++)
        {
          if (allfase.Contains(i / 3 + troffset))
            continue;
          for (int j = 0; j < selectionsId.Length; j++)
          {
            int rid = selectionsId[j] * 3;
            int id = (selectionsId[j] - troffset) * 3;
            if (id < 0 || id >= trianglesSub.Length)
              continue;
            if (trianglesSub[id] == trianglesSub[i] && !triVertexClone.ContainsKey(rid))
            {
              triVertexClone.Add(rid, nvid);
              trianglesSub[id] = nvid;
              nvid++;
            }
            id++; rid++;
            if (trianglesSub[id] == trianglesSub[i] && !triVertexClone.ContainsKey(rid))
            {
              triVertexClone.Add(rid, nvid);
              trianglesSub[id] = nvid;
              nvid++;
            }
            id++; rid++;
            if (trianglesSub[id] == trianglesSub[i] && !triVertexClone.ContainsKey(rid))
            {
              triVertexClone.Add(rid, nvid);
              trianglesSub[id] = nvid;
              nvid++;
            }
          }
        }
        triangles[k] = trianglesSub;
        troffset += trianglesSub.Length / 3;
      }
      int[] trianglesall = tmp.triangles;
      if (triVertexClone.Count > 0)
      {
        Vector3[] nvertex = new Vector3[vertex.Length + triVertexClone.Count];
        vertex.CopyTo(nvertex, 0);
        foreach (KeyValuePair<int, int> idsv in triVertexClone)
        {
          nvertex[idsv.Value] = vertex[trianglesall[idsv.Key]];
        }
        tmp.vertices = nvertex;
        for (int k = 0; k < tmp.subMeshCount; k++)
        {
          tmp.SetIndices(triangles[k], MeshTopology.Triangles, k);
        }
        trianglesall = tmp.triangles;
        Apply();
      }
      allfase.Clear();
      for (int j = 0; j < selectionsId.Length; j++)
      {
        int id = selectionsId[j] * 3;
        int vid = trianglesall[id];
        if (!allfase.Contains(vid))
          allfase.Add(vid);
        vid = trianglesall[id + 1];
        if (!allfase.Contains(vid))
          allfase.Add(vid);
        vid = trianglesall[id + 2];
        if (!allfase.Contains(vid))
          allfase.Add(vid);
      }
      SelectedVertexId = new int[allfase.Count];
      allfase.CopyTo(SelectedVertexId);
      UpdateSelectedVertex();
      return selectionsId;
    }

    void Extrude(float offset)
    {
      if (tmp == null)
        return;
      int[] selectionsId = BreakFace();
      List<int>[] edge = new List<int>[tmp.subMeshCount];
      int[] trianglesall = tmp.triangles;
      int[][] triangles = new int[tmp.subMeshCount][];
      for (int k = 0; k < tmp.subMeshCount; k++)
      {
        triangles[k] = tmp.GetIndices(k);
      }
      Vector3[] vertex = tmp.vertices;
      Vector3 normal = Vector3.zero;
      for (int i = 0; i < selectionsId.Length; i++)
      {
        int k = 0;
        int id1 = (selectionsId[i]) * 3;
        int off = 0;
        for (int j = 0; j < tmp.subMeshCount; j++)
        {
          if (id1 - off >= 0 && id1 < off + triangles[j].Length)
          {
            k = j;
            break;
          }
          off += triangles[j].Length;
        }
        if (edge[k] == null)
          edge[k] = new List<int>();
        bool one1 = true;
        bool one2 = true;
        bool one3 = true;
        Vector3 v1 = vertex[trianglesall[id1]];
        Vector3 v2 = vertex[trianglesall[id1 + 1]];
        Vector3 v3 = vertex[trianglesall[id1 + 2]];
        normal += Vector3.Cross(v2 - v1, v3 - v1).normalized;
        for (int j = 0; j < selectionsId.Length; j++)
        {
          int id2 = selectionsId[j] * 3;
          if (i == j)
            continue;
          Vector3 vk1 = vertex[trianglesall[id2]];
          Vector3 vk2 = vertex[trianglesall[id2 + 1]];
          Vector3 vk3 = vertex[trianglesall[id2 + 2]];
          if (one1 && v1 == vk2 && v2 == vk1)
            one1 = false;
          if (one1 && v1 == vk3 && v2 == vk2)
            one1 = false;
          if (one1 && v1 == vk1 && v2 == vk3)
            one1 = false;
          if (one2 && v2 == vk2 && v3 == vk1)
            one2 = false;
          if (one2 && v2 == vk3 && v3 == vk2)
            one2 = false;
          if (one2 && v2 == vk1 && v3 == vk3)
            one2 = false;
          if (one3 && v3 == vk2 && v1 == vk1)
            one3 = false;
          if (one3 && v3 == vk3 && v1 == vk2)
            one3 = false;
          if (one3 && v3 == vk1 && v1 == vk3)
            one3 = false;
          if (!one1 && !one2 && !one2)
            break;
        }
        if (one1)
        {
          edge[k].Add(id1);
          edge[k].Add(id1 + 1);
        }
        if (one2)
        {
          edge[k].Add(id1 + 1);
          edge[k].Add(id1 + 2);
        }
        if (one3)
        {
          edge[k].Add(id1 + 2);
          edge[k].Add(id1);
        }
      }
      Dictionary<int, int> polyid = new Dictionary<int, int>();
      int l1 = vertex.Length;
      for (int k = 0; k < edge.Length; k++)
        if (edge[k] != null)
        {
          int[] ntriangles = new int[triangles[k].Length + edge[k].Count * 3];
          triangles[k].CopyTo(ntriangles, 0);
          for (int i = 0; i < edge[k].Count / 2; i++)
          {
            int v1 = edge[k][i * 2];
            int v2 = edge[k][i * 2 + 1];
            if (!polyid.ContainsKey(v1))
              polyid.Add(v1, polyid.Count);
            if (!polyid.ContainsKey(v2))
              polyid.Add(v2, polyid.Count);
          }
          triangles[k] = ntriangles;
        }
      int l2 = l1 + polyid.Count;
      for (int k = 0; k < edge.Length; k++)
        if (edge[k] != null)
        {
          int l = triangles[k].Length - edge[k].Count * 3;
          for (int i = 0; i < edge[k].Count / 2; i++)
          {
            int v1 = edge[k][i * 2];
            int v2 = edge[k][i * 2 + 1];
            triangles[k][l + i * 6] = l1 + polyid[v1];
            triangles[k][l + i * 6 + 1] = l1 + polyid[v2];
            triangles[k][l + i * 6 + 2] = l2 + polyid[v1];
            triangles[k][l + i * 6 + 3] = l2 + polyid[v1];
            triangles[k][l + i * 6 + 4] = l1 + polyid[v2];
            triangles[k][l + i * 6 + 5] = l2 + polyid[v2];
          }
        }
      Vector3[] nvertex = new Vector3[vertex.Length + polyid.Count * 2];
      vertex.CopyTo(nvertex, 0);
      List<int> selection = new List<int>(SelectedVertexId);
      foreach (KeyValuePair<int, int> key in polyid)
      {
        nvertex[l1 + key.Value] = vertex[trianglesall[key.Key]];
        nvertex[l2 + key.Value] = vertex[trianglesall[key.Key]];
        selection.Add(l2 + key.Value);
      }
      SelectedVertexId = selection.ToArray();
      normal = normal.normalized * offset;
      for (int i = 0; i < SelectedVertexId.Length; i++)
      {
        nvertex[SelectedVertexId[i]] += normal;
      }
      tmp.vertices = nvertex;
      for (int k = 0; k < tmp.subMeshCount; k++)
      {
        tmp.SetIndices(triangles[k], MeshTopology.Triangles, k);
      }
      UpdateSelectedVertex();
      Apply();
    }

    int NormalizeIndex(ref int[][] ntriangles, ref int[][] triangles, ref HashSet<int> alenids)
    {
      int pm = 0;
      for (int i = 0; i < ntriangles.Length; i++)
      {
        if (ntriangles[i] == null || ntriangles[i].Length == 0)
          pm++;
        if (pm > 0 && i < ntriangles.Length - pm)
        {
          ntriangles[i] = ntriangles[i + pm];
          triangles[i] = triangles[i + pm];
        }
      }
      pm = ntriangles.Length - pm;
      for (int i = 0; i < pm; i++)
        for (int j = 0; j < ntriangles[i].Length; j++)
          foreach (int key in alenids)
            if (key < triangles[i][j])
              ntriangles[i][j]--;
      return pm;
    }

    int[] SelectionToFace()
    {
      HashSet<int> selectedIds = new HashSet<int>();
      if (SelectedVertexId != null)
        selectedIds.UnionWith(SelectedVertexId);
      List<int> SelectedFaceId = new List<int>();
      int[] triangles = tmp.triangles;
      for (int i = 0; i < triangles.Length / 3; i++)
      {
        int v1 = triangles[i * 3];
        int v2 = triangles[i * 3 + 1];
        int v3 = triangles[i * 3 + 2];
        if (selectedIds.Contains(v1) && selectedIds.Contains(v2) && selectedIds.Contains(v3))
        {
          SelectedFaceId.Add(i);
        }
      }
      return SelectedFaceId.ToArray();
    }
  }

  public class XmlAttributeConverter
  {
    public static void Save(XmlDocument xml,XmlElement Elem, string name,ref AnimationCurve val)
    {
      XmlElement Curve = xml.CreateElement(string.Empty,name, string.Empty);
      Curve.SetAttribute("Size", val.length.ToString());

      for (int i = 0; i < val.length; i++)
      {
        XmlElement KeyXML = xml.CreateElement(string.Empty, "Key_" + i.ToString(), string.Empty);
        KeyXML.SetAttribute("Time", val.keys[i].time.ToString());
        KeyXML.SetAttribute("Value", val.keys[i].value.ToString());
        KeyXML.SetAttribute("Mode", val.keys[i].tangentMode.ToString());
        KeyXML.SetAttribute("In", val.keys[i].inTangent.ToString());
        KeyXML.SetAttribute("Out", val.keys[i].outTangent.ToString());
        Curve.AppendChild(KeyXML);
      }

      Elem.AppendChild(Curve);
    }

    public static AnimationCurve Load(XmlNode Elem, string name)
    {
      Keyframe[] keys = null;
      foreach (XmlNode child in Elem.ChildNodes)
      {
        if (child.Name == name)
        {
          int len=int.Parse(child.Attributes.GetNamedItem("Size").Value);
          keys = new Keyframe[len];
          foreach (XmlNode Element in child.ChildNodes)
          {
            int id = Convert.ToInt32(Element.Name.Remove(0, Element.Name.IndexOf('_') + 1));
            keys[id] = new Keyframe(
              float.Parse(Element.Attributes.GetNamedItem("Time").Value),
              float.Parse(Element.Attributes.GetNamedItem("Value").Value),
              float.Parse(Element.Attributes.GetNamedItem("In").Value),
              float.Parse(Element.Attributes.GetNamedItem("Out").Value));
            keys[id].tangentMode = int.Parse(Element.Attributes.GetNamedItem("Mode").Value);
          }
        }
      }
      return new AnimationCurve(keys);
    }

    public static bool Bool(XmlNode PartsXML, string name)
    {
      XmlNode node = PartsXML.Attributes.GetNamedItem(name);
      if (node == null)
        return true;
      return bool.Parse(node.Value);
    }

    public static int Int(XmlNode PartsXML, string name)
    {
      XmlNode node = PartsXML.Attributes.GetNamedItem(name);
      if (node == null)
        return 0;
      return int.Parse(node.Value);
    }

    public static float Float(XmlNode PartsXML, string name)
    {
      XmlNode node = PartsXML.Attributes.GetNamedItem(name);
      if (node == null)
        return 0;
      return float.Parse(node.Value);
    }

    public static string Vector3To(Vector3 val)
    {
      return val.x.ToString()+", "+ val.y.ToString() + ", "+ val.z.ToString();
    }

    public static Vector3 ToVector3(XmlNode PartsXML, string name)
    {
      XmlNode node = PartsXML.Attributes.GetNamedItem(name);
      if (node == null)
        return new Vector3(0, 0, 0);
      string[] temp = node.Value.Split(',');
      float x = float.Parse(temp[0]);
      float y = float.Parse(temp[1]);
      float z = float.Parse(temp[2]);
      return new Vector3(x,y,z);
    }
  }

  [CustomEditor(typeof(TreegenSimpleGenerator))]
  public class TreegenSimpleGeneratorEditor : Editor
  {
    public override void OnInspectorGUI()
    {
      base.OnInspectorGUI();
      GUILayout.BeginHorizontal();
      //Settings sett = (Settings)target;
      TreegenSimpleGenerator gen = (TreegenSimpleGenerator)target;
      /*if (GUILayout.Button(new GUIContent("Save Template", "Save to XML")))
      {
        string path = EditorUtility.SaveFilePanel(
        "Save template as XML",
        Application.dataPath + "/TREEGEN/Temlpates",
        gen.name + ".xml",
        "xml");
        if (path.Length != 0)
          SaveXml(path, ref gen);
      }
      if (GUILayout.Button(new GUIContent("Load Template", "Load from XML")))
      {
        string path = EditorUtility.OpenFilePanel(
          "Load template as XML",
          Application.dataPath+"/TREEGEN/Temlpates",
          "xml");
        if (path.Length != 0)
          LoadXml(path, ref gen);
      }*/
      if (GUILayout.Button(new GUIContent("Save Mesh", "Save Mesh to asset")))
      {
        string path = EditorUtility.SaveFilePanelInProject(
        "Save Mesh to asset",
        gen.name + ".asset",
        "asset", "Please enter a file name to save the mesh to");
        if (path.Length != 0)
        {
          path = path.Replace(Application.dataPath, "Assets");
          Mesh m = gen.NewGen();
          AssetDatabase.CreateAsset(m, path);
          m = (Mesh)AssetDatabase.LoadAssetAtPath(path,typeof(Mesh));
          gen.GetComponent<MeshFilter>().sharedMesh = m;
        }
      }
      /*if (GUILayout.Button(new GUIContent("Random generation", "Generation of a tree based on random parameters")))
      {
        TreegenTreeGenerator genTree = (TreegenTreeGenerator)target;
        genTree.TrunkIterations = UnityEngine.Random.Range(0, 3);
      }*/
      GUILayout.EndHorizontal();
    }

    public void SaveXml(string file, ref TreegenSimpleGenerator gen)
    {
      XmlDocument xml = new XmlDocument();
      XmlDeclaration xmlDeclaration = xml.CreateXmlDeclaration("1.0", "UTF-8", null);
      XmlElement root = xml.DocumentElement;
      xml.InsertBefore(xmlDeclaration, root);

      XmlElement body = xml.CreateElement(string.Empty, "body", string.Empty);
      xml.AppendChild(body);

      XmlElement BaseGenerator = xml.CreateElement(string.Empty, "TreegenSimpleGenerator", string.Empty);
      BaseGenerator.SetAttribute("Version", "1.0");
      body.AppendChild(BaseGenerator);

      XmlElement General = xml.CreateElement(string.Empty, "General", string.Empty);
      General.SetAttribute("Material1", AssetDatabase.GetAssetPath(gen.Material));
      General.SetAttribute("Material2", AssetDatabase.GetAssetPath(gen.Sub));
      General.SetAttribute("Detail", gen.Detail.ToString());
      General.SetAttribute("BoxSize", XmlAttributeConverter.Vector3To(gen.BoxSize));
      General.SetAttribute("DownForce", gen.DownForse.ToString());
      General.SetAttribute("Center", XmlAttributeConverter.Vector3To(gen.Center));
      General.SetAttribute("NoiseForce", gen.NoiseForce.ToString());
      General.SetAttribute("NoiseSeed", gen.NoiseSeed.ToString());
      BaseGenerator.AppendChild(General);

      XmlElement Surfase = xml.CreateElement(string.Empty, "Surfase", string.Empty);
      Surfase.SetAttribute("SurfaceType", gen.SurfaceType.ToString());
      Surfase.SetAttribute("SSSModCount", gen.SSSModCount.ToString());
      Surfase.SetAttribute("SSSModRadius", gen.SSSModRadius.ToString());
      XmlAttributeConverter.Save(xml, Surfase, "SSSModCurve", ref gen.SSSModCurve);
      BaseGenerator.AppendChild(Surfase);

      XmlElement Spray = xml.CreateElement(string.Empty, "Spray", string.Empty);
      Spray.SetAttribute("Original", gen.Original.ToString());
      Spray.SetAttribute("Count", gen.SprayCount.ToString());
      Spray.SetAttribute("Box", XmlAttributeConverter.Vector3To(gen.SprayBox));
      Spray.SetAttribute("Seed", gen.SpraySeed.ToString());
      XmlAttributeConverter.Save(xml, Spray, "Pos", ref gen.SprayPos);
      XmlAttributeConverter.Save(xml, Spray, "Scale", ref gen.SprayScale);
      XmlAttributeConverter.Save(xml, Spray, "Normal", ref gen.SprayNormal);
      BaseGenerator.AppendChild(Spray);

      xml.Save(file);
    }

    /*public void LoadXml(string file, ref TreegenSimpleGenerator gen)
    {
      XmlDocument xml = new XmlDocument();
      xml.Load(file);

      XmlNode body = null;
      foreach (XmlNode child in xml.ChildNodes)
      {
        if (child.Name == "body")
          body = child;
      }
      if (body != null)
        foreach (XmlNode child in body.ChildNodes)
        {
          if (child.Name == "TreegenSimpleGenerator")
          {
            //string version = child.Attributes.GetNamedItem("Version").Value;
            foreach (XmlNode PartsXML in child.ChildNodes)
            {
              if (PartsXML.Name == "General")
              {
                gen.Material = (Material)AssetDatabase.LoadAssetAtPath(PartsXML.Attributes.GetNamedItem("Material1").Value,typeof(Material));
                gen.Sub = (Material)AssetDatabase.LoadAssetAtPath(PartsXML.Attributes.GetNamedItem("Material2").Value,typeof(Material));
                gen.Detail = int.Parse(PartsXML.Attributes.GetNamedItem("Detail").Value);
                gen.BoxSize = XmlAttributeConverter.ToVector3(PartsXML,"BoxSize");
                gen.DownForse = float.Parse(PartsXML.Attributes.GetNamedItem("DownForce").Value);
                gen.Center = XmlAttributeConverter.ToVector3(PartsXML,"Center");
                gen.NoiseForce = float.Parse(PartsXML.Attributes.GetNamedItem("NoiseForce").Value);
                gen.NoiseSeed = int.Parse(PartsXML.Attributes.GetNamedItem("NoiseSeed").Value);
              }
              if (PartsXML.Name == "Surfase")
              {
                gen.SurfaceType = (TreegenSimpleGenerator.SSSType)Enum.Parse(typeof(TreegenSimpleGenerator.SSSType), PartsXML.Attributes.GetNamedItem("SurfaceType").Value);
                gen.SSSModCount = int.Parse(PartsXML.Attributes.GetNamedItem("SSSModCount").Value);
                gen.SSSModRadius = float.Parse(PartsXML.Attributes.GetNamedItem("SSSModRadius").Value);
                gen.SSSModCurve = XmlAttributeConverter.Load(PartsXML,"SSSModCurve");
              }
              if (PartsXML.Name == "Spray")
              {
                gen.Original = XmlAttributeConverter.Bool(PartsXML, "Original");
                gen.SprayCount = XmlAttributeConverter.Int(PartsXML,"Count");
                gen.SprayBox = XmlAttributeConverter.ToVector3(PartsXML,"Box");
                gen.SpraySeed = int.Parse(PartsXML.Attributes.GetNamedItem("Seed").Value);
                gen.SprayPos = XmlAttributeConverter.Load(PartsXML, "Pos");
                gen.SprayScale = XmlAttributeConverter.Load(PartsXML, "Scale");
                gen.SprayNormal = XmlAttributeConverter.Load(PartsXML, "Normal");
              }
            }
          }
        }
      gen.NewGen();
    }*/
  }

  [CustomEditor(typeof(TreegenTreeGenerator))]
  public class TreegenTreeGeneratorEditor : Editor
  {
    public override void OnInspectorGUI()
    {
      base.OnInspectorGUI();
      GUILayout.BeginHorizontal();
      //Settings sett = (Settings)target;
      TreegenTreeGenerator gen = (TreegenTreeGenerator)target;
      if (GUILayout.Button(new GUIContent("Save Mesh", "Save Mesh to asset")))
      {
        string path = EditorUtility.SaveFilePanelInProject(
        "Save Mesh to asset",
        gen.name + ".asset",
        "asset", "Please enter a file name to save the mesh to");
        if (path.Length != 0)
        {
          path = path.Replace(Application.dataPath, "Assets");
          Mesh m = gen.NewGen();
          AssetDatabase.CreateAsset(m, path);
          m = (Mesh)AssetDatabase.LoadAssetAtPath(path, typeof(Mesh));
          gen.GetComponent<MeshFilter>().sharedMesh = m;
        }
      }
      
      if (GUILayout.Button(new GUIContent("Save Trunc Mesh", "Save Trunc Mesh to asset")))
      {
        string path = EditorUtility.SaveFilePanelInProject(
        "Save Trunc Mesh to asset",
        gen.name + " (Trunc)" + ".asset",
        "asset", "Please enter a file name to save the mesh to");
        if (path.Length != 0)
        {
          path = path.Replace(Application.dataPath, "Assets");
          Mesh m = gen.NewGenTrunc();
          AssetDatabase.CreateAsset(m, path);
          m = (Mesh)AssetDatabase.LoadAssetAtPath(path, typeof(Mesh));
          gen.GetComponent<MeshFilter>().sharedMesh = m;
        }
      }
      
      if (gen.CountSegment > 0)
      {
        if (GUILayout.Button(new GUIContent("Save Leaves Mesh", "Save Leaves Mesh to asset")))
        {
          string path = EditorUtility.SaveFilePanelInProject(
          "Save Leaves Mesh to asset",
          gen.name + " (Leaves)" + ".asset",
          "asset", "Please enter a file name to save the mesh to");
          if (path.Length != 0)
          {
            path = path.Replace(Application.dataPath, "Assets");
            Mesh m = gen.NewGenLeaves();
            AssetDatabase.CreateAsset(m, path);
            m = (Mesh)AssetDatabase.LoadAssetAtPath(path, typeof(Mesh));
            gen.GetComponent<MeshFilter>().sharedMesh = m;
          }
        }
      }
      GUILayout.EndHorizontal();
    }

    public void SaveXml(string file, ref TreegenTreeGenerator gen)
    {
      XmlDocument xml = new XmlDocument();
      XmlDeclaration xmlDeclaration = xml.CreateXmlDeclaration("1.0", "UTF-8", null);
      XmlElement root = xml.DocumentElement;
      xml.InsertBefore(xmlDeclaration, root);

      XmlElement body = xml.CreateElement(string.Empty, "body", string.Empty);
      xml.AppendChild(body);

      XmlElement BaseGenerator = xml.CreateElement(string.Empty, "TreegenTreeGenerator", string.Empty);
      BaseGenerator.SetAttribute("Version", "1.0");
      body.AppendChild(BaseGenerator);

      XmlElement General = xml.CreateElement(string.Empty, "General", string.Empty);
      General.SetAttribute("Material1", AssetDatabase.GetAssetPath(gen.TrunkMaterial));
      General.SetAttribute("Material2", AssetDatabase.GetAssetPath(gen.LeavesMaterial));
      General.SetAttribute("Seed", gen.Seed.ToString());
      General.SetAttribute("TreeNoiseForce", gen.TreeNoiseForce.ToString());
      General.SetAttribute("TreeNoiseSeed", gen.TreeNoiseSeed.ToString());
      BaseGenerator.AppendChild(General);

      XmlElement Trunk = xml.CreateElement(string.Empty, "Trunk", string.Empty);
      Trunk.SetAttribute("Iterations", gen.TrunkIterations.ToString());
      Trunk.SetAttribute("Parts", gen.Parts.ToString());
      Trunk.SetAttribute("Segments", gen.Segments.ToString());
      Trunk.SetAttribute("SkipBranch", gen.SkipBranch.ToString());
      Trunk.SetAttribute("HideOnLeaves", gen.HideOnLeaves.ToString());
      XmlAttributeConverter.Save(xml, Trunk, "SegmentLength", ref gen.TrunkSegmentLength);
      XmlAttributeConverter.Save(xml, Trunk, "LevelLength", ref gen.TrunkLevelLength);
      XmlAttributeConverter.Save(xml, Trunk, "Thickness", ref gen.TrunkThickness);
      XmlAttributeConverter.Save(xml, Trunk, "MaxBranch", ref gen.MaxBranch);
      XmlAttributeConverter.Save(xml, Trunk, "MinBranch", ref gen.MinBranch);
      XmlAttributeConverter.Save(xml, Trunk, "Twirl", ref gen.Twirl);
      BaseGenerator.AppendChild(Trunk);

      XmlElement Branch = xml.CreateElement(string.Empty, "Branch", string.Empty);
      Branch.SetAttribute("Segments", gen.BranchSegments.ToString());
      Branch.SetAttribute("SkipSubBranch", gen.SkipSubBranch.ToString());
      XmlAttributeConverter.Save(xml, Branch, "BranchThickness", ref gen.BranchThickness);
      XmlAttributeConverter.Save(xml, Branch, "MaxSubBranch", ref gen.MaxSubBranch);
      XmlAttributeConverter.Save(xml, Branch, "MinSubBranch", ref gen.MinSubBranch);
      XmlAttributeConverter.Save(xml, Branch, "InnerTwirl", ref gen.InnerTwirl);
      BaseGenerator.AppendChild(Branch);

      XmlElement Root = xml.CreateElement(string.Empty, "Root", string.Empty);
      Root.SetAttribute("Iterations", gen.RootIterations.ToString());
      Root.SetAttribute("Segments", gen.RootSegments.ToString());
      Root.SetAttribute("SkipRootBranch", gen.SkipRootBranch.ToString());
      Root.SetAttribute("Thickness", gen.RootThickness.ToString());
      Root.SetAttribute("BranchCount", gen.RootBranchCount.ToString());
      XmlAttributeConverter.Save(xml, Root, "RootSegmentLength", ref gen.RootSegmentLength);
      XmlAttributeConverter.Save(xml, Root, "RootLevelLength", ref gen.RootLevelLength);
      XmlAttributeConverter.Save(xml, Root, "RootTwirl", ref gen.RootTwirl);
      Root.SetAttribute("RootNoiseForce", gen.RootNoiseForce.ToString());
      Root.SetAttribute("RootNoiseSeed", gen.RootNoiseSeed.ToString());
      BaseGenerator.AppendChild(Root);

      XmlElement Leaves = xml.CreateElement(string.Empty, "Leaves", string.Empty);
      Leaves.SetAttribute("Type", gen.LeavesType.ToString());
      Leaves.SetAttribute("StartIteration", gen.StartIteration.ToString());
      Leaves.SetAttribute("EndIteration", gen.EndIteration.ToString());
      Leaves.SetAttribute("StartSegment", gen.StartSegment.ToString());
      Leaves.SetAttribute("CountSegment", gen.CountSegment.ToString());
      Leaves.SetAttribute("Scale", XmlAttributeConverter.Vector3To(gen.LeavesScale));
      XmlAttributeConverter.Save(xml, Leaves, "ScaleCurve", ref gen.LeavesScaleCurve);
      XmlAttributeConverter.Save(xml, Leaves, "ScaleSeg", ref gen.LeavesScaleSeg);
      XmlAttributeConverter.Save(xml, Leaves, "OffsetCurve", ref gen.LeavesOffsetCurve);
      Leaves.SetAttribute("Offset", gen.LeavesOffset.ToString());
      Leaves.SetAttribute("Detail", gen.LeavesDetail.ToString());
      Leaves.SetAttribute("NoiseForce", gen.LeavesNoiseForce.ToString());
      Leaves.SetAttribute("NoiseSeed", gen.LeavesNoiseSeed.ToString());
      BaseGenerator.AppendChild(Leaves);

      xml.Save(file);
    }

    public void LoadXml(string file, ref TreegenTreeGenerator gen)
    {
      XmlDocument xml = new XmlDocument();
      xml.Load(file);

      XmlNode body = null;
      foreach (XmlNode child in xml.ChildNodes)
      {
        if (child.Name == "body")
          body = child;
      }
      if (body != null)
        foreach (XmlNode child in body.ChildNodes)
        {
          if (child.Name == "TreegenTreeGenerator")
          {
            //string version = child.Attributes.GetNamedItem("Version").Value;
            foreach (XmlNode PartsXML in child.ChildNodes)
            {
              if (PartsXML.Name == "General")
              {
                gen.TrunkMaterial = (Material)AssetDatabase.LoadAssetAtPath(PartsXML.Attributes.GetNamedItem("Material1").Value,typeof(Material));
                gen.LeavesMaterial = (Material)AssetDatabase.LoadAssetAtPath(PartsXML.Attributes.GetNamedItem("Material2").Value,typeof(Material));
                gen.Seed = int.Parse(PartsXML.Attributes.GetNamedItem("Seed").Value);
                gen.TreeNoiseForce = float.Parse(PartsXML.Attributes.GetNamedItem("TreeNoiseForce").Value);
                gen.TreeNoiseSeed = int.Parse(PartsXML.Attributes.GetNamedItem("TreeNoiseSeed").Value);
              }
              if (PartsXML.Name == "Trunk")
              {
                gen.TrunkIterations = XmlAttributeConverter.Int(PartsXML,"Iterations");
                gen.Parts = XmlAttributeConverter.Int(PartsXML,"Parts");
                gen.Segments = XmlAttributeConverter.Int(PartsXML,"Segments");
                gen.SkipBranch = XmlAttributeConverter.Int(PartsXML, "SkipBranch");
                gen.HideOnLeaves = bool.Parse(PartsXML.Attributes.GetNamedItem("HideOnLeaves").Value);
                gen.TrunkSegmentLength = XmlAttributeConverter.Load(PartsXML, "SegmentLength");
                gen.TrunkLevelLength = XmlAttributeConverter.Load(PartsXML, "LevelLength");
                gen.TrunkThickness = XmlAttributeConverter.Load(PartsXML, "Thickness");
                gen.MaxBranch = XmlAttributeConverter.Load(PartsXML, "MaxBranch");
                gen.MinBranch = XmlAttributeConverter.Load(PartsXML, "MinBranch");
                gen.Twirl = XmlAttributeConverter.Load(PartsXML, "Twirl");
              }
              if (PartsXML.Name == "Branch")
              {
                gen.BranchSegments = int.Parse(PartsXML.Attributes.GetNamedItem("Segments").Value);
                gen.SkipSubBranch = int.Parse(PartsXML.Attributes.GetNamedItem("SkipSubBranch").Value);
                gen.BranchThickness = XmlAttributeConverter.Load(PartsXML, "BranchThickness");
                gen.MaxSubBranch = XmlAttributeConverter.Load(PartsXML, "MaxSubBranch");
                gen.MinSubBranch = XmlAttributeConverter.Load(PartsXML, "MinSubBranch");
                gen.InnerTwirl = XmlAttributeConverter.Load(PartsXML, "InnerTwirl");
              }
              if (PartsXML.Name == "Root")
              {
                gen.RootIterations = XmlAttributeConverter.Int(PartsXML,"Iterations");
                gen.RootSegments = XmlAttributeConverter.Int(PartsXML,"Segments");
                gen.SkipRootBranch = XmlAttributeConverter.Int(PartsXML,"SkipRootBranch");
                gen.RootBranchCount = XmlAttributeConverter.Int(PartsXML,"BranchCount");
                gen.RootThickness = XmlAttributeConverter.Float(PartsXML,"Thickness");
                gen.RootSegmentLength = XmlAttributeConverter.Load(PartsXML, "RootSegmentLength");
                gen.RootLevelLength = XmlAttributeConverter.Load(PartsXML, "RootLevelLength");
                gen.RootTwirl = XmlAttributeConverter.Load(PartsXML, "RootTwirl");
                gen.RootNoiseForce = float.Parse(PartsXML.Attributes.GetNamedItem("RootNoiseForce").Value);
                gen.RootNoiseSeed = int.Parse(PartsXML.Attributes.GetNamedItem("RootNoiseSeed").Value);
              }
              if (PartsXML.Name == "Leaves")
              {
                gen.LeavesType = (TreegenTreeGenerator.LeafType)Enum.Parse(typeof(TreegenTreeGenerator.LeafType),PartsXML.Attributes.GetNamedItem("Type").Value);
                gen.StartIteration = int.Parse(PartsXML.Attributes.GetNamedItem("StartIteration").Value);
                gen.EndIteration = int.Parse(PartsXML.Attributes.GetNamedItem("EndIteration").Value);
                gen.StartSegment = int.Parse(PartsXML.Attributes.GetNamedItem("StartSegment").Value);
                gen.CountSegment = int.Parse(PartsXML.Attributes.GetNamedItem("CountSegment").Value);
                gen.LeavesScale = XmlAttributeConverter.ToVector3(PartsXML,"Scale");
                gen.LeavesScaleCurve = XmlAttributeConverter.Load(PartsXML, "ScaleCurve");
                gen.LeavesScaleSeg = XmlAttributeConverter.Load(PartsXML, "ScaleSeg");
                gen.LeavesOffsetCurve = XmlAttributeConverter.Load(PartsXML, "OffsetCurve");
                gen.LeavesOffset = float.Parse(PartsXML.Attributes.GetNamedItem("Offset").Value);
                gen.LeavesDetail = int.Parse(PartsXML.Attributes.GetNamedItem("Detail").Value);
                gen.LeavesNoiseForce = float.Parse(PartsXML.Attributes.GetNamedItem("NoiseForce").Value);
                gen.LeavesNoiseSeed = int.Parse(PartsXML.Attributes.GetNamedItem("NoiseSeed").Value);
              }
            }
          }
        }
      gen.NewGen();
    }
  }
}