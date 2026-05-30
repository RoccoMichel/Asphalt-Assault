using UnityEngine;

public class CamraControler : MonoBehaviour
{
    public Rigidbody car;
    public Transform lukeat;
    Vector3 prodjectVecOnXZplabe(Vector3 vec) {
        Vector3 vel = new Vector3(
        car.linearVelocity.x,
        0,
        car.linearVelocity.z
        );

        return vel.magnitude > 1f ? vel.normalized : car.transform.forward;
    }
    void LateUpdate()
    {
        Vector3 vel = new Vector3(car.linearVelocity.x, 0, car.linearVelocity.z).normalized;
        transform.position = Vector3.Lerp(transform.position, car.position - prodjectVecOnXZplabe(vel + car.transform.forward*0.5f) * 5 + Vector3.up*3, Time.deltaTime * 12);
        lukeat.position = Vector3.Lerp(lukeat.position, car.position + prodjectVecOnXZplabe(vel + car.transform.forward * 0.5f)*4, Time.deltaTime * 12); 
        transform.LookAt(lukeat);
    }
}