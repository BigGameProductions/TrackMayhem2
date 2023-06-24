using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class PublicData
{
    public static PlayerBanner[] playerBannerTransfer; //transfers the leaderboard to other scenes
    public static int leaderBoardMode; //tranfers the mode of the leaderboard to the end screen
    public static bool usesTime; //shows of the leaderboard uses time or feet

    public static List<string[]> charactersInfo; //holds info of characters.csv
    public static List<string[]> recordsInfo; //holds the info of records.csv
    public static List<string[]> namesInfo; //holds the info of names.csv
    public static List<string[]> chestInfo; //holds the info for chests.csv

    public static GameData gameData; //stores all the game data that has been saved for the game
    public static int currentRunnerOn = 0; //shows the current runner that the player is on
    public static int currentRunnerUsing = 1;//shows the current runner that play is using for events
    public static int currentBoxOpening = 0; //the id of the current chest that is being opened between main menu and opening scene

    public static float spacesPerInch = (1875.03f - 1864.7f) / 36f; //spaces per inch for the game
    public static float averageSpeedDuringRun = 175; //the speed of the green bar during the run

    public static int pointsToGive = 0;


    private static int upgradeStartPrice = 100; //starting price for upgrades
    private static float upgradeScale = 3; //upgrage multiplication scale

    private static int startGoldPrice = 50; //starting price of gold
    private static float upgradeGoldScale = 3; //scale of the gold for the upgrade price


    public static int currentSelectedEventIndex = 1; //holds the index of the current event according to the records.csv
    public static string currentEventName = "LongJump";

    public static bool inDec; //if the player is in a dec
    public static int currentEventDec = -1; //event index the player is in
    public static PlayerBanner[] decPlayers; //players of the dec for each event
    public static DecathalonScores[] decPlayersScores;


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
        PublicData.gameData.allRunners.Add(new RunnerInformation(id, false)); //if it can't find one then it makes a new character with the same id
        return getCharactersInfo(id); //returns the new character

    }

    //baseObject = referance object
    //newObject = object to add colliders to
    //baseName = string to ignore when looking (ex. miximorig)
    //items = list of string names of body parts to copy colldiers to 
    public static GameObject addColldiersToRunner(GameObject baseObject, GameObject newObject, string baseName, List<string> items) //returns the game object with all the colliders said it items based on baseObject
    {
        for (int i=0; i<baseObject.transform.GetComponentsInChildren<Transform>().Length; i++)
        {
            Transform tf = baseObject.GetComponentsInChildren<Transform>()[i];
            if (tf.name.Length > baseName.Length)
            {
                if (tf.name.Substring(0, baseName.Length) == baseName) //checks root word
                {
                    if (items.Contains(tf.name.Substring(baseName.Length))) //tests if the object is in the item list
                    {
                        for (int j= 0; j < newObject.GetComponentsInChildren<Transform>().Length; j++)
                        {
                            Transform newTF = newObject.GetComponentsInChildren<Transform>()[j];
                            if (newTF.name == baseName + tf.name.Substring(baseName.Length)) //if the string matches the new object
                            {
                                BoxCollider newColl = newTF.gameObject.AddComponent<BoxCollider>(); //makes the new collider
                                newColl.size = tf.GetComponent<BoxCollider>().size;
                                newColl.center = tf.GetComponent<BoxCollider>().center;
                            }

                        }
                    }
                }
            }
            
        }
        return newObject;
    }

    //used to support runners information and runners display
    public static int upgradeLevelForTrait(int count, RunnerInformation ri, string[] att) //returns the number of the points needed for the current upgrade
    {
        int level = 0; //gets the level for the current trait
        if (count == 0)
        {
            level = ri.speedLevel;
        }
        if (count == 1)
        {
            level = ri.strengthLevel;
        }
        if (count == 2)
        {
            level = ri.agilityLevel;
        }
        if (count == 3)
        {
            level = ri.flexabilityLevel;
        }
        if (level == 10)
        {
            return -1;
        }
       
        
        int finalResult = upgradeStartPrice; //sets the starting price
        for (int i = 0; i < level; i++)
        {
            finalResult = (int)(finalResult * upgradeScale); //scales the price by the scale factor
        }
        return finalResult;
        


    }

    public static bool usesTrainingCard(int count, RunnerInformation ri, string[] att)
    {
        int level = 0; //gets the level for the current trait
        if (count == 0)
        {
            level = ri.speedLevel;
        }
        if (count == 1)
        {
            level = ri.strengthLevel;
        }
        if (count == 2)
        {
            level = ri.agilityLevel;
        }
        if (count == 3)
        {
            level = ri.flexabilityLevel;
        }
        if (level == 10)
        {
            return false;
        }
        if (level >= Int32.Parse(att[3 + count])) //tests if it is below at or above the max standards for the runner
        {
            return true;
        }
        return false;
    }

    public static int getGoldAmountForLevel(int level)
    {
        int finalResult = startGoldPrice; //sets the starting price
        for (int i = 0; i < level; i++)
        {
            finalResult = (int)(finalResult * upgradeGoldScale); //scales the price by the scale factor
        }
        return finalResult;
    }

    public static float curveValue(float value, float maxValue)
    {
        float aVal = (float)(maxValue / Math.Sqrt(10));
        return (float)(aVal * Math.Sqrt(value));
    }




}