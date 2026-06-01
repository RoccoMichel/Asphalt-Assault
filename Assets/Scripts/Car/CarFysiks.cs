using System.Collections.Generic;
using UnityEngine;

public class CarFysiks : MonoBehaviour
{
    public bool isGrounded;
    public LayerMask ground;
    public float horsePower,boost,steering, grip, springDampening;
    public Transform[] wheels;
    Rigidbody rb = new();
    float deltaAcceleration { get { 
            return Input.GetAxisRaw("Vertical") 
                * (horsePower + (Input.GetKey(KeyCode.Space) ? boost : 0)) 
                * Time.fixedDeltaTime; } 
    }

    float rotationForce { get {
            return Input.GetAxisRaw("Horizontal")
                * steering
                * Time.fixedDeltaTime 
                * Mathf.Clamp(rb.linearVelocity.magnitude, 0, 10)
                * grip; }
    }


    public float GetSuspensionForce(Vector3 oreginPos, float suspensionLenght, float stiffness) {
        float force = 0;
        Ray spring = new Ray { 
            direction = -transform.up,
            origin = oreginPos
        };

        if (Physics.Raycast(spring, out RaycastHit hit, suspensionLenght, ground)) {
            float compression = suspensionLenght - hit.distance;

            float springForce = compression * stiffness;

            float wheelVelocity =
                Vector3.Dot(
                    rb.GetPointVelocity(oreginPos),
                    transform.up
                );

            float dampingForce = wheelVelocity * springDampening;

            force = springForce - dampingForce;
            Debug.DrawLine(oreginPos, hit.point);
        }


        return Mathf.Clamp(force * Time.fixedDeltaTime, 0, 1000);
    }
    public Vector3 ProjectVelocityOnForwardLine() {
        Vector3 line = transform.forward;
        Vector3 vel = rb.linearVelocity;

        float alignment = getVelocityAlignmentScore();

        return line * alignment * vel.magnitude;
    }
    public float getVelocityAlignmentScore() {
        Vector3 line = new Vector3(transform.forward.x, 0, transform.forward.z);
        Vector3 vel = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);

        return Vector3.Dot(vel.normalized, line);
    }
    void Awake() {
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = new Vector3(0, -0.5f, 0);
    }
    void FixedUpdate() {
        isGrounded = (GetSuspensionForce(transform.position, 1, 1) != 0);
        if (isGrounded) {
            rb.AddForce(transform.forward * deltaAcceleration);
            rb.AddTorque(new Vector3(0f, rotationForce, 0f));

            Vector3 forwardVel =
            transform.forward *
            Vector3.Dot(rb.linearVelocity, transform.forward); 
            Vector3 sideVel =
                transform.right *
                Vector3.Dot(rb.linearVelocity, transform.right);

            if (sideVel.magnitude > grip/2 || Input.GetKeyDown(KeyCode.LeftShift))
                rb.linearVelocity = forwardVel + sideVel * (1f - grip * Time.fixedDeltaTime);
            else rb.linearVelocity = forwardVel; 
        }


        // Suspension
        float stiffness = 10000;
        float length = 0.3f;
        float torqueApplication  = 0.5f;
        int i = 0;
        List<Vector3> wheelPosition = new List<Vector3> {
            new Vector3(0.5f, -0.5f, 0.5f),
            new Vector3(-0.5f, -0.5f, 0.5f),
            new Vector3(0.5f, -0.5f, -0.5f),
            new Vector3(-0.5f, -0.5f, -0.5f)
        };

        foreach (Vector3 LocalSuspensionPosition in wheelPosition) {
          
            float wheelRadius = 0.4f;
            Vector3 suspensionPosition = transform.TransformPoint(LocalSuspensionPosition);

            Vector3 wheelPos = suspensionPosition;
            Ray spring = new Ray
            {
                direction = -transform.up,
                origin = suspensionPosition
            };

            if (Physics.Raycast(spring, out RaycastHit hit, length, ground)) {
                wheelPos = hit.point + Vector3.up * wheelRadius;
            }

            wheels[i++].position = wheelPos;

            rb.AddForceAtPosition(transform.up * GetSuspensionForce(suspensionPosition, length, stiffness), transform.TransformPoint(LocalSuspensionPosition * torqueApplication ));
        }
    }
}
