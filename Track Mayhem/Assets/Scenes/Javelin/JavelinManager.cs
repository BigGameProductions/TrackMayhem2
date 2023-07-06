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

    [SerializeField] private ParticleSystem jumpSparkle;


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

    [SerializeField] RunningMeterBar runningMeter;
    [SerializeField] JumpingMeter jumpMeter;
    [SerializeField] Button infoButton;

    [SerializeField] private Canvas controlsCanvas;
    [SerializeField] GameObject angleMeter;

    private bool jumpButtonHeld = false; //if the jump button is being held

    public bool godMode;

    // Start is called before the first frame update
    void Start()
    {
        itemStorage.initRunner(PublicData.currentRunnerUsing, player.transform); //inits the runner into the current scene
        rightHandTransformPosition = PublicData.rightHandTransform(player.transform);
        leaderboardManager.cinematicCamera.GetComponent<Animator>().SetInteger("event", 4); //sets animator to the current event
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
        if (isFoul)
        {
            runningMeter.updateRunMeter();
        }
        if (!isRunning && player.GetComponentInChildren<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime > 0.8f && !didThrow && player.GetComponentInChildren<Animator>().GetCurrentAnimatorStateInfo(0).IsName("JavelinThrow"))
        {
            if (player.GetComponentInChildren<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime == 1)
            {
                runningMeter.updateRunMeter();
            } else
            {
                if (!isFoul)
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
                    float arrowPos = angleAnimation.GetCurrentAnimatorStateInfo(0).normalizedTime;
                    float diff = Math.Abs(0.46f - arrowPos);
                    if (diff < 0.1)
                    {
                        anglePower = 1 - (diff / 0.1f);
                    }
                    if (anglePower > 0.8f)
                    {
                        jumpSparkle.startColor = Color.green;
                    }
                    else if (anglePower > 0.5f)
                    {
                        jumpSparkle.startColor = Color.yellow;
                    }
                    else
                    {
                        jumpSparkle.startColor = Color.red;
                    }
                    jumpSparkle.Play();
                    totalThrowPower += runPowerPercent + anglePower;
                    if (godMode) totalThrowPower = 2;
                    didThrow = true;
                    javelin.transform.parent = null;
                    Rigidbody rb = javelin.GetComponent<Rigidbody>();
                    rb.useGravity = true;
                    float totalPowerPercent = 4.5f;
                    totalPowerPercent += PublicData.curveValue(PublicData.getCharactersInfo(PublicData.currentRunnerUsing).strengthLevel, 5f, "st", currentThrowNumber);
                    totalPowerPercent += PublicData.curveValue(PublicData.getCharactersInfo(PublicData.currentRunnerUsing).speedLevel, 3.5f, "sp", currentThrowNumber);
                    totalPowerPercent += PublicData.curveValue(PublicData.getCharactersInfo(PublicData.currentRunnerUsing).agilityLevel, 4f, "ag", currentThrowNumber);
                    totalPowerPercent += PublicData.curveValue(PublicData.getCharactersInfo(PublicData.currentRunnerUsing).flexabilityLevel, 4.5f, "fl", currentThrowNumber);
                    rb.AddForce(new Vector3(-20 * totalThrowPower, totalPowerPercent * totalThrowPower, 0), ForceMode.Impulse);
                    javelin.transform.eulerAngles = new Vector3(-37.336f, 32.927f, -45.454f);
                    rb.AddRelativeTorque(new Vector3(0, 0, Math.Min(-120 + (totalPowerPercent*totalThrowPower* 4.228f), -30))); //was -40
                    runButton.gameObject.SetActive(false);
                    runningMeter.runMeterSlider.gameObject.SetActive(false);
                    jumpButton.SetActive(false);
                    angleMeter.SetActive(false);
                    StartCoroutine(showJavelinCamera(1));
                }
                
            }
        }
        if (player.transform.position.x < -1776.47f && !isFoul)
        {
            if (didThrow)
            {
                isFoul = true;
            } else
            {
                player.GetComponentInChildren<Animator>().speed = 0;
                isFoul = true;
                foulImage.gameObject.SetActive(true);
                foulImage.GetComponent<Animator>().Play("FoulSlide");
                updatePlayerBanner(-1000); //update the banner to foul
                runButton.gameObject.SetActive(false);
                jumpButton.gameObject.SetActive(false);
                runningMeter.runMeterSlider.gameObject.SetActive(false);
                angleMeter.SetActive(false);
                StartCoroutine(showPersonalBanner(2, true));

            }
        }
        if (didThrow && !measure)
        {
            javelinCamera.transform.eulerAngles = new Vector3(0, 0, 0);

           // javelinCamera.transform.position = new Vector3(javelinCamera.transform.position.x, Math.Max(javelinCamera.transform.position.y,231.4f), javelinCamera.transform.position.z);

            if (javelinCamera.transform.position.y > 231.4)
            {
                javelinCamera.transform.localPosition = new Vector3(-0.237f, 200.3f, -36); //was 170 for ys and -0.237 for x
            } else
            {
                javelinCamera.transform.position = new Vector3(javelin.transform.position.x + 0.137f, 231.4f,javelin.transform.position.z-10); //was 170 for ys and -0.237 for x
            }
            if (javelin.transform.eulerAngles.z > 46)
            {
                //javelin.GetComponentInChildren<Rigidbody>().angularVelocity = new Vector3(0, 0, 0);
            }
        }
        if (didThrow && javelin.GetComponent<JavelinCollision>().hitGround && !measure)
        {
            javelin.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePosition;
            measure = true;
            if (isFoul)
            {
                updatePlayerBanner(-1000); //update the banner to foul
            }
            else
            {
                float distance = (-1779.68f - javelin.transform.position.x) / (PublicData.spacesPerInch*0.6f);
                totalInches = leaderboardManager.roundToNearest(0.25f, distance) * 2;
                updatePlayerBanner(totalInches);
            }
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
            if (!isFoul)
            {
                if (speed > PublicData.averageSpeedDuringRun)
                {
                    speed = PublicData.averageSpeedDuringRun - (speed - PublicData.averageSpeedDuringRun); //makes it so over slows you down
                }
                player.GetComponentInChildren<Animator>().speed = speed * animationRunningSpeedRatio; //making the animation match the sunning speed
                runningMeter.updateTimeElapsed();
            }
            player.transform.Translate(new Vector3(0, 0, speed * runningSpeedRatio)); //making character move according to run meter
            
        }
    }

    public void jumpButtonPressed()
    {
        if (runningCamera.enabled && !leaderboardManager.cinematicCamera.gameObject.activeInHierarchy && isRunning)
        {
            player.GetComponentInChildren<Animator>().Play("JavelinThrow");
            if (runningMeter.runningSpeed < 50)
            {
                runningMeter.runningSpeed = 50;
            }
            isRunning = false;
            float avSpeed = runningMeter.getAverageSpeed();
            if (avSpeed > 7750 && avSpeed < 8000)
            {
                jumpSparkle.startColor = Color.green;
            }
            else if (avSpeed > 7300 && avSpeed < 10000)
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

    IEnumerator showPersonalBanner(float delay, bool hideJav=false)
    {
        yield return new WaitForSeconds(delay);
        if (hideJav)
        {
            javelin.gameObject.SetActive(false);
            foulImage.gameObject.SetActive(false);
        }
        leaderboardManager.showCurrentPlayerMarks(currentPlayerBanner, 3); //updates and shows the player leaderboard
        currentThrowNumber++; //inceases to the next jump
        runningCamera.enabled = true;
        javelinCamera.enabled = false;
        //ringAnimation.gameObject.SetActive(false);
        player.GetComponentInChildren<Animator>().speed = 1;
        player.GetComponentsInChildren<Transform>()[1].localPosition = new Vector3(0, 0, 0);
        frontCamera.enabled = true;
        if (totalInches > Int32.Parse(PublicData.gameData.leaderboardList[4][1][0]) / 100.0f && totalInches > PublicData.gameData.personalBests.javelin) //game record
        {
            PublicData.gameData.personalBests.javelin = totalInches;
            PublicData.getCharactersInfo(PublicData.currentRunnerUsing).characterBests.javelin = totalInches;
            leadF.SetLeaderBoardEntry(4, PublicData.gameData.playerName, (int)(leaderboardManager.roundToNearest(0.25f, totalInches) * 100), PublicData.gameData.countryCode + "," + PublicData.currentRunnerUsing);
            leadF.checkForOwnPlayer(4, 20); //checks to make sure it can stay in the top 20
            player.GetComponentInChildren<Animator>().Play("Exited");
            leaderboardManager.addMarkLabelToPlayer(1);
            leaderboardManager.showRecordBanner(2);
        }
        else if (totalInches > PublicData.gameData.personalBests.javelin) //checks for a new personal record
        {
            leadF.SetLeaderBoardEntry(4, PublicData.gameData.playerName, (int)(leaderboardManager.roundToNearest(0.25f, totalInches) * 100), PublicData.gameData.countryCode + "," + PublicData.currentRunnerUsing);
            leadF.checkForOwnPlayer(4, 20); //checks to make sure it can stay in the top 20
            leaderboardManager.showRecordBanner(1);
            player.GetComponentInChildren<Animator>().Play("Exited");
            PublicData.gameData.personalBests.javelin = leaderboardManager.roundToNearest(0.25f, totalInches);
            PublicData.getCharactersInfo(PublicData.currentRunnerUsing).characterBests.javelin = leaderboardManager.roundToNearest(0.25f, totalInches); ;
            leaderboardManager.addMarkLabelToPlayer(3);

        }
        else if (totalInches > PublicData.getCharactersInfo(PublicData.currentRunnerUsing).characterBests.javelin)
        {
            PublicData.getCharactersInfo(PublicData.currentRunnerUsing).characterBests.javelin = leaderboardManager.roundToNearest(0.25f, totalInches);
            player.GetComponentInChildren<Animator>().Play("Exited");
            leaderboardManager.addMarkLabelToPlayer(2);
            leaderboardManager.showRecordBanner(0);
        } else
        {
            if (isFoul)
            {
                player.GetComponentInChildren<Animator>().Play("Upset");
            }
            else
            {
                player.GetComponentInChildren<Animator>().Play("Wave");
            }
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
        angleAnimation.Play("AngleMeterAnimation", 0, 0);
        angleMeter.SetActive(false);
        anglePower = 0;
        javelin.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        foulImage.gameObject.SetActive(false);
        runningMeter.runMeterSlider.gameObject.SetActive(true);
        isFoul = false;
        javelin.GetComponent<JavelinCollision>().hitGround = false;
        frontCamera.enabled = false;
        javelin.gameObject.SetActive(true);
        //jumpClicks = 0;
        //piviotPercents = new float[3];
    }
}



