using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ShotputManager : MonoBehaviour
{
    [SerializeField] GameObject shotput;
    [SerializeField] GameObject player;
    [SerializeField] private GameObject basePlayer;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private Camera shotCamera;
    [SerializeField] private Button infoButton;

    [SerializeField] private Button runButton;
    [SerializeField] private Button jumpButton;

    [SerializeField] private Animator ringAnimation;

    [SerializeField] Canvas controlsCanvas;

    private LeaderboardFunctions leadF = new LeaderboardFunctions();

    [SerializeField] private LeaderboardManager leaderboardManager;
    [SerializeField] private RunningMeterBar runMeter;

    [SerializeField] ItemStorage itemStorage;

    private PlayerBanner currentPlayerBanner;

    private int currentThrowNumber = 0;

    private float totalInches = 0;

    private int rightHandTransformPosition = 55;
    private bool isRunning = true; //if the player is charging up throw
    private bool didThrow = false; //if player has thrown the shot yet

    private float[] perfectPiviotPoints = new float[] { 0.566f, 0.6733f, 0.8f};
    private float[] piviotPercents = new float[3];

    private int jumpClicks = 0; //amount of times the player has clicked for the meter

    private bool measure = false; //if the shot has been measured

    private GameObject[] barObjects = new GameObject[3];

    [SerializeField] private ParticleSystem jumpSparkle;
    [SerializeField] private ParticleSystem shotputSparkle;

    public bool godMode;

    // Start is called before the first frame update
    [Obsolete]
    void Start()
    {
        itemStorage.initRunner(PublicData.currentRunnerUsing, player.transform, basePlayer); //inits the runner into the current scene
        jumpSparkle.transform.parent = player.GetComponentsInChildren<Transform>()[1];
        leaderboardManager.cinematicCamera.GetComponent<Animator>().SetInteger("event", 3);
        shotput.transform.SetParent(player.GetComponentsInChildren<Transform>()[rightHandTransformPosition]); //sets the parent of the pole to 
        shotput.transform.localPosition = new Vector3(-0.00200000009f, 0.140000001f, 0.0329999998f); //alligns shot to player hand
        player.GetComponentInChildren<Animator>().Play("ShotputThrow");
        player.GetComponentInChildren<Animator>().speed = 0;
        player.GetComponentsInChildren<Transform>()[1].eulerAngles = new Vector3(0, 90, 0);
        ringAnimation.speed = 0;
        controlsCanvas.enabled = false;
        ringAnimation.gameObject.SetActive(false);
        jumpButton.gameObject.SetActive(false);
        EventTrigger trigger = jumpButton.gameObject.AddComponent<EventTrigger>();
        var pointerDown = new EventTrigger.Entry();
        pointerDown.eventID = EventTriggerType.PointerDown;
        pointerDown.callback.AddListener((e) => jumpButtonPressed());
        trigger.triggers.Add(pointerDown);
    }



    // Update is called once per frame
    [Obsolete]
    void Update()
    {
        if (shotput.transform.position.y < 227.5 && !measure && !isRunning)
        {
            measure = true;
            shotputSparkle.Play(); 
            float totalDistance = -1779.07f-shotput.transform.position.x;
            totalInches = 2 * leaderboardManager.roundToNearest(0.25f, totalDistance / (PublicData.spacesPerInch*0.79f));
            updatePlayerBanner(totalInches);
            StartCoroutine(showPersonalBanner(2));
        }
        if (!leaderboardManager.cinematicCamera.gameObject.activeInHierarchy && !playerCamera.enabled && !shotput.GetComponent<Rigidbody>().useGravity)
        {
            playerCamera.enabled = true;
            controlsCanvas.enabled = true;
        } else if (shotput.GetComponent<Rigidbody>().useGravity)
        {
            shotCamera.transform.localPosition = new Vector3(0, 5, -10);
            shotCamera.transform.eulerAngles = new Vector3(30, 0, 0);
            shotput.transform.eulerAngles = new Vector3(0, 0, 0);
        }
        if (player.GetComponentInChildren<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime > 0.35 && !measure && isRunning)
        {
            runButton.gameObject.SetActive(false);
            runMeter.runMeterSlider.gameObject.SetActive(false);
            isRunning = false;
            ringAnimation.gameObject.SetActive(true);
            jumpButton.gameObject.SetActive(true);
            if (ringAnimation.speed == 0)
            {
                ringAnimation.Play("RingBarAnimations");
                ringAnimation.speed = 0.5f;
            }
            player.GetComponentInChildren<Animator>().speed = 0.6f;
            float avSpeed = runMeter.getAverageSpeed();
            if (avSpeed > 7750 && avSpeed < 8000)
            {
                jumpSparkle.startColor = Color.green;
            } else if (avSpeed > 7300 && avSpeed < 10000)
            {
                jumpSparkle.startColor = Color.yellow;
            } else
            {
                jumpSparkle.startColor = Color.red;
            }
            jumpSparkle.Play();
        }
        if (playerCamera.enabled && !leaderboardManager.cinematicCamera.gameObject.activeInHierarchy && isRunning) //runs when the player is in the running stage
        {
            runMeter.updateRunMeter();
            if ((Input.GetKeyDown(KeyCode.Space))) //updating speed on click
            {
                runButtonPressed();
            }
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            jumpButtonPressed();
        }
        if (!isRunning && player.GetComponentInChildren<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime > 0.8 && !didThrow)
        {
            didThrow = true;
            shotput.transform.parent = null;
            shotput.GetComponent<Rigidbody>().useGravity = true; //makes it able to fall
            float averageSpeed = runMeter.getAverageSpeed(); //gets average running speed
            float totalThrowPower = 0;
            float runPowerPercent = 0;
            if (averageSpeed <= 8500) //sets percentage based on distance from 0 to 8500. 8500 is considered the perfect run
            {
                runPowerPercent = averageSpeed / 8500;
            }
            else //sets from 0 to 4500 for the top part
            {
                runPowerPercent = 1 - ((averageSpeed - 8500) / 4500);
            }
            if (godMode)
            {
                runPowerPercent = 1;
                piviotPercents[0] = 1;
                piviotPercents[1] = 1;
                piviotPercents[2] = 1;
            }
            totalThrowPower += runPowerPercent;
            totalThrowPower += piviotPercents[0];
            totalThrowPower += piviotPercents[1];
            totalThrowPower += piviotPercents[2];
            float maxPower = 77;
            maxPower += PublicData.curveValue(PublicData.getCharactersInfo(PublicData.currentRunnerUsing).strengthLevel, 80);
            maxPower += PublicData.curveValue(PublicData.getCharactersInfo(PublicData.currentRunnerUsing).agilityLevel, 30);
            maxPower += PublicData.curveValue(PublicData.getCharactersInfo(PublicData.currentRunnerUsing).speedLevel, 17);
            maxPower += PublicData.curveValue(PublicData.getCharactersInfo(PublicData.currentRunnerUsing).flexabilityLevel, 15);
            float power = (totalThrowPower / 4f) * maxPower;
            power += 50;
            shotput.GetComponent<Rigidbody>().AddForce(new Vector3(-1*power, power*0.75f, 0), ForceMode.Impulse); //adds the throwing force
            StartCoroutine(changeCameraAngle(1));
        }
    }

    private void FixedUpdate() //fixed for speed and running
    {
        if (playerCamera.enabled && isRunning) //if running or planting
        {
            float speed = runMeter.runningSpeed;
            if (speed > PublicData.averageSpeedDuringRun)
            {
                speed = PublicData.averageSpeedDuringRun - (speed - PublicData.averageSpeedDuringRun); //makes it so over slows you down
            }
            if (player.GetComponentInChildren<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.35)
            {
                player.GetComponentInChildren<Animator>().speed = (float)(0.5 * (speed / PublicData.averageSpeedDuringRun));
            }
            runMeter.updateTimeElapsed();

        }
    }

    private void updatePlayerBanner(float mark)
    {
        currentPlayerBanner = leaderboardManager.getPlayerBanner();

        if (currentThrowNumber == 0)
        {
            currentPlayerBanner.mark1 = mark;
        }
        else if (currentThrowNumber == 1)
        {
            currentPlayerBanner.mark2 = mark;
        }
        else if (currentThrowNumber == 2)
        {
            currentPlayerBanner.mark3 = mark;
        }

    }

    IEnumerator changeCameraAngle(float delay)
    {
        yield return new WaitForSeconds(delay);
        playerCamera.enabled = false;
        shotCamera.enabled = true;
        jumpButton.gameObject.SetActive(false);
    }

    IEnumerator showPersonalBanner(float delay)
    {
        yield return new WaitForSeconds(delay);
        leaderboardManager.showCurrentPlayerMarks(currentPlayerBanner, 3); //updates and shows the player leaderboard
        player.transform.position = new Vector3(-1772.14001f, 226.729996f, -115.019997f);
        player.GetComponentsInChildren<Transform>()[1].localPosition = new Vector3(0, 0, 0);
        currentThrowNumber++; //inceases to the next jump
        playerCamera.enabled = true;
        shotCamera.enabled = false;
        ringAnimation.gameObject.SetActive(false);
        player.GetComponentInChildren<Animator>().speed = 1;
        player.GetComponentInChildren<Animator>().Play("Wave");
        if (totalInches > Int32.Parse(PublicData.gameData.leaderboardList[3][1][0]) / 100.0f) //game record
        {
            PublicData.gameData.personalBests.shotput = totalInches;
            PublicData.getCharactersInfo(PublicData.currentRunnerUsing).characterBests.shotput = totalInches;
            leadF.SetLeaderBoardEntry(3, PublicData.gameData.playerName, (int)(leaderboardManager.roundToNearest(0.25f, totalInches) * 100), PublicData.gameData.countryCode + "," + PublicData.currentRunnerUsing);
            leadF.checkForOwnPlayer(3, 20); //checks to make sure it can stay in the top 20
            leaderboardManager.addMarkLabelToPlayer(1);
            leaderboardManager.showRecordBanner(2);
        }
        else if (totalInches > PublicData.gameData.personalBests.shotput) //checks for a new personal record
        {
            leadF.SetLeaderBoardEntry(3, PublicData.gameData.playerName, (int)(leaderboardManager.roundToNearest(0.25f, totalInches) * 100), PublicData.gameData.countryCode + "," + PublicData.currentRunnerUsing);
            leadF.checkForOwnPlayer(3, 20); //checks to make sure it can stay in the top 20
            leaderboardManager.showRecordBanner(1);
            PublicData.gameData.personalBests.shotput = leaderboardManager.roundToNearest(0.25f, totalInches);
            PublicData.getCharactersInfo(PublicData.currentRunnerUsing).characterBests.shotput = leaderboardManager.roundToNearest(0.25f, totalInches); ;
            leaderboardManager.addMarkLabelToPlayer(3);

        }
        else if (totalInches > PublicData.getCharactersInfo(PublicData.currentRunnerUsing).characterBests.shotput)
        {
            PublicData.getCharactersInfo(PublicData.currentRunnerUsing).characterBests.shotput = leaderboardManager.roundToNearest(0.25f, totalInches); ;
            leaderboardManager.addMarkLabelToPlayer(2);
            leaderboardManager.showRecordBanner(0);
        }
        StartCoroutine(waitAfterPersonalBanner(3));
    }

    IEnumerator waitAfterPersonalBanner(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (currentThrowNumber == 3)
        {
            SceneManager.LoadScene("EndScreen");
        }
        leaderboardManager.showRecordBanner(-1);
        leaderboardManager.hidePersonalBanner();
        leaderboardManager.showUpdatedLeaderboard();
        shotput.transform.SetParent(player.GetComponentsInChildren<Transform>()[rightHandTransformPosition]); //sets the parent of the pole to 
        shotput.transform.localPosition = new Vector3(-0.00200000009f, 0.140000001f, 0.0329999998f); //alligns shot to player hand
        player.GetComponentInChildren<Animator>().Play("ShotputThrow");
        player.GetComponentInChildren<Animator>().speed = 0;
        shotput.GetComponent<Rigidbody>().useGravity = false;
        player.GetComponentsInChildren<Transform>()[1].eulerAngles = new Vector3(0, 90, 0);
        ringAnimation.speed = 0;
        ringAnimation.gameObject.SetActive(false);
        jumpButton.gameObject.SetActive(false);
        runMeter.runMeterSlider.gameObject.SetActive(true);
        runButton.gameObject.SetActive(true);
        runMeter.runningSpeed = 0;
        player.GetComponentsInChildren<Transform>()[1].localPosition = new Vector3(0, 0, 0);
        measure = false;
        isRunning = true;
        jumpClicks = 0;
        didThrow = false;
        piviotPercents = new float[3];
        foreach (GameObject go in barObjects)
        {
            Destroy(go);
        }
        barObjects[0] = null;
        barObjects[1] = null;
        barObjects[2] = null;

    }

    public void runButtonPressed()
    {
        if (playerCamera.enabled && !leaderboardManager.cinematicCamera.gameObject.activeInHierarchy && isRunning && runMeter.runMeterSlider.gameObject.activeInHierarchy)
        {
            infoButton.gameObject.SetActive(false);
            runMeter.increaseHeight();
            if (leaderboardManager.leaderBoardVisble()) //hides the leaderboard if the player clicks
            {
                leaderboardManager.hidePersonalBanner();
            }
        }
    }

    [Obsolete]
    public void jumpButtonPressed()
    {
        if (!isRunning && jumpClicks < 3)
        {
            GameObject go = Instantiate(ringAnimation.gameObject.GetComponentsInChildren<Transform>()[1], ringAnimation.transform).gameObject;
            barObjects[jumpClicks] = go;
            float clickTime = ringAnimation.GetCurrentAnimatorStateInfo(0).normalizedTime;
            float timeDiff = Math.Abs(perfectPiviotPoints[jumpClicks] - clickTime);
            float totalPowerPercentage = 0;
            if (timeDiff < 0.05)
            {
                totalPowerPercentage = 1 - (timeDiff / 0.05f);
                if (totalPowerPercentage > 0.75)
                {
                    jumpSparkle.startColor = Color.green;
                } else
                {
                    jumpSparkle.startColor = Color.yellow;
                }
            } else
            {
                jumpSparkle.startColor = Color.red;
            }
            jumpSparkle.Play();
            piviotPercents[jumpClicks] = totalPowerPercentage;
            jumpClicks++;
        }
        
    }
}

