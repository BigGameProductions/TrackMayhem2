using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System;

public class FifteenHundredManager : MonoBehaviour
{

    [SerializeField] private ItemStorage itemStorage;
    [SerializeField] private GameObject player;

    [SerializeField] private LeaderboardManager leaderboardManager;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private Camera birdEyeView;

    [SerializeField] private Image foulImage;
    [SerializeField] private TextMeshProUGUI setText;

    [SerializeField] private RunningMeterBar runningMeter;
    [SerializeField] private Slider energyBar;

    [SerializeField] private Image runningSliderFill;

    private LeaderboardFunctions leadF = new LeaderboardFunctions();

    [SerializeField] private float runningSpeedRatio; //stores the ratio for running compared to runningSpeed
    [SerializeField] private float animationRunningSpeedRatio; //stores the ratio for animation speed compared to runningSpeed

    [SerializeField] private Button runButton;
    [SerializeField] private Button jumpButton;
    [SerializeField] private Button infoButton;
    [SerializeField] private Canvas controlCanvas;

    [SerializeField] TextMeshProUGUI lapCounter;

    [SerializeField] private float energyDepletion;
    [SerializeField] private float energyGain;

    [SerializeField] private GameObject[] competitorsList;

    [SerializeField] private float[] laneZValues;

    private float[] competitorsSpeedList = new float[8]; //speed of all the competitors
    private float[] competitorsAccelSpeedList = new float[8]; //acceleration of all the competitors
    private float[] competitorsStartSpeedList = new float[8]; //start speed of all the competitors
    private float[] competitorsMaxSpeedList = new float[8]; //max speed of all the competitors

    private float[] competitorsLapTimeProgess = new float[8]; //lapTimeProgress for all runners
    private float[] competitorsLapNumber = new float[8]; //lap number of all competitors


    bool isRunning = false; //if gun has gone off
    bool runPressed = false; //if the run button is pressed
    bool jumpPressed = false; //if the jump button has been pressed
    bool started = false; //if the player has gone out of the blocks
    bool finished = false; //if the player has crossed the finish line
    bool usedLean = false; //if the player has used their lean
    bool didLap = false; //if the player has done a lap

    bool setRun = false; //if the player is doing the set run

    private int lapNumber = 0;

    float eventTimer = 0; //keeps track of the time of the event

    float startingBarDecreaseSpeed = 300; //gets the starting decrease speed

    private float playerTime;

    private float lapTimeProgress = 1f; //time of the current lap

    [SerializeField] private float zOffset;

    PlayerBanner currentPlayerBanner;

    PlayerBanner[] laneOrders;

    private int currentStartLane;

    public bool godMode;

    // Start is called before the first frame update
    void Start()
    {
        itemStorage.initRunner(PublicData.currentRunnerUsing, player.transform); //inits the runner into the current scene
        player.GetComponentsInChildren<Animator>()[1].applyRootMotion = false; //makes it so the player can run according to the animator
        controlCanvas.enabled = false;
        setText.enabled = false;
        foulImage.gameObject.SetActive(false);
        foulImage.GetComponentInChildren<TextMeshProUGUI>().text = "False Start";
        jumpButton.gameObject.SetActive(false);
        runButton.gameObject.SetActive(false);
        player.GetComponent<Animator>().speed = 0;
        runningMeter.runningBar.transform.parent.gameObject.SetActive(false);
        foreach (GameObject go in competitorsList) //gets all competitors in the blocks
        {
            go.GetComponentsInChildren<Animator>()[0].Play("Running");
            go.GetComponentsInChildren<Animator>()[0].speed = 0;
        }
        energyBar.gameObject.SetActive(false);
        player.GetComponent<Animator>().Play("FourHundredRun", 0, 0.18f);


    }

