using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PersonalBests
{
    public float longJump;
    public float polevault;
    public float hundredMeter;
    public float shotput;
    public float javelin;
    public float fourHundred;

    public PersonalBests() //sets defualts for all event
    {
        this.longJump = 0;
        this.polevault = 0;
        this.hundredMeter = 0;
        this.shotput = 0;
        this.javelin = 0;
        this.fourHundred = 0;
    } 
}
