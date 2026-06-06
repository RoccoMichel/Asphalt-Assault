using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static CarFysiks;

public class CarFysiks : MonoBehaviour
{
    public enum growndType { 
        rode,
        terane,
        ise
    }
    public growndType GrowndType;
    Dictionary<growndType, float> groundSlipperiness = new()
    {
        { growndType.rode, 1.2f },
        { growndType.terane, 1f },
        { growndType.ise, 0.25f }
    };
    float GroundSlipperiness => groundSlipperiness[GrowndType];

    [HideInInspector] public bool isGrounded, isSliding = false;
    public LayerMask ground;
    public float horsePower,boost,steering, grip, springDampening;
    public Transform[] wheels;
    public Slider Slider;
    [HideInInspector] public Rigidbody rb = new();
    float _trust, maxTrost = 500;
    CarEfekts carEfekts;

    public void PassInInputs(float X, float Y, bool bost) {
        isGrounded = (GetSuspensionForce(transform.position, 1, 1) != 0);

        if (isGrounded)
        {
            rb.AddForce(transform.forward * deltaAcceleration * Y);
            rb.AddTorque(new Vector3(0f, rotationForce * X, 0f));

            ClampVelosetyToForwordDireksen();
        }
        else
        {
            trust++;
            carEfekts.SmokePartikals = false;
        }

        if (bost && trust > 0)
        {
            AddTrost();
            carEfekts.FigerPartikals = true;
        }
        else carEfekts.FigerPartikals = false;

        AplySuspensen();
    }

    //void FixedUpdate()
    //{
    //    isGrounded = (GetSuspensionForce(transform.position, 1, 1) != 0);

    //    if (isGrounded)
    //    {
    //        rb.AddForce(transform.forward * deltaAcceleration);
    //        rb.AddTorque(new Vector3(0f, rotationForce, 0f));

    //        ClampVelosetyToForwordDireksen();
    //    }
    //    else { 
    //        trust++;
    //        carEfekts.SmokePartikals = false;
    //    }

    //    if (Input.GetKey(KeyCode.Space) && trust > 0) {
    //        AddTrost();
    //        carEfekts.FigerPartikals = true;
    //    }
    //    else carEfekts.FigerPartikals = false;

    //    AplySuspensen();
    //}
    [HideInInspector] public float trust {
        get => _trust;
        set { 
            _trust = Mathf.Clamp(value, 0, maxTrost); 
            Slider.value = _trust; 
        }
    }
    float deltaAcceleration { get { 
            return horsePower 
                * Time.fixedDeltaTime; } 
    }
    float rotationForce { get {
            return steering
                * Time.fixedDeltaTime 
                * Mathf.Clamp(rb.linearVelocity.magnitude, 0, 10)
                * grip; }
    }

    float GetSuspensionForce(Vector3 oreginPos, float suspensionLenght, float stiffness) {
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
    float getVelocityAlignmentScore() {
        Vector3 line = new Vector3(transform.forward.x, 0, transform.forward.z);
        Vector3 vel = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);

        return Vector3.Dot(vel.normalized, line);
    }
    void AplySuspensen() {
        float stiffness = 10000;
        float length = 0.3f;
        float torqueApplication = 0.5f;
        int i = 0;
        List<Vector3> wheelPosition = new List<Vector3> {
            new Vector3(0.5f, -0.5f, 0.5f),
            new Vector3(-0.5f, -0.5f, 0.5f),
            new Vector3(0.5f, -0.5f, -0.5f),
            new Vector3(-0.5f, -0.5f, -0.5f)
        };

        foreach (Vector3 LocalSuspensionPosition in wheelPosition)
        {

            float wheelRadius = 0.4f;
            Vector3 suspensionPosition = transform.TransformPoint(LocalSuspensionPosition);

            Vector3 wheelPos = suspensionPosition;
            Ray spring = new Ray
            {
                direction = -transform.up,
                origin = suspensionPosition
            };

            if (Physics.Raycast(spring, out RaycastHit hit, length, ground))
            {
                wheelPos = hit.point + Vector3.up * wheelRadius;
            }

            wheels[i++].position = wheelPos;

            rb.AddForceAtPosition(transform.up * GetSuspensionForce(suspensionPosition, length, stiffness), transform.TransformPoint(LocalSuspensionPosition * torqueApplication));
        }
    }
    void ClampVelosetyToForwordDireksen() {

        float _grip = grip * GroundSlipperiness;
        Vector3 forwardVel =
              transform.forward *
              Vector3.Dot(rb.linearVelocity, transform.forward);
        Vector3 sideVel =
            transform.right *
            Vector3.Dot(rb.linearVelocity, transform.right);

        if (sideVel.magnitude > _grip / 2 || Input.GetKey(KeyCode.LeftShift))
        {
            rb.linearVelocity = forwardVel + sideVel * (1f - _grip * Time.fixedDeltaTime);
            trust += sideVel.magnitude * 0.1f;
            carEfekts.SmokePartikals = true;
            isSliding = true;
        }
        else { rb.linearVelocity = forwardVel; isSliding = false; carEfekts.SmokePartikals = false; }
    }
    void AddTrost() {
        rb.AddForce(transform.forward * boost);
        trust -= 5;
    }
    void Awake() {
        trust = maxTrost;
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = new Vector3(0, -0.5f, 0);
        carEfekts = GetComponent<CarEfekts>();
    }
  
}