using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputs : MonoBehaviour
{
    CarPhysics _physics;
    CarQaletyOflife QaletyOflife;
    bool rotateInAir;
    bool boost;
    bool tryBoost;

    float acceleration;
    float steer;
    
    private void Awake() {
        _physics = GetComponentInChildren<CarPhysics>();
        QaletyOflife = GetComponentInChildren<CarQaletyOflife>();
    }
    void Update()
    {
        if (Mathf.Abs(acceleration) > 0) rotateInAir = true;
        if (_physics.isGrounded) rotateInAir = false;

        if(tryBoost) boost = true;
        if (!tryBoost || _physics.thrust <= 0) boost = false;
    } 
    void FixedUpdate() {

        _physics.PassInInputs(steer, acceleration, boost);
        QaletyOflife.PassInInputs(acceleration, rotateInAir);
    }

    public void Acceleration(InputAction.CallbackContext ctx)
    {
        acceleration = ctx.ReadValue<float>();
    }

    public void Steer(InputAction.CallbackContext ctx)
    {
        steer = ctx.ReadValue<float>();
    }

    public void Boost(InputAction.CallbackContext ctx)
    {
        tryBoost = ctx.performed || ctx.started;
    }
}
