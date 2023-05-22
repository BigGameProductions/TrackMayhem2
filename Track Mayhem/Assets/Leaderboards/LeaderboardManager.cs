using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.IO;

public class LeaderboardManager : MonoBehaviour, IDataPersistance
{
    [SerializeField] private string eventName; //name of the event that is active
    [SerializeField] private bool useSeeded; //determines if personal bests should be used too seed
    [SerializeField] private float seededMark; //if useSeeded is false determines the base mark to go off of
    [SerializeField] private float seedSpreadDown; //how much the players will be spread from the seed down
    [SerializeField] private float seedSpreadUp; //how much the players will be spread from the seed up
    [SerializeField] private float seedRoundInches; //round to the nearest of what inch
    [SerializeField] private Image[] leaderboardBanners; //a list of all the leaderboard banners that need to have information on them
    [SerializeField] public Camera cinematicCamera; // the camaera that plays the animations before the event. Used by other classes to determine when to start
    [SerializeField] private GameObject leaderBoardHeader; //the header for the main leaderboard;
    [SerializeField] private GameObject personalBanner; //the banner shown for the player after each jump

    private PlayerBanner[] currentEventBanners;

    private List<PlayerBanner> finalBarHeightsBanners = new List<PlayerBanner>();

    private float currentBarHeight = -10;
    private float openingHeight = 0;

    [SerializeField] private Sprite[] flags;


    private PersonalBests personalBests; //store personals bests for seeding
    private string playerName; //store the name of the player
    private int animationStage = 0; //stores the stage for the cinematic camera before an event

    private float basedSeed; //stores the based seed for the current event

    private PlayerBanner personalBannersMarks; //stores the given set of marks for the event

    public void LoadData(GameData data)
    {
        this.personalBests = data.personalBests; //load the personal bests
        this.playerName = data.playerName; //name of the player
    }

    public void SaveData(ref GameData data)
    {
        //nothing to save
    }

    private void Start()
    {

        //nothing for now
        if (SceneManager.GetActiveScene().name != "EndScreen") //tests to make sure it is an event screen
        {
            //temp
            cinematicCamera.GetComponent<Animator>().speed = 10;
            //temp
        } else
        {
            currentEventBanners = PublicData.playerBannerTransfer;
            setLeaderboard(currentEventBanners, PublicData.leaderBoardMode); //shows the leaderboard sorted
        }



    }


    private void Update()
    {
        if (SceneManager.GetActiveScene().name != "EndScreen")
        {
            if (cinematicCamera.GetComponent<Animator>().GetInteger("Stage") != animationStage)
            {
                animationStage = cinematicCamera.GetComponent<Animator>().GetInteger("Stage"); //update the current stage
                updateCinematicStage(animationStage); //update the leaderboard
            }
        }
        
    }

    private int findFlagIndexOfCountry(string code) //find the index of flags given the country code
    {
        for (int i = 0; i < flags.Length;i++)
        {
            if (flags[i].name == code.ToLower())
            {
                return i;
            }
        }
        return 0;
    }

    public void simRemainingJumps() //simulates and sorts all banners for end display
    {
        //getPlayerBanner().bestMark = currentBarHeight;
        //finalBarHeightsBanners.Add(getPlayerBanner());
    
        while (finalBarHeightsBanners.Count < 8 && currentBarHeight < 1200)
        {
            simulateBarMark(currentEventBanners); //three jumps
            simulateBarMark(currentEventBanners);
            simulateBarMark(currentEventBanners);
            List<PlayerBanner> newBannerList = new List<PlayerBanner>();
            for (int i = 0; i < currentEventBanners.Length; i++)
            {
                if (currentEventBanners[i].mark3 != -10000)
                {
                    newBannerList.Add(currentEventBanners[i]); //adds only the passing players to the leaderboard
                }
                else
                {
                    if (openingHeight == currentBarHeight)
                    {
                        currentEventBanners[i].bestMark = -100000; //no height

                    }
                    else
                    {
                        currentEventBanners[i].bestMark = currentBarHeight - 6;

                    }

                    finalBarHeightsBanners.Add(currentEventBanners[i]);
                }
            }
            currentEventBanners = newBannerList.ToArray(); //converts the list back to the array for use
            currentBarHeight += 6;
            clearPlayerMarks();

        }
        PublicData.playerBannerTransfer = sortBanners(finalBarHeightsBanners.ToArray(), true);
        PublicData.leaderBoardMode = 1;
    }

