using UnityEngine;

public class CheckpointController : MonoBehaviour
{
    public int id;
    public bool hasId;

    void SetID()
    {
        if (hasId) return;

        int allCheckpoint = FindObjectsByType<CheckpointController>(sortMode: FindObjectsSortMode.None).Length;

        id = allCheckpoint;

        gameObject.name = $"Checkpoint: {id}";
        
        hasId = true;
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        if (!hasId)
        {
            SetID();
        }
    }
#endif
}