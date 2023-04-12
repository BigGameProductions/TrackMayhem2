using System.Collections;
using System.Collections.Generic;
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

}