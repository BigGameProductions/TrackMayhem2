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
    public int trainingCards; //the amount of training cards the player has
    public bool tutorial; //if player has done tutorial
    public SerializationDictionary<string, bool> tempDict;
    public PersonalBests personalBests; //personal bests class
    public List<RunnerInformation> allRunners;
    public ChestInfo[] chestSlots;
    public List<ChestInfo> futureChests;

    public int[] offlineRecords; //list of ints which are the runner index in the order of the events for offline records
    //Game Data

    public GameData() //intial values for the game data
    {
        this.playerName = "NONAME";
        this.countryCode = "us";
        this.tokens = 0;
        this.trainingCards = 0;
        this.tutorial = false;
        this.tempDict = new SerializationDictionary<string,bool>();
        this.personalBests = new PersonalBests();
        this.allRunners = new List<RunnerInformation>();
        this.chestSlots = new ChestInfo[4];
        this.futureChests = new List<ChestInfo>();

        //start characters
        allRunners.Add(new RunnerInformation(0, true));
        allRunners.Add(new RunnerInformation(1, true));
    }
}
