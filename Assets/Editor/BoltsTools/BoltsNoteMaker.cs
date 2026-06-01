using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace editor.BoltsTools
{
    public class BoltsNoteMaker : EditorWindow
    {
        // State
        List<StickyNote> notes = new();
        const string SAVE_DIR = "Assets/Editor/BoltsTools/Notes";
        const string LINES_PATH = "Assets/Editor/BoltsTools/Notes/Lines.json";
        StickyNote resizingNote;
        Vector2 resizeStart;
        Rect resizeStartRect;
        StickyNote draggingNote;
        Vector2 dragOffset;
        
        // Lines
        StickyNote lineSourceNote;
        bool isDrawingLine;
        List<NoteLine> lines = new();

        // Pan And Zoom
        Vector2 pan = Vector2.zero;
        float zoom = 1;
        bool isPanning;
        Vector2 lastMousePos;
        
        // Context Menu
        bool showContextMenu;
        Vector2 contextMenuPos;
        Rect contextMenuRect;
        
        // Note Context Menu
        bool showNoteContextMenu;
        Vector2 noteContextMenuPos;
        Rect noteContextMenuRect;
        StickyNote noteContextMenuTarget;
        
        const float TAB_HEIGHT = 25.5f;

        readonly Color[] noteColors =
        {
            new(1, 0.96f, 0.6f), // Yellow
            new(0.6f, 1, 0.7f), //Green
            new(0.6f, 0.85f, 1), // Blue
            new(1, 0.75f, 0.75f), // Pink
            new(0.9f, 0.75f, 1), // Purple
        };

        [MenuItem("Tools/Bolts Tools/Notes &n")]
        public static void Open()
        {
            var win = GetWindow<BoltsNoteMaker>("Notes");
            win.minSize = new(600, 400);
        }

        void OnEnable()
        {
            Refresh();
        }

        void OnDisable()
        {
            SaveNotes();
        }

        void SaveNotes()
        {
            if (!Directory.Exists(SAVE_DIR))
                Directory.CreateDirectory(SAVE_DIR);

            foreach (var file in Directory.GetFiles(SAVE_DIR, "*.json"))
                if(file != LINES_PATH.Replace("/", Path.DirectorySeparatorChar.ToString()))
                {
                    File.Delete(file);
                    File.Delete(file + ".meta");
                }
            var nameCounts = new Dictionary<string, int>();
            foreach (var note in notes)
            {
                var baseName = string.IsNullOrWhiteSpace(note.name) ? "Sticky Note" : note.name;
                nameCounts.TryAdd(baseName, 0);
                nameCounts[baseName]++;

                var count = nameCounts[baseName];
                var fileName = count == 1 ? baseName : $"{baseName} {count}";
                var path = Path.Combine(SAVE_DIR, $"{fileName}.json");
                
                File.WriteAllText(path, JsonUtility.ToJson(note, true));
            }

            var wrapper = new NoteListWrapper { lines = lines };
            File.WriteAllText(LINES_PATH, JsonUtility.ToJson(wrapper, true));
            
            AssetDatabase.Refresh();
        }

        void Refresh()
        {
            notes = new List<StickyNote>();
            lines = new List<NoteLine>();
            
            if(!Directory.Exists(SAVE_DIR)) return;

            // Load All Notes
            foreach (var file in Directory.GetFiles(SAVE_DIR, "*.json"))
            {
                var normalized = file.Replace("\\", "/");
                if(normalized == LINES_PATH.Replace("\\", "/")) continue;

                var note = JsonUtility.FromJson<StickyNote>(File.ReadAllText(file));
                if(note != null) notes.Add(note);
            }

            foreach (var note in notes)
            {
                foreach (var targetId in note.connectedIDs)
                {
                    // Skip If The TTarget Note Doesn't Exist
                    if(!notes.Exists(n => n.id == targetId)) continue;

                    // Skip Duplicates
                    bool exists = lines.Exists(l =>
                        (l.sourceId == note.id && l.targetId == targetId) ||
                        (l.sourceId == targetId && l.targetId == note.id));
                    
                    if(!exists)
                        lines.Add((new NoteLine{sourceId = note.id, targetId = targetId}));
                }
            }
            
            Repaint();
        }

        void OnGUI()
        {
            DrawGrid();
            
            GUI.BeginGroup(new Rect(pan.x * zoom, pan.y * zoom - TAB_HEIGHT,
                100000, 100000));
            
            foreach (var line in lines)
            {
                var a = notes.Find(n => n.id == line.sourceId);
                var b = notes.Find(n => n.id == line.targetId);
                if(a == null || b == null) continue;
                DrawGradientLine(new Vector2(a.rect.center.x, a.rect.center.y),
                    new Vector2(b.rect.center.x, b.rect.center.y),
                    a.color, b.color);
            }

            if (isDrawingLine && lineSourceNote != null)
            {
                var mouseInCanvas = ScreenToCanvas(Event.current.mousePosition);
                DrawGradientLine(
                    new Vector2(lineSourceNote.rect.center.x, lineSourceNote.rect.center.y),
                    mouseInCanvas,
                    lineSourceNote.color, lineSourceNote.color);
                Repaint();
            }
            
            foreach (var note in notes)
                DrawStickyNote(note);
            
            GUI.EndGroup();
            
            HandleEvents();

            if(showContextMenu)
            {
                DrawContextMenu();

                if (!contextMenuRect.Contains(Event.current.mousePosition))
                    showContextMenu = false;
            }
            
            if(showNoteContextMenu)
            {
                DrawNoteContextMenu();

                if (Event.current.type == EventType.MouseDown && !noteContextMenuRect.Contains(Event.current.mousePosition))
                    showNoteContextMenu = false;
            }
            
            var saveBtn = new Rect(8, 8, 70, 22);
            if(GUI.Button(saveBtn, "💾 Save"))
                SaveNotes();

            var refreshBtn = new Rect(8, 36, 70, 22);
            var refreshContent = new GUIContent(" Refresh", EditorGUIUtility.IconContent("Refresh").image);
            if(GUI.Button(refreshBtn, refreshContent))
                Refresh();
        }

        void DrawGrid()
        {
            var bgColor = new Color(0.18f, 0.18f, 0.18f);
            EditorGUI.DrawRect(new(0, 0, position.width, position.height), bgColor);

            DrawGridLines(20 * zoom, new Color(1, 1, 1, 0.04f));
            DrawGridLines(100 * zoom, new Color(1, 1, 1, 0.08f));
        }

        void DrawGridLines(float spacing, Color color)
        {
            float offsetX = pan.x * zoom % spacing;
            for(float x = offsetX; x < position.width; x += spacing)
                EditorGUI.DrawRect(new(x, 0,1, position.height), color);
            
            float offsetY = pan.y * zoom % spacing;
            for(float y = offsetY; y < position.height; y += spacing)
                EditorGUI.DrawRect(new(0,y,position.width, 1), color);
        }

        void DrawStickyNote(StickyNote note)
        {
            // Shadow
            var shadow = new Rect(note.rect.x + 4, note.rect.y + 4, note.rect.width, note.rect.height);
            EditorGUI.DrawRect(shadow, new (0, 0, 0, 0.3f));
            
            // Body
            EditorGUI.DrawRect(note.rect, note.color);
            
            // Header Bar
            var header = new Rect(note.rect.x, note.rect.y, note.rect.width, 38);
            EditorGUI.DrawRect(header, new(0, 0, 0, 0.15f));
            
            // Label In Header
            var nameFieldStyle = new GUIStyle(EditorStyles.textField) { fontStyle = FontStyle.Bold, fontSize = 12 };
            var nameRect = new Rect(header.x + 6, header.y + 17, header.width - 34, 18);
            var newName = GUI.TextField(nameRect, note.name, nameFieldStyle);
            note.name = newName;
            if(Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Return)
                SaveNotes();
            
            // Delete Button
            var deleteBtn = new Rect(note.rect.xMax - 22, note.rect.y + 17, 18, 18);
            var deleteStyle = new GUIStyle(EditorStyles.label)
            {
                normal = { textColor = Color.black },
                hover = { textColor = new Color(0.6f, 0, 0) }
            };
            if (GUI.Button(deleteBtn, "X", deleteStyle))
            {
                notes.Remove(note);

                lines.RemoveAll(l => l.sourceId == note.id || l.targetId == note.id);
                SaveNotes();
                Repaint();
                GUIUtility.ExitGUI();
                return;
            }
            
            // Text Area
            var textArea = new Rect(note.rect.x + 4, note.rect.y + 42, note.rect.width - 8, note.rect.height - 50);
            var style = new GUIStyle(EditorStyles.textArea)
            {
                wordWrap = true,
                fontSize = 12,
            };
            var newText = GUI.TextArea(textArea, note.text, style);
            note.text = newText;
            if(Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Return)
            {
                SaveNotes();
            }
            
            // Resize Handle
            var handle = new Rect(note.rect.xMax - 12, note.rect.yMax - 12, 12, 12);
            EditorGUI.DrawRect(handle, new(0, 0, 0, 0.25f));
            EditorGUIUtility.AddCursorRect(handle, MouseCursor.ResizeUpLeft);
            
            // Color Picker Dots At Bottom
            float dotSize = 10;
            float dotY = note.rect.yMax - 14;
            float dotStartX = note.rect.x + 6;
            for(int i = 0; i < noteColors.Length; i++)
            {
                var dotRect = new Rect(dotStartX + i * 14, dotY, dotSize, dotSize);
                EditorGUI.DrawRect(dotRect, noteColors[i]);
                if (GUI.Button(dotRect, GUIContent.none, GUIStyle.none))
                    note.color = noteColors[i];
            }

            if (Event.current.type == EventType.ContextClick && note.rect.Contains(Event.current.mousePosition))
            {
                var screenPos = CanvasToScreen(new Vector2(note.rect.center.x, note.rect.y));
                
                noteContextMenuRect = new Rect(screenPos.x - 90, screenPos.y - 36, 180, 36);
                noteContextMenuTarget = note;
                showNoteContextMenu = true;
                showContextMenu = false;
                Event.current.Use();
            }
            
            // Handle Drag And resize
            HandleNoteDrag(note, header, handle);
        }

        void HandleNoteDrag(StickyNote note, Rect header, Rect handle)
        {
            var e = Event.current;
            var mousePos = e.mousePosition;

            if (isDrawingLine)
            {
                if (e.type == EventType.MouseDown && e.button == 0 && note.rect.Contains(e.mousePosition) &&
                    note != lineSourceNote)
                {
                    bool exists = lines.Exists(l =>
                        (l.sourceId == lineSourceNote.id && l.targetId == note.id ||
                         (l.sourceId == note.id && l.targetId == lineSourceNote.id)));

                    if (!exists)
                    {
                        lines.Add(new NoteLine { sourceId = lineSourceNote.id, targetId = note.id });
                        lineSourceNote.connectedIDs.Add(note.id);
                        SaveNotes();
                    }

                    isDrawingLine = false;
                    lineSourceNote = null;
                    e.Use();
                }
                
                return;
            }
            
            if (e.type == EventType.MouseDown && handle.Contains(mousePos) && e.button == 0)
            {
                resizingNote = note;
                resizeStart = mousePos;
                resizeStartRect = note.rect;
                e.Use();
            }

            if (resizingNote == note)
            {
                if (e.type == EventType.MouseDrag)
                {
                    var delta = mousePos - resizeStart;
                    note.rect = new Rect(
                        resizeStartRect.x,
                        resizeStartRect.y,
                        Mathf.Max(140, resizeStartRect.width + delta.x),
                        Mathf.Max(100, resizeStartRect.height + delta.y));
                    Repaint();
                    e.Use();
                }

                if (e.type == EventType.MouseUp)
                {
                    resizingNote = null;
                    SaveNotes();
                    e.Use();
                }
            }
            
            // Drag Via Header
            if (e.type == EventType.MouseDown && header.Contains(mousePos) && e.button == 0
                && !handle.Contains(mousePos))
            {
                draggingNote = note;
                dragOffset = mousePos - new Vector2(note.rect.x, note.rect.y);
                GUIUtility.hotControl = GUIUtility.GetControlID(FocusType.Passive);
                e.Use();
            }

            // Drag The Note
            if (e.type == EventType.MouseDrag && draggingNote == note)
            {
                var newPos = mousePos - dragOffset;
                note.rect.x = Mathf.Max(0, newPos.x);
                note.rect.y = Mathf.Max(0, newPos.y);
                Repaint();
                e.Use();
            }

            // Stops Dragging Note
            if (e.type == EventType.MouseUp && draggingNote == note)
            {
                draggingNote = null;
                SaveNotes();
                e.Use();
            }
        }

        void HandleEvents()
        {
            var e = Event.current;
            
            var canvasMouse = ScreenToCanvas(e.mousePosition);
            bool clickedNote = notes.Exists(n => n.rect.Contains(canvasMouse));
            
            switch (e.type)
            {
                // Focus On Noting
                case EventType.MouseDown when e.button == 0:
                    if (!clickedNote)
                    {
                        GUI.FocusControl(null);
                        Repaint();
                    }
                    break;
                
                // Bring Up The Context Menu
                case EventType.ContextClick:
                case EventType.MouseDown when e.button == 1:
                    contextMenuPos = e.mousePosition;
                    contextMenuRect = new Rect(contextMenuPos.x, contextMenuPos.y, 180, 36);
                    showContextMenu = true;
                    e.Use();
                    break;
                
                // Start Panning
                case EventType.MouseDown when e.button == 2 || (e.button == 0 && e.alt):
                    isPanning = true;
                    lastMousePos = e.mousePosition;
                    e.Use();
                    break;
                
                // Panning
                case EventType.MouseDrag when isPanning:
                    pan += (e.mousePosition - lastMousePos) / zoom;
                    pan.x = Mathf.Min(pan.x, 0);
                    pan.y = Mathf.Min(pan.y, 0);
                    lastMousePos = e.mousePosition;
                    Repaint();
                    e.Use();
                    break;
                
                // Stop Panning
                case EventType.MouseUp when isPanning:
                    isPanning = false;
                    e.Use();
                    break;
                
                // Zoom
                case EventType.ScrollWheel:
                    var zoomDelta = -e.delta.y * 0.05f;
                    zoom = Mathf.Clamp(zoom + zoomDelta, 0.3f, 3f);
                    Repaint();
                    e.Use();
                    break;
                
                // Stop Showing Context Menu
                case EventType.MouseDown when showContextMenu && !contextMenuRect.Contains(e.mousePosition):
                    showContextMenu = false;
                    Repaint();
                    break;
                
                // Stop Drawing Line
                case EventType.KeyDown when e.keyCode == KeyCode.Escape && isDrawingLine:
                    isDrawingLine = false;
                    lineSourceNote = null;
                    showNoteContextMenu = false;
                    Repaint();
                    e.Use();
                    break;
                
                // Stop Showing Note Context Menu
                case EventType.MouseDown when showNoteContextMenu && !noteContextMenuRect.Contains(e.mousePosition):
                    showNoteContextMenu = false;
                    Repaint();
                    break;
            }
        }

        void DrawNoteContextMenu()
        {
            // Background
            EditorGUI.DrawRect(noteContextMenuRect, new Color(0.22f, 0.22f, 0.22f));
            
            // Border
            var border = new Color(0.45f, 0.45f, 0.45f);
            EditorGUI.DrawRect(new Rect(noteContextMenuRect.x, noteContextMenuRect.y, noteContextMenuRect.width, 1), border);
            EditorGUI.DrawRect(new Rect(noteContextMenuRect.x, noteContextMenuRect.yMax - 1, noteContextMenuRect.width, 1), border);
            EditorGUI.DrawRect(new Rect(noteContextMenuRect.x, noteContextMenuRect.y, 1, noteContextMenuRect.height), border);
            EditorGUI.DrawRect(new Rect(noteContextMenuRect.xMax - 1, noteContextMenuRect.y, 1, noteContextMenuRect.height), border);

            var btnRect = new Rect(noteContextMenuRect.x + 1, noteContextMenuRect.y + 1, noteContextMenuRect.width - 2,
                34);
            var style = new GUIStyle(EditorStyles.label)
            {
                normal = { textColor = Color.white },
                hover = { textColor = Color.white, background = MakeTex(1, 1, new Color(0.3f, 0.5f, 0.9f, 0.5f)) },
                padding = new RectOffset(10, 0, 0, 0),
                fontSize = 12
            };

            if (GUI.Button(btnRect, "📏  Draw Line", style))
            {
                lineSourceNote = noteContextMenuTarget;
                isDrawingLine = true;
                showNoteContextMenu = false;
                Repaint();
            }
        }
        
        void DrawContextMenu()
        {
            // Background
            EditorGUI.DrawRect(contextMenuRect, new Color(0.22f, 0.22f, 0.22f));
            
            // Border
            var border = new Color(0.45f, 0.45f, 0.45f);
            EditorGUI.DrawRect(new Rect(contextMenuRect.x, contextMenuRect.y, contextMenuRect.width, 1), border);
            EditorGUI.DrawRect(new Rect(contextMenuRect.x, contextMenuRect.yMax - 1, contextMenuRect.width, 1), border);
            EditorGUI.DrawRect(new Rect(contextMenuRect.x, contextMenuRect.y, 1, contextMenuRect.height), border);
            EditorGUI.DrawRect(new Rect(contextMenuRect.xMax - 1, contextMenuRect.y, 1, contextMenuRect.height), border);

            var btnRect = new Rect(contextMenuRect.x + 1, contextMenuRect.y + 1, contextMenuRect.width - 2, 34);
            var style = new GUIStyle(EditorStyles.label)
            {
                normal = { textColor = Color.white },
                hover = { textColor = Color.white, background = MakeTex(1, 1, new Color(0.3f, 0.5f, 0.9f, 0.5f)) },
                padding = new RectOffset(10, 0, 0, 0),
                fontSize = 12
            };
            
            if (GUI.Button(btnRect, "📝  Create Sticky Note", style))
            {
                var canvasPos = ScreenToCanvas(contextMenuPos);
                notes.Add(new StickyNote(canvasPos));
                SaveNotes();
                showContextMenu = false;
                Repaint();
            }
        }

        Texture2D MakeTex(int w, int h, Color col)
        {
            var tex = new Texture2D(w, h);
            tex.SetPixel(0,0, col);
            tex.Apply();
            return tex;
        }

        void DrawGradientLine(Vector2 from, Vector2 to, Color colorA, Color colorB, int segments = 40)
        {
            if(Event.current.type != EventType.Repaint) return;

            var handelColor = Handles.color;
            for (int i = 0; i < segments; i++)
            {
                float t0 = (float)i / segments;
                float t1 = (float)(i + 1) / segments;
                var p0 = Vector2.Lerp(from, to, t0);
                var p1 = Vector2.Lerp(from, to, t1);
                Handles.color = Color.Lerp(colorA, colorB, (t0 + t1) / 2);
                Handles.DrawAAPolyLine(3, new Vector3(p0.x, p0.y), new Vector3(p1.x, p1.y));
            }

            Handles.color = handelColor;
        }

        Vector2 ScreenToCanvas(Vector2 screenPos) =>
            new (screenPos.x / zoom - pan.x, screenPos.y / zoom - pan.y);

        Vector2 CanvasToScreen(Vector2 canvasPos) =>
            new ((canvasPos.x + pan.x) * zoom, (canvasPos.y + pan.y) * zoom);
        
        [Serializable]
        public class StickyNote
        {
            public string id;
            public List<string> connectedIDs;
            public string name;
            public Rect rect;
            public string text;
            public Color color;
            
            public StickyNote(Vector2 position)
            {
                id = Guid.NewGuid().ToString();
                connectedIDs = new List<string>();
                name = "Sticky Note";
                rect = new Rect(position.x, position.y, 200, 150);
                text = "";
                color = new Color(1f, 0.96f, 0.6f);
            }
        }
        
        [Serializable]
        public class NoteListWrapper
        {
            public List<NoteLine> lines;
        }
        
        [Serializable]
        public class NoteLine
        {
            public string sourceId;
            public string targetId;
        }
    }
}
