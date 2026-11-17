using DG.Tweening;
using UnityEngine;

public class Golpost : MonoBehaviour
{
    public Chekpont[] chekponts;
    public int ponter;
    // Aktivates when a player pases a chekpont
    public void PastChekpont() {
        StartCoroutine(chekponts[ponter].Kill());// Desabol the old chekpont
        
        if (ponter++ >= chekponts.Length - 1) { // Lope the ponter after a lap
            ponter = 0;
            GM.carPonters[0].lapConter++;
            LokolSavingSystem.instens.OnFinish();//Save the new best time if you biten the orld rekord
        }

        StartCoroutine(chekponts[ponter].Enabol());// Aktivats the next chekpont

        GM.carPonters[0].BostCar(1000);
        GM.carPonters[0].IncreseBosatomiter(30);
    }

    
    private void Start() {
        ponter = 0;
        int num = transform.childCount;
        chekponts = new Chekpont[num];
        for (int i = 0; i < num; i++)
            chekponts[i] = transform.GetChild(i).GetComponent<Chekpont>();
    }
    void Update()
    {
        
    }
}
