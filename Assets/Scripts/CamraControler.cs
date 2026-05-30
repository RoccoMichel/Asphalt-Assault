using UnityEngine;

public class CamraControler : MonoBehaviour
{
    public Rigidbody car;
    public Transform lukeat;
    Vector3 prodjectVecOnXZplabe(Vector3 vec) {
        return (new Vector3 { 
            x = vec.x,
            y = 0,
            z = vec.z
        }).normalized;
    }
    void Update()
    {
        lukeat.position = Vector3.Lerp(lukeat.position, car.position + car.linearVelocity, Time.deltaTime * 50);
        transform.LookAt(lukeat);

        transform.position = Vector3.Lerp(transform.position, car.position - prodjectVecOnXZplabe(car.linearVelocity.normalized + car.transform.forward*0.5f) * 8 + Vector3.up*3, Time.deltaTime*50);
    }
}