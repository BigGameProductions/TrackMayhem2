using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;
using TMPro;
using System;

public class MainButtonLogic : MonoBehaviour
{

    public void PlayEvent()
    {
        if (PublicData.currentSelectedEventIndex == 0)
        {
            SceneManager.LoadScene("HundredMeter");
        }
        else if (PublicData.currentSelectedEventIndex == 1)
        {
            SceneManager.LoadScene("LongJump");
        } else if (PublicData.currentSelectedEventIndex == 2)
        {
            SceneManager.LoadScene("PoleVault");
        }
        else if (PublicData.currentSelectedEventIndex == 3)
        {
            SceneManager.LoadScene("Shotput");
        }
        else if (PublicData.currentSelectedEventIndex == 4)
        {
            SceneManager.LoadScene("Javelin");
        }
        else if (PublicData.currentSelectedEventIndex == 5)
        {
            SceneManager.LoadScene("FourHundred");
        }
        else if (PublicData.currentSelectedEventIndex == 6)
        {
            SceneManager.LoadScene("Discus");
        }
        else if (PublicData.currentSelectedEventIndex == 7)
        {
            SceneManager.LoadScene("HighJump");
        }
        else if (PublicData.currentSelectedEventIndex == 8)
        {
            SceneManager.LoadScene("Hurdles");
        }
        else if (PublicData.currentSelectedEventIndex == 9)
        {
            SceneManager.LoadScene("FifteenHundred");
        }
        else if (PublicData.currentSelectedEventIndex == 10)
        {
            SceneManager.LoadScene("Decathalon");
        }
    }

    public void showTutorial()
    {
        SceneManager.LoadScene("Tutorial");
    }


    public void runnerMenu()
    {
        SceneManager.LoadScene("RunnersMenu");
    }

    public void mainMenu()
    {
        if (SceneManager.GetActiveScene().name == "EndScreen" && PublicData.inDec)
        {
            PublicData.currentEventDec++;
            SceneManager.LoadScene("Decathalon");
        } else
        {
            PublicData.runnerMenuPosition = 0;
            SceneManager.LoadScene("MainScreen");
        }
    }

    public void leaderboardDisplay()
    {
        SceneManager.LoadScene("Leaderboard");
    }

    public void showProfile()
    {
        SceneManager.LoadScene("MakeCharacter");
    }

    public void selectRunner()
    {
        PublicData.currentRunnerUsing = PublicData.currentRunnerOn;
        PublicData.runnerMenuPosition = 0;
        SceneManager.LoadScene("MainScreen");
    }

    public void showRunnerInfo()
    {
        foreach (Transform tf in UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.transform) //find the name object
        {
            if (tf.name == "Name")
            {
                for (int i = 0; i < PublicData.charactersInfo.Count; i++) //mataches name with characters.csv 
                {
                    if (i != 0)
                    {
                        if (PublicData.charactersInfo.ElementAt(i)[1] == tf.GetComponent<TextMeshProUGUI>().text) //is they are the same then set current runner to that
                        {
                            PublicData.currentRunnerOn = Int32.Parse(PublicData.charactersInfo.ElementAt(i)[0]); //sets current runner to that
                        }
                    }
                }
            }
        }
        PublicData.runnerMenuPosition = transform.parent.parent.transform.localPosition.y;
        SceneManager.LoadScene("RunnerInfo");
    }

    public void setCurrentEvent(int index)
    {
        PublicData.currentSelectedEventIndex = index;
        if (PublicData.recordsInfo.ElementAt(index+1)[3] == "FALSE")
        {
            PublicData.usesTime = true;
        } else
        {
            PublicData.usesTime = false;
        }
        SceneManager.LoadScene("MainScreen");
    }

    public void selectEvent()
    {
        SceneManager.LoadScene("EventSelection");
    }

    public void openChest(int slot)
    {
        PublicData.currentBoxOpening = PublicData.gameData.chestSlots[slot].chestID;
        PublicData.gameData.chestSlots[slot] = null;
        SceneManager.LoadScene("ChestOpening");
    }
    
}
