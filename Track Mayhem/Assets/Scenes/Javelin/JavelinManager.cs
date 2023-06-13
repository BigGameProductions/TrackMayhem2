using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class JavelinManager : MonoBehaviour
{
    [SerializeField] private GameObject player; //the player controller
    [SerializeField] private ItemStorage itemStorage; //stores all the flags and character models

    [SerializeField] private LeaderboardManager leaderboardManager; //stores the current leaderboard manager

    [SerializeField] private Camera runningCamera;//camera for running
    [SerializeField] private Camera jumpingCamera;//camera for jumping
    [SerializeField] private Camera frontCamera; //camera for front facing celebration
    [SerializeField] private Camera javelinCamera; //camera for watching the javelin

    [SerializeField] private float runningSpeedRatio; //stores the ratio for running compared to runningSpeed
    [SerializeField] private float animationRunningSpeedRatio; //stores the ratio for animation speed compared to runningSpeed

    [SerializeField] private GameObject javelin; 


    [SerializeField] private GameObject jumpButton;
    [SerializeField] private GameObject runButton;

    [SerializeField] private Animator angleAnimation;


    private LeaderboardFunctions leadF = new LeaderboardFunctions();

    private int currentThrowNumber = 0; //stores the amount of jumps that player has taken
    PlayerBanner currentPlayerBanner; //stores the current player data in a banner class

    private Vector3 startingPlayerPosition = new Vector3(-2103.53f, 226.73f, -369.71f); //starting position of the runner
    private int rightHandTransformPosition = 55;

    private bool isFoul = false; //tells if the current jump is a foul

    private bool isRunning = false;
    private bool didThrow = false;
    private bool measure = false;

    private bool startedAngle = false; //if the player has started the angle

    private float totalInches = 0;

    private float anglePower = 0;

    [SerializeField] private Image foulImage; //is the image that appears when you foul or don't land in the sand

    [SerializeField] private ParticleSystem jumpSparkle;

    [SerializeField] RunningMeterBar runningMeter;
    [SerializeField] JumpingMeter jumpMeter;
    [SerializeField] Button infoButton;

    [SerializeField] private Canvas controlsCanvas;
    [SerializeField] GameObject angleMeter;

    private bool jumpButtonHeld = false; //if the jump button is being held

    // Start is called before the first frame update
    void Start()
    {
        itemStorage.initRunner(PublicData.currentRunnerUsing, player.transform); //inits the runner into the current scene
        player.GetComponentInChildren<Animator>().Play("JavelinRun");
        javelin.transform.SetParent(player.GetComponentsInChildren<Transform>()[rightHandTransformPosition]); //sets the parent of the pole to 
        javelin.transform.localPosition = new Vector3(-0.00200000009f, 0.140000001f, 0.0329999998f); //alligns shot to player hand
        javelin.GetComponent<Rigidbody>().centerOfMass = new Vector3(-10, 0, 0);
        angleAnimation.speed = 0;
        isRunning = true;
        controlsCanvas.enabled = false;
        angleMeter.SetActive(false);
        EventTrigger trigger = jumpButton.AddComponent<EventTrigger>();
        var pointerDown = new EventTrigger.Entry();
        pointerDown.eventID = EventTriggerType.PointerDown;
        pointerDown.callback.AddListener((e) => { jumpButtonHeld = true; if (isRunning) jumpButtonPressed(); });
        trigger.triggers.Add(pointerDown);
        var pointerUp = new EventTrigger.Entry();
        pointerUp.eventID = EventTriggerType.PointerUp;
        pointerUp.callback.AddListener((e) => jumpButtonHeld = false);
        trigger.triggers.Add(pointerUp);
    }

    // Update is called once per frame
    void Update()
    {
        if (!isRunning && player.GetComponentInChildren<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime > 0.8f && !didThrow && player.GetComponentInChildren<Animator>().GetCurrentAnimatorStateInfo(0).IsName("JavelinThrow"))
        {
            if (player.GetComponentInChildren<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime == 1)
            {
                runningMeter.updateRunMeter();
            } else
            {
                float averageSpeed = runningMeter.getAverageSpeed(); //gets average running speed
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
                totalThrowPower += runPowerPercent + anglePower;
                didThrow = true;
                javelin.transform.parent = null;
                Rigidbody rb = javelin.GetComponent<Rigidbody>();
                rb.useGravity = true;
                rb.AddForce(new Vector3(-20*totalThrowPower, 15*totalThrowPower, 0), ForceMode.Impulse);
                javelin.transform.eulerAngles = new Vector3(-37.336f, 32.927f, -45.454f);
                rb.AddRelativeTorque(new Vector3(0, 0, -40));
                controlsCanvas.enabled = false;
                angleMeter.SetActive(false);
                StartCoroutine(showJavelinCamera(1));
            }
        }
        if (didThrow && !measure)
        {
            javelinCamera.transform.eulerAngles = new Vector3(0, 0, 0);

            javelinCamera.transform.localPosition = new Vector3(-0.237f, 170.3f, -36);
            if (javelin.transform.eulerAngles.z > 46)
            {
                //javelin.GetComponentInChildren<Rigidbody>().angularVelocity = new Vector3(0, 0, 0);
            }
        }
        if (didThrow && javelin.transform.position.y < 230 && !measure)
        {
            measure = true;
            javelin.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePosition;
            float distance = (-1779.68f - javelin.transform.position.x) / PublicData.spacesPerInch;
            totalInches = leaderboardManager.roundToNearest(0.25f, distance)*2;
            updatePlayerBanner(totalInches);
            StartCoroutine(showPersonalBanner(2));
        }
        if (Input.GetKey(KeyCode.P) || jumpButtonHeld)
        {
            onJumpButtonDown();
        }
        else
        {
            angleAnimation.speed = 0;
            if (startedAngle)
            {
                float arrowPos = angleAnimation.GetCurrentAnimatorStateInfo(0).normalizedTime;
                float diff = Math.Abs(0.46f - arrowPos);
                if (diff < 0.1)
                { 
                    anglePower = 1-(diff / 0.1f);
                }
            }
        }
        if (runningCamera.enabled && !leaderboardManager.cinematicCamera.gameObject.activeInHierarchy && isRunning) //change this so it works for enabled
        {
            if (controlsCanvas.enabled == false)
            {
                controlsCanvas.enabled = true;
                infoButton.gameObject.SetActive(true);
            }
            runningMeter.updateRunMeter();
            if (Input.GetKeyDown(KeyCode.Space)) //updating speed on click
            {
                runButtonPressed();
            }
            if (Input.GetKeyDown(KeyCode.P))
            {
                jumpButtonPressed();
            }
            
        }
    }

    void FixedUpdate()
    {
        if (runningCamera.enabled && !didThrow)
        {
            float speed = runningMeter.runningSpeed;
            if (speed > PublicData.averageSpeedDuringRun)
            {
                speed = PublicData.averageSpeedDuringRun - (speed - PublicData.averageSpeedDuringRun); //makes it so over slows you down
            }
            player.transform.Translate(new Vector3(0, 0, speed * runningSpeedRatio)); //making character move according to run meter
            player.GetComponentInChildren<Animator>().speed = speed * animationRunningSpeedRatio; //making the animation match the sunning speed
            runningMeter.updateTimeElapsed();
        }
    }

    public void jumpButtonPressed()
    {
        if (runningCamera.enabled && !leaderboardManager.cinematicCamera.gameObject.activeInHierarchy && isRunning)
        {
            player.GetComponentInChildren<Animator>().Play("JavelinThrow");
            isRunning = false;
            Debug.Log("CAlled");
        }
    }

    public void onJumpButtonDown()
    {
        if (runningCamera.enabled)
        {
            if (!startedAngle)
            {
                angleAnimation.speed = 0.25f;
                startedAngle = true;
                angleMeter.SetActive(true);
            }
        }
        
    }

    public void runButtonPressed()
    {
        if (runningMeter.runMeterSlider.gameObject.activeInHierarchy)
        {
            if (infoButton.gameObject.activeInHierarchy)
            {
                infoButton.gameObject.SetActive(false);
            }
            runningMeter.increaseHeight();
            if (leaderboardManager.leaderBoardVisble()) //hides the leaderboard if the player clicks
            {
                leaderboardManager.hidePersonalBanner();
            }
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

    IEnumerator showJavelinCamera(float delay)
    {
        yield return new WaitForSeconds(delay);
        javelinCamera.transform.localEulerAngles = new Vector3(61.235f, 44.67f, 61.181f);
        javelinCamera.transform.localPosition = new Vector3(-0.237f, 146.3f, -36);
        javelinCamera.enabled = true;
        runningCamera.enabled = false;
    }

    IEnumerator showPersonalBanner(float delay)
    {
        yield return new WaitForSeconds(delay);
        leaderboardManager.showCurrentPlayerMarks(currentPlayerBanner, 3); //updates and shows the player leaderboard
        currentThrowNumber++; //inceases to the next jump
        runningCamera.enabled = true;
        javelinCamera.enabled = false;
        //ringAnimation.gameObject.SetActive(false);
        player.GetComponentInChildren<Animator>().speed = 1;
        player.GetComponentInChildren<Animator>().Play("Wave");
        if (totalInches > Int32.Parse(PublicData.gameData.leaderboardList[4][1][0]) / 100.0f) //game record
        {
            PublicData.gameData.personalBests.shotput = totalInches;
            PublicData.getCharactersInfo(PublicData.currentRunnerUsing).characterBests.javelin = totalInches;
            leadF.SetLeaderBoardEntry(4, PublicData.gameData.playerName, (int)(leaderboardManager.roundToNearest(0.25f, totalInches) * 100), PublicData.gameData.countryCode + "," + PublicData.currentRunnerUsing);
            leadF.checkForOwnPlayer(4, 20); //checks to make sure it can stay in the top 20
            leaderboardManager.addMarkLabelToPlayer(1);
            leaderboardManager.showRecordBanner(2);
        }
        else if (totalInches > PublicData.gameData.personalBests.javelin) //checks for a new personal record
        {
            leadF.SetLeaderBoardEntry(4, PublicData.gameData.playerName, (int)(leaderboardManager.roundToNearest(0.25f, totalInches) * 100), PublicData.gameData.countryCode + "," + PublicData.currentRunnerUsing);
            leadF.checkForOwnPlayer(4, 20); //checks to make sure it can stay in the top 20
            leaderboardManager.showRecordBanner(1);
            PublicData.gameData.personalBests.javelin = leaderboardManager.roundToNearest(0.25f, totalInches);
            PublicData.getCharactersInfo(PublicData.currentRunnerUsing).characterBests.javelin = leaderboardManager.roundToNearest(0.25f, totalInches); ;
            leaderboardManager.addMarkLabelToPlayer(3);

        }
        else if (totalInches > PublicData.getCharactersInfo(PublicData.currentRunnerUsing).characterBests.javelin)
        {
            PublicData.getCharactersInfo(PublicData.currentRunnerUsing).characterBests.javelin = leaderboardManager.roundToNearest(0.25f, totalInches); ;
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
        javelin.transform.SetParent(player.GetComponentsInChildren<Transform>()[rightHandTransformPosition]); //sets the parent of the pole to 
        javelin.transform.localPosition = new Vector3(-0.00200000009f, 0.140000001f, 0.0329999998f); //alligns shot to player hand
        javelin.transform.localEulerAngles = new Vector3(0, 0, 0);
        player.GetComponentInChildren<Animator>().Play("JavelinRun");
        player.GetComponentInChildren<Animator>().speed = 0;
        javelin.GetComponent<Rigidbody>().useGravity = false;
        player.GetComponentsInChildren<Transform>()[1].eulerAngles = new Vector3(0, -90, 0);
        player.transform.position = new Vector3(-1650.90002f, 226.729996f, -161.929993f);
        //ringAnimation.speed = 0;   
        //ringAnimation.gameObject.SetActive(false);
        jumpButton.gameObject.SetActive(true);
        //runMeter.runMeterSlider.gameObject.SetActive(true);
        runButton.gameObject.SetActive(true);
        runningMeter.runningSpeed = 0;
        player.GetComponentsInChildren<Transform>()[1].localPosition = new Vector3(0, 0, 0);
        measure = false;
        didThrow = false;
        isRunning = true;
        startedAngle = false;
        angleAnimation.speed = 0;
        angleAnimation.Play("AngleMeterAnimation");
        anglePower = 0;
        javelin.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        //jumpClicks = 0;
        //piviotPercents = new float[3];
    }
}



//TODO make it based on collider hitting the ground not y position