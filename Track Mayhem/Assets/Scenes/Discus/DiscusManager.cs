using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DiscusManager : MonoBehaviour
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

    private float[] perfectPiviotPoints = new float[] { 0.566f, 0.6733f, 0.8f };
    private float[] piviotPercents = new float[3];
    private float[] secondPiviotPercents = new float[3];

    private GameObject[] pivotBars = new GameObject[3];

    [SerializeField] private ParticleSystem jumpSparkle;

    private int ringAnimationStage = 0;

    private int jumpClicks = 0; //amount of times the player has clicked for the meter

    private bool measure = false; //if the shot has been measured

    public bool godMode;

    // Start is called before the first frame update
    void Start()
    {
        itemStorage.initRunner(PublicData.currentRunnerUsing, player.transform, basePlayer); //inits the runner into the current scene
        shotput.transform.SetParent(player.GetComponentsInChildren<Transform>()[rightHandTransformPosition]); //sets the parent of the pole to
        leaderboardManager.cinematicCamera.GetComponent<Animator>().SetInteger("event", 6);
        shotput.transform.localPosition = new Vector3(-0.00200000009f, 0.140000001f, 0.0329999998f); //alligns shot to player hand
        shotput.transform.localEulerAngles = new Vector3(-94.45f, 76.457f,-75.442f); //alligns shot to player hand
        player.GetComponentInChildren<Animator>().Play("DiscusThrow");
        player.GetComponentsInChildren<Transform>()[1].eulerAngles = new Vector3(0, 110, 0);
        ringAnimation.speed = 0;
        controlsCanvas.enabled = false;
        ringAnimation.gameObject.SetActive(false);
        jumpButton.gameObject.SetActive(false);
        shotput.GetComponent<BoxCollider>().enabled = false;
        player.GetComponentInChildren<Animator>().speed = 0;
        EventTrigger trigger = jumpButton.gameObject.AddComponent<EventTrigger>();
        var pointerDown = new EventTrigger.Entry();
        pointerDown.eventID = EventTriggerType.PointerDown;
        pointerDown.callback.AddListener((e) => jumpButtonPressed());
        trigger.triggers.Add(pointerDown);
    }



    // Update is called once per frame
    void Update()
    {
        if (ringAnimationStage == 2 && ringAnimation.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99)
        {
            ringAnimationStage = 3;
            player.GetComponentInChildren<Animator>().speed = 0.6f;
            ringAnimation.gameObject.SetActive(false);
            foreach (GameObject go in pivotBars)
            {
                if (go != null)
                {
                    Destroy(go);
                }
            }
            pivotBars[0] = null;
            pivotBars[0] = null;
            pivotBars[0] = null;
        }
        if (ringAnimationStage == 1 && ringAnimation.GetCurrentAnimatorStateInfo(0).normalizedTime>0.99)
        {
            ringAnimation.Play("RingBarAnimationsBack");
            jumpClicks = 0;
            ringAnimationStage = 2;
            ringAnimation.speed = 0.85f;
            player.GetComponentInChildren<Animator>().speed = 0.3f;
            foreach (GameObject go in pivotBars)
            {
                if (go != null)
                {
                    Destroy(go);
                }
            }
            pivotBars[0] = null;
            pivotBars[0] = null;
            pivotBars[0] = null;
        }
       
        if (shotput.transform.position.y < 227.5 && !measure && !isRunning)
        {
            measure = true;
            float xDistance = -1755.8f - shotput.transform.position.x;
            float zDistance = Math.Abs(-249.38f - shotput.transform.position.z);
            float totalDistance = (float)Math.Sqrt(Math.Pow(xDistance, 2) + Math.Pow(zDistance, 2));
            totalInches = 2 * leaderboardManager.roundToNearest(0.25f, totalDistance / (PublicData.spacesPerInch*0.865f));
            updatePlayerBanner(totalInches);
            StartCoroutine(showPersonalBanner(2));
        }
        if (!leaderboardManager.cinematicCamera.gameObject.activeInHierarchy && !playerCamera.enabled && !shotput.GetComponent<Rigidbody>().useGravity)
        {
            playerCamera.enabled = true;
            controlsCanvas.enabled = true;
        }
        else if (shotput.GetComponent<Rigidbody>().useGravity)
        {
            shotCamera.transform.localPosition = new Vector3(0, 5, -10);
            shotCamera.transform.eulerAngles = new Vector3(30, 0, 0);
            shotput.transform.eulerAngles = new Vector3(0, 0, 0);
        }
        /*if (player.GetComponentInChildren<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime > 0.35 && !measure)
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
        }*/
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
            if (godMode)
            {
                piviotPercents[0] = 1;
                piviotPercents[1] = 1;
                piviotPercents[2] = 1;
                secondPiviotPercents[0] = 1;
                secondPiviotPercents[1] = 1;
                secondPiviotPercents[2] = 1;

            }
            float totalThrowPower = 0;
            totalThrowPower += piviotPercents[0]/2;
            totalThrowPower += piviotPercents[1]/2;
            totalThrowPower += piviotPercents[2];
            totalThrowPower += secondPiviotPercents[0]/2;
            totalThrowPower += secondPiviotPercents[1]/2;
            totalThrowPower += secondPiviotPercents[2];
            float throwPercent = 0;
            throwPercent += PublicData.curveValue(PublicData.getCharactersInfo(PublicData.currentRunnerUsing).strengthLevel, 1.6f);
            throwPercent += PublicData.curveValue(PublicData.getCharactersInfo(PublicData.currentRunnerUsing).speedLevel, 0.2f);
            throwPercent += PublicData.curveValue(PublicData.getCharactersInfo(PublicData.currentRunnerUsing).agilityLevel, 0.7f);
            throwPercent += PublicData.curveValue(PublicData.getCharactersInfo(PublicData.currentRunnerUsing).flexabilityLevel, 1.18f);
            float power = (3.38f + throwPercent) * totalThrowPower; //was 3.93
            power += 2;
            shotput.GetComponent<Rigidbody>().AddForce(new Vector3(-1 * power, power * 0.5f, Math.Min(10,power/4)), ForceMode.Impulse); //adds the throwing force
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
                //player.GetComponentInChildren<Animator>().speed = (float)(0.5 * (speed / PublicData.averageSpeedDuringRun));
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
        shotput.GetComponent<BoxCollider>().enabled = true;
        playerCamera.enabled = false;
        shotCamera.enabled = true;
        jumpButton.gameObject.SetActive(false);
    }

    IEnumerator showPersonalBanner(float delay)
    {
        yield return new WaitForSeconds(delay);
        leaderboardManager.showCurrentPlayerMarks(currentPlayerBanner, 3); //updates and shows the player leaderboard
        currentThrowNumber++; //inceases to the next jump
        playerCamera.enabled = true;
        shotCamera.enabled = false;
        ringAnimation.gameObject.SetActive(false);
        player.GetComponentInChildren<Animator>().speed = 1;
        player.GetComponentInChildren<Animator>().Play("Wave");
        if (totalInches > Int32.Parse(PublicData.gameData.leaderboardList[6][1][0]) / 100.0f && totalInches > PublicData.gameData.personalBests.discus) //game record
        {
            PublicData.gameData.personalBests.discus = totalInches;
            PublicData.getCharactersInfo(PublicData.currentRunnerUsing).characterBests.discus = totalInches;
            leadF.SetLeaderBoardEntry(6, PublicData.gameData.playerName, (int)(leaderboardManager.roundToNearest(0.25f, totalInches) * 100), PublicData.gameData.countryCode + "," + PublicData.currentRunnerUsing);
            leadF.checkForOwnPlayer(6, 20); //checks to make sure it can stay in the top 20
            leaderboardManager.addMarkLabelToPlayer(1);
            leaderboardManager.showRecordBanner(2);
        }
        else if (totalInches > PublicData.gameData.personalBests.discus) //checks for a new personal record
        {
            leadF.SetLeaderBoardEntry(6, PublicData.gameData.playerName, (int)(leaderboardManager.roundToNearest(0.25f, totalInches) * 100), PublicData.gameData.countryCode + "," + PublicData.currentRunnerUsing);
            leadF.checkForOwnPlayer(6, 20); //checks to make sure it can stay in the top 20
            leaderboardManager.showRecordBanner(1);
            PublicData.gameData.personalBests.discus = leaderboardManager.roundToNearest(0.25f, totalInches);
            PublicData.getCharactersInfo(PublicData.currentRunnerUsing).characterBests.discus = leaderboardManager.roundToNearest(0.25f, totalInches); ;
            leaderboardManager.addMarkLabelToPlayer(3);

        }
        else if (totalInches > PublicData.getCharactersInfo(PublicData.currentRunnerUsing).characterBests.discus)
        {
            PublicData.getCharactersInfo(PublicData.currentRunnerUsing).characterBests.discus = leaderboardManager.roundToNearest(0.25f, totalInches); ;
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
        shotput.transform.localEulerAngles = new Vector3(-94.45f, 76.457f, -75.442f); //alligns shot to player hand
        player.GetComponentInChildren<Animator>().Play("DiscusThrow");
        player.GetComponentInChildren<Animator>().speed = 0;
        shotput.GetComponent<Rigidbody>().useGravity = false;
        player.GetComponentsInChildren<Transform>()[1].eulerAngles = new Vector3(0, 110, 0);
        ringAnimation.speed = 0;
        ringAnimation.gameObject.SetActive(false);
        jumpButton.gameObject.SetActive(false);
        runButton.gameObject.SetActive(true);
        runMeter.runningSpeed = 0;
        player.GetComponentsInChildren<Transform>()[1].localPosition = new Vector3(0, 0, 0);
        shotput.GetComponent<BoxCollider>().enabled = false;
        measure = false;
        isRunning = true;
        jumpClicks = 0;
        didThrow = false;
        piviotPercents = new float[3];
    }

    public void runButtonPressed()
    {
        if (leaderboardManager.leaderBoardVisble()) //hides the leaderboard if the player clicks
        {
            leaderboardManager.hidePersonalBanner();
        }
        jumpButton.gameObject.SetActive(true);
        player.GetComponentInChildren<Animator>().speed = 0.2f;
        ringAnimation.gameObject.SetActive(true);
        ringAnimation.Play("RingBarAnimations");
        ringAnimationStage = 1;
        runButton.gameObject.SetActive(false);
        ringAnimation.speed = 0.65f;
        isRunning = false;
    }

    public void jumpButtonPressed()
    {
        if (!isRunning && jumpClicks < 3)
        {
            Transform tf = Instantiate(ringAnimation.gameObject.GetComponentsInChildren<Transform>()[1], ringAnimation.transform);
            float clickTime = ringAnimation.GetCurrentAnimatorStateInfo(0).normalizedTime;
            float timeDiff = 1;
            if (ringAnimationStage == 1)
            {
                timeDiff = Math.Abs(perfectPiviotPoints[jumpClicks] - clickTime);
            }
            else
            {
                timeDiff = Math.Abs(1-perfectPiviotPoints[2-jumpClicks] - clickTime);
            }
            float totalPowerPercentage = 0;
            if (timeDiff < 0.05)
            {
                totalPowerPercentage = 1 - (timeDiff / 0.05f);
            }
            if (ringAnimationStage == 1)
            {
                piviotPercents[jumpClicks] = totalPowerPercentage;

            } else
            {
                secondPiviotPercents[jumpClicks] = totalPowerPercentage;
            }
            if (timeDiff < 0.05)
            {
                totalPowerPercentage = 1 - (timeDiff / 0.05f);
                if (totalPowerPercentage > 0.75)
                {
                    jumpSparkle.startColor = Color.green;
                }
                else
                {
                    jumpSparkle.startColor = Color.yellow;
                }
            }
            else
            {
                jumpSparkle.startColor = Color.red;
            }
            jumpSparkle.Play();
            pivotBars[jumpClicks] = tf.gameObject;
            jumpClicks++;
        }

    }
}

