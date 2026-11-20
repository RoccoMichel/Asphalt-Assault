using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Splines;
using System.Collections;
using UnityEngine.SceneManagement;
using BoltsTools;
using System.Collections.Generic;
using Unity.VisualScripting;

public class LevelManager : MonoBehaviour
{

    public int activeLevel;
    float betsTime = Mathf.Infinity;
    [SerializeField] private RectTransform circuitButtonsParent;
    [SerializeField] private CircuitTransition circuitTransition;
    [SerializeField] public LevelReferences[] levels;
    [SerializeField] private TMP_Text BetsTimeOnLevel;
    [SerializeField] public int nuberOfLevels;
    [SerializeField] public List<GameObject> TrofysPrefabs;


    [System.Serializable]
    public struct LevelReferences
    {
        public string circuitName;
        public string sceneName;
        public Sprite sceneGraphic;
        public SplineExtrude circuitSpline;

    }

    // the rekierd times for etsh trofy
    public Dictionary<int, float[]> LevTtofys = new() {
        // {index, {brons, silver, gold, dimend}}
        {0, new float[]{60*2, 60+45, 60+30, 60+23.8f} },
        {1, new float[]{60*2, 60+45, 60+30, 60+20} }
    };
    private void Start()
    {
        GenerateCircuitButtons();
        UppdateTime();
    }

   
    void UppdateTime(){
        // Fetshens the bets time on the given level
        betsTime = BoltsSave.LoadClass<SaveFile>("Save data " + SceneManager.GetActiveScene().name).bestTimes[0];
        if (betsTime == -1) betsTime = Mathf.Infinity; // Sets the time to infinety if the havent bin 

        if (betsTime != Mathf.Infinity)
        {
            int milSek = Mathf.RoundToInt((betsTime % 1) * 100);
            int sek = Mathf.RoundToInt((betsTime - milSek / 100) % 60);
            int min = Mathf.RoundToInt((betsTime - milSek / 100 - sek) / 60);

            BetsTimeOnLevel.text = min + ":" + sek + ":" + milSek;
        }
        else
            BetsTimeOnLevel.text = "No recorded time";
    }

    public void GenerateCircuitButtons()
    {
        // Destroy ze old Buttons ja
        Button[] oldButtons = circuitButtonsParent.GetComponentsInChildren<Button>();
        foreach (Button go in oldButtons) Destroy(go.gameObject);

        // Instantiate ze new ones
        for (int i = 0; i < levels.Length; i++)
        {
            GameObject newButton = Instantiate((GameObject)Resources.Load($"UI/CircuitButton"), circuitButtonsParent);

            newButton.GetComponentInChildren<TMP_Text>().text = levels[i].circuitName;
            newButton.name = i.ToString() + " | " + levels[i].circuitName;
            newButton.GetComponent<CircuitButton>().levelManager = this;
            newButton.GetComponent<CircuitButton>().index = i;
        }
    }
    public void LoadCircuit(int index)
    {
        circuitTransition.LoadCircuit(levels[activeLevel].circuitSpline, levels[index].circuitSpline);
        if (activeLevel != index) activeLevel = index;
        UppdateTime();
    }

    public void StartButton()
    {
        SceneManager.LoadScene(levels[activeLevel].sceneName);
    }

    public void SetObjectActive(GameObject go)
    {
        go.SetActive(true);
    }
    public void SetObjectInactive(GameObject go)
    {
        go.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R)) BoltsSave.ResetSave(); // Sesets Save

     
    }
}
