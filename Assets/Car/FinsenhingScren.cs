using BoltsTools;
using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FinsenhingScren : MonoBehaviour
{
    public Transform marcer;
    public GameObject finiseingTimeCard;
    public GameObject Trofys;
    public Animation animation;

    string timeToTimer(float time) {

        int milSek = Mathf.RoundToInt((time % 1) * 100);
        int sek = Mathf.RoundToInt((time - milSek / 100) % 60);
        int min = Mathf.RoundToInt((time - milSek / 100 - sek) / 60);
        return min + ":" + sek + ":" + milSek;
    }
    void Awake() {

        animation.Play();

        // we carate a sortedd list of teh best time wher the curent time gand is negativ as a marker
        List<float> times = GM.LevelSaveFile.bestTimes.ToList();
        float newTime = LokolSavingSystem.instens.time;
        times.Add(newTime);
        times.Sort();
        times[times.FindIndex(t => Mathf.Abs(t - newTime) <= 0.001f)] = -newTime;
        
        int num = times.Count;

        float ofsetX = 0.25f * Screen.currentResolution.width;
        float ofsetY = 0.05f * Screen.currentResolution.height;
        for (int i = 0; i < num; i++) {
            GameObject card = Instantiate(finiseingTimeCard, marcer.position + Vector3.down*i* ofsetY - Vector3.right* ofsetX, marcer.rotation);
            card.transform.SetParent(finiseingTimeCard.transform.parent);
            card.GetComponentInChildren<TextMeshProUGUI>().text = timeToTimer(Mathf.Abs(times[i]));
            card.transform.DOMoveX(ofsetX, 0.15f).SetDelay(i / 20f);
            if (times[i] < 0)
                card.GetComponent<Image>().enabled = true;
        }
        finiseingTimeCard.SetActive(false);

        for (int i = 0; i < Trofys.transform.childCount; i++)
            if (LevelManager.LevTtofys[0][i] > newTime && (i == Trofys.transform.childCount -1 || LevelManager.LevTtofys[0][i+1] < newTime)) 
                Trofys.transform.GetChild(i).gameObject.SetActive(true);
    }
    public void LodeOpsens(bool maneMenu) {
        if (maneMenu) SceneManager.LoadScene(0);
        else SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

    }
}
