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


    int currentEvent = 0;

    // Start is called before the first frame update
    void Start()
    {
        //cover.GetComponent<Image>().enabled = true;
        currentEvent = 1;
        eventTitle.text = "LongJump";
        leaderBoardData = PublicData.gameData.leaderboardList[currentEvent];
        updateLeaderboard(leaderBoardData);
    }

    // Update is called once per frame
    void Update()
    {
        /*if (leadF.isUpdate)
        {
            cover.GetComponent<Image>().enabled = false; //TODO make error message when there is no connection //TODO offline records
            leadF.isUpdate = false;
            leaderBoardData = leadF.leaderboardData;
            updateLeaderboard(leaderBoardData);
        }*/
    }


    public void showEventLeaderboard(int eventCode)
    {
        //cover.GetComponent<Image>().enabled = true;
        leaderBoardData = PublicData.gameData.leaderboardList[eventCode];
        currentEvent = eventCode; 
        eventTitle.text = PublicData.recordsInfo.ElementAt(eventCode + 1)[0]; //sets the current event title
        updateLeaderboard(leaderBoardData);
    }

    private void updateLeaderboard(string[][] data) //updates leaderboard witht he current data
    {
        for (int i=0; i<10; i++) //of ten places
        {
            Names.GetComponentsInChildren<TextMeshProUGUI>()[i].text = data[0][i]; //sets the right name
            if (data[2][i] != null && data[2][i] != "") 
            {
                Runners.GetComponentsInChildren<TextMeshProUGUI>()[i].text = PublicData.charactersInfo.ElementAt(Int32.Parse(data[2][i].Split(",")[1])+1)[1]; //gets the runner name
                Scores.GetComponentsInChildren<TextMeshProUGUI>()[i].text = getMark(Int32.Parse(data[1][i]), PublicData.recordsInfo.ElementAt(currentEvent + 1)[3] == "FALSE", currentEvent == 10); //sets the right mark
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

    private string fillZeros(string item, bool hasMin)
    {
        if (item.Contains("."))
        {
            if (hasMin && item.Split(".")[0].Length == 1)
            {
                item = "0" + item;
            }
            if (item.Split(".")[1].Length != 2)
            {
                return item + "0";
            }
            else
            {
                return item;
            }
        }
        else
        {
            if (hasMin && item.Length == 1)
            {
                item = "0" + item;
            }
            return item + ".00";
        }
    }

    private string getMark(float mark, bool asTime = false, bool dec=false)
    {
        if (asTime)
        {
            mark = Math.Abs(mark - PublicData.maxInteger) /100;
        } else
        {
            mark /= 100;
        }
        if (dec)
        {
            return Math.Round(mark, 0).ToString();
        }
        if (mark == -10000)
        {
            return "X";
        }
        else if (mark == -100)
        {
            return "-";
        }
        else if (mark == -1000)
        {
            return "FOUL";
        }
        else if (mark == -10)
        {
            return "O";
        }
        else if (mark == -100000)
        {
            return "NH";
        }
        else
        {
            if (asTime)
            {
                if (mark == 0)
                {
                    return "FS";
                }
                else if (mark >= 60)
                {
                    int min = (int)(mark / 60);
                    float seconds = mark - (min * 60);
                    return min + ":" + fillZeros(Math.Round(seconds, 2).ToString(), true);
                }
                else
                {
                    return fillZeros(Math.Round(mark, 2).ToString(), false);
                }
            }
            else
            {
                return ((int)mark / 12) + "' " + Math.Round(mark % 12, 2) + "''";
            }
        }

    }
}
