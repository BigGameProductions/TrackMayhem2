using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SerializationLeaderboardTypes
{
    public string[] names;
    public string[] scores;
    public string[] extra;
    public string[] dates;

    public string[] get(int i)
    {
        if (i==0)
        {
            return names;
        }
        if (i == 1)
        {
            return scores;
        }
        if (i == 2)
        {
            return extra;
        }
        if (i == 3)
        {
            return dates;
        }
        return null;
    }

    public void set(int i, string[] data)
    {
        if (i == 0)
        {
            names = data;
        }
        if (i == 1)
        {
            scores = data;
        }
        if (i == 2)
        {
            extra = data;
        }
        if (i == 3)
        {
            dates = data;
        }
    }
}
