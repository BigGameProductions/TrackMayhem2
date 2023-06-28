using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System;

public class FourHundredManager : MonoBehaviour
{

    [SerializeField] private ItemStorage itemStorage;
    [SerializeField] private GameObject player;

    [SerializeField] private LeaderboardManager leaderboardManager;
    [SerializeField] private Camera playerCamera;

    [SerializeField] private Image foulImage;
    [SerializeField] private TextMeshProUGUI setText;

    [SerializeField] private RunningMeterBar runningMeter;
    [SerializeField] private Slider energyBar;

    [SerializeField] private Image runningSliderFill;

    [SerializeField] private float runningSpeedRatio; //stores the ratio for running compared to runningSpeed
    [SerializeField] private float animationRunningSpeedRatio; //stores the ratio for animation speed compared to runningSpeed

    [SerializeField] private Button runButton;
    [SerializeField] private Button jumpButton;
    [SerializeField] private Button infoButton;
    [SerializeField] private Canvas controlCanvas;

    [SerializeField] private float energyDepletion;
    [SerializeField] private float energyGain;

    [SerializeField] private GameObject[] competitorsList;

    [SerializeField] private float[] laneZValues;

    PlayerBanner[] laneOrders;

    private float[] competitorsSpeedList = new float[8]; //speed of all the competitors
    private float[] competitorsAccelSpeedList = new float[8]; //acceleration of all the competitors
    private float[] competitorsStartSpeedList = new float[8]; //start speed of all the competitors
    private float[] competitorsMaxSpeedList = new float[8]; //max speed of all the competitors
    private bool[] competitorsMarkedTimeList = new bool[8]; //max speed of all the competitors


    private float[] competitorsLapTimes = new float[8]; //lap time for all the competitors


    bool isRunning = false; //if gun has gone off
    bool runPressed = false; //if the run button is pressed
    bool jumpPressed = false; //if the jump button has been pressed
    bool started = false; //if the player has gone out of the blocks
    bool finished = false; //if the player has crossed the finish line
    bool usedLean = false; //if the player has used their lean

    float eventTimer = 0; //keeps track of the time of the event

    float lapTimeProgress = 0;

    public int currentLane = 3;
    public float laneSpace = 2.3f;

    float startingBarDecreaseSpeed = 300; //gets the starting decrease speed

    public float laneTimeOffset;

    public float zOffset;

    PlayerBanner currentPlayerBanner;

    private float playerTime;

    [SerializeField] ParticleSystem dashParticles;
    [SerializeField] ParticleSystem jumpSparkle;

    public bool godMode;

