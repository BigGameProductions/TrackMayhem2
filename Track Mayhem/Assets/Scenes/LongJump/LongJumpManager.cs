using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;

public class LongJumpManager : MonoBehaviour
{
    [SerializeField] private GameObject player; //the player controller
    [SerializeField] private ItemStorage itemStorage; //stores all the flags and character models

    [SerializeField] private LeaderboardManager leaderboardManager; //stores the current leaderboard manager

    [SerializeField] private Camera runningCamera;//camera for running
    [SerializeField] private Camera jumpingCamera;//camera for jumping
    [SerializeField] private Camera frontCamera; //camera for front facing celebration

    [SerializeField] private float runningSpeedRatio; //stores the ratio for running compared to runningSpeed
    [SerializeField] private float animationRunningSpeedRatio; //stores the ratio for animation speed compared to runningSpeed

    [SerializeField] private float powerToAnimationSpeedRatio;
    [SerializeField] private float pullInLegPower;

    private bool runPressed;
    private bool jumpPressed;

    [SerializeField] private GameObject jumpButton;
    [SerializeField] private GameObject runButton;

    private LeaderboardFunctions leadF = new LeaderboardFunctions(); 

    private int currentJumpNumber = 0; //stores the amount of jumps that player has taken
    PlayerBanner currentPlayerBanner; //stores the current player data in a banner class

    private Vector3 startingPlayerPosition = new Vector3(-2103.53f, 226.73f, -369.71f); //starting position of the runner

    private bool isFoul = false; //tells if the current jump is a foul


    [SerializeField] private Image foulImage; //is the image that appears when you foul or don't land in the sand
    [SerializeField] private Image prImage; //is the image that appears when you pr

    [SerializeField] private ParticleSystem sandEffect;
    [SerializeField] private ParticleSystem jumpSparkle;

    [SerializeField] RunningMeterBar runningMeter;
    [SerializeField] JumpingMeter jumpMeter;

    
    // Start is called before the first frame update
    void Start()
    {
        itemStorage.initRunner(PublicData.currentRunnerUsing, player.transform); //inits the runner into the current scene
        

        jumpMeter.jumpBar.gameObject.transform.parent.gameObject.SetActive(false); //hides the jump meter
        player.GetComponentInChildren<Animator>().Play("Running");

        player.transform.position = startingPlayerPosition; //puts player in starting position

        leaderboardManager.cinematicCamera.GetComponent<Animator>().SetInteger("event", 1); //sets animator to the current event

        foulImage.enabled = false; //hide the foul icon
        prImage.enabled = false; //hides the pr image

        runningMeter.barDecreaseSpeed -= PublicData.getCharactersInfo(PublicData.currentRunnerUsing).speedLevel * 10; //lowers bar slower
        runningMeter.speedPerClick -= PublicData.getCharactersInfo(PublicData.currentRunnerUsing).strengthLevel * 0.5f; //raises bar a bit slower

        jumpMeter.jumpBarSpeed -= PublicData.getCharactersInfo(PublicData.currentRunnerUsing).agilityLevel * 20; //makes the jump abr go slower

        pullInLegPower += PublicData.getCharactersInfo(PublicData.currentRunnerUsing).flexabilityLevel * 0.5f;

    }

