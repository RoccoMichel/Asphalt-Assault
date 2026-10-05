using UnityEngine;

public class PlayerInputs : MonoBehaviour
{
    CarPhysics _physics;
    CarQaletyOflife QaletyOflife;
    bool RotateInAre = false, bost = false;
    private void Awake() {
        _physics = GetComponentInChildren<CarPhysics>();
        QaletyOflife = GetComponentInChildren<CarQaletyOflife>();
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.S)) RotateInAre = true;
        if (_physics.isGrounded) RotateInAre = false;

        if (Input.GetKeyDown(KeyCode.Space)) bost = true;
        if (Input.GetKeyUp(KeyCode.Space) || _physics.thrust <= 0) bost = false;
    } 
    void FixedUpdate() {

        _physics.PassInInputs(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"), bost);
        QaletyOflife.PassInInputs(Input.GetAxisRaw("Vertical"), RotateInAre);
    }
}
