using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;

public class RewardManager : MonoBehaviour
{
    int[] pointsList = new int[] { 0, 10, 8, 6, 5, 4, 3, 2, 1 }; //array of points that are given for each place in the compeition

    [SerializeField] private GameObject chest;
    [SerializeField] private Material chestColor;
    [SerializeField] private ItemStorage itemStorage;
    [SerializeField] private GameObject decathlonPoints;

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

        if (PublicData.gameData.futureChests.Count <=5) //when the list is low enough to be refilled
        {
            makeFutureChests(10); //add more chests to the list
        }
        if (player.place < 4 && !PublicData.inDec) //if top three positions
        {
            addChest(PublicData.gameData.futureChests.ElementAt(0).chestID); //adds the chest from the chest list to player
            chest.SetActive(true); //shows the visible chest
            chestColor.color = itemStorage.chestColors[PublicData.gameData.futureChests.ElementAt(0).chestID]; //sets the visible chest color
            PublicData.gameData.futureChests.RemoveAt(0); //removes the first item so all the chests shift
        }
        else
        {
            chest.SetActive(false); //hide the visibles chest as the player did not win anything
        }
        if (PublicData.inDec)
        {
            decathlonPoints.SetActive(true);
        }
    }

    private void addChest(int level) //adds a chest to the player of a specific level
    {
        for (int i=0; i<PublicData.gameData.chestSlots.Length; i++)
        {
            if (PublicData.gameData.chestSlots[i] == null)
            {
                PublicData.gameData.chestSlots[i] = new ChestInfo(level, true);
                break; //only one spot can get a chest
            }
        }
    }

    private void makeFutureChests(int count)
    {
        for (int i = 0; i<count; i++)
        {
            int chestId = 1;
            int chance = UnityEngine.Random.Range(0, 100);
            if (chance > 70 && chance < 85)
            {
                chestId = 2;
            }
            else if (chance > 85 && chance < 95)
            {
                chestId = 3;
            }
            else if (chance > 95)
            {
                chestId = 4;
            }
            PublicData.gameData.futureChests.Add(new ChestInfo(chestId, true));
            
        }
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
