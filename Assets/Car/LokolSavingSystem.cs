using BoltsTools;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;


public class LokolSavingSystem : MonoBehaviour
{
    public GostSaveFile gostSaveFile;
    float recapTimer = 0;
    Transform car;
    Vector3 oldPos = Vector3.zero;
    public static LokolSavingSystem instens;
    float bestTime = 0;
    public float time = 0;

    

    private void Start() {
        car = GM.carPonters[0].rb.transform;
        oldPos = car.position;

        gostSaveFile = new GostSaveFile();
        gostSaveFile.posisens.Add(oldPos);

        BoltsSave.Initialize();
        instens = this;
        bestTime = BoltsSave.GetFloat("Best time " + SceneManager.GetActiveScene().name);
        if (bestTime == -1) bestTime = Mathf.Infinity;
    }
    public void OnFinish() {
        if (bestTime > time) {
            BoltsSave.SaveFloatValue("Best time " + SceneManager.GetActiveScene().name, time);
            BoltsSave.SaveClassVariable("Gost data " + SceneManager.GetActiveScene().name, gostSaveFile);
            Debug.Log("New rekord! " + bestTime + " to " + time);
        }
    }
    void Update() {
        time += Time.deltaTime;

        recapTimer += Time.deltaTime;
        float DistensTillNextSave = 0.5f * Mathf.Max(0.5f, GM.carPonters[0].rb.linearVelocity.magnitude);
        if (Vector3.Distance(gostSaveFile.posisens.Last(), car.position) > DistensTillNextSave)
        {
            gostSaveFile.posisens.Add(car.position);
            gostSaveFile.timeBetinRecordedPosisens.Add(recapTimer);
            recapTimer = 0;
        }
    }
}
[System.Serializable]
public class GostSaveFile {
    public List<float> timeBetinRecordedPosisens = new();
    public List<Vector3> posisens = new(); 
}
