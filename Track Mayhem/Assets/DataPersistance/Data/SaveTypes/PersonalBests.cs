using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PersonalBests
{
    public float longJump;
    public float polevault;

    public PersonalBests() //sets defualts for all event
    {
        this.longJump = 0;
        this.polevault = 0;
    } 
}
