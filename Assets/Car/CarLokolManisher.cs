using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class CarLokolManisher : MonoBehaviour
{
    public Rigidbody rb;
    public CarPhysics physics;
    public TMP_Text LapConter;
    public TMP_Text Timer;
    [SerializeField] private PlayerInputs playerInputs;
    [SerializeField] private GameObject playerUi;
    void Start()
    {
        rb = GetComponentInChildren<Rigidbody>();
        physics = GetComponentInChildren<CarPhysics>();
        
        // Code will nedd to be updated when we add lokol mutiplyer
        GM.carPonters[0] = new();
        GM.carPonters[0].car = gameObject;
        GM.carPonters[0].SetUpcarPonter(rb, physics, LapConter);

        Debug.Log(rb.IsUnityNull() + " is rb null at oriden");
        Debug.Log(physics.IsUnityNull() + " is physiks null at oriden");

        Debug.Log("GameObject: " + gameObject.name);
        Debug.Log("Rigidbody: " + rb);
        Debug.Log("CarPhysics: " + physics);
    }

    public void ShotOf() {
        playerInputs.enabled = false;
        playerUi.active = false;
    } 
    private void Update()
    {
        float time = LokolSavingSystem.instens.time;
        int milSek = Mathf.RoundToInt((time % 1)*100);
        int sek = Mathf.RoundToInt((time - milSek/100)%60);
        int min = Mathf.RoundToInt((time - milSek/100 - sek)/60);
        
        Timer.text =  min + ":" + sek + ":" + milSek;
    }


}
