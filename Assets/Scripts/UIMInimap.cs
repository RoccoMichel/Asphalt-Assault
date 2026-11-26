using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.Splines;

[System.Serializable]
public class MinimapSetings {
    public int nuberOfPonts;
}
public class UIMInimap : MonoBehaviour
{
    public MinimapSetings MinimapSetings = new();
    public LineRenderer lineRenderer;
    public Transform MinimapBacrond;
    public SplineContainer spline;
    public Transform PlayerIcon;
    Camera cam;

    void Start() {
        cam = GetComponentInParent<Camera>();
        spline = RoadMaker.instes.splineContainer;
        lineRenderer.positionCount = MinimapSetings.nuberOfPonts;
    }
   
    float FindRadios(Vector3 center, Vector3[] pont) {
        int cont = lineRenderer.positionCount;
        float radios = 0;
        for (int i = 0; i < cont; i++) {
            float distens = Vector3.Distance(center, pont[i]);
            if (distens > radios)
                radios = distens;
        }
        return radios;
    }
    void DrawDebugCircle(Vector3 center, float radius, int segments = 100)
    {
        Vector3 prev = center + new Vector3(radius, 0f, 0f);   // angle 0

        for (int i = 1; i <= segments; i++)
        {
            float a = (i / (float)segments) * 2f * Mathf.PI;
            Vector3 next = center + new Vector3(Mathf.Cos(a), 0f, Mathf.Sin(a)) * radius;
            Debug.DrawLine(prev, next, Color.green);
            prev = next;
        }
    }
    Vector3 MovePontToCamra(Vector3 pont, Vector3 center, float radios, Camera cam) {

        Vector3 f = Camera.main.transform.forward;
        f.y = 0f;
        float ang = f.sqrMagnitude > 0.0001f ? Mathf.Atan2(f.x, f.z) : 0f;  
        Vector3 lokolPont = RotatePont(pont - center, -ang);

        lokolPont = cam.transform.localToWorldMatrix.MultiplyVector(new Vector3(lokolPont.x, lokolPont.z, 0));
        Vector2 MapCecter = MinimapBacrond.position;

        float screenRadius = cam.orthographicSize * 0.35f;      
        float scalfaktor = screenRadius / radios;

        Vector3 offset = lokolPont * scalfaktor;

        RectTransform rt = (RectTransform)MinimapBacrond;
        float pxRadius = rt.rect.width * 0.5f * rt.lossyScale.x;
        float worldPerPixel = 2f * cam.orthographicSize / Screen.height;
        float maxRadius = pxRadius * worldPerPixel;

        offset = Vector3.ClampMagnitude(offset, maxRadius);

        pont = offset + cam.ScreenToWorldPoint(MapCecter);

        return pont + cam.transform.forward;
    }

    Vector3 RotatePont(Vector3 pont, float ang)   
    {
        float c = Mathf.Cos(ang);
        float s = Mathf.Sin(ang);

        return new Vector3(
            pont.x * c + pont.z * s,
            pont.y,
            -pont.x * s + pont.z * c);
    }
    void Update() {
        Vector3 pl = GM.carPonters[0].rb.position;
        int cont = lineRenderer.positionCount;

        var pont = new Vector3[cont];
        Vector3 center = Vector3.zero;
        for (int i = 0; i < cont; i++) {
            float t = i / (float)cont;                 
            Vector3 p = spline.EvaluatePosition(t);
            center += p;
            pont[i] = p;
        }
        center *= 1 / (0f+cont);
        float radios = FindRadios(center, pont);

        DrawDebugCircle(center, radios, 50);

       

        if (!Input.GetKey(KeyCode.Tab)) center = pl;
        for (int i = 0; i < cont; i++)
            pont[i] = MovePontToCamra(pont[i], center, radios, cam);

        PlayerIcon.position = MovePontToCamra(pl, center, radios, cam) - cam.transform.forward*0.5f;

        lineRenderer.SetPositions(pont);
    }
}
