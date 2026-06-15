using UnityEngine;
using UnityEngine.Splines;
using System.Collections;

public class CircuitTransition : MonoBehaviour
{
    public float transitionSpeed = 1;
    public float transitionPause;
    [SerializeField] private SplineAnimate previewDriver;
    [HideInInspector] public bool busy;

    private void Start()
    {
        previewDriver.GetComponent<Animator>().Play("Pop-In");
    }

    public void LoadCircuit(SplineExtrude outCircuit, SplineExtrude inCircuit) // make it work without busy flag.   // I think i did it?
    {
        if (busy) return;
        StartCoroutine(Transition(outCircuit, inCircuit));
    }

    private IEnumerator Transition(SplineExtrude outCircuit, SplineExtrude inCircuit)
    {
        busy = true;
        previewDriver.GetComponent<Animator>().Play("Shrink");
        StartCoroutine(TransitionOutCircuit(outCircuit, transitionSpeed));

        yield return new WaitForSeconds((1 / transitionSpeed) + transitionPause);

        previewDriver.GetComponent<Animator>().Play("Pop-In");
        previewDriver.Container = inCircuit.GetComponent<SplineContainer>();
        previewDriver.Restart(true);

        StartCoroutine(TransitionInCircuit(inCircuit, transitionSpeed));

        busy = false;

        yield break;
    }

    internal IEnumerator TransitionInCircuit(SplineExtrude spline, float speed)
    {
        // change the spline range from (0, 0) to (0, 1)
        spline.gameObject.SetActive(true);
        spline.Range = new Vector2(0, 0);

        while (spline.Range.y < 1)
        {
            spline.Range = new Vector2(spline.Range.x, Mathf.Clamp01(spline.Range.y + (speed * Time.deltaTime)));
            spline.Rebuild();
            yield return new WaitForEndOfFrame();
        }

        yield break;
    }
    internal IEnumerator TransitionOutCircuit(SplineExtrude spline, float speed)
    {
        // change the spline range from (0, 1) to (1, 1)
        spline.gameObject.SetActive(true);
        spline.Range = new Vector2(0, 1);

        while (spline.Range.x < 1)
        {
            spline.Range = new Vector2(Mathf.Clamp01(spline.Range.x + (speed * Time.deltaTime)), spline.Range.y);
            spline.Rebuild();
            yield return new WaitForEndOfFrame();
        }

        spline.gameObject.SetActive(false);

        yield break;
    }
}
