using System;
using UnityEditor;
using UnityEditor.Splines;
using UnityEngine;
using UnityEngine.Splines;

[CustomEditor(typeof(RoadMaker))]
public class RoadMakerDrawer : Editor
{
    void OnEnable()
    {
        EditorSplineUtility.AfterSplineWasModified += OnSplineModified;
    }

    void OnDisable()
    {
        EditorSplineUtility.AfterSplineWasModified -= OnSplineModified;
    }

    void OnSplineModified(Spline _)
    {
        var gen = (RoadMaker)target;
        gen.GenerateMesh();
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        if(GUILayout.Button("Generate Mesh"))
            ((RoadMaker)target).GenerateMesh();
        
        if(GUILayout.Button("Save Road"))
            ((RoadMaker)target).SaveMesh();
    }
}
