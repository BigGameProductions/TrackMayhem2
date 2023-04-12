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
        SceneManager.LoadScene("LongJump");
    }

    public void runnerMenu()
    {
        SceneManager.LoadScene("RunnersMenu");
    }

    public void mainMenu()
    {
        SceneManager.LoadScene("MainScreen");
    }

    public void showRunnerInfo()
    {
       foreach (Transform tf in UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.transform) //find the name object
        {
            if (tf.name == "Name")
            {
                for (int i=0; i<PublicData.charactersInfo.Count; i++) //mataches name with characters.csv 
                {
                    if (i!=0)
                    {
                        if (PublicData.charactersInfo.ElementAt(i)[1] == tf.GetComponent<TextMeshProUGUI>().text) //is they are the same then set current runner to that
                        {
                            PublicData.currentRunnerOn = Int32.Parse(PublicData.charactersInfo.ElementAt(i)[0]); //sets current runner to that
                        }
                    }
                }
            }
        }
        SceneManager.LoadScene("RunnerInfo");
    }

    public void selectRunner()
    {
        PublicData.currentRunnerUsing = PublicData.currentRunnerOn;
        SceneManager.LoadScene("MainScreen");
    }

    public void setCurrentEvent(int index)
    {
        PublicData.currentSelectedEventIndex = index;
        SceneManager.LoadScene("MainScreen");
    }

    public void selectEvent()
    {
        SceneManager.LoadScene("EventSelection");
    }
    
}
