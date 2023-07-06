using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class TeamBuilderManager : MonoBehaviour
{
    [SerializeField] private GameObject[] teamMembers;
    [SerializeField] private ItemStorage itemStorage;

    [SerializeField] private GameObject[] chemistryLines;
    [SerializeField] private Vector2[] chemistryLineNodes;

    [SerializeField] TextMeshProUGUI powerDisplay;

    [SerializeField] Color[] colors;

    private int[] nodeMaxPower = new int[]
    {
        1000,800,600,1000,600,600,600,600,800,600,400
    };

    private int[] nodeCurrentPower = new int[11];

    private int totalPowerOfTeam = 0;

    // Start is called before the first frame update
    void Start()
    {
        if (PublicData.selectingCharacter)
        {
            PublicData.selectingCharacter = false;
            if (PublicData.gameData.teamCharacters.ToList().Contains(PublicData.currentRunnerOn))
            {
                PublicData.gameData.teamCharacters[PublicData.gameData.teamCharacters.ToList().IndexOf(PublicData.currentRunnerOn)] = -1;
            }
            PublicData.gameData.teamCharacters[PublicData.currentSelectEvent] = PublicData.currentRunnerOn;
            
        }
        showChemistry();
        showTeam();
        powerDisplay.text = totalPowerOfTeam.ToString();
    }

    private void showTeam()
    {
        for (int j = 0; j< teamMembers.Length;j++)
        {
            for (int i = 0; i < teamMembers[j].GetComponentsInChildren<Transform>().Length; i++)
            {
                if (teamMembers[PublicData.currentSelectEvent].GetComponentsInChildren<Transform>()[i].name == "RunnerImage")
                {
                    if (PublicData.gameData.teamCharacters[j] != -1)
                    {
                        teamMembers[j].GetComponentsInChildren<Transform>()[i].GetComponent<Image>().sprite = itemStorage.runnerImages[PublicData.gameData.teamCharacters[j]];
                    }
                    
                }
                if (teamMembers[PublicData.currentSelectEvent].GetComponentsInChildren<Transform>()[i].name == "EventName")
                {
                    if (PublicData.gameData.teamCharacters[j] == -1)
                    {
                        teamMembers[j].GetComponentsInChildren<Transform>()[i].GetComponent<TextMeshProUGUI>().color = Color.black;
                    } else
                    {
                        string[] charevents = PublicData.charactersInfo.ElementAt(PublicData.gameData.teamCharacters[j] + 1)[16].Split(";");
                        if (charevents.ToList().Contains(j.ToString()))
                        {
                            teamMembers[j].GetComponentsInChildren<Transform>()[i].GetComponent<TextMeshProUGUI>().color = colors[0];
                        }
                        else
                        {
                            teamMembers[j].GetComponentsInChildren<Transform>()[i].GetComponent<TextMeshProUGUI>().color = colors[1];

                        }
                    }
                }
                if (teamMembers[PublicData.currentSelectEvent].GetComponentsInChildren<Transform>()[i].name == "StatBoosted")
                {
                    string[] charevents = PublicData.charactersInfo.ElementAt(PublicData.gameData.teamCharacters[j] + 1)[16].Split(";");
                    int percentNum = (int)Math.Round(((float)nodeCurrentPower[j] / (float)nodeMaxPower[j]) * 100, 0);
                    if (charevents.ToList().Contains(j.ToString()))
                    {
                        percentNum += 15; //maybe change
                    }
                    if (percentNum >= 75)
                    {
                        teamMembers[j].GetComponentsInChildren<Transform>()[i].GetComponent<TextMeshProUGUI>().color = colors[0];
                    } else if (percentNum >= 50)
                    {
                        teamMembers[j].GetComponentsInChildren<Transform>()[i].GetComponent<TextMeshProUGUI>().color = colors[2];

                    } else
                    {
                        teamMembers[j].GetComponentsInChildren<Transform>()[i].GetComponent<TextMeshProUGUI>().color = colors[1];
                    }
                    teamMembers[j].GetComponentsInChildren<Transform>()[i].GetComponent<TextMeshProUGUI>().text =  percentNum + "%";
                    RunnerInformation runnerInfo = PublicData.getCharactersInfo(PublicData.gameData.teamCharacters[j]);
                    if (PublicData.gameData.teamCharacters[j] != -1)
                    {
                        int powerTotal = runnerInfo.strengthLevel + runnerInfo.agilityLevel + runnerInfo.speedLevel + runnerInfo.flexabilityLevel + 4;
                        totalPowerOfTeam += (int)Math.Round(powerTotal * (percentNum / 100f));
                    } 
                }
            }
        }
        

    }

    private void showChemistry()
    {
        for (int i=0; i<chemistryLines.Length;i++)
        {
            int chemNum = getChemistryNumber((int)chemistryLineNodes[i].x, (int)chemistryLineNodes[i].y);
            chemistryLines[i].GetComponentInChildren<Image>().color = Color.red;
            if (chemNum >=100)
            {
                chemistryLines[i].GetComponentInChildren<Image>().color = Color.yellow;
            }
            if (chemNum >= 200)
            {
                chemistryLines[i].GetComponentInChildren<Image>().color = Color.green;
            }
            if (chemNum >= 300)
            {
                chemistryLines[i].GetComponentInChildren<Image>().color = Color.blue;
            }
        }
    }

    private int getChemistryNumber(int nodeOne, int nodeTwo)
    {
        if (PublicData.gameData.teamCharacters[nodeOne] == -1 || PublicData.gameData.teamCharacters[nodeTwo] == -1)
        {
            return 0;
        }
        string[] nodeOneEvents = PublicData.charactersInfo.ElementAt(PublicData.gameData.teamCharacters[nodeOne] + 1)[16].Split(";");
        string[] nodeTwoEvents = PublicData.charactersInfo.ElementAt(PublicData.gameData.teamCharacters[nodeTwo] + 1)[16].Split(";");
        int currentNum = 0;
        if (PublicData.charactersInfo.ElementAt(PublicData.gameData.teamCharacters[nodeOne]+1)[14] == PublicData.charactersInfo.ElementAt(PublicData.gameData.teamCharacters[nodeTwo]+1)[14])
        {
            currentNum += 100;
        }
        if (PublicData.charactersInfo.ElementAt(PublicData.gameData.teamCharacters[nodeOne]+1)[15] == PublicData.charactersInfo.ElementAt(PublicData.gameData.teamCharacters[nodeTwo]+1)[15])
        {
            currentNum += 100;
        }
        if (shareEvents(nodeOneEvents,nodeTwoEvents))
        {
            currentNum += 100;
        }
        nodeCurrentPower[nodeOne] += currentNum;
        nodeCurrentPower[nodeTwo] += currentNum;
        return currentNum;
    }

    private bool shareEvents(string[] nodeOne, string[] nodeTwo)
    {
        foreach (string str in nodeOne)
        {
            foreach (string strTwo in nodeTwo)
            {
                if (strTwo == str)
                {
                    return true;
                }
            }
        }
        return false;
    }
}