    [SerializeField] private GameObject fourHundredBlocks;
    [SerializeField] private float[] stagger200Marks;

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
        for (int i=0; i<competitorsList.Length; i++)
        {
            competitorsList[i].GetComponentsInChildren<Animator>()[0].Play("BlockStart");
            setStartingPosition(i + 1, false, competitorsList[i].transform);

        }
        energyBar.gameObject.SetActive(false);
        setText.gameObject.transform.parent.GetComponent<Animator>().speed = 0;

    }

    private void setStartingPosition(int lanePosition, bool isPlayer, Transform tf)
    {
        if (isPlayer)
        {
            lanePosition = currentLane;
            lapTimeProgress = laneTimeOffset * (lanePosition - 1);
        }
        zOffset = -16;
        competitorsLapTimes[lanePosition-1] = laneTimeOffset * (lanePosition - 1);
        float circleRad = ((308.8f - 16.39f) / 2.0f);//start and end pos
        Vector3 ogPos = new Vector3(-2162.03f, 226.7f, circleRad * -1); //start is -16.39
        circleRad += (laneSpace * (lanePosition - 1));
        Vector3 unitCirclePos = new Vector3((float)Math.Cos((Math.PI / ((100 + stagger200Marks[lanePosition - 1]) / 100f)) * competitorsLapTimes[lanePosition - 1]), player.transform.position.y, (float)Math.Sin((Math.PI / ((100 + stagger200Marks[lanePosition - 1]) / 100f)) * competitorsLapTimes[lanePosition - 1]));
        //player.transform.position = ogPos;
        tf.position = new Vector3(circleRad * unitCirclePos.z * -1, 0, (circleRad * unitCirclePos.x) + zOffset) + ogPos;
        tf.eulerAngles = new Vector3(0, 270 - (180 / (((100 + stagger200Marks[lanePosition - 1]) / 100f)) * competitorsLapTimes[lanePosition - 1]), 0);
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
            if (energyBar.value >0.1)
            {
                runningSliderFill.color = Color.blue;
            } else
            {
                runningSliderFill.color = Color.red;
            }
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
                        currentLane = i + 1;
                        setStartingPosition(currentLane, true, player.transform);
                        //player.transform.position = competitorsList[i].transform.position - new Vector3(-2.3f, 0, 0);
                        currentLane = i + 1;
                        competitorsList[i].SetActive(false);
                    }
                }
            }
            runningMeter.updateRunMeter();
            float curveTime = ((100 + stagger200Marks[currentLane - 1]) / 100f);
            if (isRunning && lapTimeProgress>=(curveTime*2+2) && !finished)
            {
                finished = true;
                playerTime = eventTimer * 2;
                runningMeter.runMeterSlider.gameObject.SetActive(false);
                runButton.gameObject.SetActive(false);
                StartCoroutine(waitAfterFinish(2));
            }
            for (int i=0; i<competitorsList.Length; i++)
            {
                GameObject go = competitorsList[i];
                if (i+1 != currentLane)
                {
                    float curveTimePersonal = ((100 + stagger200Marks[i]) / 100f);
                    if (competitorsLapTimes[i] >= curveTimePersonal*2+2 && !competitorsMarkedTimeList[Int32.Parse(go.name[4].ToString()) - 1])
                    {
                        competitorsMarkedTimeList[Int32.Parse(go.name[4].ToString()) - 1] = true;
                        PlayerBanner pb = laneOrders[Int32.Parse(go.name[4].ToString()) - 1];
                        leaderboardManager.changeBannerBest(pb.flagNumber, pb.player, eventTimer*2);
                        //temp
                        /*if (i == 4)
                        {
                            Debug.Log("Lane 5: " + eventTimer * 2);
                        }*/
                        //temp
                    }
                }
                
            }
            /*if (isRunning && competitorsList[3].transform.position.x <= -2161.52)
             {
                 Debug.Log("Lane 4 " + eventTimer + ":" + leaderboardManager.getPlayersInLaneOrder()[3].bestMark);
                 finished = true;
             }*/

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
                    if (setText.text == "GO")
                    {
                        if (eventTimer < 0.25)
                        {
                            jumpSparkle.startColor = Color.green;
                        }
                        else if (eventTimer < 0.4)
                        {
                            jumpSparkle.startColor = Color.yellow;
                        }
                        else
                        {
                            jumpSparkle.startColor = Color.red;
                        }
                        jumpSparkle.Play();
                    }
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
                player.GetComponentsInChildren<Animator>()[1].Play("BlockStart");
                setText.gameObject.transform.parent.GetComponent<Animator>().Play("FadeText");
                setText.gameObject.transform.parent.GetComponent<Animator>().speed = 0.75f;
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
            setText.gameObject.transform.parent.GetComponent<Animator>().speed = 0.75f;
            foreach (GameObject go in competitorsList) //gets all competitors up in the set position
            {
                if (go.activeInHierarchy) //stop animator warning
                {
                    go.GetComponentsInChildren<Animator>()[0].Play("BlockStartUp");
                }
            }
            player.GetComponentsInChildren<Animator>()[1].Play("BlockStartUp");
            leaderboardManager.getOtherRunnersTime(); //gets the time for the other runners
            StartCoroutine(showGo(UnityEngine.Random.Range(1.0f, 2.5f)));
        }

    }

    IEnumerator showGo(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (!foulImage.gameObject.activeInHierarchy)
        {
            setText.text = "GO";
            setText.gameObject.transform.parent.GetComponent<Animator>().Play("FadeText");
            setText.gameObject.transform.parent.GetComponent<Animator>().speed = 0.75f;
            isRunning = true;
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
            float time = leaderboardManager.getPlayersInLaneOrder()[index].bestMark;
            //float time = 13.87f;
            /*time -= delay;
            competitorsMaxSpeedList[index] = (275.05f / ((0.75f * time) / 0.02f)) / runningSpeedRatio;//spaces per second was 0.55
            competitorsAccelSpeedList[index] = time / 6; //was 0.4
            competitorsStartSpeedList[index] = 90;*/
            float bestMark = leaderboardManager.getPlayersInLaneOrder()[index].bestMark;
            competitorsSpeedList[index] = 1;
            float secDiffPercent = bestMark / 60;
            competitorsMaxSpeedList[index] = 201.25f / secDiffPercent;

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

    private void moveCharacterOnLane(float lapTime, int lanePosition, Transform tf)
    {
        //other end z is -308.22
        float curveTime = ((100 + stagger200Marks[lanePosition - 1]) / 100f);
        if (lapTime < curveTime)
        {
            float circleRad = ((308.8f - 16.39f) / 2.0f);//start and end pos
            Vector3 ogPos = new Vector3(-2162.03f, 226.73f, circleRad * -1); //start is -16.39
            circleRad += (laneSpace * (lanePosition - 1));
            Vector3 unitCirclePos = new Vector3((float)Math.Cos((Math.PI / curveTime) * lapTime), tf.position.y, (float)Math.Sin((Math.PI / curveTime) * lapTime));
            //player.transform.position = ogPos;
            tf.position = new Vector3(circleRad * unitCirclePos.z * -1, 0, (circleRad * unitCirclePos.x) + zOffset) + ogPos;
            tf.eulerAngles = new Vector3(0, 270 - (180 / curveTime * lapTime), 0);
        }
        else if (lapTime < curveTime + 1)
        {
            if (fourHundredBlocks.activeInHierarchy)
            {
                fourHundredBlocks.SetActive(false);
            }
            //-1832 for hundred meter end
            //-2162.03 for start
            float diff = 2162.03f - 1832f;
            tf.position = new Vector3(-2162.03f + (diff * (lapTime - curveTime)), tf.position.y, tf.position.z);
        }
        else if (lapTime < curveTime * 2 + 1)
        {
            float circleRad = ((308.22f - 16.39f) / 2.0f);//start and end pos
            Vector3 ogPos = new Vector3(-1832, 226.73f, circleRad * -1); //start is -16.39
            circleRad += (laneSpace * (lanePosition - 1));
            Vector3 unitCirclePos = new Vector3((float)Math.Cos((Math.PI / curveTime) * (lapTime - 1 - curveTime)), tf.position.y, (float)Math.Sin((Math.PI / curveTime) * (lapTime - 1 - curveTime)));
            //player.transform.position = ogPos;
            tf.position = new Vector3(circleRad * unitCirclePos.z, 0, (circleRad * -1 * unitCirclePos.x) + zOffset) + ogPos;
            tf.eulerAngles = new Vector3(0, 90 - (180 / curveTime * (lapTime - 1 - curveTime)), 0);
        }
        else if (lapTime < curveTime * 2 + 2)
        {
            float diff = 2162.03f - 1832f;
            tf.position = new Vector3(-1832 - (diff * (lapTime - (curveTime * 2 + 1))), tf.position.y, tf.position.z);
        } else if (lapTime == curveTime*2+2)
        {
           
        }
    }

    private void FixedUpdate()
    {
        if ((playerCamera.enabled && isRunning) || finished)
        {
            float speed = runningMeter.runningSpeed;
            if (godMode)
            {
                if (energyBar.value >0)
                {
                    speed = 221;
                } else
                {
                    speed = PublicData.averageSpeedDuringRun - 1;
                }
            }
            if (speed > PublicData.averageSpeedDuringRun && energyBar.value==0)
            {
                speed = PublicData.averageSpeedDuringRun - (speed - PublicData.averageSpeedDuringRun); //makes it so over slows you down
            }
            if (speed > 220 && energyBar.value != 0)
            {
                energyBar.value -= energyDepletion;
                dashParticles.Play();
                if (energyBar.value < 0.05f) energyBar.value = 0;
            } else
            {
                dashParticles.Stop();
                if (speed < PublicData.averageSpeedDuringRun)
                {
                    energyBar.value += energyGain;
                }
            }
            float maxSpeedPercent = 0;
            maxSpeedPercent += PublicData.curveValue(PublicData.getCharactersInfo(PublicData.currentRunnerUsing).strengthLevel, 0.07f);
            maxSpeedPercent += PublicData.curveValue(PublicData.getCharactersInfo(PublicData.currentRunnerUsing).speedLevel, 0.18f);
            maxSpeedPercent += PublicData.curveValue(PublicData.getCharactersInfo(PublicData.currentRunnerUsing).agilityLevel, 0.3f);
            maxSpeedPercent += PublicData.curveValue(PublicData.getCharactersInfo(PublicData.currentRunnerUsing).flexabilityLevel, 0.12f);

            speed *= 0.96f + maxSpeedPercent;
            //math for arch
            float curveTimed = ((100 + stagger200Marks[currentLane-1]) / 100f);
            if (lapTimeProgress < curveTimed*2+2)
            {
                moveCharacterOnLane(lapTimeProgress, currentLane, player.transform);
            }
            for (int i = 0; i < competitorsLapTimes.Length; i++)
            {
                if (i + 1 != currentLane)
                {
                    moveCharacterOnLane(competitorsLapTimes[i], i + 1, competitorsList[i].transform);
                }
            }

            //math for arch
            //player.transform.Translate(new Vector3(0, 0, speed * runningSpeedRatio)); //making character move according to run meter
            player.GetComponentsInChildren<Animator>()[1].speed = speed * animationRunningSpeedRatio; //making the animation match the sunning speed
            /*for (int i = 0; i < competitorsList.Length; i++)
            {
                //float time = 13.87f;
                float time = leaderboardManager.getPlayersInLaneOrder()[i].bestMark;
                if (competitorsMaxSpeedList[i] != competitorsSpeedList[i] && competitorsList[i].transform.position.x > -2122.37) //if not max speed
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
                competitorsList[i].GetComponentsInChildren<Animator>()[1].speed = competitorsSpeedList[i] * animationRunningSpeedRatio; //making the animation match the sunning speed
                if (competitorsList[i].transform.position.x > -2161.52 && competitorsList[i].transform.position.x < -2157 && competitorsList[i].GetComponentsInChildren<Animator>()[1].GetCurrentAnimatorStateInfo(0).IsName("Running"))
                {
                    competitorsList[i].GetComponentsInChildren<Animator>()[1].Play("RunningLean");
                }
            }*/
           
            runningMeter.updateTimeElapsed();
            eventTimer += Time.deltaTime;
            for(int i=0; i< competitorsLapTimes.Length;i++)
            {
                if (i+1 == currentLane)
                {
                    competitorsLapTimes[i] = speed / 75000f;
                } else
                {
                    if (competitorsSpeedList[i] != 0)
                    {
                        float curveTime = ((100 + stagger200Marks[i]) / 100f);
                        if (competitorsLapTimes[i] < curveTime * 2 + 2)
                        {
                            competitorsLapTimes[i] += competitorsMaxSpeedList[i] / 75000f;
                        }
                        else if (competitorsMaxSpeedList[i] > 0) {
                            competitorsMaxSpeedList[i] -= 10;
                            competitorsList[i].transform.Translate(0, 0, competitorsMaxSpeedList[i]/300);
                        }
                    }
                }
            }
            float curveTimee = ((100 + stagger200Marks[currentLane-1]) / 100f);
            if (lapTimeProgress < curveTimee * 2 + 2)
            {
                lapTimeProgress += speed / 75000; //normal mode
            }
            else if (competitorsMaxSpeedList[currentLane-1] > 0)
            {
                competitorsMaxSpeedList[currentLane-1] -= 10;
                player.transform.Translate(0, 0, competitorsMaxSpeedList[currentLane-1] / 300);
            }
                                            //lapTimeProgress += 0.005f; //fast mode

            //lapTimeProgress = Math.Min(lapTimeProgress,curveTime*2+2); //cap progress at 2
            //runningMeter.barDecreaseSpeed = Math.Min(startingBarDecreaseSpeed + (eventTimer * 20), 400); //make the bar decrease faster as you go on
            //runningMeter.speedPerClick = 75 + (eventTimer * 5);
           
        }

    }

    IEnumerator waitAfterFinish(float delay)
    {
        yield return new WaitForSeconds(delay);
        currentPlayerBanner = leaderboardManager.getPlayerBanner();
        currentPlayerBanner.mark2 = playerTime;
        PersonalBests characterPB = PublicData.getCharactersInfo(PublicData.currentRunnerUsing).characterBests;
        if (playerTime < Int32.Parse(PublicData.gameData.leaderboardList[5][1][0]) / 100.0f) //game record
        {
            PublicData.gameData.personalBests.fourHundred = playerTime;
            PublicData.getCharactersInfo(PublicData.currentRunnerUsing).characterBests.fourHundred = playerTime;
            leaderboardManager.addMarkLabelToPlayer(1);
            leaderboardManager.showRecordBanner(2);
        }
        else if (playerTime < PublicData.gameData.personalBests.fourHundred || PublicData.gameData.personalBests.fourHundred == 0) //if pr or first time doing it
        {
            PublicData.gameData.personalBests.fourHundred = playerTime; //sets pr
            PublicData.getCharactersInfo(PublicData.currentRunnerUsing).characterBests.fourHundred = playerTime; //sets cb too
            leaderboardManager.addMarkLabelToPlayer(3);
            leaderboardManager.showRecordBanner(1);

        }
        else if (playerTime < characterPB.fourHundred || characterPB.fourHundred == 0)
        {
            PublicData.getCharactersInfo(PublicData.currentRunnerUsing).characterBests.fourHundred = playerTime;
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

//TODO fix lean going diffrent ways
//TODO even out lanes

