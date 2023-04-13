using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class PublicData
{
    public static PlayerBanner[] playerBannerTransfer; //transfers the leaderboard to other scenes
    public static int leaderBoardMode; //tranfers the mode of the leaderboard to the end screen

    public static List<string[]> charactersInfo; //holds info of characters.csv
    public static List<string[]> recordsInfo; //holds the info of records.csv
    public static List<string[]> namesInfo; //holds the info of names.csv

    public static GameData gameData; //stores all the game data that has been saved for the game
    public static int currentRunnerOn = 0; //shows the current runner that the player is on
    public static int currentRunnerUsing = 0;//shows the current runner that play is using for events

    public static int currentSelectedEventIndex = 1; //holds the index of the current event according to the records.csv

    public static RunnerInformation getCharactersInfo(int id) //function to get the current data from game data of that runner
    {
        List<RunnerInformation> runnerData = PublicData.gameData.allRunners; //used in all classes to get the current character information
        for (int i = 0; i < runnerData.Count; i++) //loops through characters
        {
            if (runnerData.ElementAt(i).runnerId == id) //find matching ids
            {
                return runnerData.ElementAt(i);
            }
        }
        PublicData.gameData.allRunners.Add(new RunnerInformation(id)); //if it can't find one then it makes a new character with the same id
        return getCharactersInfo(id); //returns the new character

    }


}