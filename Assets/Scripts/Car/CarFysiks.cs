using UnityEngine;

public class CarFysiks : MonoBehaviour
{
    public LayerMask Grownd;
    public float horspower,stering, grip;
    Rigidbody rb = new();
    float deltaAcselurashen { get { 
            return Input.GetAxisRaw("Vertical") 
                * horspower 
                * Time.deltaTime; } }
    

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
        Vector3 line = transform.forward;
        Vector3 vel = rb.linearVelocity;

        return Vector3.Dot(vel.normalized, line);
    }
    void Awake() {
        rb = GetComponent<Rigidbody>();
    }
    void Update() {
        if (GetSuspesenForse(transform.position, 1, 1) != 0) {
            rb.AddForce(transform.forward * deltaAcselurashen);
            transform.rotation *= Quaternion.Euler(0f, Input.GetAxisRaw("Horizontal") * stering * Time.deltaTime * rb.linearVelocity.magnitude, 0f);

            if (1 - getVelosetyAlamentScore() < grip)
                rb.linearVelocity = ProdjectVelosetyOnForwordLine();
        }


        // SuspesenSimulasnen
        float stifnes = 10000;
        float legf = 0.3f;
        float torkApliskesn = 0.5f;

        Vector3 LocSupsensenPosisen = new Vector3(0.5f, -0.5f, 0.5f);
        Vector3 SupsensenPosisen = transform.TransformPoint(LocSupsensenPosisen);
        rb.AddForceAtPosition(Vector3.up * GetSuspesenForse(SupsensenPosisen, legf, stifnes), transform.TransformPoint(LocSupsensenPosisen * torkApliskesn));
        
        LocSupsensenPosisen = new Vector3(-0.5f, -0.5f, 0.5f);
        SupsensenPosisen = transform.TransformPoint(LocSupsensenPosisen);
        rb.AddForceAtPosition(Vector3.up * GetSuspesenForse(SupsensenPosisen, legf, stifnes), transform.TransformPoint(LocSupsensenPosisen * torkApliskesn));

        LocSupsensenPosisen = new Vector3(0.5f, -0.5f, -0.5f);
        SupsensenPosisen = transform.TransformPoint(LocSupsensenPosisen);
        rb.AddForceAtPosition(Vector3.up * GetSuspesenForse(SupsensenPosisen, legf, stifnes), transform.TransformPoint(LocSupsensenPosisen * torkApliskesn));

        LocSupsensenPosisen = new Vector3(-0.5f, -0.5f, -0.5f);
        SupsensenPosisen = transform.TransformPoint(LocSupsensenPosisen);
        rb.AddForceAtPosition(Vector3.up * GetSuspesenForse(SupsensenPosisen, legf, stifnes), transform.TransformPoint(LocSupsensenPosisen * torkApliskesn));
    }
}
