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
        Vector3 vel = new Vector3(car.linearVelocity.x, 0, car.linearVelocity.z).normalized;
        transform.position = Vector3.Lerp(transform.position, car.position - prodjectVecOnXZplabe(vel + car.transform.forward*0.5f) * 8 + Vector3.up*3, Time.deltaTime*50);
        lukeat.position = Vector3.Lerp(lukeat.position, car.position + prodjectVecOnXZplabe(vel + car.transform.forward * 0.5f), Time.deltaTime * 50);
        transform.LookAt(lukeat);
    }
}