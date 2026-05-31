using UnityEngine;

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
    void FixedUpdate() {
        if (!fysiks.isGrownded) {
            rb.linearDamping = ligerDamp * 0.25f;
            rb.angularDamping = aglerDamp * 0.25f;
        }
        else {
            rb.linearDamping = ligerDamp;
            rb.angularDamping = aglerDamp;
        }
    }
}
