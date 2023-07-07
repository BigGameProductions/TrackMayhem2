using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    //Game Data
    public string playerName; //players name
    public string countryCode; //code of the country by string
    public int tokens; //the amount of tokens the player has
    public int rubies;
    public int trainingCards; //the amount of training cards the player has
    public int gearShards;
    public bool tutorial; //if player has done tutorial
    public SerializationDictionary<int, string> offlineLeaderboard;
    public PersonalBests personalBests; //personal bests class
    public List<RunnerInformation> allRunners;
    public ChestInfo[] chestSlots;
    public List<ChestInfo> futureChests;
    public string[][][] leaderboardList; //list of the online leaderbaord
    public SerializationLeaderboard saveLeaderboard;
    public int[] teamCharacters; //the list for the team with character ids

    public int[] offlineRecords; //list of ints which are the runner index in the order of the events for offline records
    //Game Data

    public GameData() //intial values for the game data
    {
        this.playerName = "NONAME";
        this.countryCode = "NOCOUNTRY";
        this.tokens = 0;
        this.rubies = 100; //TODO add starting rubies amount
        this.trainingCards = 0;
        this.gearShards = 0;
        this.tutorial = false;
        this.offlineLeaderboard = new SerializationDictionary<int,string>();
        this.personalBests = new PersonalBests();
        this.allRunners = new List<RunnerInformation>();
        for (int i=0; i<23; i++)
        {
            this.allRunners.Add(new RunnerInformation(i, false));
        }
        this.chestSlots = new ChestInfo[4];
        this.futureChests = new List<ChestInfo>();
        this.futureChests.Add(new ChestInfo(2));
        this.futureChests.Add(new ChestInfo(0));
        this.futureChests.Add(new ChestInfo(2));
        this.futureChests.Add(new ChestInfo(0));
        this.futureChests.Add(new ChestInfo(4));
        this.leaderboardList = new string[11][][];
        this.teamCharacters = new int[11];
        for (int i=0; i< teamCharacters.Length;i++)
        {
            teamCharacters[i] = -1;
        }
    }
}
