using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using System.Linq;

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

    LeaderboardFunctions leadF = new LeaderboardFunctions(); //makes a leaderboard functions class for getting the leadrboard


    // Start is called before the first frame update
    void Start()
    {
        leadF.GetLeaderboard(0);
    }

    // Update is called once per frame
    void Update()
    {
        if (leadF.isUpdate)
        {
            leadF.isUpdate = false;
            leaderBoardData = leadF.leaderboardData;
            updateLeaderboard(leaderBoardData);
        }
    }

    private void updateLeaderboard(string[][] data) //updates leaderboard witht he current data
    {
        for (int i=0; i<10; i++) //of ten places
        {
            Names.GetComponentsInChildren<TextMeshProUGUI>()[i].text = data[0][i]; //sets the right name
            Scores.GetComponentsInChildren<TextMeshProUGUI>()[i].text = data[1][i]; //sets the right mark
            Runners.GetComponentsInChildren<TextMeshProUGUI>()[i].text = PublicData.charactersInfo.ElementAt(Int32.Parse(data[2][i].Split(",")[1]))[1]; //gets the runner name
            Dates.GetComponentsInChildren<TextMeshProUGUI>()[i].text = data[3][i]; //sets the right dates
        }
    
    }
    
}