    public void showCurrentPlayerMarks(PlayerBanner marks, int stageNum) //shows the single banner of the player preformace
    {
        personalBannersMarks = marks;
        updateCinematicStage(stageNum);
    }

    public void hidePersonalBanner() //hides the personal banner for the player
    {
        updateCinematicStage(0);
    }

    private void clearPlayerMarks() //clears all the marks of the player and oppenents and updates the fails and misses
    {
        if (personalBannersMarks.mark1 == -10)
        {
            personalBannersMarks.lastMakeAttempt = 1;
        } else if (personalBannersMarks.mark2 == -10)
        {
            personalBannersMarks.lastMakeAttempt = 2;
            personalBannersMarks.totalFails += 1;
        }
        else if (personalBannersMarks.mark3 == -10)
        {
            personalBannersMarks.lastMakeAttempt = 3;
            personalBannersMarks.totalFails += 2;
        }

        personalBannersMarks.mark1 = -100;
        personalBannersMarks.mark2 = -100;
        personalBannersMarks.mark3 = -100;

        foreach (PlayerBanner pb in currentEventBanners)
        {
            if (pb.mark1 == -10)
            {
                pb.lastMakeAttempt = 1;
            }
            else if (pb.mark2 == -10)
            {
                pb.lastMakeAttempt = 2;
                pb.totalFails += 1;
            }
            else if (pb.mark3 == -10)
            {
                pb.lastMakeAttempt = 3;
                pb.totalFails += 2;
            }
            pb.mark1 = -100;
            pb.mark2 = -100;
            pb.mark2 = -100;
        }

    }

    public void showUpdatedLeaderboard() //shows the updated numbers for all oppenents
    {
        updateCinematicStage(4); //show the updated leaderboard
        StartCoroutine(timeOfLeaderboard(3)); //hides the leaderboard after x seconds
    }

    public PlayerBanner getPlayerBanner()
    {
        foreach (PlayerBanner pb in currentEventBanners)
        {
            if (pb.isPlayer)
            {
                return pb;
            }
        }
        return null;
    }

    IEnumerator timeOfLeaderboard(int time)
    {
        yield return new WaitForSeconds(time);
        updateCinematicStage(0);
    }

    public bool leaderBoardVisble() //returns if leaderboard is visible
    {
        return leaderBoardHeader.activeInHierarchy;
    }

    public float roundToNearest(float nearest, float mark) //rounds the mark to the nearest float that is in the parameter. public for all classes
    {
        for (int i = 0; i < 1 / nearest; i++) //checks all increments from the nearest to the whole number
        {
            if ((int)mark + (nearest * i) < mark && (int)mark + (nearest * (i + 1)) > mark)
            { //checks the mark before and after to see if the mark is in the range
                if (Math.Abs((int)mark + (nearest * i) - mark) < Math.Abs((int)mark + (nearest * (i + 1))) - mark) //check which bound to return
                {
                    return (int)mark + (nearest * i); //returns the lower bound
                }
                else
                {
                    return (int)mark + (nearest * (i + 1)); //returns the higher bound
                }
            }
        }
        return mark;
    }

    

    public void updateCurrentBarHeight(float amount, float isOpeningHeight)
    {
        currentBarHeight = amount;
        openingHeight = isOpeningHeight;
    }

