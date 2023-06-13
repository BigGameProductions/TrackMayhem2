using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dan.Main;
using System;
using Dan.Models;

public class LeaderboardFunctions
{
    private string[] keys = new string[] {
        "10edeb578b951bf058f872f9c82026b662b11325355a4c1216dcff76bad59d8d",
        "183c8ab3d8f816348561d28303c3e1fa4c2c420d600adeea6e2d9b9f038dc7e9",
        "4a734ebe10d0abc558de1c91369b39325736237422b30a50875532c9206432d9",
        "2edf44095753d058361b58698a9030db438194af8ffaa7c5b9f31fdba9d0a76d",
        "7f09b9167c63d701445b1b24afacdf3c5e35d95f367ca5d4a1d7efbb8645e240",
        "defefae600935e249b4fa78944c0347a4e6801724d07a7be2e4bbe6850f4221a"
    };

    public string[][][] mainLeaderboard = new string[12][][];
    public bool isUpdate = false;

    private int currentEventOn = 0;

    public void GetLeaderboard(int eventCode)
    {
        LeaderboardCreator.GetLeaderboard(keys[eventCode], ((msg) =>
        {
            string[][] currentData = new string[4][];
            currentData[0] = new string[10];
            currentData[1] = new string[10];
            currentData[2] = new string[10];
            currentData[3] = new string[10];
            for (int i =0; i < Math.Min(msg.Length, 10); i++) {
                currentData[0][i] = msg[i].Username;
                currentData[1][i] = msg[i].Score.ToString();
                currentData[2][i] = msg[i].Extra;
                currentData[3][i] = UnixTimeStampToDateTime((long)msg[i].Date).ToShortDateString(); //coverts the long to a readable string for date
            }
            mainLeaderboard[currentEventOn] = currentData;
            PublicData.gameData.leaderboardList[currentEventOn] = currentData;
            currentEventOn++; //goes to the next event counter
            if (currentEventOn != 6)
            {
                GetLeaderboard(currentEventOn); //gets the next event
            } else
            {
                //set save leaderboard
                for (int i=0; i<mainLeaderboard.Length; i++)
                {
                    if (i<6)
                    {
                        SerializationLeaderboardTypes tempSave = new SerializationLeaderboardTypes();
                        tempSave.set(0, mainLeaderboard[i][0]);
                        tempSave.set(1, mainLeaderboard[i][1]);
                        tempSave.set(2, mainLeaderboard[i][2]);
                        tempSave.set(3, mainLeaderboard[i][3]);

                        PublicData.gameData.saveLeaderboard.set(i, tempSave); //TODO add to save manager taking the saved leaderboard and making it on the game data
                    }
                    
                }
                PublicData.gameData.leaderboardList = mainLeaderboard;
            }
            
        }));

    }


    public void SetLeaderBoardEntry(int eventCode, string name, int mark, string extra)
    {
        LeaderboardCreator.UploadNewEntry(keys[eventCode], name, mark, extra, ((msg) =>
        {
            
        }));
    }


    //find if the player is greater than the max rank and if so then deletes it
    public void checkForOwnPlayer(int eventCode, int maxRank)
    {
        LeaderboardCreator.GetLeaderboard(keys[eventCode], ((msg) =>
        {
            for (int i = 0; i < Math.Min(msg.Length, 10); i++)
            {
                if (msg[i].IsMine())
                {
                    if (msg[i].Rank > maxRank)
                    {
                        LeaderboardCreator.DeleteEntry(keys[eventCode]);
                    }
                }
            }
        }));
    }


    public void setMainLeaderboardVariables()
    {
        LeaderboardCreator.Ping(isServerReachable => {
            if (isServerReachable)
            {
                GetLeaderboard(currentEventOn);
            }
            else
            {
                Debug.Log("No connection");
            }

        });
    }


    private DateTime UnixTimeStampToDateTime(long unixTimeStamp) //converts the unix time to the date time for display
    {
        DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
        dtDateTime = dtDateTime.AddSeconds(unixTimeStamp);
        return dtDateTime;
    }
}
