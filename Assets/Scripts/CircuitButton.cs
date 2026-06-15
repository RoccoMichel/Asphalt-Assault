using UnityEngine;

public class CircuitButton : MonoBehaviour
{
    public LevelManager levelManager;
    public int index;

    public void SelectCircuit()
    {
        levelManager.LoadCircuit(index);
    }
}