    public void buttonPressed(int code)
    {
        if (code == 0)
        {
            runPressed = true;
        } else if (code == 1)
        {
            jumpPressed = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (runningCamera.enabled && !leaderboardManager.cinematicCamera.gameObject.activeInHierarchy) //change this so it works for enabled
        {
            runningMeter.updateRunMeter();
            if ((Input.GetKeyDown(KeyCode.Space) || runPressed) && runningMeter.runningBar.transform.parent.gameObject.activeInHierarchy) //updating speed on click
            {
                runPressed = false;
                runningMeter.increaseHeight();
                if (leaderboardManager.leaderBoardVisble()) //hides the leaderboard if the player clicks
                {
                    leaderboardManager.hidePersonalBanner();
                }
            }
            if (player.transform.position.x > -1901 && runningMeter.runningBar.transform.parent.gameObject.activeInHierarchy) //testing for an automatic foul by running past the board
            {
                isFoul = true; //set the foul to true
                runningMeter.runningBar.transform.parent.gameObject.SetActive(false); //hides the run meter
                foulImage.enabled = true;
                StartCoroutine(runThroughWait(1.5f));
            }
            if (Input.GetKeyDown(KeyCode.P) || jumpPressed) //if the player presses the jump button
            {
                jumpPressed = false;
                runButton.SetActive(false);
                runningCamera.enabled = false;
                jumpingCamera.enabled = true;
                runningMeter.runningBar.transform.parent.gameObject.SetActive(false); //hide run meter
                player.GetComponentInChildren<Animator>().speed = 0; //make running animation stop
                jumpMeter.jumpBar.gameObject.transform.parent.gameObject.SetActive(true); //sets the jump meter to showing
                jumpMeter.setToRegularSpeed(); //setting the bar speed to normal speed
                float averageSpeed = runningMeter.getAverageSpeed();
                if (averageSpeed > 7500 && averageSpeed < 9500)
                {
                    jumpSparkle.startColor = Color.green;
                } else if (averageSpeed > 6000 && averageSpeed < 10500)
                {
                    jumpSparkle.startColor = Color.yellow;
                } else
                {
                    jumpSparkle.startColor = Color.red;
                }
                jumpSparkle.Play();
                StartCoroutine(jumpMeterTimeLimit(3)); //makes a time limit of x seconds for jumping angle
            }
        }
        if (jumpMeter.jumpBar.gameObject.transform.parent.gameObject.activeInHierarchy) //about to jump
        {
            jumpMeter.updateJumpMeter();
            if (Input.GetKeyDown(KeyCode.Space) || jumpPressed) //makes jump
            {
                jumpPressed = false;
                jumpMeter.MakeJump();
                float jumpMeterSpeed = jumpMeter.jumpMeterSpeed;
                if (player.transform.position.x > -1895.73) //testing got jumping foul
                {
                    isFoul = true;
                    foulImage.enabled = true;
                }
                if (jumpMeterSpeed > 90 && jumpMeterSpeed < 110)
                {
                    jumpSparkle.startColor = Color.green;
                } else if (jumpMeterSpeed > 70 && jumpMeterSpeed < 130)
                {
                    jumpSparkle.startColor = Color.yellow;
                } else
                {
                    jumpSparkle.startColor = Color.red;
                }
                jumpSparkle.Play();
                StartCoroutine(jumpMeterHold(0.5f)); //calls waiting method 
            }
        }
        if (player.GetComponent<Rigidbody>().useGravity) //if in jumping animation
        {
            if (Input.GetKeyDown(KeyCode.Space) || jumpPressed)
            {
                jumpPressed = false;
                float playerHeight = player.transform.position.y;
                if (playerHeight<227.4 && playerHeight>225) //checks if the leg pull is within an optimal range to work
                {
                    player.GetComponent<Rigidbody>().velocity = new Vector3(pullInLegPower, 0, 0); //gives a little extra boost
                    StartCoroutine(legKickVelocity(0.5f)); //make sure the kick doesn't last for ever
                    sandEffect.Play();

                }
                else
                {
                    player.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0); //makes the player stop the jump
                }
                player.GetComponentInChildren<Animator>().Play("LegPull"); //pulls in legs




            }
            if (player.transform.position.y<224.9)
            {
                player.GetComponent<Rigidbody>().useGravity = false; //stops the jumping animation loop
                StartCoroutine(legKickVelocity(0.2f)); //stop the landing from going forever
                if (player.transform.position.x < -1890) //checks if the player is on the sandpit
                {
                    isFoul = true;
                    foulImage.enabled = true;
                } else
                {
                    sandEffect.Play();
                }
                StartCoroutine(waitAfterJump());
            }
        }
        
    }

    IEnumerator jumpMeterTimeLimit(float time) //waits x seconds before saying the player has taken too long with their jumping meter
    {
        yield return new WaitForSeconds(time);
        if (jumpMeter.jumpBar.transform.parent.gameObject.activeInHierarchy)
        {
            jumpMeter.jumpBar.transform.parent.gameObject.SetActive(false);
            runningCamera.enabled = true;
            runningMeter.runningSpeed = 10000;
            foulImage.enabled = true;
            yield return new WaitForSeconds(1.5f);
            runningCamera.enabled = false;
            updatePlayerBanner(-1000);
            afterJump();
        }

    }

    IEnumerator runThroughWait(float time) //waits for the player to run through before calling a foul
    {
        yield return new WaitForSeconds(time);
        runningCamera.enabled = false; //stops the running loop
        player.GetComponentInChildren<Animator>().speed = 1; //normal playing speed instead of running speed
        updatePlayerBanner(-1000); //update the banner to foul
        afterJump();
    }