    //stage same as mode for setLeaderboard
    private void updateCinematicStage(int stage)
    {
        if (stage==0) //hides the leaderboard when it should not be shown
        {
            leaderBoardHeader.SetActive(false);
            personalBanner.SetActive(false);
            return;
        } else if (!leaderBoardHeader.activeInHierarchy && stage != 3) //make sure leaderboard is visible for other stages too
        {
            leaderBoardHeader.SetActive(true);
        }
        PlayerBanner[] playerBanners = new PlayerBanner[0];
        if (stage == 1)
        {
            playerBanners = generateBanners(7, true);
            currentEventBanners = playerBanners;
            
        } else if (stage == 2)
        {
            string[] currentRecordInfo = getEventRecordByEvent(eventName);
            playerBanners = new PlayerBanner[] {
            new PlayerBanner(0, findFlagIndexOfCountry(currentRecordInfo[4]), currentRecordInfo[1], float.Parse(currentRecordInfo[2])),
            new PlayerBanner(0, 0, "World", 98),
            new PlayerBanner(0, findFlagIndexOfCountry("us"), playerName, getMarkForEvent(SceneManager.GetActiveScene().name))
             };
        } else if (stage == 3) //current player jump
        {
            playerBanners = new PlayerBanner[0];
            personalBanner.SetActive(true);
        } else if (stage == 4)
        {
            if (currentBarHeight == -10)
            {
                currentEventBanners = simulateMark(currentEventBanners, eventName, 2, 5); //makes marks for oppenents
            } else
            {
                currentEventBanners = simulateBarMark(currentEventBanners); //always sim one
                if (personalBannersMarks.mark1 == -10) //sims last two if got on first try
                {
                    getPlayerBanner().makeAttempt = 1;
                    currentEventBanners = simulateBarMark(currentEventBanners);
                    currentEventBanners = simulateBarMark(currentEventBanners);

                } else if (personalBannersMarks.mark2 == -10) //sim last one if got on second try
                {
                    getPlayerBanner().makeAttempt = 2;
                    currentEventBanners = simulateBarMark(currentEventBanners);
                }
                
            }
            playerBanners = currentEventBanners;
            if (currentBarHeight == -10) //check for bar event
            {
                PublicData.playerBannerTransfer = currentEventBanners; //sets the end screen leaderboard to match the current one
                PublicData.leaderBoardMode = 4; //sets leaderboard mode for the end screen
            } 
            
        }

        setLeaderboard(playerBanners, stage);
        if (currentEventBanners != null && currentBarHeight != -10 && stage == 4) //checks if it is a bar event and makes sure it is not null
        {
            List<PlayerBanner> newBannerList = new List<PlayerBanner>();
            for (int i = 0; i < currentEventBanners.Length; i++)
            {
                if (currentEventBanners[i].mark3 != -10000)
                {
                    newBannerList.Add(currentEventBanners[i]); //adds only the passing players to the leaderboard
                } else
                {
                    if (openingHeight == currentBarHeight-6)
                    {
                        currentEventBanners[i].bestMark = -100000; //no height
                    }
                    else
                    {
                        currentEventBanners[i].bestMark = currentBarHeight - 12;
                    }
                    finalBarHeightsBanners.Add(currentEventBanners[i]);
                }
            }
            currentEventBanners = newBannerList.ToArray(); //converts the list back to the array for use
            if (personalBannersMarks.mark1 == -10 || personalBannersMarks.mark2 == -10 || personalBannersMarks.mark3 == -10) //if the player has cleared it
            {
                clearPlayerMarks(); //clears all marks for the next height
            }
        }
        
    }

    private string[] getEventRecordByEvent(string ev) {
        for (int i=0; i<PublicData.recordsInfo.Count; i++) //loops through csv file
        {
            if (i!=0) //avoids header
            {
                if (PublicData.recordsInfo.ElementAt(i)[0] == ev) //checks if the event matches
                {
                    return PublicData.recordsInfo.ElementAt(i); //returns the attributes of the record
                }
            }
        }
        return null;
    }

    private string getRandomName() //gets a random name from names.csv
    {
        return PublicData.namesInfo.ElementAt(UnityEngine.Random.Range(0, 19417))[4];
    }

    //int size for amount of banners
    //bool addPlayer if the player should be added to the list
    private PlayerBanner[] generateBanners(int size, bool addPlayer) //generates the banners for size number of people based off of the seeding and returns a list og them
    {
        PlayerBanner[] banners = new PlayerBanner[size + (addPlayer ? 1:0)];
        for (int i = 0; i<size; i++)
        {
            int flagNum = UnityEngine.Random.Range(0, 254); //random country
            string name = getRandomName(); //gets random name
            name = char.ToUpper(name[0]) + name.Substring(1).ToLower(); //making it format as first letter upper
            float personalBest = 0;
            if (useSeeded)
            {
                personalBest = seedTimesForEvent(eventName, useSeeded, seedSpreadDown, seedSpreadUp);
            } else
            {
                basedSeed = seededMark;
                personalBest = seedTimesForEvent(eventName, useSeeded, seedSpreadDown, seedSpreadUp);
            }
            banners[i] = new PlayerBanner(i, flagNum, name, roundToNearest(0.25f, personalBest)); //round personal bests to only two places

        }
        banners[banners.Length - 1] = new PlayerBanner(0, findFlagIndexOfCountry("us"), playerName, getMarkForEvent(SceneManager.GetActiveScene().name), isPlayer:true);
        return sortBanners(banners, true, barEvent:true); //sorting banners based on bar events
    }

