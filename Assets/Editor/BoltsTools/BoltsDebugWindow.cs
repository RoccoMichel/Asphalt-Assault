using System;
using UnityEditor;
using UnityEngine;
using BoltsTools;

namespace editor.BoltsTools
{
    public class BoltsDebugWindow : EditorWindow
    {
        BoltsDebugMenuSettings config;
        SerializedObject serializedObject;

        [MenuItem("Tools/Bolts Tools/Debug Settings &d")]
        public static void OpenWindow()
        {
            BoltsDebugWindow window = GetWindow<BoltsDebugWindow>(true, "Debug Settings Window", true);
            
            window.minSize = new(400, 300);
            window.maxSize = new(400, 300);
        }

        void OnEnable()
        {
            LoadConfig();
        }

        void LoadConfig()
        {
            config = Resources.Load<BoltsDebugMenuSettings>("DebugSettings");

            if (config == null)
            {
                Debug.LogError("Could Not Find Debug Asset");
                return;
            }
            
            serializedObject = new SerializedObject(config);
        }
        
        void OnGUI()
        {
            EditorGUILayout.LabelField("Debug Window Settings", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            if (config == null)
            {
                EditorGUILayout.HelpBox("Debug Asset Not Found", MessageType.Error);
                return;
            }
            
            serializedObject.Update();

            SerializedProperty prop = serializedObject.GetIterator();
            prop.NextVisible(true);
            while (prop.NextVisible(false))
                EditorGUILayout.PropertyField(prop, true);

            if (GUILayout.Button("Change Path"))
                config.ChangePath();

            GUIStyle logPathStyle = new GUIStyle(GUI.skin.box) { alignment = TextAnchor.MiddleCenter, richText = true };
            GUILayout.Label($"<color=white>Path: {config.logPath}</color>", logPathStyle);
            
            SerializedProperty playerTag = serializedObject.FindProperty("playerTag");
            
            string[] allTags = UnityEditorInternal.InternalEditorUtility.tags;
            int index = Array.IndexOf(allTags, playerTag.stringValue);

            index = EditorGUILayout.Popup("Player Tag", index, allTags);
            playerTag.stringValue = allTags[index];

            serializedObject.ApplyModifiedProperties(); 
        }
    }
}

