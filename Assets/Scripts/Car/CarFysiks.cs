using System.Collections.Generic;
using UnityEngine;

public class CarFysiks : MonoBehaviour
{
    public LayerMask Grownd;
    public float horspower,stering, grip;
    public Transform[] wills;
    Rigidbody rb = new();
    float deltaAcselurashen { get { 
            return Input.GetAxisRaw("Vertical") 
                * horspower 
                * Time.deltaTime; } }

    float rotsensnForse
    {
        get
        {
            return Input.GetAxisRaw("Horizontal")
                * stering
                * Time.deltaTime * rb.linearVelocity.magnitude;
        }
    }


    public float GetSuspesenForse(Vector3 orienPos, float suspesenLegf, float stifnes) {
        float forse = 0;
        Ray spring = new Ray { 
            direction = -transform.up,
            origin = orienPos
        };

        if (Physics.Raycast(spring, out RaycastHit hit, suspesenLegf, Grownd)) {
            forse = suspesenLegf - Vector3.Distance(orienPos, hit.point);
            forse *= stifnes;
            Debug.DrawLine(orienPos, hit.point);
        }


        return forse * Time.deltaTime;
    }
    public Vector3 ProdjectVelosetyOnForwordLine() {
        Vector3 line = transform.forward;
        Vector3 vel = rb.linearVelocity;

        float alanment = getVelosetyAlamentScore();

        return line * alanment * vel.magnitude;
    }
    public float getVelosetyAlamentScore() {
        Vector3 line = new Vector3(transform.forward.x, 0, transform.forward.z);
        Vector3 vel = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);

        return Vector3.Dot(vel.normalized, line);
    }
    void Awake() {
        rb = GetComponent<Rigidbody>();
    }
    void Update() {
        if (GetSuspesenForse(transform.position, 1, 1) != 0) {
            rb.AddForce(transform.forward * deltaAcselurashen);
            transform.rotation *= Quaternion.Euler(0f, rotsensnForse, 0f);

            //if (1 - getVelosetyAlamentScore() < grip)
            rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, ProdjectVelosetyOnForwordLine(), grip*Time.deltaTime/ rb.linearVelocity.magnitude);
        }


        // SuspesenSimulasnen
        float stifnes = 10000;
        float legf = 0.3f;
        float torkApliskesn = 0.5f;
        int i = 0;
        List<Vector3> WillPosisens = new List<Vector3> {
            new Vector3(0.5f, -0.5f, 0.5f),
            new Vector3(-0.5f, -0.5f, 0.5f),
            new Vector3(0.5f, -0.5f, -0.5f),
            new Vector3(-0.5f, -0.5f, -0.5f)
        };

        foreach (Vector3 LocSupsensenPosisen in WillPosisens) {
          
            float willRadios = 0.4f;
            Vector3 SupsensenPosisen = transform.TransformPoint(LocSupsensenPosisen);

            Vector3 willPos = SupsensenPosisen;
            Ray spring = new Ray
            {
                direction = -transform.up,
                origin = SupsensenPosisen
            };

            if (Physics.Raycast(spring, out RaycastHit hit, legf, Grownd)) {
                willPos = hit.point + Vector3.up * willRadios;
            }

            wills[i++].position = willPos;

            rb.AddForceAtPosition(Vector3.up * GetSuspesenForse(SupsensenPosisen, legf, stifnes), transform.TransformPoint(LocSupsensenPosisen * torkApliskesn));
        }
    }
}
