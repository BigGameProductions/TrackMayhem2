using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DecathalonManager : MonoBehaviour
{

    [SerializeField] private Canvas eventCanvas;
    [SerializeField] private LeaderboardManager leaderboardManager;

    private string[] eventOrder = new string[]
    {
        "HundredMeter",
        "LongJump",
        "Shotput",
        "HighJump",
        "FourHundred",
        "Hurdles",
        "Discus",
        "PoleVault",
        "Javelin",
        "HundredMeter"
    };

    // Start is called before the first frame update
    void Start()
    {
        PublicData.inDec = true;
        if (PublicData.currentEventDec != -1 && PublicData.currentEventDec != 10)
        {
            if (PublicData.currentEventDec == 0 || PublicData.currentEventDec == 4 || PublicData.currentEventDec == 5 || PublicData.currentEventDec == 9)
            {
                PublicData.usesTime = true;
            } else
            {
                PublicData.usesTime = false;
            }
            SceneManager.LoadScene(eventOrder[PublicData.currentEventDec]);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!leaderboardManager.cinematicCamera.gameObject.activeInHierarchy && PublicData.currentEventDec == -1)
        {
            PublicData.currentEventDec = 0;
            SceneManager.LoadScene(eventOrder[PublicData.currentEventDec]);
        }
    }
}
