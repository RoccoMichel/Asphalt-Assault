using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CarPhysics : MonoBehaviour 
{
    public enum groundType { 
        road,
        terrain,
        ice
    }
    public groundType GroundType;
    Dictionary<groundType, float> groundSlipperiness = new()
    {
        { groundType.road, 1.2f },
        { groundType.terrain, 1f },
        { groundType.ice, 0.25f }
    };
    float GroundSlipperiness => groundSlipperiness[GroundType];

    [HideInInspector] public bool isGrounded, isSliding = false;
    public LayerMask ground;
    public float horsePower,boost,steering, grip, springDampening;
    public Transform[] wheels;
    public Slider slider;
    [HideInInspector] public Rigidbody rb = new();
    float _thrust, maxThrust = 500;
    CarEffects carEffects;

    public void PassInInputs(float X, float Y, bool bost) {
        isGrounded = (GetSuspensionForce(transform.position, 1, 1) != 0);

        if (isGrounded)
        {
            rb.AddForce(transform.forward * deltaAcceleration * Y);
            rb.AddTorque(new Vector3(0f, rotationForce * X, 0f));

            ClampVelocityToForwordDiretion();
        }
        else
        {
            thrust++;
            carEffects.SmokeParticles = false;
        }

        if (bost && thrust > 0)
        {
            AddThrust();
            carEffects.FigerParticles = true;
        }
        else carEffects.FigerParticles = false;

        ApplySuspensen();
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
    [HideInInspector] public float thrust {
        get => _thrust;
        set { 
            _thrust = Mathf.Clamp(value, 0, maxThrust); 
            slider.value = _thrust; 
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

    float GetSuspensionForce(Vector3 originPos, float suspensionLenght, float stiffness) {
        float force = 0;
        Ray spring = new Ray { 
            direction = -transform.up,
            origin = originPos
        };

        if (Physics.Raycast(spring, out RaycastHit hit, suspensionLenght, ground)) {
            float compression = suspensionLenght - hit.distance;

            float springForce = compression * stiffness;

            float wheelVelocity =
                Vector3.Dot(
                    rb.GetPointVelocity(originPos),
                    transform.up
                );

            float dampingForce = wheelVelocity * springDampening;

            force = springForce - dampingForce;
            Debug.DrawLine(originPos, hit.point);
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
    void ApplySuspensen() {
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
    void ClampVelocityToForwordDiretion() {

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
            thrust += sideVel.magnitude * 0.1f;
            carEffects.SmokeParticles = true;
            isSliding = true;
        }
        else { rb.linearVelocity = forwardVel; isSliding = false; carEffects.SmokeParticles = false; }
    }
    void AddThrust() {
        rb.AddForce(transform.forward * boost);
        thrust -= 5;
    }
    void Awake() {
        thrust = maxThrust;
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = new Vector3(0, -0.5f, 0);
        carEffects = GetComponent<CarEffects>();
    }
  
}