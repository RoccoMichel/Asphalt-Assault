using BoltsTools;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class CircuitButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public LevelManager levelManager;
    public int index;
    
    GameObject Trofys;
    public void SelectCircuit()
    {
        levelManager.LoadCircuit(index);
    }

    public void OnPointerExit(PointerEventData eventData) { destroyTrofy(); }

    public void OnPointerEnter(PointerEventData eventData) { DisplayTrof(index); }

    public void destroyTrofy() { Destroy(Trofys); }
    public void DisplayTrof(int i) {
        Debug.Log("Spont trofy");
        float _betsTime = BoltsSave.LoadClass<SaveFile>("Save data " + SceneManager.GetActiveScene().name).bestTimes[0]; // Verey inafisent neds update evesely
        
        if (_betsTime == -1) _betsTime = Mathf.Infinity;
        float[] _times = levelManager.LevTtofys[i];
        int _bestTrofy = -1;

        for (int j = 0; j < _times.Length; j++)
            if (levelManager.LevTtofys[i][j] > _betsTime)
                _bestTrofy = j;

        if (_bestTrofy != -1) {
            Trofys = Instantiate(levelManager.TrofysPrefabs[_bestTrofy]).gameObject;
            Trofys.transform.position = Camera.main.ScreenToWorldPoint(transform.position) + new Vector3(1.7f, 0.1f, 5);
        }
    }
    void Update()
    {
        if (!Trofys.IsUnityNull())
            Trofys.transform.rotation *= Quaternion.Euler(0, 90 * Time.deltaTime, 0);
    }
}
