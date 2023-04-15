using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI currentRunnerPoints;
    [SerializeField] private TextMeshProUGUI tempEventName;
    [SerializeField] private ItemStorage itemStorage;
    [SerializeField] private GameObject player;

    [SerializeField] private GameObject currencyLabels;

    [SerializeField] private GameObject[] chestSlots;

    // Start is called before the first frame update
    void Start()
    {
        itemStorage.initRunner(PublicData.currentRunnerUsing, player.transform);
        currentRunnerPoints.text = PublicData.gameData.allRunners.ElementAt(indexOfCurrentRunner()).points.ToString();
        tempEventName.text = PublicData.recordsInfo.ElementAt(PublicData.currentSelectedEventIndex + 1)[0];
        PublicData.gameData.chestSlots[0] = new ChestInfo(1, true);
        PublicData.gameData.chestSlots[1] = new ChestInfo(2, true);
        setChests();
        updateCurrency();
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

    private void updateCurrency()
    {
        foreach (Transform tf in currencyLabels.transform)
        {
            if (tf.name == "Tokens")
            {
                tf.GetComponent<TextMeshProUGUI>().text = PublicData.gameData.tokens.ToString();
            }
            if (tf.name == "TrainingCards")
            {
                tf.GetComponent<TextMeshProUGUI>().text = PublicData.gameData.trainingCards.ToString();
            }
        }
    }

    private void setChests() //sets the image of chests
    {
        for (int i=0; i<4; i++) //loops through the chest images
        {
            if (PublicData.gameData.chestSlots[i] != null) //makes sure there is a chest to find
            {
                if (!PublicData.gameData.chestSlots[i].placeHolder) //checks if it is a placeholde 
                {
                    chestSlots[i].GetComponentsInChildren<RectTransform>(true)[1].gameObject.SetActive(true); //shows chest if there is one
                }
            }
            
        }
    }


}
