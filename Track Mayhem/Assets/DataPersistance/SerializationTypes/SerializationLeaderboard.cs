using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class SerializationLeaderboard
{
    public SerializationLeaderboardTypes hundredMeter;
    public SerializationLeaderboardTypes longjump;
    public SerializationLeaderboardTypes polevault;
    public SerializationLeaderboardTypes shotput;
    public SerializationLeaderboardTypes javalin;
    public SerializationLeaderboardTypes fourhundred;
    public SerializationLeaderboardTypes discus;
    public SerializationLeaderboardTypes highjump;
    public SerializationLeaderboardTypes hurdles;
    public SerializationLeaderboardTypes mile;
    public SerializationLeaderboardTypes decathalon;
    public SerializationLeaderboardTypes points;

    public SerializationLeaderboardTypes get(int i)
    {
        if (i==0)
        {
            return hundredMeter;
        }
        if (i == 1)
        {
            return longjump;
        }
        if (i == 2)
        {
            return polevault;
        }
        if (i == 3)
        {
            return shotput;
        }
        if (i == 4)
        {
            return javalin;
        }
        if (i == 5)
        {
            return fourhundred;
        }
        if (i == 6)
        {
            return discus;
        }
        if (i == 7)
        {
            return highjump;
        }
        if (i == 8)
        {
            return hurdles;
        }
        if (i == 9)
        {
            return mile;
        }
        if (i == 10)
        {
            return decathalon;
        }
        if (i == 11)
        {
            return points;
        }
        return null;
    }

    public void set(int i, SerializationLeaderboardTypes data)
    {
        if (i == 0)
        {
            hundredMeter = data;
        }
        if (i == 1)
        {
            longjump = data;

        }
        if (i == 2)
        {
            polevault = data;

        }
        if (i == 3)
        {
            shotput = data;

        }
        if (i == 4)
        {
            javalin = data;

        }
        if (i == 5)
        {
            fourhundred = data;

        }
        if (i == 6)
        {
            discus = data;

        }
        if (i == 7)
        {
            highjump = data;

        }
        if (i == 8)
        {
            hurdles = data;

        }
        if (i == 9)
        {
            mile = data;

        }
        if (i == 10)
        {
            decathalon = data;

        }
        if (i == 11)
        {
            points = data;

        }
    }
}
