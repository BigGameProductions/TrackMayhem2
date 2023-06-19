using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System;

public class hundredMeterController : MonoBehaviour
{

    [SerializeField] private ItemStorage itemStorage;
    [SerializeField] private GameObject player;

    [SerializeField] private LeaderboardManager leaderboardManager;
    [SerializeField] private Camera playerCamera;

    [SerializeField] private Image foulImage;
    [SerializeField] private TextMeshProUGUI setText;

    [SerializeField] private RunningMeterBar runningMeter;

    [SerializeField] private float runningSpeedRatio; //stores the ratio for running compared to runningSpeed
    [SerializeField] private float animationRunningSpeedRatio; //stores the ratio for animation speed compared to runningSpeed

    [SerializeField] private Button runButton;
    [SerializeField] private Button jumpButton;
    [SerializeField] private Button infoButton;
    [SerializeField] private Canvas controlCanvas;

    [SerializeField] private GameObject[] competitorsList;

    [SerializeField] private float[] laneZValues;

    [SerializeField] private ParticleSystem jumpSparkle;
    [SerializeField] private ParticleSystem runTrail;

    private float[] competitorsSpeedList = new float[8]; //speed of all the competitors
    private float[] competitorsAccelSpeedList = new float[8]; //acceleration of all the competitors
    private float[] competitorsStartSpeedList = new float[8]; //start speed of all the competitors
    private float[] competitorsMaxSpeedList = new float[8]; //max speed of all the competitors
    private bool[] competitorsMarkedTimeList = new bool[8]; //max speed of all the competitors


    bool isRunning = false; //if gun has gone off
    bool runPressed = false; //if the run button is pressed
    bool jumpPressed = false; //if the jump button has been pressed
    bool started = false; //if the player has gone out of the blocks
    bool finished = false; //if the player has crossed the finish line
    bool usedLean = false; //if the player has used their lean

    float eventTimer = 0; //keeps track of the time of the event

    float startingBarDecreaseSpeed = 300; //gets the starting decrease speed

    PlayerBanner currentPlayerBanner;

    PlayerBanner[] laneOrders;

    float playerTime = 0;

    [SerializeField] LeanDetector leadD;

    public bool godMode;

