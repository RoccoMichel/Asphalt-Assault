using BoltsTools;
using System.Collections.Generic;
using UnityEngine;

public class Destruktibol : MonoBehaviour
{
    public float sturtynes;
    [BoltsComment(@"Fore the code to work
1) have tow coloder on the obj whan triget on top and whan diaktivated on the botom
2) Have a partikal sysem on the botom of the child list
3) Inclod a colider and rb (bofe diaktivated) on evry physisk pise of the ovj")]
    List<Rigidbody> rbs = new();
    List<Collider> colliders = new();
    Collider[] col;
    ParticleSystem Destruksen;

    void Awake() {
        col = GetComponents<Collider>();
        Destruksen = GetComponentInChildren<ParticleSystem>();
        for (int i = 0; i < transform.childCount-1; i++) 
            if (transform.GetChild(i).GetComponent<Rigidbody>() != null)
            {
                rbs.Add(transform.GetChild(i).GetComponent<Rigidbody>());
                colliders.Add(transform.GetChild(i).GetComponent<Collider>());
            }
        
    }

    float CulkulateKenetikEnery(Rigidbody rb) {
        Vector3 dir = (transform.position - rb.position).normalized;
        float vel = Vector3.Dot(rb.linearVelocity.normalized, dir) * rb.linearVelocity.magnitude;

        return rb.mass * (vel*vel)/2;
    }

    bool isAllSliping() {
        bool isSliping = true;
        foreach (Rigidbody rb in rbs) 
            if (!rb.IsSleeping()) isSliping = false;

        return isSliping;
    }
    private void OnTriggerEnter(Collider other)
    {
        float enery = CulkulateKenetikEnery(other.gameObject.GetComponent<Rigidbody>());
        if (sturtynes < enery)
        {
            Destruksen.Play();
            foreach (var collider in col)
            { collider.enabled = false; }

            for (int i = 0; i < rbs.Count; i++)
            {
                int index = i;
                colliders[index].enabled = true;
                rbs[index].isKinematic = false;
                Vector3 forse = -(other.transform.position - transform.position).normalized * enery;
                rbs[index].AddForce(forse * Random.Range(0.9f, 1.1f) * 0.5f);

                StartCoroutine(BoltsTimer.WaitFor(() => rbs[index].IsSleeping(), () =>
                {
                    rbs[index].isKinematic = true;
                    colliders[index].enabled = false;
                }));
            }

            StartCoroutine(BoltsTimer.WaitFor(() => isAllSliping(), () => { DestroyImmediate(gameObject); }));
        }
        else {
            col[1].enabled = true;
        }
    }

    void OnTriggerExit(Collider other) {
        col[1].enabled = false;
    }

}
