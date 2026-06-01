using UnityEngine;
using UnityEngine.ProBuilder;

public class CarQaletyOflife : MonoBehaviour
{
    Rigidbody rb = new();
    CarFysiks fysiks = new();
    float ligerDamp, aglerDamp;
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
                rb.AddTorque(TorqueAxes * Mathf.Clamp(5 - hit.distance, 0, 5), ForceMode.Acceleration);
            }

        }
        else {
            rb.linearDamping = ligerDamp;
            rb.angularDamping = aglerDamp;
        }
    }
}
