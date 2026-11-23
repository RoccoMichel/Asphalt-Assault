using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Splines;

public class AICar : MonoBehaviour
{
    public LayerMask ground;
    public float horsePower,boost,steering, grip, springDampening;
    
    public SplineContainer currentSpline;
    public int nextSplineIndex;
    
    Spline Spline => currentSpline.Spline;

    void Start()
    {
        nextSplineIndex = NextIndex(FindClosestKnot());
    }

    void Update()
    {
        Vector3 target = KnotWorldPosition(nextSplineIndex);
        
        if(transform.position == target)
            nextSplineIndex = NextIndex(nextSplineIndex);
    }

    int FindClosestKnot()
    {
        int closest = 0;
        float bestDist = float.MaxValue;

        for (int i = 0; i < Spline.Count; i++)
        {
            float dist = Vector3.Distance(transform.position, KnotWorldPosition(i));
            if (dist < bestDist)
            {
                bestDist = dist;
                closest = i;
            }
        }
        
        return closest;
    }

    int NextIndex(int i)
    {
        if (Spline.Closed)
            return (i + 1)  % Spline.Count;
        return Mathf.Min(i + 1, Spline.Count - 1);
    }

    Vector3 KnotWorldPosition(int i)
    {
        return currentSpline.transform.TransformPoint(Spline[i].Position);
    }

    [Button]
    void TestMove()
    {
        transform.position = KnotWorldPosition(nextSplineIndex);
    }
}
