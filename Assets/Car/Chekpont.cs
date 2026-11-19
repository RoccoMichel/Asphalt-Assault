using System.Collections;
using UnityEngine;

public class Chekpont : MonoBehaviour
{
    public float spiningSped;
    public BoxCollider coll;
    public Animation Animation;
    public AnimationClip kill, enabol;

    private void OnTriggerEnter(Collider other) {
        if (other.transform.parent.gameObject == GM.carPonters[0].car)
            GM.instance.GolpostInstens.PastChekpont();
    }

    public IEnumerator Kill() {
        coll.enabled = false;
        float sped = 40;
        Animation.clip = kill;
        Animation.Play();
        yield return new WaitForSeconds(kill.averageDuration);
        gameObject.SetActive(false);
    }
    public IEnumerator Enabol()
    {
        gameObject.SetActive(true);
        coll.enabled = true;
        float sped = 40;
        Animation.clip = enabol;
        Animation.Play();
        yield return new WaitForSeconds(enabol.averageDuration);
    }

    void Update(){
        transform.rotation *= Quaternion.Euler(0, 0, Time.deltaTime*spiningSped);   
    }
}
