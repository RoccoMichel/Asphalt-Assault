using UnityEngine;

public class CarQaletyOflife : MonoBehaviour
{
    Rigidbody rb = new();
    CarPhysics _physics = new();
    float ligerDamp, aglerDamp;
    bool RotateInAre;
    void Awake(){
        rb = GetComponent<Rigidbody>();
        _physics = rb.GetComponent<CarPhysics>();
        ligerDamp = rb.linearDamping;
        aglerDamp = rb.angularDamping;
    }
    RaycastHit getNormolOfPontUnderCar() {
        Ray ray = new Ray { 
            direction = Vector3.down,
            origin = transform.position
        };

        if (Physics.Raycast(ray, out RaycastHit hit, _physics.ground)) { 
            return hit;
        }

        return new RaycastHit { normal = Vector3.zero };
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.S))
            RotateInAre = true;
    }

    public void PassInInputs(float Y, bool RotateInAre) {
        if (!_physics.isGrounded)
        {
            rb.linearDamping = ligerDamp * 0.25f;
            rb.angularDamping = aglerDamp * 0.25f;

            RaycastHit hit = getNormolOfPontUnderCar();
            if (hit.normal != Vector3.zero)
            {
                Vector3 TorqueAxes = Vector3.Cross(transform.up, hit.normal);
                rb.AddTorque(TorqueAxes * Mathf.Clamp(7.5f - hit.distance, 0, 7.5f), ForceMode.Acceleration);
            }

            // are controle
            if (RotateInAre && Y != 0)
            {
                rb.AddTorque(transform.right * Y * 15);
                _physics.thrust--;
            }
            Debug.DrawLine(transform.position, transform.position + transform.up * 100);
            if (RotateInAre)
            {
                Debug.DrawLine(transform.position, transform.position + transform.forward * 100);
            }
        }
        else
        {
            rb.linearDamping = ligerDamp;
            rb.angularDamping = aglerDamp;
        }

    }
    void FixedUpdate() {
        // Spoler (forses car down if yure going fast)

        rb.AddForce(-transform.up * _physics.ProjectVelocityOnForwardLine().magnitude * (_physics.isGrounded ? 0.5f : 0.75f));
    }
}
