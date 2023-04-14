using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ChestInfo
{
    public bool unlocked; //shows if the chest has been unlocked and ready to be opened

    public int chestID; //the id level of the chest determines how good the chest is

    public bool placeHolder;

    //id 0 = starter chest garuntee char
    //id 1 = basic chest
    //id 2 = gold chest
    //id 3 = epic chest
    //id 4 = legendary chest
    public ChestInfo(int id, bool real=true)
    {
        this.unlocked = true; //for testing purposes
        this.chestID = id;
        this.placeHolder = real;
    }
}
