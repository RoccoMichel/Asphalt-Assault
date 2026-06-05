using UnityEngine;

public class PlayerInputs : MonoBehaviour
{
    CarFysiks fysiks;
    CarQaletyOflife QaletyOflife;

    private void Awake() {
        fysiks = GetComponentInChildren<CarFysiks>();
        QaletyOflife = GetComponentInChildren<CarQaletyOflife>();
    }
    void Update() {
        
    }
}
