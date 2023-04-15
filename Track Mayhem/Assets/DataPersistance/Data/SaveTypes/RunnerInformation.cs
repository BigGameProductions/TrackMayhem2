using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RunnerInformation
{
    public int runnerId;
    public int speedLevel;
    public int strengthLevel;
    public int agilityLevel;
    public int flexabilityLevel;
    public int points;

    public int upgradePoints;

    public bool unlocked;

    public RunnerInformation(int id)
    {
        this.runnerId = id;
        this.speedLevel = 0;
        this.strengthLevel = 0;
        this.agilityLevel = 0;
        this.flexabilityLevel = 0;
        this.points = 0;
        this.upgradePoints = 0;
        this.unlocked = false;
    }

    public void increasePoints(int num)
    {
        points += num;
    }
}