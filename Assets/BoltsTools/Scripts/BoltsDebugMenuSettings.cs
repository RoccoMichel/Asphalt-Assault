using UnityEditor;
using UnityEngine;

namespace BoltsTools
{
    [Icon("Assets/BoltsTools/Sprites/DebugLogo.png")]
    public class BoltsDebugMenuSettings : ScriptableObject
    {
        public KeyCode keyToOpenDebug = KeyCode.F3;

        public bool showFPS, showPlayerPos, saveLog;
        
        [HideInInspector]
        public string logPath = "Logs";
        
        [BoltsToolTip("Shows The Cursor When Typing A Command")]
        public bool unlockCursor = true;

        [HideInInspector]
        public string playerTag = "Player";

        public KeyCode keyToOpenCommands = KeyCode.F2;

        public bool showDebug = true, showCommands = true;

        public void ChangePath()
        {
            logPath = EditorUtility.OpenFolderPanel("Select Folder", "Assets", "");

            if (!string.IsNullOrEmpty(logPath))
            {
                string projectPath = Application.dataPath.Replace("/Assets", "");
                logPath = logPath.Replace(projectPath + "/", "");
            }
        }
    }
}
