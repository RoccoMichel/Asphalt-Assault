using System;
using BoltsTools;
using UnityEditor;
using UnityEngine;

namespace editor.BoltsTools
{
    [CustomEditor(typeof(BoltsDebugMenuSettings))]
    public class BoltsDebugMenuSettingsDrawer : Editor
    {
        public override void OnInspectorGUI()
        {
            BoltsDebugMenuSettings bdms = (BoltsDebugMenuSettings)target;
            
            DrawDefaultInspector();
            
            if (GUILayout.Button("Change Path"))
                bdms.ChangePath();

            GUIStyle logPathStyle = new GUIStyle(GUI.skin.box) { alignment = TextAnchor.MiddleCenter, richText = true };
            GUILayout.Label($"<color=white>Path: {bdms.logPath}</color>", logPathStyle);

            SerializedProperty playerTag = serializedObject.FindProperty("playerTag");
            
            string[] allTags = UnityEditorInternal.InternalEditorUtility.tags;
            int index = Array.IndexOf(allTags, playerTag.stringValue);

            index = EditorGUILayout.Popup("Player Tag", index, allTags);
            playerTag.stringValue = allTags[index];

            serializedObject.ApplyModifiedProperties();
        }
    }

    [CustomEditor(typeof(BoltsDebugMenu))]
    public class BoltsDebugMenuDrawer : Editor
    {
        public override void OnInspectorGUI()
        {
            BoltsDebugMenu bdm = (BoltsDebugMenu)target;
            
            GUI.enabled = false;
            DrawDefaultInspector();
            GUI.enabled = true;
            
            if(GUILayout.Button("Set Settings"))
                bdm.Reset();
        }
    }

    [CustomEditor(typeof(BoltsCommands))]
    public class BoltsCommandsDrawer : Editor
    {
        public override void OnInspectorGUI()
        {
            BoltsCommands bc = (BoltsCommands)target;
            
            GUI.enabled = false;
            DrawDefaultInspector();
            GUI.enabled = true;
            
            if(GUILayout.Button("Set Settings"))
                bc.Reset();
        }
    }
}
