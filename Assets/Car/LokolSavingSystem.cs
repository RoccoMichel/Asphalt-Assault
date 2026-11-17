using BoltsTools;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;


public class LokolSavingSystem : MonoBehaviour
{
    public static LokolSavingSystem instens;
    public float bestTime = 0;
    public float time = 0;

    private void Start() {
        BoltsSave.Initialize();
        instens = this;
        bestTime = BoltsSave.GetFloat("Best time " + SceneManager.GetActiveScene().name);
        if (bestTime == -1) bestTime = Mathf.Infinity;
    }
    public void OnFinish() {
        if (bestTime > time) {
            BoltsSave.SaveFloatValue("Best time " + SceneManager.GetActiveScene().name, time);
            Debug.Log("New rekord! " + bestTime + " to " + time);
        }
    }
    void Update() {
        time += Time.deltaTime;
    }
}
