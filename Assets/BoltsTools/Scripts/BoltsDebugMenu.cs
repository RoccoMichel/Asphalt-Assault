using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

namespace BoltsTools
{
    [AddComponentMenu("Bolts Tools/Bolts Debug Menu")]
    public class BoltsDebugMenu : MonoBehaviour
    {
        public static BoltsDebugMenu Instance;

        public BoltsDebugMenuSettings settings;
        
        float currentFps, frameCount, time;

        readonly Queue<float> avg10sFrames = new();
        readonly Queue<float> avg1mFrames = new();

        bool showDebug;

        string currentLogName;
        
        public Transform player;
        
        void OnGUI()
        {
            if(!showDebug) return;

            if (settings.showPlayerPos && player != null)
            {
                if (player == null)
                {
                    Debug.LogError("Player Not Assigned!!!");
                    return;
                }
                
                Vector3 playerPos = player.position;

                GUIStyle style = new GUIStyle() { font = Font.CreateDynamicFontFromOSFont("Courier New", 25)};

                string text = string.Format("XYZ: X:{0,-8:F2}  Y:{1,-8:F2}  Z:{2,-8:F2}", playerPos.x, playerPos.y,
                    playerPos.z);
                
                float xPos = Screen.width - 600 - 100;
                style.alignment = TextAnchor.MiddleRight;
                
                Rect playerPosRect = new(xPos, 100, 600, 200);
                GUI.TextArea(playerPosRect, text, style);
            }
            
            if (settings.showFPS)
            {
                GUIStyle fpsStyle = new GUIStyle(GUI.skin.box) {alignment = TextAnchor.MiddleLeft, richText = true, fontSize = 25};
                
                Rect fpsRect = new(50, 50, 200,75);
                GUI.TextArea(fpsRect, $"FPS: {currentFps:F1}", fpsStyle);

                Rect avg10sFps = new(50, 150, 250, 75);
                GUI.TextArea(avg10sFps, $"Avg 10 Sec: {GetAverageFPS(avg10sFrames):f1}", fpsStyle);

                Rect avg1mFps = new Rect(50, 250, 250, 75);
                GUI.TextArea(avg1mFps, $"Avg 1 Min: {GetAverageFPS(avg1mFrames):F1}", fpsStyle);
            }
        }
        
        void Update()
        {
            if(settings == null) return;
            
            if(!settings.showCommands) return;

            if (settings.showFPS)
            {
                time += Time.unscaledDeltaTime;
                currentFps = 1 / Time.unscaledDeltaTime;
                frameCount++;
            
                if (time >= 1)
                {
                    float avgFpsThisSecond = frameCount / time;
                    avg10sFrames.Enqueue(avgFpsThisSecond);
                    avg1mFrames.Enqueue(avgFpsThisSecond);

                    if (avg10sFrames.Count > 10)
                        avg10sFrames.Dequeue();

                    if (avg1mFrames.Count > 60)
                        avg1mFrames.Dequeue();

                    time -= 1;
                    frameCount = 0;
                }
            }

            if (Input.GetKeyDown(settings.keyToOpenDebug))
                showDebug = !showDebug;

            if (player == null && LoadBoltsDebugMenu._settings.showPlayerPos && GameObject.FindGameObjectWithTag(settings.playerTag) != null)
                player = GameObject.FindGameObjectWithTag(settings.playerTag).transform;
        }

        void Awake()
        {
            Reset();

            if (settings.saveLog)
            {
                if (!Directory.Exists(settings.logPath))
                    Directory.CreateDirectory(settings.logPath);

                currentLogName = $"/{DateTime.Now:HH:m:s:} Log.txt";
                
                File.WriteAllText(settings.logPath + currentLogName, "");

                Application.logMessageReceived += AddLog;
            }
            
            if (Instance == null)
                Instance = this;
            else if(Instance != this)
                Destroy(gameObject);
        }

        public void Reset()
        {
            LoadBoltsDebugMenu.Initialize();
            
            if (Instance == null)
                Instance = this;
            else if(Instance != this)
                Destroy(gameObject);

            if (LoadBoltsDebugMenu._settings != null)
                settings = LoadBoltsDebugMenu._settings;
        }

        public static void AddLog(string logString, string stackTrace, LogType type)
        {
            string entry = $"[{type}] {DateTime.Now:HH:m:s} - {logString}";

            if (type == LogType.Error || type == LogType.Exception)
                entry += $"\n{stackTrace}";
            
            File.AppendAllText( Instance.settings.logPath + Instance.currentLogName, entry + "\n");
        }

        float GetAverageFPS(Queue<float> queue)
        {
            if (queue.Count == 0) return 0;

            float sum = 0;
            foreach (float val in queue)
                sum += val;

            return sum / queue.Count;
        }

        void OnDestroy()
        {
            Application.logMessageReceived -= AddLog;
        }
    }
    
    static class LoadBoltsDebugMenu
    {
        public static BoltsDebugMenuSettings _settings;
        static bool _isLoading;
        
#if UNITY_EDITOR
        [InitializeOnLoadMethod]
        static void InitializeInEditor()
        {
            Initialize();
        }
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
#endif
        public static void Initialize()
        {
            if(_settings != null || _isLoading)
                return;

            _isLoading = true;

            _settings = Resources.Load<BoltsDebugMenuSettings>("DebugSettings");
            Debug.Log("Debug Settings Loaded");

            _isLoading = false;
        }

        [MenuItem("GameObject/Bolts Debug Object #t", false, 5)]
        static void CreateOBJ(MenuCommand menuCommand)
        {
            GameObject obj = new GameObject("Bolts Debug");

            obj.AddComponent<BoltsDebugMenu>();
            obj.AddComponent<BoltsCommands>();
            
            GameObjectUtility.SetParentAndAlign(obj,menuCommand.context as GameObject);
            
            Undo.RegisterCreatedObjectUndo(obj, "Create Bots Debug Object");

            Selection.activeObject = obj;
        }
    }
}
