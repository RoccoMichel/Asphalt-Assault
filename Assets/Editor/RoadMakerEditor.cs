using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Splines;

public class RoadMakerEditor : EditorWindow
{
    RoadMaker rm;
    SplineContainer sc;

    bool showSpline;
    
    GameObject selectedObj;
    
    [MenuItem("Tools/Road Maker")]
    public static void OpenWindow()
    {
        GetWindow<RoadMakerEditor>();
    }

    void OnGUI()
    {
        selectedObj = Selection.activeObject.GameObject();
        if (selectedObj == null)
        {
            EditorGUILayout.HelpBox("No Object Selected", MessageType.Error);
            return;
        }

        if (selectedObj.GetComponent<RoadMaker>() == null)
        {
            EditorGUILayout.HelpBox("Object Is Not A Road Maker", MessageType.Error);
            return;
        }

        rm = selectedObj.GetComponent<RoadMaker>();
        sc = rm.splineContainer;

        showSpline = EditorGUILayout.Foldout(showSpline, "Show Spline Editor");
        if(showSpline)
            DrawSplineContainer();
    }

    void DrawSplineContainer()
    {
        Spline spline = sc.Spline;

        EditorGUILayout.BeginVertical(GUI.skin.box);
        GUILayout.Space(4);
        
        for (int i = 0; i < spline.Count; i++)
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);
            BezierKnot knot = spline[i];
            
            EditorGUILayout.LabelField($"Knot [{i}]");
            
            Vector3 pos = EditorGUILayout.Vector3Field("pos", knot.Position);
            knot.Position = pos;
            
            Vector3 rot = EditorGUILayout.Vector3Field("rot", ((Quaternion)knot.Rotation).eulerAngles);
            knot.Rotation = Quaternion.Euler(rot);

            spline[i] = knot;

            EditorGUILayout.EndVertical();
            
            EditorUtility.SetDirty(sc);
        }
        
        GUILayout.Space(4);
        EditorGUILayout.EndVertical();
        
        if(GUILayout.Button("Add Knot"))
            AddKnotToEnd();
    }

    void AddKnotToEnd()
    {
        Undo.RecordObject(sc, "Add Knot");
        Spline spline = sc.Spline;

        Vector3 pos = spline[^1].Position;
        Vector3 rot = ((Quaternion)spline[^1].Rotation).eulerAngles;
        
        spline.Add(new BezierKnot{Position = pos, Rotation = Quaternion.Euler(rot)});
        EditorUtility.SetDirty(sc);
    }
}
