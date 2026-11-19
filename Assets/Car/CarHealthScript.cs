using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CarHealthScript : MonoBehaviour 
{

    public float health = 100;
    public Slider slider;
    float maxHealth, HealthClap01;
    Rigidbody rb;
    CarEffects _carEffects;

    void Awake() {
        maxHealth = health;
        rb = GetComponent<Rigidbody>();
        _carEffects = GetComponent<CarEffects>();
    }
    void LateUpdate() {
        HealthClap01 = health / maxHealth;

        if (HealthClap01 < 0)
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // tep def condisen

        slider.value = HealthClap01;
    }

    void OnCollisionEnter(Collision collision)
    {
        float Gforce = collision.impulse.magnitude / 10;
        if (Gforce > 5)
        {
            health -= Gforce;
            if (Gforce > 15)
                _carEffects.PlayHitParticles(collision.contacts[0].point);
            else
                _carEffects.PlayHitParticlesLvl2(collision.contacts[0].point, collision.contacts[0].normal);
        }


    }

 

}
