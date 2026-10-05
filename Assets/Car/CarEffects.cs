using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CarEffects : MonoBehaviour
{
 
    public TextMeshProUGUI text;
    float km;
    bool _smokeParticles = new(), figerParticle = new();
    Rigidbody rb;
    [HideInInspector] public bool SmokeParticles { get => _smokeParticles; 
        set {
            _smokeParticles = value;
            SetSmokeParticlesState(value);
        } 
    }
    [HideInInspector] public bool FigerParticles { get => figerParticle; 
        set {
            figerParticle = value;
            SetFigerParticlesState(value); 
        } 
    }
    [HideInInspector] public int Damnitsh { 
        get { return Damnitsh; }
    }
    CarPhysics physics; 
    public List<ParticleSystem> smokeParticles = new List<ParticleSystem>();
    public ParticleSystem Boost, HitParticlesLvl1, HitParticlesLvl2;

    void SetFigerParticlesState(bool active) 
    {
        if (active)
        {
            if (!Boost.isPlaying) Boost.Play();
        }
        else if (Boost.isPlaying) Boost.Stop();
    }
    void SetSmokeParticlesState(bool active) 
    {
        if (active && physics.rb.linearVelocity.magnitude / 20f > 0.5f)
            for (int i = 0; i < smokeParticles.Count; i++) 
            {
                smokeParticles[i].transform.localScale = Vector3.one * Mathf.Clamp(physics.rb.linearVelocity.magnitude / 20f, 0, 1.5f);
                if (!smokeParticles[i].isPlaying)
                    smokeParticles[i].Play();
            }
        else
            for (int i = 0; i < smokeParticles.Count; i++)
                if (smokeParticles[i].isPlaying)
                    smokeParticles[i].Stop();
    }
    public void PlayHitParticles(Vector3 pos) 
    {
        if(HitParticlesLvl1 == null)
            return;
        
        HitParticlesLvl1.transform.position = pos;
        HitParticlesLvl1.Play();
    }
    public void PlayHitParticlesLvl2(Vector3 pos, Vector3 norm)
    {
        if(HitParticlesLvl2 == null)
            return;
        
        HitParticlesLvl2.transform.position = pos;
        HitParticlesLvl2.transform.forward = norm;
        HitParticlesLvl2.Play();
    }
    
    private void Awake() 
    {
        rb = GetComponent<Rigidbody>();
        physics = GetComponent<CarPhysics>();
        for (int i = 0; i < physics.wheels.Length; i++) {
            if (i > 1) {
                try { smokeParticles.Add(physics.wheels[i].gameObject.GetComponentInChildren<ParticleSystem>()); }
                catch { Debug.LogWarning("Wheel Doesn't Contain A Particle System"); }
            }

        }
    }
    
    void Update() 
    {
        km = Mathf.Round(rb.linearVelocity.magnitude*2);
        
        if(text != null)
            text.text = km + " km/h";
    }
}