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
    [SerializeField] private Slider pointSlider;
    [SerializeField] private Image rankImage;
    [SerializeField] private TextMeshProUGUI rankText;
    [SerializeField] private TextMeshProUGUI tempEventName;
    [SerializeField] private ItemStorage itemStorage;
    [SerializeField] private GameObject player;

    [SerializeField] private GameObject currencyLabels;

    [SerializeField] private GameObject[] chestSlots;

    [SerializeField] private Color[] rankColors;

    [SerializeField] private Material[] chestMats;

    [SerializeField] private Image playerFlag;
    [SerializeField] private TextMeshProUGUI playerName;

    private LeaderboardFunctions leadF = new LeaderboardFunctions();


    private int[] rankList = new int[] //list of all rank amounts
    {
        10,30,50,100,200,350,500,700,900,1100,1300,1500,2000,2500
    };

    // Start is called before the first frame update
    void Start()
    {
        PublicData.fromShop = false;
        //PublicData.gameData.chestSlots = new ChestInfo[4];
        //PublicData.gameData.chestSlots[0] = new ChestInfo(4, true);
        //making profile button appear
        //make chests hide is not real
        //temp
        PublicData.gameData.allRunners[0].unlocked = true;
        PublicData.gameData.allRunners[0].unlocked = true;
        //temp



        for (int i=0; i<PublicData.gameData.chestSlots.Length; i++)
        {
            if (PublicData.gameData.chestSlots[i] != null)
            {
                if (!PublicData.gameData.chestSlots[i].placeHolder)
                {
                    PublicData.gameData.chestSlots[i] = null;
                }
            }
            
        }
        //make chests hide if not real
        playerFlag.sprite = itemStorage.flags[itemStorage.findFlagIndexOfCountry(PublicData.gameData.countryCode)];
        playerName.text = PublicData.gameData.playerName;
        //making profile button appear
        itemStorage.initRunner(PublicData.currentRunnerUsing, player.transform);
        if (PublicData.pointsToGive > 0)
        {
            PublicData.gameData.allRunners.ElementAt(indexOfCurrentRunner()).increasePoints(PublicData.pointsToGive);
        }

        string currentPoints = PublicData.gameData.allRunners.ElementAt(indexOfCurrentRunner()).points.ToString();
        string nextRankAmount = "/";
        int rankIndex = 0;
        for (int i=0; i<rankList.Length; i++)
        {
            if (Int32.Parse(currentPoints) < rankList[i])
            {
                nextRankAmount += rankList[i].ToString();
                rankIndex = i;
                if (i!= 0)
                {
                    if (Int32.Parse(currentPoints) - PublicData.pointsToGive < rankList[i - 1])
                    {
                        PublicData.gameData.trainingCards += (i / 2) + 1;
                    }
                }
                
                break;
            }
        }
        PublicData.pointsToGive = 0;
        currentRunnerPoints.text = currentPoints + nextRankAmount;
        if (Int32.Parse(currentPoints) < 10)
        {
            pointSlider.maxValue = 10;
            pointSlider.value = Int32.Parse(currentPoints);
        } else if (Int32.Parse(currentPoints) > 1500)
        {
            pointSlider.maxValue = 1;
            pointSlider.value = 1;
        } else
        {
            pointSlider.maxValue = rankList[rankIndex] - rankList[rankIndex - 1];
            pointSlider.value = Int32.Parse(currentPoints) - rankList[rankIndex - 1];
        }
        //set rank text
        rankText.text = "Rank " + (rankIndex+1);
        Color rankColor = rankColors[(rankIndex+1)/3]; //TODO fix
        rankColor.a = 1;
        rankImage.color = rankColor;
        //rankImage.color = new Color((byte)255.0 *rankColor.r, (byte)255.0 *rankColor.g, (byte)255.0 *rankColor.b); //copies all properties of color
        tempEventName.text = PublicData.recordsInfo.ElementAt(PublicData.currentSelectedEventIndex + 1)[0];
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
            if (tf.name == "Rubies")
            {
                tf.GetComponent<TextMeshProUGUI>().text = PublicData.gameData.rubies.ToString();
            }
        }
    }

    private void setChests() //sets the image of chests
    {
        for (int i=0; i<4; i++) //loops through the chest images
        {
            if (PublicData.gameData.chestSlots[i] != null) //makes sure there is a chest to find
            {
                chestSlots[i].GetComponentsInChildren<Transform>(true)[1].gameObject.SetActive(true); //shows chest if there is one
                chestMats[i].color = itemStorage.chestColors[PublicData.gameData.chestSlots[i].chestID];
            } else
            {
                chestSlots[i].GetComponentsInChildren<Transform>(true)[1].gameObject.SetActive(false); //shows chest if there is one
            }

        }
    }


}
