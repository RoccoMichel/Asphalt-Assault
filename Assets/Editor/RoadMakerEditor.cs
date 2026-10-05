using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Splines;

public class RoadMakerEditor : EditorWindow
{
    RoadMaker rm;
    SplineContainer sc;
    
    Vector2 scrollPos;

    bool showSpline = true;
    
    bool showKnots = true;
    List<bool> knotFoldouts = new();
    
    GameObject selectedObj;

    float gridSize = 1;
    float angleStep = 30;
    
    [MenuItem("Tools/Road Maker ^&r")]
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
        
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        
        showSpline = EditorGUILayout.Foldout(showSpline, "Show Spline Editor");
        try
        {
            if (showSpline)
            {
                EditorGUILayout.BeginHorizontal(GUI.skin.box);
                
                gridSize = EditorGUILayout.FloatField("Grid Size", gridSize);
                if (GUILayout.Button("Snap To Grid"))
                    SnapToGrid();
                
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal(GUI.skin.box);
                
                angleStep = EditorGUILayout.FloatField("Angle Step", angleStep);
                if (GUILayout.Button("Angel Snap"))
                    AngelSnap();
                
                EditorGUILayout.EndHorizontal();

                DrawSplineContainer();
            }
        }
        finally
        {
            EditorGUILayout.EndScrollView();
        }
    }

    void DrawSplineContainer()
    {
        Spline spline = sc.Spline;
        
        while(knotFoldouts.Count < spline.Count) knotFoldouts.Add(true);
        while (knotFoldouts.Count > spline.Count) knotFoldouts.RemoveAt(knotFoldouts.Count - 1);
        
        showKnots = EditorGUILayout.Foldout(showKnots, $"Knots ({spline.Count})", true);

        if (showKnots)
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);
            GUILayout.Space(4);
        
            for (int i = 0; i < spline.Count; i++)
            {
                knotFoldouts[i] = EditorGUILayout.Foldout(knotFoldouts[i], $"Knot [{i}]", true);
                if (knotFoldouts[i])
                {
                    EditorGUILayout.BeginVertical(GUI.skin.box);
                    
                    BezierKnot knot = spline[i];
            
                    EditorGUILayout.LabelField($"Knot [{i}]");
            
                    Vector3 pos = EditorGUILayout.Vector3Field("pos", knot.Position);
                    Vector3 rot = EditorGUILayout.Vector3Field("rot", ((Quaternion)knot.Rotation).eulerAngles);

                    if (EditorGUI.EndChangeCheck())
                    {
                        Undo.RecordObject(sc, "Edit Knot");
                        knot.Position = pos;
                        knot.Rotation = Quaternion.Euler(rot);
                        sc.Spline[i] = knot;
                        EditorUtility.SetDirty(sc);
                    }
                    
                    EditorGUILayout.EndVertical();
                }
            }
        
            GUILayout.Space(4);
            EditorGUILayout.EndVertical();
        }
        
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

    void SnapToGrid()
    {
        Undo.RecordObject(sc, "Snap To Grid");
        for(int i = 0; i <sc.Spline.Count; i++)
        {
            BezierKnot knot = sc.Spline[i];
            
            // Calculates The Closest Grip Position
            Vector3 newPos = new Vector3(
                Mathf.Round(knot.Position.x / gridSize) * gridSize,
                Mathf.Round(knot.Position.y / gridSize) * gridSize,
                Mathf.Round(knot.Position.z / gridSize) * gridSize);
            
            knot.Position = newPos;
            sc.Spline[i] = knot;
        }
    }
    
    void AngelSnap()
    {
        Undo.RecordObject(sc, "Angel Snap");
        for(int i = 0; i <sc.Spline.Count; i++)
        {
            BezierKnot knot = sc.Spline[i];
            Vector3 rot = ((Quaternion)knot.Rotation).eulerAngles;
            
            // Calculates The Closest Angel Step
            Vector3 newRot = new Vector3(
                Mathf.Round(rot.x / angleStep) * angleStep,
                Mathf.Round(rot.y / angleStep) * angleStep,
                Mathf.Round(rot.z / angleStep) * angleStep);
            
            knot.Rotation = Quaternion.Euler(newRot);
            sc.Spline[i] = knot;
        }
    }
}
