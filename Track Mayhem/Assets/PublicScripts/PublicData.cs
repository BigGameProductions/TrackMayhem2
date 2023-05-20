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
    public static int currentBoxOpening = 0; //the id of the current chest that is being opened between main menu and opening scene

    public static float spacesPerInch = (1875.03f - 1864.7f) / 36f; //spaces per inch for the game



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

    //baseObject = referance object
    //newObject = object to add colliders to
    //baseName = string to ignore when looking (ex. miximorig)
    //items = list of string names of body parts to copy colldiers to 
    public static GameObject addColldiersToRunner(GameObject baseObject, GameObject newObject, string baseName, List<string> items) //returns the game object with all the colliders said it items based on baseObject
    {
        for (int i=0; i<baseObject.transform.GetComponentsInChildren<Transform>().Length; i++)
        {
            Transform tf = baseObject.GetComponentsInChildren<Transform>()[i];
            //Debug.Log(tf.name);
            if (tf.name.Length > baseName.Length)
            {
                //Debug.Log(tf.name);
                if (tf.name.Substring(0, baseName.Length) == baseName) //checks root word
                {
                    Debug.Log("Got herer"); 
                    if (items.Contains(tf.name.Substring(baseName.Length))) //tests if the object is in the item list
                    {
                        Debug.Log("Found it");
                        for (int j= 0; j < newObject.GetComponentsInChildren<Transform>().Length; j++)
                        {
                            Transform newTF = newObject.GetComponentsInChildren<Transform>()[j];
                            if (newTF.name == baseName + tf.name.Substring(baseName.Length)) //if the string matches the new object
                            {
                                Debug.Log(newTF.name);
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


}