    public void buttonPressed(int code)
    {
        if (code == 0)
        {
            runPressed = true;
        }
        else if (code == 1)
        {
            jumpPressed = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (runningMeter.runningSpeed > 220)
        {
            if (energyBar.value > 0.1)
            {
                runningSliderFill.color = Color.blue;
            }
            else
            {
                runningSliderFill.color = Color.red;
            }
        }
        if (runningMeter.runningSpeed > 150 && runningMeter.runningSpeed <220)
        {
            if (energyBar.value > 0.1)
            {
                runningSliderFill.color = Color.green;
            }
            else
            {
                runningSliderFill.color = Color.red;
            }
        }
        if (runningMeter.runningSpeed < 150)
        {
            runningSliderFill.color = Color.yellow;
        }
        if (!leaderboardManager.cinematicCamera.gameObject.activeInHierarchy)
        {
            if (!runButton.gameObject.activeInHierarchy)
            {
                controlCanvas.enabled = true;
                runButton.gameObject.SetActive(true); //show controls
                energyBar.gameObject.SetActive(true);
                runningMeter.runningBar.transform.parent.gameObject.SetActive(true); //show controls
                laneOrders = leaderboardManager.getPlayersInLaneOrder();
                for (int i = 0; i < laneOrders.Length; i++)
                {
                    if (laneOrders[i].isPlayer)
                    {
                        player.transform.position = competitorsList[i].transform.position - new Vector3(-2.3f, 0, 0);
                        competitorsList[i].SetActive(false);
                        currentStartLane = i + 1;
                    }
                }
            }
            runningMeter.updateRunMeter();            
            if (player.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime < 0.8)
            {
                didLap = false;
            }
            /*if (isRunning && competitorsList[3].transform.position.x <= -2161.52)
             {
                 Debug.Log("Lane 4 " + eventTimer + ":" + leaderboardManager.getPlayersInLaneOrder()[3].bestMark);
                 finished = true;
             }*/
            if (isRunning && player.transform.position.x < -2125 && !finished && !usedLean)
            {
                jumpButton.gameObject.SetActive(true);
            }
            if ((Input.GetKeyDown(KeyCode.Space) || runPressed) && runningMeter.runMeterSlider.gameObject.activeInHierarchy && !finished && !foulImage.gameObject.activeInHierarchy) //updating speed on click
            {
                runPressed = false;
                infoButton.gameObject.SetActive(false);
                if (!isRunning)
                {
                    foulImage.gameObject.SetActive(true);
                    foulImage.GetComponent<Animator>().Play("FoulSlide");
                    runningMeter.runMeterSlider.gameObject.SetActive(false);
                    runButton.gameObject.SetActive(false);
                    isRunning = true;
                    runningMeter.runningSpeed = 800;
                    StartCoroutine(foulRun(2)); //wait then change screens
                }
                if (!started)
                {
                    started = true;
                    player.GetComponentsInChildren<Animator>()[1].Play("Running");
                    player.GetComponent<Animator>().Play("FourHundredRun");
                    player.GetComponent<Animator>().speed = 0.3f;
                }
                runPressed = false;
                runningMeter.increaseHeight();
                if (leaderboardManager.leaderBoardVisble()) //hides the leaderboard if the player clicks
                {
                    leaderboardManager.hidePersonalBanner();
                }
            }
            if ((Input.GetKeyDown(KeyCode.P) || jumpPressed) && runningMeter.runMeterSlider.gameObject.activeInHierarchy && !finished && !foulImage.gameObject.activeInHierarchy)
            {
                jumpPressed = false;
                usedLean = true;
                jumpButton.gameObject.SetActive(false);
                player.GetComponentsInChildren<Animator>()[1].Play("RunningLean");
            }
            if (!playerCamera.enabled)
            {
                playerCamera.enabled = true;
                setText.enabled = true;
                player.GetComponentsInChildren<Animator>()[1].Play("Running");
                player.GetComponentsInChildren<Animator>()[1].speed=0;
                setText.gameObject.transform.parent.GetComponent<Animator>().Play("FadeText");
                setText.gameObject.transform.parent.GetComponent<Animator>().speed = 0.5f;
                StartCoroutine(showSet(3));
            }
        }
    }

    IEnumerator showSet(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (!foulImage.gameObject.activeInHierarchy)
        {
            setText.text = "Set";
            setText.gameObject.transform.parent.GetComponent<Animator>().Play("FadeText");
            setText.gameObject.transform.parent.GetComponent<Animator>().speed = 0.5f;
            setRun = true;
            StartCoroutine(setRunning(0.5f));
            player.GetComponentsInChildren<Animator>()[1].speed = 1;
            foreach (GameObject go in competitorsList) //gets all competitors in the blocks
            {
                go.GetComponentsInChildren<Animator>()[0].speed = 1;
            }
            /*foreach (GameObject go in competitorsList) //gets all competitors up in the set position
            {
                if (go.activeInHierarchy) //stop animator warning
                {
                    go.GetComponentsInChildren<Animator>()[1].Play("BlockStartUp");
                }
            }
            player.GetComponentsInChildren<Animator>()[1].Play("BlockStartUp");*/
            leaderboardManager.getOtherRunnersTime(); //gets the time for the other runners
            StartCoroutine(showGo(UnityEngine.Random.Range(1.0f, 2.5f)));
        }

    }

    IEnumerator showGo(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (!foulImage.gameObject.activeInHierarchy)
        {
            isRunning = true;
            setText.text = "GO";
            setText.gameObject.transform.parent.GetComponent<Animator>().Play("FadeText");
            setText.gameObject.transform.parent.GetComponent<Animator>().speed = 0.5f;
            lapCounter.gameObject.SetActive(true);
            for (int i = 0; i < competitorsLapTimeProgess.Length; i++)
            {
                competitorsLapTimeProgess[i] = 1;
            }
            for (int i = 0; i < competitorsList.Length; i++)
            {
                StartCoroutine(oppenentBlockStart(UnityEngine.Random.Range(0.1f, 0.5f), i));
                //StartCoroutine(oppenentBlockStart(0.2f, i));
            }

        }

    }


    IEnumerator oppenentBlockStart(float delay, int index)
    {
        yield return new WaitForSeconds(delay);
        if (competitorsList[index].activeInHierarchy) //stop animator warning
        {
            competitorsList[index].GetComponentsInChildren<Animator>()[0].Play("Running");
            competitorsList[index].GetComponentsInChildren<Animator>()[0].speed = 1;
            /*float time = leaderboardManager.getPlayersInLaneOrder()[index].bestMark;
            //float time = 13.87f;
            time -= delay;
            competitorsMaxSpeedList[index] = (275.05f / ((0.75f * time) / 0.02f)) / runningSpeedRatio;//spaces per second was 0.55
            competitorsAccelSpeedList[index] = time / 6; //was 0.4
            competitorsStartSpeedList[index] = 90;*/
            //competitorsSpeedList[index] = UnityEngine.Random.Range(0.005f, 0.01f);
            float theBestMark = leaderboardManager.getPlayersInLaneOrder()[index].bestMark;
            float secDiffPercent = theBestMark / 180f;
            competitorsSpeedList[index] = 0.01001f/secDiffPercent;
        }

    }

    IEnumerator foulRun(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (setText.text != "Set")
        {
            leaderboardManager.getOtherRunnersTime(); //gets the time for the other runners
        }
        leaderboardManager.addPlayerTime(0);
        SceneManager.LoadScene("EndScreen");

    }

    IEnumerator setRunning(float length)
    {
        yield return new WaitForSeconds(length);
        setRun = false;
        player.GetComponentsInChildren<Animator>()[1].speed = 0;
        foreach (GameObject go in competitorsList) //gets all competitors in the blocks
        {
            go.GetComponentsInChildren<Animator>()[0].speed = 0;
        }
    }

    private void updateRunnerPosition(GameObject runner, float lapTime)
    {
        float speed = 1;
        if (lapTime < 1)
        {
            float circleRad = (308.8f - 16.39f) / 2.0f;//start and end pos
            Vector3 ogPos = new Vector3(-2162.03f, 226.73f, circleRad * -1); //start is -16.39
            Vector3 unitCirclePos = new Vector3((float)Math.Cos(Math.PI * lapTime), runner.transform.position.y, (float)Math.Sin(Math.PI * lapTime));
            //player.transform.position = ogPos;
            runner.transform.position = new Vector3(circleRad * unitCirclePos.z * -1, 0, (circleRad * unitCirclePos.x) + zOffset) + ogPos;
            runner.transform.eulerAngles = new Vector3(0, 270 - (180 * lapTime), 0);
        }
        else if (lapTime < 2)
        {
            //-1832 for hundred meter end
            //-2162.03 for start
            float diff = 2162.03f - 1832f;

            runner.transform.position = new Vector3(-2162.03f + (diff * (lapTime - 1)), runner.transform.position.y, (float)(runner.transform.position.z < -308.22 && speed > 0 ? runner.transform.position.z + 0.3 : runner.transform.position.z));
        }
        else if (lapTime < 3)
        {
            float circleRad = (308.22f - 16.39f) / 2.0f;//start and end pos
            Vector3 ogPos = new Vector3(-1832, 226.73f, circleRad * -1); //start is -16.39
            Vector3 unitCirclePos = new Vector3((float)Math.Cos(Math.PI * lapTime), runner.transform.position.y, (float)Math.Sin(Math.PI * lapTime));
            //player.transform.position = ogPos;
            runner.transform.position = new Vector3(circleRad * unitCirclePos.z, 0, (circleRad * -1 * unitCirclePos.x) + zOffset) + ogPos;
            runner.transform.eulerAngles = new Vector3(0, 90 - (180 * (lapTime - 2)), 0);
        }
        else if (lapTime < 4)
        {
            float diff = 2162.03f - 1832f;
            runner.transform.position = new Vector3(-1832 - (diff * (lapTime - 3)), runner.transform.position.y, runner.transform.position.z);
        }
    }

    private void FixedUpdate()
    {
        if (setRun)
        {
            player.transform.Translate(new Vector3(0, 0, 0.5f));
            foreach (GameObject go in competitorsList) //gets all competitors in the blocks
            {
                go.transform.Translate(new Vector3(0, 0, 0.5f));
            }
        }
        if ((playerCamera.enabled && isRunning) || finished)
        {
            float speed = runningMeter.runningSpeed;
            if (godMode) {
                if (energyBar.value > 0)
                {
                    speed = PublicData.averageSpeedDuringRun;
                } else
                {
                    speed = 150;
                }
            }
            if (speed > PublicData.averageSpeedDuringRun - 25 && energyBar.value == 0)
            {
                speed = PublicData.averageSpeedDuringRun -25 - (speed - (PublicData.averageSpeedDuringRun-25)); //makes it so over slows you down
            }
            if (speed > 150 && energyBar.value != 0)
            {
                energyBar.value -= energyDepletion*((speed-150)/150);
                if (energyBar.value < 0.05f) energyBar.value = 0;
            }
            //player.transform.Translate(new Vector3(0, 0, speed * runningSpeedRatio)); //making character move according to run meter
            if (!finished)
            {
                updateRunnerPosition(player, lapTimeProgress);
            }
            for (int i=0; i< competitorsLapTimeProgess.Length; i++)
            {
                if (i+1 != currentStartLane && competitorsLapNumber[i] < 4)
                {
                    updateRunnerPosition(competitorsList[i], competitorsLapTimeProgess[i]);
                }
            }
            if (birdEyeView.enabled)
            {
                player.GetComponent<Animator>().speed = speed / 200f;
            }
            player.GetComponentsInChildren<Animator>()[1].speed = speed * animationRunningSpeedRatio; //making the animation match the sunning speed
            for (int i = 0; i < competitorsList.Length; i++)
            {
                //float time = 13.87f;
                float time = leaderboardManager.getPlayersInLaneOrder()[i].bestMark;
                /*if (competitorsMaxSpeedList[i] != competitorsSpeedList[i] && competitorsList[i].transform.position.x > -2122.37) //if not max speed
                {
                    if (eventTimer >= competitorsAccelSpeedList[i]) //if past max speed on time
                    {
                        competitorsMaxSpeedList[i] = ((2161.52f + competitorsList[i].transform.position.x) / ((time - eventTimer) / 0.02f)) / runningSpeedRatio;//spaces per second was 0.55
                        competitorsSpeedList[i] = competitorsMaxSpeedList[i]; //set to max speed
                    }
                    else
                    {
                        competitorsSpeedList[i] = competitorsStartSpeedList[i] + (competitorsMaxSpeedList[i] * eventTimer * eventTimer) / ((time / 6) * (time / 6)); //acceleration
                    }
                }
                else if (competitorsList[i].transform.position.x <= -2122.37)
                {
                    competitorsSpeedList[i] = competitorsMaxSpeedList[i] + 0;
                }
                competitorsList[i].transform.Translate(new Vector3(0, 0, competitorsSpeedList[i] * runningSpeedRatio)); //making character move according to run meter
                competitorsList[i].GetComponentsInChildren<Animator>()[0].speed = competitorsSpeedList[i] * animationRunningSpeedRatio; //making the animation match the sunning speed
                /*if (competitorsList[i].transform.position.x > -2161.52 && competitorsList[i].transform.position.x < -2157 && competitorsList[i].GetComponentsInChildren<Animator>()[1].GetCurrentAnimatorStateInfo(0).IsName("Running"))
                {
                    competitorsList[i].GetComponentsInChildren<Animator>()[0].Play("RunningLean");
                }*/
            }
            eventTimer += Time.deltaTime;
            for (int i = 0; i < competitorsLapTimeProgess.Length; i++)
            {
                competitorsLapTimeProgess[i] += competitorsSpeedList[i];
                if (competitorsLapTimeProgess[i] >= 4 && competitorsLapNumber[i] < 4)
                {
                    competitorsLapNumber[i]++;
                    if (competitorsLapNumber[i] == 4)
                    {
                        PlayerBanner pb = laneOrders[Int32.Parse(competitorsList[i].name[4].ToString()) - 1];
                        leaderboardManager.changeBannerBest(pb.flagNumber, pb.player, eventTimer * 6);
                    }
                    else
                    {
                        competitorsLapTimeProgess[i] = 0;
                    }
                } else if (competitorsLapTimeProgess[i] >= 4 && competitorsLapNumber[i] == 4)
                {
                    competitorsList[i].transform.Translate(0, 0, competitorsSpeedList[i] * 100);
                    competitorsSpeedList[i] -= 0.005f;
                }
            }
            if (!finished)
            {
                runningMeter.updateTimeElapsed();
                float speedAdjuster = 33000;
                speedAdjuster -= PublicData.curveValue(PublicData.getCharactersInfo(PublicData.currentRunnerUsing).strengthLevel, 1500);
                speedAdjuster -= PublicData.curveValue(PublicData.getCharactersInfo(PublicData.currentRunnerUsing).speedLevel, 2500);
                speedAdjuster -= PublicData.curveValue(PublicData.getCharactersInfo(PublicData.currentRunnerUsing).agilityLevel, 3500);
                speedAdjuster -= PublicData.curveValue(PublicData.getCharactersInfo(PublicData.currentRunnerUsing).flexabilityLevel, 5500);
                energyDepletion = speedAdjuster / 12500000; //makes energy match the speed
                lapTimeProgress += speed / speedAdjuster; //normal mode
                
                //lapTimeProgress += 0.005f; //fast mode
                lapTimeProgress = Math.Min(lapTimeProgress, 4); //cap progress at 4
                if (lapTimeProgress == 4)
                {
                    lapNumber++;
                    lapCounter.text = "Lap " + lapNumber + "/4";
                    if (lapNumber == 4 && !finished)
                    {
                        finished = true;
                        playerTime = eventTimer * 6;
                        StartCoroutine(waitAfterFinish(2));
                    }
                    else
                    {
                        lapTimeProgress = 0;
                    }
                }
                
                //runningMeter.barDecreaseSpeed = Math.Min(startingBarDecreaseSpeed + (eventTimer * 20), 400); //make the bar decrease faster as you go on
                //runningMeter.speedPerClick = 75 + (eventTimer * 5);
            } else
            {
                player.transform.Translate(0,0,speed/100);
                //speed -= 2;
            }
        }

    }

    IEnumerator waitAfterFinish(float delay)
    {
        yield return new WaitForSeconds(delay);
        currentPlayerBanner = leaderboardManager.getPlayerBanner();
        currentPlayerBanner.mark2 = playerTime;
        PersonalBests characterPB = PublicData.getCharactersInfo(PublicData.currentRunnerUsing).characterBests;
        int leaderboardTime = PublicData.maxInteger - ((int)(playerTime * 100));
        if (leaderboardTime > Int32.Parse(PublicData.gameData.leaderboardList[9][1][0])) //game record
        {
            leadF.SetLeaderBoardEntry(9, PublicData.gameData.playerName, leaderboardTime, PublicData.gameData.countryCode + "," + PublicData.currentRunnerUsing);
            leadF.checkForOwnPlayer(9, 20); //checks to make sure it can stay in the top 20
            PublicData.gameData.personalBests.mile = playerTime;
            PublicData.getCharactersInfo(PublicData.currentRunnerUsing).characterBests.mile = playerTime;
            leaderboardManager.addMarkLabelToPlayer(1);
            leaderboardManager.showRecordBanner(2);
        }
        else if (playerTime < PublicData.gameData.personalBests.mile || PublicData.gameData.personalBests.mile == 0) //if pr or first time doing it
        {
            leadF.SetLeaderBoardEntry(9, PublicData.gameData.playerName, leaderboardTime, PublicData.gameData.countryCode + "," + PublicData.currentRunnerUsing);
            leadF.checkForOwnPlayer(9, 20); //checks to make sure it can stay in the top 20
            PublicData.gameData.personalBests.mile = playerTime; //sets pr
            PublicData.getCharactersInfo(PublicData.currentRunnerUsing).characterBests.mile = playerTime; //sets cb too
            leaderboardManager.addMarkLabelToPlayer(3);
            leaderboardManager.showRecordBanner(1);

        }
        else if (playerTime < characterPB.mile || characterPB.mile == 0)
        {
            PublicData.getCharactersInfo(PublicData.currentRunnerUsing).characterBests.mile = playerTime;
            leaderboardManager.addMarkLabelToPlayer(2);
            leaderboardManager.showRecordBanner(0);
        }
        leaderboardManager.showCurrentPlayerMarks(currentPlayerBanner, 3); //updates and shows the player leaderboard
        leaderboardManager.addPlayerTime(playerTime); //adds player to the banners
        StartCoroutine(showEndScreen(3));

    }

    IEnumerator showEndScreen(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene("EndScreen");
    }

}


