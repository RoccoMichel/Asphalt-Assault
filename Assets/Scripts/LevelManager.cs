using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Splines;
using System.Collections;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    private int activeLevel;
    [SerializeField] private RectTransform circuitButtonsParent;
    [SerializeField] private CircuitTransition circuitTransition;
    [SerializeField] private LevelReferences[] levels;
    [System.Serializable]
    public struct LevelReferences
    {
        public string circuitName;
        public string sceneName;
        public Sprite sceneGraphic;
        public SplineExtrude circuitSpline;
    }

    private void Start()
    {
        GenerateCircuitButtons();
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
}
