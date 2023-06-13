using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.IO;

public class LeaderboardManager : MonoBehaviour
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

    [SerializeField] private Image pictogramImage;

    private PlayerBanner[] currentEventBanners;

    private List<PlayerBanner> finalBarHeightsBanners = new List<PlayerBanner>();

    private float currentBarHeight = -10;
    private float openingHeight = 0;

    [SerializeField] private ItemStorage itemStorage;
    [SerializeField] private TextMeshProUGUI eventNameTitle;
    [SerializeField] private TextMeshProUGUI leaderboardDescriptionTitle;

    [SerializeField] private Image recordBanner; //banner for showing records

    private PersonalBests personalBests; //store personals bests for seeding
    private string playerName; //store the name of the player
    private int animationStage = 0; //stores the stage for the cinematic camera before an event

    private float basedSeed; //stores the based seed for the current event

    private PlayerBanner personalBannersMarks; //stores the given set of marks for the event

    private bool useTime; //shows if time is being used for the current event

    /*public void LoadData(GameData data)
    {
        this.personalBests = data.personalBests; //load the personal bests
        this.playerName = data.playerName; //name of the player
    }

    public void SaveData(ref GameData data)
    {
        //nothing to save
    }*/

    private void Start()
    {

        
        recordBanner.gameObject.SetActive(false);
        if (SceneManager.GetActiveScene().name != "EndScreen") //tests to make sure it is an event screen
        {
            cinematicCamera.GetComponent<Animator>().speed = 1;
            PublicData.currentEventName = eventName;
        } else
        {
            gameObject.GetComponentInChildren<Animator>().Play("Nothing");
            eventName = PublicData.currentEventName;
            useTime = PublicData.usesTime;
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
            if (Input.GetMouseButtonDown(0)) //if clicked
            {
                if (animationStage < 3 && animationStage != 0) //if the stage in the begginging
                {
                    cinematicCamera.GetComponent<Animator>().SetBool("Skip", true); //skip the stage
                }
            }
        }
        
    }

    public void getOtherRunnersTime()
    {
        currentEventBanners = simulateRunMark(currentEventBanners, "HundredMeter", seedSpreadUp, seedSpreadDown);
    }

    public void addPlayerTime(float time)
    {
        foreach (PlayerBanner pb in currentEventBanners)
        {
            if (pb.isPlayer) //making sure the banner is not for the player
            {
                pb.bestMark = (float)Math.Round(time, 2); //rounds to the nearest two places for leaderboard
            }

        }
        PublicData.playerBannerTransfer = sortBanners(currentEventBanners, false);
        PublicData.leaderBoardMode = 1;
        PublicData.usesTime = true;
    }

    public void addMarkLabelToPlayer(int idLabel) //adds a mark label to the current player
    {
        foreach (PlayerBanner pb in currentEventBanners)
        {
            if (pb.isPlayer)
            {
                pb.markLabelID = idLabel;
            }
        }
    }

    public PlayerBanner[] getPlayersInLaneOrder() //seeds current banners into running lanes
    {
        PlayerBanner[] returnList = new PlayerBanner[8];
        currentEventBanners = sortBanners(currentEventBanners, false);
        returnList[0] = currentEventBanners[6];
        returnList[1] = currentEventBanners[4];
        returnList[2] = currentEventBanners[2];
        returnList[3] = currentEventBanners[0];
        returnList[4] = currentEventBanners[1];
        returnList[5] = currentEventBanners[3];
        returnList[6] = currentEventBanners[5];
        returnList[7] = currentEventBanners[7];
        return returnList;

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
        gameObject.GetComponentInChildren<Animator>().Play("CurrentStandingsAnimationOut");
        StartCoroutine(waitForAnimationOut(0.5f));
    }

    IEnumerator waitForAnimationOut(float delay)
    {
        yield return new WaitForSeconds(delay);
        updateCinematicStage(0);
    }

    public bool leaderBoardVisble() //returns if leaderboard is visible
    {
        return leaderBoardHeader.activeInHierarchy;
    }

    //-1 is for hiding
    public void showRecordBanner(int id)
    {
        if (id == -1)
        {
            recordBanner.gameObject.SetActive(false);
        } else
        {
            recordBanner.gameObject.SetActive(true);
            recordBanner.GetComponent<Animator>().Play("SlideDown");
            string[] titles = new string[] { "Character Best", "Personal Record", "Game Record" };
            recordBanner.GetComponentInChildren<TextMeshProUGUI>().text = titles[id];
            recordBanner.color = itemStorage.bannerColors[id];

        }
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
        playerName = PublicData.gameData.playerName; //setting vars //TODO change location
        personalBests = PublicData.gameData.personalBests; //setting vars
        if (stage==0) //hides the leaderboard when it should not be shown
        {
            leaderBoardHeader.SetActive(false);
            personalBanner.SetActive(false);
            gameObject.GetComponentInChildren<Animator>().Play("Nothing");
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
            new PlayerBanner(0, itemStorage.findFlagIndexOfCountry(currentRecordInfo[4]), currentRecordInfo[1], float.Parse(currentRecordInfo[2])),
            new PlayerBanner(0, itemStorage.findFlagIndexOfCountry(PublicData.gameData.leaderboardList[getEventID(eventName)][2][0].Split(",")[0]), PublicData.gameData.leaderboardList[getEventID(eventName)][0][0], Int32.Parse(PublicData.gameData.leaderboardList[getEventID(eventName)][1][0])/100.0f),
            new PlayerBanner(0, itemStorage.findFlagIndexOfCountry(PublicData.gameData.countryCode), playerName, getMarkForEvent(eventName, true))
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
                PublicData.playerBannerTransfer = sortBanners(convertAttemptsToBestMark(currentEventBanners), true); //sets the end screen leaderboard to match the current one
                PublicData.leaderBoardMode = 1; //sets leaderboard mode for the end screen
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

    private PlayerBanner[] convertAttemptsToBestMark(PlayerBanner[] playerBanners)
    {
        foreach (PlayerBanner pb in playerBanners)
        {
            if (pb.mark1 != -100 && pb.mark2 != -100 && pb.mark3 != -100)
            {
                float bestAttempt = Math.Max(pb.mark1, Math.Max(pb.mark2, pb.mark3));
                if (bestAttempt > pb.bestMark && !pb.isPlayer)
                {
                    pb.markLabelID = 3; //set pr for oppenent
                }
                pb.bestMark = bestAttempt;
            }

        }
        return playerBanners;
    }

    private string[] getEventRecordByEvent(string ev) {
        for (int i=0; i<PublicData.recordsInfo.Count; i++) //loops through csv file
        {
            if (i!=0) //avoids header
            {
                if (PublicData.recordsInfo.ElementAt(i)[0] == ev) //checks if the event matches
                {
                    if (PublicData.recordsInfo.ElementAt(i)[3] == "FALSE") //checks for time or feet
                    {
                        useTime = true; //sets it to time
                    }
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
            banners[i] = new PlayerBanner(i, flagNum, name, useTime ? personalBest:roundToNearest(0.25f, personalBest)); //round personal bests to only two places except times

        }
        banners[banners.Length - 1] = new PlayerBanner(0, itemStorage.findFlagIndexOfCountry(PublicData.gameData.countryCode), playerName, getMarkForEvent(SceneManager.GetActiveScene().name, true), isPlayer:true);;
        return sortBanners(banners, !useTime, currentBarHeight!=-10); //sorting banners based on bar events
    }

    //theEvent is the event that is used
    //useSeeded is if seeding on personal bests should be used
    //seedSpread[Down/Up] is the spread of the seeding from the original down and up from that mark
    //basedSeed is optional for if useSeeded is false what to base seeding around
    private float seedTimesForEvent(string theEvent, bool useSeeded, float seedSpreadDown, float seedSpreadUp) //based on the event it gives a random seed based on that number
    {
        if (useSeeded)
        {
            basedSeed = getMarkForEvent(theEvent, true);
        }
        float finalMark = roundToNearest(seedRoundInches, UnityEngine.Random.Range((float)basedSeed - seedSpreadDown, basedSeed + seedSpreadUp + 1f));
        if (useTime)
        {
            return finalMark / 100.0f; //converts hundreths of second to seconds
        } else
        {
            return finalMark;
        }

    }

    public float getMarkForEvent(string theEvent, bool character) //returns the player mark in the save files for the given event
    {
        if (theEvent == "LongJump")
        {
            return character ? PublicData.getCharactersInfo(PublicData.currentRunnerUsing).characterBests.longJump:PublicData.gameData.personalBests.longJump;

        } else if (theEvent == "PoleVault")
        {
            return character ? PublicData.getCharactersInfo(PublicData.currentRunnerUsing).characterBests.polevault:PublicData.gameData.personalBests.polevault;
        }
        else if (theEvent == "HundredMeter")
        {
            return character ? PublicData.getCharactersInfo(PublicData.currentRunnerUsing).characterBests.hundredMeter:PublicData.gameData.personalBests.hundredMeter;
        }
        else if (theEvent == "Shotput")
        {
            return character ? PublicData.getCharactersInfo(PublicData.currentRunnerUsing).characterBests.shotput : PublicData.gameData.personalBests.shotput;
        }
        else if (theEvent == "Javelin")
        {
            return character ? PublicData.getCharactersInfo(PublicData.currentRunnerUsing).characterBests.javelin : PublicData.gameData.personalBests.javelin;
        }
        else if (theEvent == "FourHundred")
        {
            return character ? PublicData.getCharactersInfo(PublicData.currentRunnerUsing).characterBests.fourHundred : PublicData.gameData.personalBests.fourHundred;
        }
        else if (theEvent == "Discus")
        {
            return character ? PublicData.getCharactersInfo(PublicData.currentRunnerUsing).characterBests.discus : PublicData.gameData.personalBests.discus;
        }
        else if (theEvent == "Hurdles")
        {
            return character ? PublicData.getCharactersInfo(PublicData.currentRunnerUsing).characterBests.hurdles : PublicData.gameData.personalBests.hurdles;
        }
        return 0;
    }

    private int getEventID(string theEvent) { //gets the event id of a string event
        for (int i=0; i<PublicData.recordsInfo.Count; i++)
        {
            if (PublicData.recordsInfo.ElementAt(i)[0] == theEvent)
            {
                return i - 1;
            }
        }
        return -1;
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
                if (pb.bestMark-currentBarHeight < 0 && make) //if oppenent has had a pr
                {
                    pb.markLabelID = 3; //set pr in the player banner
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

    private PlayerBanner[] simulateRunMark(PlayerBanner[] playerBanners, string theEvent, float spreadUp, float spreadDown)
    {
        foreach (PlayerBanner pb in playerBanners)
        {
            if (!pb.isPlayer) //making sure the banner is not for the player
            {
                float newMark = UnityEngine.Random.Range((float)pb.bestMark - spreadDown / 100, pb.bestMark + spreadUp / 100 + 1f);
                newMark = (float)Math.Round(newMark, 2); //make the speed go only 2 digits out
                if (newMark < pb.bestMark)
                {
                    pb.markLabelID = 3;
                }
                pb.bestMark = newMark;
                
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
        List<PlayerBanner> removedBanners = new List<PlayerBanner>();
        for (int i=0; i<sortedBanners.Count; i++)
        {
            if (sortedBanners.ElementAt(i).bestMark == 0) //if it is a fs or fault
            {
                removedBanners.Add(sortedBanners.ElementAt(i)); //adds to remove list
                sortedBanners.RemoveAt(i); //removes it
                i--; //keeps index in line
            }
        }
        foreach (PlayerBanner playerB in removedBanners) //adds removed banners in back or front for reverse
        {
            if (bigOnTop) //if reversing the list
            {
                sortedBanners.Insert(0,playerB);
            } else
            {
                sortedBanners.Add(playerB);
            }
        }
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
        //titles
        eventNameTitle.text = "Open " + itemStorage.eventNames[getEventID(eventName)];
        pictogramImage.sprite = itemStorage.pictogramSprites[getEventID(eventName)];
        pictogramImage.GetComponent<RectTransform>().sizeDelta = itemStorage.pictogramSizes[getEventID(eventName)];
        if (SceneManager.GetActiveScene().name != "EndScreen" && mode != 3 && mode !=2)
        {
            if (!gameObject.GetComponentInChildren<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Nothing"))
            {
                gameObject.GetComponentInChildren<Animator>().Play("LeaderboardBannerSlide");
            } else
            {
                gameObject.GetComponentInChildren<Animator>().Play("CurrentStandingsAnimation");
            }

        }
        if (mode == 2)
        {
            leaderboardDescriptionTitle.text = "Records";
        } else
        {
            if (SceneManager.GetActiveScene().name == "EndScreen")
            {
                leaderboardDescriptionTitle.text = "Results - Final";
            } else if (cinematicCamera.gameObject.activeInHierarchy)
            {
                leaderboardDescriptionTitle.text = "Start List - Final";
            } else
            {
                leaderboardDescriptionTitle.text = "Current Standings";
            }
        }
        //titles
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
            textBoxes[0].gameObject.SetActive(true); //show the place numbers
            //Add flags for leaderboard
            textBoxes[1].text = playerBanners[i].player; //playerName text box
            if (mode == 1)
            {
                leaderboardBanners[i].GetComponentsInChildren<RectTransform>(true)[4].gameObject.SetActive(true); //best mark label
                leaderboardBanners[i].GetComponentsInChildren<RectTransform>(true)[3].gameObject.GetComponent<Image>().sprite = itemStorage.flags[playerBanners[i].flagNumber]; //temp
                if (playerBanners[i].markLabelID != -1)
                {
                    leaderboardBanners[i].GetComponentsInChildren<RectTransform>(true)[6].gameObject.GetComponent<Image>().sprite = itemStorage.labelSprites[playerBanners[i].markLabelID];
                    leaderboardBanners[i].GetComponentsInChildren<RectTransform>(true)[6].gameObject.GetComponent<Image>().gameObject.SetActive(true);
                } else
                {
                    leaderboardBanners[i].GetComponentsInChildren<RectTransform>(true)[6].gameObject.GetComponent<Image>().gameObject.SetActive(false);
                }
                textBoxes[2].text = markToString(playerBanners[i].bestMark, useTime); //best mark text box
            }
            else if (mode == 2)
            {
                leaderboardBanners[i].GetComponentsInChildren<RectTransform>(true)[1].gameObject.SetActive(false); //record mark label
                leaderboardBanners[i].GetComponentsInChildren<RectTransform>(true)[3].gameObject.GetComponent<Image>().sprite = itemStorage.flags[playerBanners[i].flagNumber]; //temp
                leaderboardBanners[i].GetComponentsInChildren<RectTransform>(true)[5].gameObject.SetActive(true); //record mark label
                leaderboardBanners[i].GetComponentsInChildren<RectTransform>(true)[6].gameObject.SetActive(true); //record label mark
                leaderboardBanners[i].GetComponentsInChildren<RectTransform>(true)[6].GetComponent<Image>().sprite = itemStorage.labelSprites[i]; //sets mark labels to each character
                textBoxes[3].text = markToString(playerBanners[i].bestMark, useTime); //setting record marks

            } else if (mode == 4)
            {
                leaderboardBanners[i].GetComponentsInChildren<RectTransform>(true)[1].gameObject.SetActive(true); //record mark label
                leaderboardBanners[i].GetComponentsInChildren<RectTransform>(true)[3].gameObject.GetComponent<Image>().sprite = itemStorage.flags[playerBanners[i].flagNumber]; //temp
                leaderboardBanners[i].GetComponentsInChildren<RectTransform>(true)[7].gameObject.SetActive(true); //record label mark
                leaderboardBanners[i].GetComponentsInChildren<RectTransform>(true)[8].gameObject.SetActive(true); //record mark label
                leaderboardBanners[i].GetComponentsInChildren<RectTransform>(true)[9].gameObject.SetActive(true); //record label mark
                textBoxes[4].text = useTime? "":markToString(playerBanners[i].mark1);
                textBoxes[5].text = markToString(playerBanners[i].mark2, useTime);
                textBoxes[6].text = useTime ? "":markToString(playerBanners[i].mark3);

            }

        }
        for (int i = playerBanners.Length; i < 8; i++) //remove banners that are not needed
        {
            leaderboardBanners[i].gameObject.SetActive(false);
        }
        if (SceneManager.GetActiveScene().name != "EndScreen" && mode == 2) //animation after the banners are hidden
        {
            gameObject.GetComponentInChildren<Animator>().Play("LeaderboardBannerThreeSlide");

        }   
        if (mode == 3)
        {
            gameObject.GetComponentInChildren<Animator>().Play("PersonalBannerSlide");
            TextMeshProUGUI[] textBoxes = personalBanner.GetComponentsInChildren<TextMeshProUGUI>(true);
            personalBanner.GetComponentsInChildren<RectTransform>(true)[3].gameObject.GetComponent<Image>().sprite = itemStorage.flags[personalBannersMarks.flagNumber]; //temp
            for (int j = 4; j < personalBanner.GetComponentsInChildren<RectTransform>(true).Length; j++)
            {
                if ((j <=6 || j>9) && j!=0) personalBanner.GetComponentsInChildren<RectTransform>(true)[j].gameObject.SetActive(false); //make the banner empty
                //j==0 is the main banner
                // j <= 6; j > 9 is the bound for the items that are needed
            }
            textBoxes[4].text = useTime ? "":markToString(personalBannersMarks.mark1, useTime); //hides if time
            textBoxes[5].text = markToString(personalBannersMarks.mark2, useTime);
            textBoxes[6].text = useTime ? "":markToString(personalBannersMarks.mark3, useTime); //hides if time
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
                if (mark == 0)
                {
                    return "FS";
                } else if (mark >= 60) {
                    int min = (int)(mark / 60);
                    float seconds = mark - (min * 60);
                    return min + ":" + Math.Round(seconds, 2).ToString();
                } else
                {
                    return Math.Round(mark, 2).ToString();
                }
            } else
            {
                return ((int)mark / 12) + "' " + Math.Round(mark % 12, 2) + "''";
            }
        }
        
    }


}

//TODO Animations for the leaderboard
//TODO Make zeros on end of
//TODO make 0 not FS for first play

//TODO add fouls for simulated oppenents
