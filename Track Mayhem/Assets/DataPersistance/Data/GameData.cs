using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    //Game Data
    public string playerName; //plays name
    public bool tutorial; //if player has done tutorial
    public SerializationDictionary<string, bool> tempDict;
    public PersonalBests personalBests; //personal bests class
    public List<RunnerInformation> allRunners;
    //Game Data

    public GameData() //intial values for the game data
    {
        this.playerName = "NONAME";
        this.tutorial = false;
        this.tempDict = new SerializationDictionary<string,bool>();
        this.personalBests = new PersonalBests();
        this.allRunners = new List<RunnerInformation>();

        //start characters
        allRunners.Add(new RunnerInformation(0));
        allRunners.Add(new RunnerInformation(1));
    }
}
