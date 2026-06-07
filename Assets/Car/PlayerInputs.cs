using UnityEngine;

public class PlayerInputs : MonoBehaviour
{
    CarFysiks fysiks;
    CarQaletyOflife QaletyOflife;
    bool RotateInAre = false, bost = false;
    private void Awake() {
        fysiks = GetComponentInChildren<CarFysiks>();
        QaletyOflife = GetComponentInChildren<CarQaletyOflife>();
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.S)) RotateInAre = true;
        if (fysiks.isGrounded) RotateInAre = false;

        if (Input.GetKeyDown(KeyCode.Space)) bost = true;
        if (Input.GetKeyUp(KeyCode.Space) || fysiks.trust <= 0) bost = false;
    } 
    void FixedUpdate() {

        fysiks.PassInInputs(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"), bost);
        QaletyOflife.PassInInputs(Input.GetAxisRaw("Vertical"), RotateInAre);
    }
}
