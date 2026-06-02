using System.Collections.Generic;
using UnityEngine;

public class CarEfekts : MonoBehaviour
{
    bool smokePartikals = new(), figerPartikals = new();
    [HideInInspector] public bool SmokePartikals { get => smokePartikals; 
        set {
            smokePartikals = value;
            SetSmokeParticalsState(value);
        } 
    }
    [HideInInspector] public bool FigerPartikals { get => figerPartikals; 
        set {
            figerPartikals = value;
            SetFigerParkikalsState(value); 
        } 
    }

    CarFysiks fysiks;
    public List<ParticleSystem> smokeParticals = new List<ParticleSystem>();
    public ParticleSystem Bost;
    void SetFigerParkikalsState(bool aktiv) {
        if (aktiv)
        { if (!Bost.isPlaying) Bost.Play(); }
        else if (Bost.isPlaying) Bost.Stop();
    }
    void SetSmokeParticalsState(bool aktiv) {
        if (aktiv && fysiks.rb.linearVelocity.magnitude / 20f > 0.5f)
            for (int i = 0; i < smokeParticals.Count; i++) {
                smokeParticals[i].transform.localScale = Vector3.one * Mathf.Clamp(fysiks.rb.linearVelocity.magnitude / 20f, 0, 1.5f);
                if (!smokeParticals[i].isPlaying)
                    smokeParticals[i].Play();
            }
        else
            for (int i = 0; i < smokeParticals.Count; i++)
                if (smokeParticals[i].isPlaying)
                    smokeParticals[i].Stop();
    }
    private void Awake() {
        fysiks = GetComponent<CarFysiks>();
        for (int i = 0; i < fysiks.wheels.Length; i++) {
            if (i > 1) {
                try { smokeParticals.Add(fysiks.wheels[i].gameObject.GetComponentInChildren<ParticleSystem>()); }
                catch { Debug.LogWarning("Will dusnet contan a parical system"); }
            }

        }
    }
}
