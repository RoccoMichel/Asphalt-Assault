using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CarEfekts : MonoBehaviour
{

    public TextMeshProUGUI text;
    float km;
    bool smokePartikals = new(), figerPartikals = new();
    Rigidbody rb;
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
    [HideInInspector] public int Damnitsh { 
        get { return Damnitsh; }
    }
    CarFysiks fysiks;
    public List<ParticleSystem> smokeParticals = new List<ParticleSystem>();
    public ParticleSystem Bost, HitPartikalsLev1, HitPartikalsLev2;

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
    public void PlayHitParticals(Vector3 pos) {
        HitPartikalsLev1.transform.position = pos;
        HitPartikalsLev1.Play();
    }
    public void PlayHitParticalsLev2(Vector3 pos, Vector3 norm)
    {
        HitPartikalsLev2.transform.position = pos;
        HitPartikalsLev2.transform.forward = norm;
        HitPartikalsLev2.Play();
    }
    private void Awake() {
        rb = GetComponent<Rigidbody>();
        fysiks = GetComponent<CarFysiks>();
        for (int i = 0; i < fysiks.wheels.Length; i++) {
            if (i > 1) {
                try { smokeParticals.Add(fysiks.wheels[i].gameObject.GetComponentInChildren<ParticleSystem>()); }
                catch { Debug.LogWarning("Will dusnet contan a parical system"); }
            }

        }
    }
    void Update() {
        km = Mathf.Round(rb.linearVelocity.magnitude*2);
        text.text = km + " km/h";
    }
}