    //theEvent is the event that is used
    //useSeeded is if seeding on personal bests should be used
    //seedSpread[Down/Up] is the spread of the seeding from the original down and up from that mark
    //basedSeed is optional for if useSeeded is false what to base seeding around
    private float seedTimesForEvent(string theEvent, bool useSeeded, float seedSpreadDown, float seedSpreadUp) //based on the event it gives a random seed based on that number
    {
        if (useSeeded)
        {
            basedSeed = getMarkForEvent(theEvent);
        }
        return roundToNearest(seedRoundInches, UnityEngine.Random.Range((float)basedSeed - seedSpreadDown, basedSeed + seedSpreadUp + 1f));
        
    }

    public float getMarkForEvent(string theEvent) //returns the player mark in the save files for the given event
    {
        if (theEvent == "LongJump")
        {
            return PublicData.gameData.personalBests.longJump;

        } else if (theEvent == "PoleVault")
        {
            return PublicData.gameData.personalBests.polevault;
        }
        return 0;
    }


    private PlayerBanner[] simulateBarMark(PlayerBanner[] playerBanners) //simulates one mark for the bar events
    {
        foreach (PlayerBanner pb in playerBanners)
        {
            if (!pb.isPlayer) //making sure the banner is not for the player
            {
                bool make; //adjusted depending on seeded height
                if(pb.bestMark-currentBarHeight > 24)
                {
                    make = UnityEngine.Random.Range(0, 5) != 0;
                } else if (pb.bestMark - currentBarHeight > 12)
                {
                    make = UnityEngine.Random.Range(0, 4) != 0;

                }
                else if (pb.bestMark - currentBarHeight > 0)
                {
                    make = UnityEngine.Random.Range(0, 3) != 0;

                } else if (pb.bestMark - currentBarHeight > -12)
                {
                    make = UnityEngine.Random.Range(0, 2) == 0;
                } else if (pb.bestMark - currentBarHeight > -24)
                {
                    make = UnityEngine.Random.Range(0, 8) == 0;
                } else
                {
                    make = UnityEngine.Random.Range(0, 12) == 0;

                }
                if (pb.mark1 == -100)
                {
                    pb.mark1 = make ? -10 : -10000;
                    if (make)
                    {
                        pb.makeAttempt = 1;
                        pb.mark2 = -100;
                        pb.mark3 = -100;
                    }
                    continue;
                }
                if (pb.mark2 == -100 && pb.mark1 != -10)
                {
                    pb.mark2 = make ? -10 : -10000;
                    if (make)
                    {
                        pb.makeAttempt = 2;
                        pb.mark3 = -100;
                    }
                    continue;
                }
                if (pb.mark3 == -100 && pb.mark2 != -10 && pb.mark1 != -10)
                {
                    pb.mark3 = make ? -10 : -10000;
                    if (make) pb.makeAttempt = 3;
                    continue;
                }
            }

        }
        /*foreach (PlayerBanner ppbb in playerBanners)
        {
            if (ppbb.isPlayer)
            {
                Debug.Log("Has player");
            }
        }*/
        return sortBanners(playerBanners, true, barEvent:true);
    }

    //-10 is pass
    //-100 is empty
    //-1000 is foul
    //-10000 is miss
    //-100000 is NH
    //simulates marks for the oppenents based on the spread given in the parameters
    private PlayerBanner[] simulateMark(PlayerBanner[] playerBanners, string theEvent, float spreadUp, float spreadDown)
    {
        foreach (PlayerBanner pb in playerBanners)
        {
            if (!pb.isPlayer) //making sure the banner is not for the player
            {
                if (pb.mark1 == -100)
                {
                    pb.mark1 = UnityEngine.Random.Range((float)pb.bestMark - spreadDown, pb.bestMark + spreadUp + 1f);
                    pb.mark1 = roundToNearest(0.25f, pb.mark1);
                    continue;
                }
                if (pb.mark2 == -100)
                {
                    pb.mark2 = UnityEngine.Random.Range((float)pb.bestMark - spreadDown, pb.bestMark + spreadUp + 1f);
                    pb.mark2 = roundToNearest(0.25f, pb.mark2);
                    continue;
                }
                if (pb.mark3 == -100)
                {
                    pb.mark3 = UnityEngine.Random.Range((float)pb.bestMark - spreadDown, pb.bestMark + spreadUp + 1f);
                    pb.mark3 = roundToNearest(0.25f, pb.mark3);
                    continue;
                }
            }
            
        }
        return sortBanners(playerBanners, true, true);

    }

