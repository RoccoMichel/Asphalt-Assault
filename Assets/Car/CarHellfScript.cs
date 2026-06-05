using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CarHellfScript : MonoBehaviour
{

    public float hellf;
    public Slider slider;
    float maxHellf, HellfClap01;
    Rigidbody rb;
    CarEfekts CarEfekts;

    void Awake() {
        maxHellf = hellf;
        rb = GetComponent<Rigidbody>();
        CarEfekts = GetComponent<CarEfekts>();
    }
    void LateUpdate() {
        HellfClap01 = hellf / maxHellf;

        if (HellfClap01 < 0)
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // tep def condisen

        slider.value = HellfClap01;
    }

    void OnCollisionEnter(Collision collision)
    {
        float Gforse = collision.impulse.magnitude / 10;
        if (Gforse > 10)
        {
            hellf -= Gforse;
            if (Gforse > 15)
                CarEfekts.PlayHitParticals(collision.contacts[0].point);
            else
                CarEfekts.PlayHitParticalsLev2(collision.contacts[0].point, collision.contacts[0].normal);
        }


    }

}
