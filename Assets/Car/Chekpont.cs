using System.Collections;
using UnityEngine;

public class Chekpont : MonoBehaviour
{
    public float spiningSped;
    public BoxCollider coll;

    private void OnTriggerEnter(Collider other) {
        if (other.transform.parent.gameObject == GM.carPonters[0].car)
            GM.instance.GolpostInstens.PastChekpont();
    }

    public IEnumerator Kill() {
        coll.enabled = false;
        float sped = 40;
        while (transform.localScale.magnitude > 1) {
            transform.localScale = Vector3.Lerp(transform.localScale, Vector3.zero, Time.deltaTime * sped);
            transform.localRotation *= Quaternion.Euler(0, 0, Time.deltaTime * sped);
            yield return new WaitForEndOfFrame();
        }
        gameObject.SetActive(false);
    }
    public IEnumerator Enabol()
    {
        coll.enabled = true;
        float sped = 40;
        while (transform.localScale.magnitude < 1000)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(1000, 1000, 300), Time.deltaTime * sped);
            transform.localRotation *= Quaternion.Euler(0, 0, Time.deltaTime * sped);
            yield return new WaitForEndOfFrame();
        }
        gameObject.SetActive(true);
    }

    void Update(){
        transform.rotation *= Quaternion.Euler(0, 0, Time.deltaTime*spiningSped);   
    }
}
