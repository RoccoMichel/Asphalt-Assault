using UnityEngine;

public class CarQaletyOflife : MonoBehaviour
{
    Rigidbody rb = new();
    CarFysiks fysiks = new();
    float ligerDamp, aglerDamp;
    bool RotateInAre;
    void Awake(){
        rb = GetComponent<Rigidbody>();
        fysiks = rb.GetComponent<CarFysiks>();
        ligerDamp = rb.linearDamping;
        aglerDamp = rb.angularDamping;
    }

    RaycastHit getNormolOfPontUnderCar() {
        Ray ray = new Ray { 
            direction = Vector3.down,
            origin = transform.position
        };

        if (Physics.Raycast(ray, out RaycastHit hit, fysiks.ground)) { 
            return hit;
        }

        return new RaycastHit { normal = Vector3.zero };
    }
    void FixedUpdate() {
        if (!fysiks.isGrounded) {
            rb.linearDamping = ligerDamp * 0.25f;
            rb.angularDamping = aglerDamp * 0.25f;

            RaycastHit hit = getNormolOfPontUnderCar();
            if (hit.normal != Vector3.zero) {
                Vector3 TorqueAxes = Vector3.Cross(transform.up, hit.normal);
                rb.AddTorque(TorqueAxes * Mathf.Clamp(7.5f - hit.distance, 0, 7.5f), ForceMode.Acceleration);
            }

            // are controle
            //if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.S)) 
                RotateInAre = true;

            if (RotateInAre && Input.GetAxisRaw("Vertical") != 0) { 
                rb.AddTorque(transform.right * Input.GetAxisRaw("Vertical") * 15);
                fysiks.trust--; 
            }
            Debug.DrawLine(transform.position, transform.position + transform.up * 100);
            if (RotateInAre) { 
                Debug.DrawLine(transform.position, transform.position + transform.forward*100);
            }
        }
        else {
            RotateInAre = false;
            rb.linearDamping = ligerDamp;
            rb.angularDamping = aglerDamp;
        }
    }
}
