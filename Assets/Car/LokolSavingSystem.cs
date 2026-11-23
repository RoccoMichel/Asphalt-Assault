using BoltsTools;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;


public class LokolSavingSystem : MonoBehaviour
{
    public SaveFile SaveFile;
    public GameObject EndScren;
    float recapTimer = 0;
    Transform car;
    Vector3 oldPos = Vector3.zero;
    public static LokolSavingSystem instens;
    public float bestTime = 0;
    public float time = 0;
    bool timeConts= true;
    
    public float timeBetweenSave = 0.25f;

    bool finished;
    

    private void Start() {
        BoltsSave.Initialize();

        car = GM.carPonters[0].rb.transform;
        oldPos = car.position;

        SaveFile = new SaveFile();
        SaveFile.posisens.Add(oldPos);

        SaveFile.bestTimes = GM.LevelSaveFile.bestTimes;
       
        instens = this;

     
        if (GM.LevelSaveFile.bestTimes.Count == 0) bestTime = Mathf.Infinity;
        else { bestTime = GM.LevelSaveFile.bestTimes[0]; }
    }

    IEnumerator StopRecording() {
        EndScren.SetActive(true);
        GM.LevelSaveFile.bestTimes.Add(time);
        GM.LevelSaveFile.bestTimes.Sort();
        int Leg = GM.LevelSaveFile.bestTimes.Count;
        if (Leg > 30) GM.LevelSaveFile.bestTimes.RemoveAt(Leg-1);
             
        GM.carPonters[0].car.GetComponent<CarLokolManisher>().ShotOf();
      

        yield return new WaitForSeconds(1);

        // overites the gost data if a prives time whose faster
        if (bestTime <= time)
        {
            SaveFile.timeBetinRecordedPosisens = new List<float>(GM.LevelSaveFile.timeBetinRecordedPosisens);
            SaveFile.posisens = new List<Vector3>(GM.LevelSaveFile.posisens);
        }
           
        BoltsSave.SaveClassVariable("Save data " + SceneManager.GetActiveScene().name, SaveFile);

        yield return new WaitForSeconds(5);

        SceneManager.LoadScene(0); // temp
    }
    public void OnFinish() 
    {
        if(finished) return;
        finished = true;
        timeConts = false;
        
        SaveFile.posisens.Add(car.position);
        SaveFile.timeBetinRecordedPosisens.Add(recapTimer);
        
        StartCoroutine(StopRecording());
    }
    void Update() 
    {
        if (timeConts) time += Time.deltaTime;
        if(finished) return;

        //safty
        if (SaveFile.posisens.Count == 0) {
            SaveFile.posisens.Add(oldPos);
        }

        recapTimer += Time.deltaTime;
        float DistensTillNextSave = timeBetweenSave * Mathf.Max(0.5f, GM.carPonters[0].rb.linearVelocity.magnitude);
        if (Vector3.Distance(SaveFile.posisens.Last(), car.position) > DistensTillNextSave)
        {
            SaveFile.posisens.Add(car.position);
            SaveFile.timeBetinRecordedPosisens.Add(recapTimer);
            recapTimer = 0;
        }
    }
}
[System.Serializable]
public class SaveFile {
    public List<float> bestTimes = new();

    public List<float> timeBetinRecordedPosisens = new();
    public List<Vector3> posisens = new(); 
}
