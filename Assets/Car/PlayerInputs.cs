using UnityEngine;

public class PlayerInputs : MonoBehaviour
{
    CarFysiks fysiks;
    CarQaletyOflife QaletyOflife;

    private void Awake() {
        fysiks = GetComponentInChildren<CarFysiks>();
        QaletyOflife = GetComponentInChildren<CarQaletyOflife>();
    }
    void FixedUpdate() {
        fysiks.PassInInputs(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"), Input.GetKey(KeyCode.Space));
    }
}