    IEnumerator legKickVelocity(float time) //holds the velocity of the kicking
    {
        yield return new WaitForSeconds(time);
        player.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0); //makes the player stop the kick velocity

    }

    IEnumerator waitAfterJump() //holds the player for x seconds after they jump
    {
        yield return new WaitForSeconds(1);
        //-1899.73 At 0 feet 
        //-1864.7 At 19 feet
        //-1875.03 At 16 feet
        if (isFoul) // checks for fouls
        {
            updatePlayerBanner(-1000);
        } else {
            float spacesPerInch = PublicData.spacesPerInch; //16 feet minus 19 feet divided by the inches in 3 feet
            float totalInches = (1930.4792068f + player.transform.position.x) / spacesPerInch; //finds total inches that have been jumped (distance jumped divided by spaces per inch of the sand)
            if (currentJumpNumber == 0) //fix random bugs for jumping distance
            {
                totalInches -= 2;
            } else
            {
                totalInches += 5;
            }
            if (totalInches > leaderboardManager.getMarkForEvent("LongJump")) //checks for a new personal record
            {
                leadF.SetLeaderBoardEntry(1, PublicData.gameData.playerName, (int)(leaderboardManager.roundToNearest(0.25f, totalInches) * 100), PublicData.gameData.countryCode + "," + PublicData.currentRunnerUsing);
                leadF.checkForOwnPlayer(1, 20); //checks to make sure it can stay in the top 20
                prImage.enabled = true; //shows the pr banner
                PublicData.gameData.personalBests.longJump = totalInches;

            }
            updatePlayerBanner(leaderboardManager.roundToNearest(0.25f, totalInches));
        }
        afterJump();
       
    }
    

    private void updatePlayerBanner(float mark)
    {
        currentPlayerBanner = leaderboardManager.getPlayerBanner();

        if (currentJumpNumber == 0)
        {
            currentPlayerBanner.mark1 = mark;
        } else if (currentJumpNumber == 1)
        {
            currentPlayerBanner.mark2 = mark;
        } else if (currentJumpNumber == 2)
        {
            currentPlayerBanner.mark3 = mark;
        }
        
    }

    private void afterJump()
    {
        jumpButton.SetActive(false);
        leaderboardManager.showCurrentPlayerMarks(currentPlayerBanner, 3); //updates and shows the player leaderboard
        currentJumpNumber++; //inceases to the next jump
        if (isFoul) //if scratched
        {
            player.GetComponentInChildren<Animator>().Play("Upset"); //Animation for after the jump
        } else if (prImage.enabled) //if got a personal record
        {
            player.GetComponentInChildren<Animator>().Play("Exited"); //Animation for after the jump
        } else
        {
            player.GetComponentInChildren<Animator>().Play("Wave"); //Animation for after the jump
        }
        frontCamera.enabled = true;
        jumpingCamera.enabled = false;
        player.transform.position = new Vector3(-1966, 226.73f, -370.56f); //makes the player in the middle of the runway for show
        runningMeter.runningSpeed = 0; //resets running speed
        StartCoroutine(waitAfterPersonalBanner(3));
    }

    IEnumerator waitAfterPersonalBanner(int time)
    {
        yield return new WaitForSeconds(time);
        if (currentJumpNumber == 3)
        {
            SceneManager.LoadScene("EndScreen");
        }
        player.transform.position = startingPlayerPosition;
        player.GetComponentInChildren<Animator>().Play("Running");
        runningCamera.enabled = true; //shows running camera
        jumpingCamera.enabled = false; //hides jumping camera
        frontCamera.enabled = false;
        runningMeter.runningBar.transform.parent.gameObject.SetActive(true); //shows run meter bar
        leaderboardManager.hidePersonalBanner(); //hides personal banner
        player.GetComponentsInChildren<Transform>()[1].eulerAngles = new Vector3(0, 90, 0); //reset rotation
        player.GetComponentsInChildren<Transform>()[1].localPosition = new Vector3(0, 0, 0); //reset position
        foulImage.enabled = false; //hides the image
        prImage.enabled = false;// hides the image
        isFoul = false; //makes the jump not a foul
        runButton.SetActive(true);
        jumpButton.SetActive(true);
        leaderboardManager.showUpdatedLeaderboard();
    }

    IEnumerator jumpMeterHold(float time) //holds the meter forzen for time so you can she what it landed on
    {
        yield return new WaitForSeconds(time);
        jumpMeter.jumpBar.gameObject.transform.parent.gameObject.SetActive(false); //sets the jump meter to hiding
        float powerPercent = 1; //percent of max power used
        float averageSpeed = runningMeter.getAverageSpeed(); //gets average running speed
        Debug.Log(averageSpeed);
        if (averageSpeed <= 8500) //sets percentage based on distance from 0 to 8500. 8500 is considered the perfect run
        {
            powerPercent = averageSpeed / 8500;
        } else //sets from 0 to 4500 for the top part
        {
            powerPercent = 1 - ((averageSpeed - 8500) / 4500);
        }
        float power = 5 + PublicData.getCharactersInfo(PublicData.currentRunnerUsing).strengthLevel * 1;
        power += PublicData.getCharactersInfo(PublicData.currentRunnerUsing).agilityLevel * 0.1f;
        power *= powerPercent;
        float jumpPercent = 1 - (Math.Abs(100 - jumpMeter.jumpMeterSpeed) / 100);
        power *= jumpPercent;
        power += 10;
        player.GetComponentInChildren<Animator>().Play("LongJump");
        player.GetComponentInChildren<Animator>().speed = power * powerToAnimationSpeedRatio;
        player.GetComponent<Rigidbody>().velocity = new Vector3(power, power*0.6f, 0); //make charcter jump
        player.GetComponent<Rigidbody>().useGravity = true;
    }

    private void FixedUpdate()
    {
        if (runningCamera.enabled)
        {
            player.transform.Translate(new Vector3(0, 0, runningMeter.runningSpeed * runningSpeedRatio)); //making character move according to run meter
            player.GetComponentInChildren<Animator>().speed = runningMeter.runningSpeed * animationRunningSpeedRatio; //making the animation match the sunning speed
            runningMeter.updateTimeElapsed();
        }
    }

    
        
}


//TODO extend sand pit

//TODO effects. Make them more accurate and look nicer

//TODO fix the ending not sorting banners right


