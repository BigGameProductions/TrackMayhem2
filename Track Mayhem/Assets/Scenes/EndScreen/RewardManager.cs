using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class RewardManager : MonoBehaviour
{
    int[] pointsList = new int[] { 0, 10, 8, 6, 5, 4, 3, 2, 1 }; //array of points that are given for each place in the compeition

    // Start is called before the first frame update
    void Start()
    {
        PlayerBanner player = new PlayerBanner(0, 0, "placeholder"); //placeholder for when it is set later
        foreach (PlayerBanner pb in PublicData.playerBannerTransfer) //find the player among the leaderboard banners
        {
            if (pb.isPlayer)
            {
                player = pb;
            }
        }
        PublicData.pointsToGive = pointsList[player.place]; //applies the points awarded to the given runner
    }

   /* private int indexOfCurrentRunner() //find the current index of the runner in the runner array in game data
    {
        for (int i=0; i<PublicData.gameData.allRunners.Count;i++) //loops through runners
        {
            if (PublicData.gameData.allRunners.ElementAt(i).runnerId == PublicData.currentRunnerUsing) //checks if id matches
            {
                return i; //returns index
            }
        }
        return 0;
    }*/
}
