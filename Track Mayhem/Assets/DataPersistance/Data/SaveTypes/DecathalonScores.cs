using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DecathalonScores
{
    public float[] eventMarks;
    public int[] pointsMarks;
    public string name;
    public int flagId;

    public DecathalonScores(string name, int flagId)
    {
        eventMarks = new float[10];
        pointsMarks = new int[10];
        this.name = name;
        this.flagId = flagId;
    }
   
}