    private PlayerBanner[] sortBanners(PlayerBanner[] banners, bool bigOnTop, bool ofThree=false, bool barEvent=false) //sorts the banners by big or small on the top in the array
    {
        List<PlayerBanner> sortedBanners = new List<PlayerBanner>();
        if (barEvent)
        {
            sortedBanners.Add(new PlayerBanner(0, 0, "SortingPlaceholder", makeAttempt:1, lastMakeAttempt:1, totalFails:0)); //best senario used for sorting
        } else
        {
            sortedBanners.Add(new PlayerBanner(0, 0, "SortingPlaceholder", float.MaxValue)); //placeholder to help sort the items in the array by value
        }
        foreach (PlayerBanner pb in banners)
        {
            for (int i = 0; i < sortedBanners.Count; i++)
            {
                PlayerBanner sb = sortedBanners.ElementAt(i);
                if (barEvent)
                {
                    if (pb.makeAttempt == sb.makeAttempt)
                    {
                        if (pb.lastMakeAttempt == sb.lastMakeAttempt)
                        {
                            if (pb.totalFails >= sb.totalFails)
                            {
                                sortedBanners.Insert(i, pb);
                                break;
                            }
                        } else if (pb.lastMakeAttempt > sb.lastMakeAttempt)
                        {
                            sortedBanners.Insert(i, pb);
                            break;
                        }
                    } else if (pb.makeAttempt > sb.makeAttempt)
                    {
                        sortedBanners.Insert(i, pb);
                        break;
                    }
                } else
                {
                    if ((ofThree ? Math.Max(Math.Max(pb.mark1, pb.mark2), pb.mark3) : pb.bestMark) < sortedBanners.ElementAt(i).bestMark) //sort my best mark
                    {
                        sortedBanners.Insert(i, pb);
                        break;
                    }
                }
                


            }

        }
        sortedBanners.RemoveAt(sortedBanners.Count - 1); //remove the placeholder
        PlayerBanner[] newBanners = bigOnTop ? sortedBanners.ToArray().Reverse<PlayerBanner>().ToArray() : sortedBanners.ToArray(); //if bigOnTop then reverse the list to make the biggest in top
        for (int i = 0; i<newBanners.Length; i++)
        {
            newBanners[i].place = i + 1;
        }
        /*foreach (PlayerBanner ppbb in newBanners)
        {
            if (ppbb.isPlayer)
            {
                Debug.Log("Has player");
            }
        }*/
        return newBanners;

    }

