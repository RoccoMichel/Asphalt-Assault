using BoltsTools;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class CarPonter : MonoBehaviour {
    public GameObject car = new();
    public int index = 0;
    int _lapConter;
    public Rigidbody rb = new();
    CarPhysics CarPhysics = new();
    public TMP_Text lapText;
    public int lapConter
    {
        get { return _lapConter; }
        set
        {
            _lapConter = value;
            lapText.text = _lapConter + "/" + GM.totiolLaps;
            if (_lapConter == GM.totiolLaps)
            {
                GM.instance.CarFinsist(index);

            }
        }
    }

    
    // Sets up evryfing neded for the car ponter to work
    public void SetUpcarPonter(Rigidbody _rb, CarPhysics _CarPhysics, TMP_Text _lapText) {
        rb = _rb;
        CarPhysics = _CarPhysics;
        lapText = _lapText;
        lapConter = 0;
    }
    
    // Alows obj to bost the car forewords
    public void BostCar(float bost) {
        rb.AddForce(rb.transform.forward * bost);
    }

    // Adds bost to the car
    public void IncreseBosatomiter(float amount) {
        CarPhysics.thrust += amount;
    }
}
public class GM : MonoBehaviour {
    public static GM instance;
    public Golpost GolpostInstens;
    public static CarPonter[] carPonters = new CarPonter[1];
    public static int totiolLaps = 3;
    public int TotiolLaps;

    public static SaveFile LevelSaveFile;
    private void Awake() {
        LevelSaveFile = BoltsSave.LoadClass<SaveFile>("Save data " + SceneManager.GetActiveScene().name);
        instance = this;
        totiolLaps = TotiolLaps;
        carPonters[0].lapConter = 0;
    }
   
    
    // Runs when a car finst all laps
    public void CarFinsist(int index) {
        LokolSavingSystem.instens.OnFinish();//Save the new best time if you biten the orld rekord
    }

}
