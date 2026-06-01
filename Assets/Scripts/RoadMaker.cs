using System.IO;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Splines;

public class RoadMaker : MonoBehaviour
{
    public float roadWidth = 8;
    public int segmentsPerUnit = 2;
    public float maxMiterScale = 3;

    // public float textureScale = 10;
    public float surfaceOffset = 0.02f;

    public Texture2D roadTexture;
    public float textureTileLenght = 8;
    
    public LayerMask terrainLayer;
    
    public SplineContainer splineContainer;
    
    Mesh mesh;

    void Awake()
    {
        splineContainer = GetComponent<SplineContainer>();
    }

    public void GenerateMesh()
    {
        Spline spline = splineContainer.Spline;
        float length = spline.GetLength();
        int segments = Mathf.Max(1, Mathf.RoundToInt(length * segmentsPerUnit));

        if (roadTexture != null)
        {
            float aspect = (float)roadTexture.height / roadTexture.width;
            textureTileLenght = roadWidth * aspect;

            var mat = GetComponent<MeshRenderer>().sharedMaterial;
            if (mat != null) mat.mainTexture = roadTexture;
        }
        
        var vertices = new Vector3[(segments + 1) * 2];
        var uvs = new Vector2[vertices.Length];
        var triangles = new int[segments * 6];

        var positions = new Vector3[segments + 1];
        var rights = new Vector3[segments + 1];
        
        for (int i = 0; i <= segments; i++)
        {
            float t = i / (float)segments;
            spline.Evaluate(t, out float3 pos, out float3 tan, out float3 _);
            positions[i] = pos;
            rights[i] = Vector3.Cross(Vector3.Normalize(tan), Vector3.up).normalized;
        }
        
        for (int i = 0; i <= segments; i++)
        {
            float t = i / (float)segments;
            float dist = t * length;

            Vector3 miter;
            if (i == 0)
                miter = rights[0];
            else if (i == segments)
                miter = rights[segments];
            else
                miter = (rights[i - 1] + rights[i]).normalized;

            float dot = Vector3.Dot(miter, rights[i]);
            float scale = Mathf.Approximately(dot, 0) ? 1 : Mathf.Min(1 / dot, maxMiterScale);
            
            float halfW = roadWidth * 0.5f;
            float halfWMiter = halfW * scale;

            float halfRight = halfWMiter;
            float halfLeft = halfWMiter;

            if (i > 0 && i < segments)
            {
                float turnRadius = CircumRadius(positions[i - 1], positions[i], positions[i + 1]);
                if (turnRadius < halfW)
                {
                    Vector3 prevFwd = (positions[i] - positions[i - 1]).normalized;
                    Vector3 nextFwd = (positions[i + 1] - positions[i]).normalized;

                    bool leftTurn = Vector3.Cross(prevFwd, nextFwd).y < 0;

                    float clampedHalf = Mathf.Max(0.1f, turnRadius) * scale;

                    if (leftTurn)
                        halfLeft = clampedHalf;
                    else
                        halfRight = clampedHalf;
                }
            }
            
            int v1 = i * 2;

            vertices[v1 + 0] = SnapToTerrain(positions[i] - miter * halfRight);
            vertices[v1 + 1] = SnapToTerrain(positions[i] + miter * halfLeft);
            

            float v = dist / textureTileLenght;
            uvs[v1 + 0] = new Vector2(0, v);
            uvs[v1 + 1] = new Vector2(1, v);
        }

        int triIdx = 0;
        for (int i = 0; i < segments; i++)
        {
            int a = i * 2, b = i * 2 + 1;
            int c = (i + 1) * 2, d = (i + 1) * 2 + 1;

            triangles[triIdx++] = a;
            triangles[triIdx++] = b;
            triangles[triIdx++] = c;

            triangles[triIdx++] = b;
            triangles[triIdx++] = d;
            triangles[triIdx++] = c;
        }
        
        if (mesh == null) mesh = new Mesh();
        mesh.name = "Road";
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.uv = uvs;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        GetComponent<MeshFilter>().sharedMesh = mesh;
    }
    
    Vector3 SnapToTerrain(Vector3 pos)
    {
        // Unity Terrain
        if (Terrain.activeTerrain != null)
        {
            pos.y = Terrain.activeTerrain.SampleHeight(pos) + surfaceOffset;
            return pos;
        }

        // Mesh/collider fallback
        if (Physics.Raycast(pos + Vector3.up * 50f, Vector3.down, out RaycastHit hit, 100f, terrainLayer))
        {
            pos.y = hit.point.y + surfaceOffset;
        }

        return pos;
    }

    static float CircumRadius(Vector3 a, Vector3 b, Vector3 c)
    {
        a.y = b.y = c.y = 0;
        float ab = Vector3.Distance(a, b);
        float bc = Vector3.Distance(b, c);
        float ca = Vector3.Distance(c, a);
        float area = Vector3.Cross(b - a, c - a).magnitude * 0.5f;
        return area < 0.0001f ? float.MaxValue : (ab * bc * ca) / (4 * area);
    }

    void OnDrawGizmos()
    {
        var sc = GetComponent<SplineContainer>();
        if(sc == null) return;

        Spline spline = sc.Spline;
        float lenght = spline.GetLength();
        int segments = Mathf.Max(1, Mathf.RoundToInt(lenght * segmentsPerUnit));

        var positions = new Vector3[segments + 1];
        for (int i = 0; i <= segments; i++)
        {
            float t = i / (float)segments;
            spline.Evaluate(t, out float3 pos, out float3 _, out float3 _);
            positions[i] = pos;
        }

        float minRadius = roadWidth * 0.5f;
        for (int i = 1; i < segments; i++)
        {
            if (CircumRadius(positions[i - 1], positions[i], positions[i + 1]) < minRadius)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(positions[i], 0.5f);
            }
        }
    }
    
    public void SaveMesh()
    {
        GenerateMesh();
        string sceneName = SceneManager.GetActiveScene().name;
        string path = "Assets/Scenes/" + sceneName;
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
            AssetDatabase.Refresh();
        }

        string pathAndName = AssetDatabase.GenerateUniqueAssetPath(path + "/Road.asset");
        
        AssetDatabase.CreateAsset(mesh, pathAndName);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        
        Mesh asset = AssetDatabase.LoadAssetAtPath<Mesh>(pathAndName);
        EditorGUIUtility.PingObject(asset);
        
        Debug.Log("Road mesh saved to " + pathAndName);
    }
}
