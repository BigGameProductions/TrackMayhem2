using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dan.Main;
using System;

public class LeaderboardFunctions
{
    private string[] keys = new string[] {
        "",
        "183c8ab3d8f816348561d28303c3e1fa4c2c420d600adeea6e2d9b9f038dc7e9",
        "4a734ebe10d0abc558de1c91369b39325736237422b30a50875532c9206432d9"
    };

    public string[][] leaderboardData = new string[4][];
    public bool isUpdate = false;

    public void GetLeaderboard(int eventCode)
    {
        leaderboardData[0] = new string[10];
        leaderboardData[1] = new string[10];
        leaderboardData[2] = new string[10];
        leaderboardData[3] = new string[10];
        LeaderboardCreator.GetLeaderboard(keys[eventCode], ((msg) =>
        {
            for (int i =0; i < Math.Min(msg.Length, 10); i++) {
                leaderboardData[0][i] = msg[i].Username;
                leaderboardData[1][i] = msg[i].Score.ToString();
                leaderboardData[2][i] = msg[i].Extra;
                leaderboardData[3][i] = UnixTimeStampToDateTime((long)msg[i].Date).ToShortDateString(); //coverts the long to a readable string for date
            }
            isUpdate = true;
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

    private DateTime UnixTimeStampToDateTime(long unixTimeStamp) //converts the unix time to the date time for display
    {
        DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
        dtDateTime = dtDateTime.AddSeconds(unixTimeStamp);
        return dtDateTime;
    }
}
