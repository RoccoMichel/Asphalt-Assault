using BoltsTools;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GostAnimator : MonoBehaviour
{
    public GostSaveFile saveFile = new();
    public int ponter = 1;
    public float recapTimer = 0;
    void Start() {
        saveFile = BoltsSave.LoadClass<GostSaveFile>("Gost data " + SceneManager.GetActiveScene().name); 
        if (saveFile.posisens.Count == 0) gameObject.active = false;
    }

    Vector3 Beziey(Vector3 p1, Vector3 p2, Vector3 p3, float time) {
        //Move p2 otvord sow the gost pases trow p2
        Vector3 closest = p1 + Vector3.Project(p2 - p1, p3 - p1);
        Vector3 offset = p2 - closest;
        p2 = closest + offset * 2; // new walu for p2

        //Cukutat the curePos
        Vector3 P1 = Vector3.Lerp(p1, p2, time);
        Vector3 P2 = Vector3.Lerp(p2, p3, time);
        Vector3 ret = Vector3.Lerp(P1, P2, time);
        Debug.DrawLine(p1, p2);
        Debug.DrawLine(p2, p3);
        Debug.DrawLine(P1, P2);
        return ret;
    }

    void Update() {
        recapTimer += Time.deltaTime;
        float _maxTime = saveFile.timeBetinRecordedPosisens[ponter] + saveFile.timeBetinRecordedPosisens[ponter+1];
        if (Vector3.Distance(transform.position, saveFile.posisens[ponter]) < Time.deltaTime*20 || recapTimer >= _maxTime) {
            ponter += recapTimer >= _maxTime ? 2 : 1;
            recapTimer = 0;
            if (ponter >= saveFile.timeBetinRecordedPosisens.Count-1)
                gameObject.GetComponent<GostAnimator>().enabled = false;
        }


        float _time = recapTimer/_maxTime;
        transform.position = Beziey(saveFile.posisens[ponter - 1], saveFile.posisens[ponter], saveFile.posisens[ponter + 1], _time);
        //transform.position = Vector3.Lerp(saveFile.posisens[ponter - 1], saveFile.posisens[ponter], time);
    }
}
