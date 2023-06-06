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
    [SerializeField] private Image prImage;
    [SerializeField] private TextMeshProUGUI setText;

    [SerializeField] private RunningMeterBar runningMeter;

    [SerializeField] private float runningSpeedRatio; //stores the ratio for running compared to runningSpeed
    [SerializeField] private float animationRunningSpeedRatio; //stores the ratio for animation speed compared to runningSpeed

    [SerializeField] private Button runButton;
    [SerializeField] private Button jumpButton;

    [SerializeField] private GameObject[] competitorsList;

    private float[] competitorsSpeedList = new float[7]; //speed of all the competitors
    private float[] competitorsAccelSpeedList = new float[7]; //acceleration of all the competitors
    private float[] competitorsStartSpeedList = new float[7]; //max speed of all the competitors
    private float[] competitorsMaxSpeedList = new float[7]; //max speed of all the competitors


    bool isRunning = false; //if gun has gone off
    bool runPressed = false; //if the run button is pressed
    bool started = false; //if the player has gone out of the blocks
    bool finished = false; //if the player has crossed the finish line
    bool usedLean = false; //if the player has used their lean

    float eventTimer = 0; //keeps track of the time of the event

    float startingBarDecreaseSpeed = 300; //gets the starting decrease speed

    PlayerBanner currentPlayerBanner;

    // Start is called before the first frame update
    void Start()
    {
        itemStorage.initRunner(PublicData.currentRunnerUsing, player.transform); //inits the runner into the current scene
        setText.enabled = false;
        prImage.enabled = false;
        foulImage.enabled = false;
        jumpButton.gameObject.SetActive(false);
        runButton.gameObject.SetActive(false);
        runningMeter.runningBar.transform.parent.gameObject.SetActive(false);
        foreach (GameObject go in competitorsList) //gets all competitors in the blocks
        {
            go.GetComponentInChildren<Animator>().Play("BlockStart");
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (!leaderboardManager.cinematicCamera.gameObject.activeInHierarchy)
        {
            if (!runButton.gameObject.activeInHierarchy)
            {
                runButton.gameObject.SetActive(true);
                runningMeter.runningBar.transform.parent.gameObject.SetActive(true);
            }
            runningMeter.updateRunMeter();
            if (isRunning && player.transform.position.x <= -2163.52)
            {
                finished = true;
                StartCoroutine(waitAfterFinish(2));
            }
            if (isRunning && player.transform.position.x < -2125 && !finished && !usedLean)
            {
                jumpButton.gameObject.SetActive(true);
            }
            if ((Input.GetKeyDown(KeyCode.Space) || runPressed) && runningMeter.runningBar.transform.parent.gameObject.activeInHierarchy && !finished && !foulImage.enabled) //updating speed on click
            {
                if (!isRunning)
                {
                    foulImage.enabled = true;
                    isRunning = true;
                    runningMeter.runningSpeed = 800;
                    StartCoroutine(foulRun(2)); //wait then change screens
                }
                if (!started)
                {
                    started = true;
                    player.GetComponentInChildren<Animator>().Play("Running");
                }
                runPressed = false;
                runningMeter.increaseHeight();
                if (leaderboardManager.leaderBoardVisble()) //hides the leaderboard if the player clicks
                {
                    leaderboardManager.hidePersonalBanner();
                }
            }
            if ((Input.GetKeyDown(KeyCode.P) || false) && runningMeter.runningBar.transform.parent.gameObject.activeInHierarchy && !finished && !foulImage.enabled) {
                usedLean = true;
                jumpButton.gameObject.SetActive(false);
                player.GetComponentInChildren<Animator>().Play("RunningLean");
            }
            if (!playerCamera.enabled)
            {
                playerCamera.enabled = true;
                setText.enabled = true;
                player.GetComponentInChildren<Animator>().Play("BlockStart");
                StartCoroutine(showSet(3));
            }
        }
    }

    IEnumerator showSet(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (!foulImage.enabled)
        {
            setText.text = "Set";
            foreach (GameObject go in competitorsList) //gets all competitors up in the set position
            {
                go.GetComponentInChildren<Animator>().Play("BlockStartUp");
            }
            player.GetComponentInChildren<Animator>().Play("BlockStartUp");
            leaderboardManager.getOtherRunnersTime(); //gets the time for the other runners
            StartCoroutine(showGo(UnityEngine.Random.Range(1.0f, 3.0f)));
        }
        
    }

    IEnumerator showGo(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (!foulImage.enabled)
        {
            setText.text = "GO";
            isRunning = true;
            for (int i=0; i<competitorsList.Length;i++)
            {
                StartCoroutine(oppenentBlockStart(UnityEngine.Random.Range(0.1f, 0.5f), i));
            }

        }

    }

    IEnumerator oppenentBlockStart(float delay, int index)
    {
        yield return new WaitForSeconds(delay);
        competitorsList[index].GetComponentInChildren<Animator>().Play("Running");
        competitorsMaxSpeedList[index] = 150;
        competitorsAccelSpeedList[index] = 3;
        competitorsStartSpeedList[index] = 100;
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
            player.transform.Translate(new Vector3(0, 0, speed * runningSpeedRatio)); //making character move according to run meter
            player.GetComponentInChildren<Animator>().speed = speed * animationRunningSpeedRatio; //making the animation match the sunning speed
            for (int i=0; i<competitorsList.Length; i++)
            {
                if (competitorsMaxSpeedList[i] != competitorsSpeedList[i]) //if not max speed
                {
                    if (eventTimer >= competitorsAccelSpeedList[i]) //if past max speed on time
                    {
                        competitorsSpeedList[i] = competitorsMaxSpeedList[i]; //set to max speed
                    } else
                    {
                        competitorsSpeedList[i] = competitorsStartSpeedList[i] + ((competitorsMaxSpeedList[i]-competitorsStartSpeedList[i])/competitorsAccelSpeedList[i])*eventTimer; //acceleration
                    }
                }
                competitorsList[i].transform.Translate(new Vector3(0, 0, competitorsSpeedList[i] * runningSpeedRatio)); //making character move according to run meter
                competitorsList[i].GetComponentInChildren<Animator>().speed = competitorsSpeedList[i] * animationRunningSpeedRatio; //making the animation match the sunning speed
            }
            if (!finished)
            {
                runningMeter.updateTimeElapsed();
                eventTimer += Time.deltaTime;
                runningMeter.barDecreaseSpeed = Math.Min(startingBarDecreaseSpeed + (eventTimer * 20), 400); //make the bar decrease faster as you go on
                //runningMeter.speedPerClick = 75 + (eventTimer * 5);
            }
        } 

    }

    IEnumerator waitAfterFinish(float delay)
    {
        yield return new WaitForSeconds(delay);
        currentPlayerBanner = leaderboardManager.getPlayerBanner();
        currentPlayerBanner.mark2 = eventTimer;
        leaderboardManager.showCurrentPlayerMarks(currentPlayerBanner, 3); //updates and shows the player leaderboard
        leaderboardManager.addPlayerTime(eventTimer); //adds player to the banners
        if (eventTimer < PublicData.gameData.personalBests.hundredMeter || PublicData.gameData.personalBests.hundredMeter == 0) //if pr or first time doing it
        {
            PublicData.gameData.personalBests.hundredMeter = eventTimer; //sets pr 
            prImage.enabled = true; //shows pr image
        }
        StartCoroutine(showEndScreen(3));

    }

    IEnumerator showEndScreen(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene("EndScreen");
    }

}

//TODO fix lean going diffrent ways
