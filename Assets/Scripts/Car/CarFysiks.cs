using System.Collections.Generic;
using UnityEngine;

public class CarFysiks : MonoBehaviour
{
    public bool isGrownded;
    public LayerMask Grownd;
    public float horspower,bost,stering, grip, springdapening;
    public Transform[] wills;
    Rigidbody rb = new();
    float deltaAcselurashen { get { 
            return Input.GetAxisRaw("Vertical") 
                * (horspower + (Input.GetKey(KeyCode.Space) ? bost : 0)) 
                * Time.fixedDeltaTime; } }

    float rotsensnForse
    {
        get
        {
            return Input.GetAxisRaw("Horizontal")
                * stering
                * Time.fixedDeltaTime * rb.linearVelocity.magnitude;
        }
    }


    public float GetSuspesenForse(Vector3 orienPos, float suspesenLegf, float stifnes) {
        float forse = 0;
        Ray spring = new Ray { 
            direction = -transform.up,
            origin = orienPos
        };
        float copresen = 0;

        if (Physics.Raycast(spring, out RaycastHit hit, suspesenLegf, Grownd)) {
            float compression = suspesenLegf - hit.distance;

            float springForce = compression * stifnes;

            float wheelVelocity =
                Vector3.Dot(
                    rb.GetPointVelocity(orienPos),
                    transform.up
                );

            float dampingForce = wheelVelocity * springdapening;

            forse = springForce - dampingForce;
            Debug.DrawLine(orienPos, hit.point);
        }


        return Mathf.Clamp(forse * Time.fixedDeltaTime, 0, 1000);
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
        rb.centerOfMass = new Vector3(0, -0.5f, 0);
    }
    void FixedUpdate() {
        isGrownded = (GetSuspesenForse(transform.position, 1, 1) != 0);
        if (isGrownded) {
            rb.AddForce(transform.forward * deltaAcselurashen);
            rb.AddTorque(new Vector3(0f, rotsensnForse, 0f));

            Vector3 forwardVel =
            transform.forward *
            Vector3.Dot(rb.linearVelocity, transform.forward); 
            Vector3 sideVel =
                transform.right *
                Vector3.Dot(rb.linearVelocity, transform.right);

            rb.linearVelocity =
                forwardVel +
                sideVel * (1f - grip * Time.fixedDeltaTime);
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

            rb.AddForceAtPosition(transform.up * GetSuspesenForse(SupsensenPosisen, legf, stifnes), transform.TransformPoint(LocSupsensenPosisen * torkApliskesn));
        }
    }
}
