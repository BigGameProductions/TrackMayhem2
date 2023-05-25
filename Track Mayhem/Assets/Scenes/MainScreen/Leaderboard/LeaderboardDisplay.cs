using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using System.Linq;
using UnityEngine.UI;

public class LeaderboardDisplay : MonoBehaviour
{
    private string[][] leaderBoardData;

    //add the rows on the leaderboard
    [SerializeField] GameObject Names;
    [SerializeField] GameObject Countries;
    [SerializeField] GameObject Scores;
    [SerializeField] GameObject Runners;
    [SerializeField] GameObject RunnerIcons;
    [SerializeField] GameObject Dates;

    [SerializeField] TextMeshProUGUI eventTitle;

    [SerializeField] ItemStorage itemStorage;

    [SerializeField] GameObject cover;

    LeaderboardFunctions leadF = new LeaderboardFunctions(); //makes a leaderboard functions class for getting the leadrboard

    int currentEvent = 0;

    // Start is called before the first frame update
    void Start()
    {
        cover.GetComponent<Image>().enabled = true;
        leadF.GetLeaderboard(1);
        currentEvent = 1;
        eventTitle.text = "LongJump";
    }

    // Update is called once per frame
    void Update()
    {
        if (leadF.isUpdate)
        {
            cover.GetComponent<Image>().enabled = false; //TODO make error message when there is no connection //TODO offline records
            leadF.isUpdate = false;
            leaderBoardData = leadF.leaderboardData;
            updateLeaderboard(leaderBoardData);
        }
    }


    public void showEventLeaderboard(int eventCode)
    {
        cover.GetComponent<Image>().enabled = true;
        leadF.GetLeaderboard(eventCode);
        currentEvent = eventCode; 
        eventTitle.text = PublicData.recordsInfo.ElementAt(eventCode + 1)[0]; //sets the current event title
    }

    private void updateLeaderboard(string[][] data) //updates leaderboard witht he current data
    {
        for (int i=0; i<10; i++) //of ten places
        {
            Names.GetComponentsInChildren<TextMeshProUGUI>()[i].text = data[0][i]; //sets the right namexw
            if (data[2][i] != null) 
            {
                Runners.GetComponentsInChildren<TextMeshProUGUI>()[i].text = PublicData.charactersInfo.ElementAt(Int32.Parse(data[2][i].Split(",")[1])+1)[1]; //gets the runner name
                Scores.GetComponentsInChildren<TextMeshProUGUI>()[i].text = getMark(Int32.Parse(data[1][i]), PublicData.recordsInfo.ElementAt(currentEvent + 1)[3] == "FAlSE"); //sets the right mark
                Countries.GetComponentsInChildren<Image>()[i].sprite = itemStorage.flags[itemStorage.findFlagIndexOfCountry(data[2][i].Split(",")[0])]; //sets the flag to the player

            }
            else
            {
                Scores.GetComponentsInChildren<TextMeshProUGUI>()[i].text = "";
                Runners.GetComponentsInChildren<TextMeshProUGUI>()[i].text = "";
                RunnerIcons.GetComponentsInChildren<Image>()[i].sprite = null;
                Countries.GetComponentsInChildren<Image>()[i].sprite = null;

            }
            Dates.GetComponentsInChildren<TextMeshProUGUI>()[i].text = data[3][i]; //sets the right dates
        }
    
    }

    private string getMark(int rawMark, bool isTime) //converts the data to feet 
    {
        float mark = (float)((float)rawMark / 100.00);
        if (isTime)
        {
            return mark.ToString();
        }
        else
        {
            return ((int)mark / 12) + "' " + Math.Round(mark % 12, 2) + "''";
        }
    }
    
}
