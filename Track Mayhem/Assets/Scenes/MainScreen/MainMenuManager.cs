using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI currentRunnerPoints;
    [SerializeField] private TextMeshProUGUI tempEventName;

    // Start is called before the first frame update
    void Start()
    {
        currentRunnerPoints.text = PublicData.gameData.allRunners.ElementAt(indexOfCurrentRunner()).points.ToString();
        tempEventName.text = PublicData.recordsInfo.ElementAt(PublicData.currentSelectedEventIndex + 1)[0];
    }

    private int indexOfCurrentRunner() //find the current index of the runner in the runner array in game data
    {
        for (int i = 0; i < PublicData.gameData.allRunners.Count; i++) //loops through runners
        {
            if (PublicData.gameData.allRunners.ElementAt(i).runnerId == PublicData.currentRunnerUsing) //checks if id matches
            {
                return i; //returns index
            }
        }
        return 0;
    }


}