    // Start is called before the first frame update
    void Start()
    {
        itemStorage.initRunner(PublicData.currentRunnerUsing, player.transform); //inits the runner into the current scene
        controlCanvas.enabled = false;
        setText.enabled = false;
        leaderboardManager.cinematicCamera.GetComponent<Animator>().SetInteger("event", 0);
        foulImage.gameObject.SetActive(false);
        foulImage.GetComponentInChildren<TextMeshProUGUI>().text = "False Start";
        jumpButton.gameObject.SetActive(false);
        runButton.gameObject.SetActive(false);
        runningMeter.runningBar.transform.parent.gameObject.SetActive(false);
        foreach (GameObject go in competitorsList) //gets all competitors in the blocks
        {
            Destroy(go.GetComponentsInChildren<Transform>()[1].gameObject);
            int charNum = UnityEngine.Random.Range(0, 2);
            itemStorage.initRunner(charNum, go.transform);
            go.GetComponentInChildren<Animator>().Play("BlockStart");
        }
        runTrail.gameObject.SetActive(false);

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
    [Obsolete]
    void Update()
    {
        if (!leaderboardManager.cinematicCamera.gameObject.activeInHierarchy)
        {
            if (!runButton.gameObject.activeInHierarchy)
            {
                controlCanvas.enabled = true;
                runButton.gameObject.SetActive(true); //show controls
                runningMeter.runningBar.transform.parent.gameObject.SetActive(true); //show controls
                laneOrders = leaderboardManager.getPlayersInLaneOrder();
                for (int i=0; i<laneOrders.Length; i++)
                {
                    if (laneOrders[i].isPlayer)
                    {
                        player.transform.position = competitorsList[i].transform.position - new Vector3(-2.1f,0,0);
                        competitorsList[i].SetActive(false);
                    }
                }
            }
            runningMeter.updateRunMeter();
            if (isRunning && leadD.endRace && !finished) // player.transform.position.x <= -2161.52
            {
                finished = true;
                playerTime = eventTimer;
                runningMeter.runningSpeed = 220;
                runningMeter.barDecreaseSpeed = 80;
                controlCanvas.enabled = false;
                player.GetComponentsInChildren<Transform>()[1].localPosition = new Vector3(0, 0, 0);
                StartCoroutine(waitAfterFinish(2));
            }
            foreach (GameObject go in competitorsList)
            {
                if (go.transform.position.x < -2162.1 && !competitorsMarkedTimeList[Int32.Parse(go.name[4].ToString()) - 1])
                {
                    competitorsMarkedTimeList[Int32.Parse(go.name[4].ToString()) - 1] = true;
                    PlayerBanner pb = laneOrders[Int32.Parse(go.name[4].ToString())-1];
                    leaderboardManager.changeBannerBest(pb.flagNumber, pb.player, eventTimer);
                }
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
                    player.GetComponentInChildren<Animator>().Play("Running");
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
            if ((Input.GetKeyDown(KeyCode.P) || jumpPressed) && runningMeter.runMeterSlider.gameObject.activeInHierarchy && !finished && !foulImage.gameObject.activeInHierarchy) {
                jumpPressed = false;
                usedLean = true;
                jumpButton.gameObject.SetActive(false);
                player.GetComponentInChildren<Animator>().Play("RunningLean");
                float distanceDiff = (float)Math.Abs(-2157.41 - player.transform.position.x);
                if (distanceDiff < 1)
                {
                    jumpSparkle.startColor = Color.green;
                } else if (distanceDiff < 2)
                {
                    jumpSparkle.startColor = Color.yellow;
                } else
                {
                    jumpSparkle.startColor = Color.red;
                }
                jumpSparkle.Play();
            }
            if (!playerCamera.enabled)
            {
                playerCamera.enabled = true;
                setText.enabled = true;
                player.GetComponentInChildren<Animator>().Play("BlockStart");
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
            setText.gameObject.transform.parent.GetComponent<Animator>().speed = 0.75f;
            foreach (GameObject go in competitorsList) //gets all competitors up in the set position
            {
                if (go.activeInHierarchy) //stop animator warning
                {
                    go.GetComponentInChildren<Animator>().Play("BlockStartUp");
                }
            }
            player.GetComponentInChildren<Animator>().Play("BlockStartUp");
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
            isRunning = true;
            for (int i=0; i<competitorsList.Length;i++)
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
            competitorsList[index].GetComponentInChildren<Animator>().Play("Running");
            float time = leaderboardManager.getPlayersInLaneOrder()[index].bestMark;
            //float time = 13.87f;
            time -= delay;
            competitorsMaxSpeedList[index] = (275.05f / ((0.75f * time) / 0.02f)) / runningSpeedRatio;//spaces per second was 0.55
            competitorsAccelSpeedList[index] = time/6; //was 0.4
            competitorsStartSpeedList[index] = 90;
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

    private void FixedUpdate()
    {
        if ((playerCamera.enabled && isRunning) || finished)
        {
            float speed = runningMeter.runningSpeed;
            if (speed > PublicData.averageSpeedDuringRun)
            {
                speed = PublicData.averageSpeedDuringRun - (speed - PublicData.averageSpeedDuringRun); //makes it so over slows you down
            }
            if (godMode) speed = PublicData.averageSpeedDuringRun;
            speed -= 12.7f;
            speed = Math.Max(speed, 0);
            player.transform.Translate(new Vector3(0, 0, speed * runningSpeedRatio)); //making character move according to run meter
            player.GetComponentInChildren<Animator>().speed = speed * animationRunningSpeedRatio; //making the animation match the sunning speed
            //player.GetComponentsInChildren<Transform>()[1].localPosition = new Vector3(player.GetComponentsInChildren<Transform>()[1].localPosition.x, 0, player.GetComponentsInChildren<Transform>()[1].localPosition.z);
            //player.GetComponentsInChildren<Transform>()[1].localEulerAngles = new Vector3(0, 0, 0);
            for (int i=0; i<competitorsList.Length; i++)
            {
                if (competitorsMarkedTimeList[i])
                {
                    competitorsSpeedList[i] -= 0.5f;
                }
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
                        competitorsSpeedList[i] = competitorsStartSpeedList[i] + (competitorsMaxSpeedList[i]*eventTimer*eventTimer)/((time/6)*(time/6)); //acceleration
                    }
                } else if (competitorsList[i].transform.position.x <= -2122.37 && !competitorsMarkedTimeList[i])
                {
                    competitorsSpeedList[i] = competitorsMaxSpeedList[i] + 0;
                }
                competitorsList[i].transform.Translate(new Vector3(0, 0, competitorsSpeedList[i] * runningSpeedRatio)); //making character move according to run meter
                competitorsList[i].GetComponentInChildren<Animator>().speed = competitorsSpeedList[i] * animationRunningSpeedRatio; //making the animation match the sunning speed
                if (competitorsList[i].transform.position.x > -2161.52 && competitorsList[i].transform.position.x < -2157 && competitorsList[i].GetComponentInChildren<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Running"))
                {
                    competitorsList[i].GetComponentInChildren<Animator>().Play("RunningLean");
                }
            }
            float avSpeed = runningMeter.getAverageSpeed();
            if (avSpeed > 8000)
            {
                runTrail.gameObject.SetActive(true);
                runTrail.startSpeed = (avSpeed - 8000)/20;
            } else
            {
                runTrail.gameObject.SetActive(false);
            }
            eventTimer += Time.deltaTime;
            if (!finished)
            {
                runningMeter.updateTimeElapsed();
                runningMeter.barDecreaseSpeed = Math.Min(startingBarDecreaseSpeed + (eventTimer * 20), 400); //make the bar decrease faster as you go on
                //runningMeter.speedPerClick = 75 + (eventTimer * 5);
            }
        } 

    }

    IEnumerator waitAfterFinish(float delay)
    {
        yield return new WaitForSeconds(delay);
        currentPlayerBanner = leaderboardManager.getPlayerBanner();
        currentPlayerBanner.mark2 = playerTime;
        PersonalBests characterPB = PublicData.getCharactersInfo(PublicData.currentRunnerUsing).characterBests;
        if (playerTime < Int32.Parse(PublicData.gameData.leaderboardList[0][1][0]) / 100.0f) //game record
        {
            PublicData.gameData.personalBests.hundredMeter = playerTime;
            PublicData.getCharactersInfo(PublicData.currentRunnerUsing).characterBests.hundredMeter = playerTime;
            leaderboardManager.addMarkLabelToPlayer(1);
            leaderboardManager.showRecordBanner(2);
        }
        else if (playerTime < PublicData.gameData.personalBests.hundredMeter || PublicData.gameData.personalBests.hundredMeter == 0) //if pr or first time doing it
        {
            PublicData.gameData.personalBests.hundredMeter = playerTime; //sets pr
            PublicData.getCharactersInfo(PublicData.currentRunnerUsing).characterBests.hundredMeter = playerTime; //sets cb too
            leaderboardManager.addMarkLabelToPlayer(3);
            leaderboardManager.showRecordBanner(1);

        }
        else if (playerTime < characterPB.hundredMeter || characterPB.hundredMeter == 0)
        {
            PublicData.getCharactersInfo(PublicData.currentRunnerUsing).characterBests.hundredMeter = playerTime;
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