    //mode 0 = hide leaderboard
    //mode 1 = normal 8 person leaderboard with best marks
    //mode 2 = olympic and world records and personal bests
    //mode 3 = current jumps
    //mode 4 = 3 best marks for all players
    //mode 5 = 3 attempt jumps 
    private void setLeaderboard(PlayerBanner[] playerBanners, int mode) //sets the leaderboard according to the array of playerBanners that it is given
    {
        for (int i = 0; i < playerBanners.Length; i++) //make all banners appear
        {
            leaderboardBanners[i].gameObject.SetActive(true);
        }
        for (int i=0; i<playerBanners.Length; i++)
        {
            for (int j = 0; j<leaderboardBanners[i].GetComponentsInChildren<RectTransform>(true).Length; j++)
            {
                if (j>1) leaderboardBanners[i].GetComponentsInChildren<RectTransform>(true)[j].gameObject.SetActive(false); //make the banner empty
            }
            leaderboardBanners[i].GetComponentsInChildren<RectTransform>(true)[1].gameObject.SetActive(true); 
            leaderboardBanners[i].GetComponentsInChildren<RectTransform>(true)[2].gameObject.SetActive(true); 
            leaderboardBanners[i].GetComponentsInChildren<RectTransform>(true)[3].gameObject.SetActive(true); 
            TextMeshProUGUI[] textBoxes = leaderboardBanners[i].GetComponentsInChildren<TextMeshProUGUI>(true);
            textBoxes[0].text = playerBanners[i].place.ToString(); //place text box
            //Add flags for leaderboard
            textBoxes[1].text = playerBanners[i].player; //playerName text box
            if (mode == 1)
            {
                leaderboardBanners[i].GetComponentsInChildren<RectTransform>(true)[4].gameObject.SetActive(true); //best mark label
                leaderboardBanners[i].GetComponentsInChildren<RectTransform>(true)[3].gameObject.GetComponent<Image>().sprite = flags[playerBanners[i].flagNumber]; //temp
                textBoxes[2].text = markToString(playerBanners[i].bestMark); //best mark text box
            }
            else if (mode == 2)
            {
                leaderboardBanners[i].GetComponentsInChildren<RectTransform>(true)[1].gameObject.SetActive(false); //record mark label
                leaderboardBanners[i].GetComponentsInChildren<RectTransform>(true)[3].gameObject.GetComponent<Image>().sprite = flags[playerBanners[i].flagNumber]; //temp
                leaderboardBanners[i].GetComponentsInChildren<RectTransform>(true)[5].gameObject.SetActive(true); //record mark label
                leaderboardBanners[i].GetComponentsInChildren<RectTransform>(true)[6].gameObject.SetActive(true); //record label mark
                textBoxes[3].text = markToString(playerBanners[i].bestMark); //setting record marks

            } else if (mode == 4)
            {
                leaderboardBanners[i].GetComponentsInChildren<RectTransform>(true)[1].gameObject.SetActive(false); //record mark label
                leaderboardBanners[i].GetComponentsInChildren<RectTransform>(true)[3].gameObject.GetComponent<Image>().sprite = flags[playerBanners[i].flagNumber]; //temp
                leaderboardBanners[i].GetComponentsInChildren<RectTransform>(true)[7].gameObject.SetActive(true); //record label mark
                leaderboardBanners[i].GetComponentsInChildren<RectTransform>(true)[8].gameObject.SetActive(true); //record mark label
                leaderboardBanners[i].GetComponentsInChildren<RectTransform>(true)[9].gameObject.SetActive(true); //record label mark
                textBoxes[4].text = markToString(playerBanners[i].mark1);
                textBoxes[5].text = markToString(playerBanners[i].mark2);
                textBoxes[6].text = markToString(playerBanners[i].mark3);

            }

        }
        for (int i = playerBanners.Length; i < 8; i++) //remove banners that are not needed
        {
            leaderboardBanners[i].gameObject.SetActive(false);
        }
        if (mode == 3)
        {
            TextMeshProUGUI[] textBoxes = personalBanner.GetComponentsInChildren<TextMeshProUGUI>(true);
            personalBanner.GetComponentsInChildren<RectTransform>(true)[3].gameObject.GetComponent<Image>().sprite = flags[personalBannersMarks.flagNumber]; //temp
            for (int j = 4; j < personalBanner.GetComponentsInChildren<RectTransform>(true).Length; j++)
            {
                if (j <=6 || j>9) personalBanner.GetComponentsInChildren<RectTransform>(true)[j].gameObject.SetActive(false); //make the banner empty
                //j==0 is the main banner
                // j <= 6; j > 9 is the bound for the items that are needed
            }
            textBoxes[4].text = markToString(personalBannersMarks.mark1);
            textBoxes[5].text = markToString(personalBannersMarks.mark2);
            textBoxes[6].text = markToString(personalBannersMarks.mark3);
            if (currentBarHeight != -10)
            {
                personalBanner.GetComponentsInChildren<Transform>(true)[10].gameObject.SetActive(true);
                personalBanner.GetComponentsInChildren<Transform>(true)[11].gameObject.SetActive(true);
                personalBanner.GetComponentsInChildren<Transform>(true)[12].gameObject.SetActive(true);
                personalBanner.GetComponentsInChildren<Transform>(true)[12].GetComponent<TextMeshProUGUI>().text = markToString(currentBarHeight);
            }

        } 

    }

    private string markToString(float mark, bool asTime=false)
    {
        if (mark == -10000)
        {
            return "X";
        }
        else if (mark == -100)
        {
            return "-";
        } else if (mark == -1000)
        {
            return "FOUL";
        } else if (mark == -10)
        {
            return "O";
        } else if (mark == -100000)
        {
            return "NH";
        }
        else
        {
            if (asTime)
            {
                return mark.ToString();
            } else
            {
                return ((int)mark / 12) + "' " + Math.Round(mark % 12, 2) + "''";
            }
        }
        
    }


}

//TODO Animations for the leaderboard
//TODO flags for the leaderboard
//TODO Make names list for players

//TODO add fouls for simulated oppenents
