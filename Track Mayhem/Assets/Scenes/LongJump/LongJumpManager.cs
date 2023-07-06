using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;
using TMPro;
using UnityEngine.EventSystems;

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

    private float ljSpaces = 3.08333333333f/12;

    [SerializeField] EventSystem ev;


    [SerializeField] private Image foulImage; //is the image that appears when you foul or don't land in the sand

    [SerializeField] private ParticleSystem sandEffect;
    [SerializeField] private ParticleSystem jumpSparkle;

    [SerializeField] RunningMeterBar runningMeter;
    [SerializeField] JumpingMeter jumpMeter;
    [SerializeField] Button infoButton;

    [SerializeField] private Canvas controlsCanvas;

    [SerializeField] private Canvas boardChangeCanvas;
    [SerializeField] private GameObject boardChangeButton;
    [SerializeField] private GameObject jumpBoard;
    [SerializeField] private Camera boardChangeCamera;
    [SerializeField] private TextMeshProUGUI boardChangeText;

    private GameObject playerLeftFoot;
    private GameObject playerRightFoot;

    private float boardDistance = 96; //distance of the long jump board
    private float startingBoardX; //x position for 8ft board

    bool firstTimeShow = false; //for first time objects to show

    public bool godMode;

    private float timeDiff = 10;

    
    // Start is called before the first frame update
    void Start()
    {

        boardChangeButton.gameObject.SetActive(false);
        jumpMeter.jumpBar.gameObject.transform.parent.GetComponent<Animator>().speed = 0;
        startingBoardX = jumpBoard.transform.position.x;
        controlsCanvas.enabled = false;
        jumpButton.GetComponentInChildren<TextMeshProUGUI>().text = "Takeoff";
        itemStorage.initRunner(PublicData.currentRunnerUsing, player.transform); //inits the runner into the current scene
        for(int i=0; i<player.GetComponentsInChildren<Transform>().Length; i++)
        {
            Transform tf = player.GetComponentsInChildren<Transform>()[i];
            if (tf.name == "mixamorig1:RightToe_End")
            {
                playerRightFoot = tf.gameObject;
            }
            if (tf.name == "mixamorig1:LeftToe_End")
            {
                playerLeftFoot = tf.gameObject;
            }
        }
        infoButton.gameObject.SetActive(false);
        jumpMeter.jumpBar.gameObject.transform.parent.gameObject.SetActive(false); //hides the jump meter
        player.GetComponentInChildren<Animator>().Play("Running");

        player.transform.position = startingPlayerPosition; //puts player in starting position

        leaderboardManager.cinematicCamera.GetComponent<Animator>().SetInteger("event", 1); //sets animator to the current event

        foulImage.gameObject.SetActive(false); //hide the foul icon

        /*runningMeter.barDecreaseSpeed -= PublicData.getCharactersInfo(PublicData.currentRunnerUsing).speedLevel * 10; //lowers bar slower
        runningMeter.speedPerClick -= PublicData.getCharactersInfo(PublicData.currentRunnerUsing).strengthLevel * 0.5f; //raises bar a bit slower*/

        //jumpMeter.jumpBarSpeed -= PublicData.getCharactersInfo(PublicData.currentRunnerUsing).agilityLevel * 20; //makes the jump abr go slower

        pullInLegPower += PublicData.curveValue(PublicData.getCharactersInfo(PublicData.currentRunnerUsing).flexabilityLevel, 5, "fl");

        boardDistance = PublicData.getCharactersInfo(PublicData.currentRunnerUsing).eventPrefs.longJumpBoard;
        boardChangeButton.GetComponentInChildren<TextMeshProUGUI>().text = "Board: " + boardDistance / 12 + "'";
        boardChangeText.text = "Change board to: " + boardDistance / 12 + "'";
        jumpBoard.transform.position = new Vector3(startingBoardX - (boardDistance*ljSpaces), jumpBoard.transform.position.y, jumpBoard.transform.position.z);
        EventTrigger trigger = jumpButton.gameObject.AddComponent<EventTrigger>();
        var pointerDown = new EventTrigger.Entry();
        pointerDown.eventID = EventTriggerType.PointerDown;
        pointerDown.callback.AddListener((e) => jumpPressed = true);
        trigger.triggers.Add(pointerDown);

    }

    public void buttonPressed(int code)
    {
        if (code == 0)
        {
            runPressed = true;
        } else if (code == 1)
        {
            jumpPressed = true;
            if (jumpButton.GetComponentInChildren<TextMeshProUGUI>().text == "Pike")
            {
                jumpButton.SetActive(false);
            }
        }
    }

    // Update is called once per frame
    [Obsolete]
    void Update()
    {
        if (runningCamera.enabled && !leaderboardManager.cinematicCamera.gameObject.activeInHierarchy) //change this so it works for enabled
        {
            if (controlsCanvas.enabled == false && !boardChangeCamera.enabled)
            {
                controlsCanvas.enabled = true;
                if (!firstTimeShow)
                {
                    boardChangeButton.gameObject.SetActive(true);
                    infoButton.gameObject.SetActive(true);
                    firstTimeShow = true;
                }
                


            }
            runningMeter.updateRunMeter();
            if ((Input.GetKeyDown(KeyCode.Space) || runPressed) && runningMeter.runMeterSlider.gameObject.activeInHierarchy) //updating speed on click
            {
                runPressed = false;
                boardChangeButton.gameObject.SetActive(false);
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
            if (player.transform.position.x > jumpBoard.transform.position.x && runningMeter.runMeterSlider.gameObject.activeInHierarchy) //testing for an automatic foul by running past the board
            {
                isFoul = true; //set the foul to true
                runningMeter.runMeterSlider.gameObject.SetActive(false); //hides the run meter
                foulImage.gameObject.SetActive(true);
                foulImage.GetComponent<Animator>().Play("FoulSlide");
                jumpButton.SetActive(false);
                runButton.SetActive(false);
                StartCoroutine(runThroughWait(1.5f));
            }
            if (Input.GetKeyDown(KeyCode.P) || jumpPressed) //if the player presses the jump button
            {
                if (!isFoul && player.transform.position.x < jumpBoard.transform.position.x)
                {
                    jumpButton.GetComponentInChildren<TextMeshProUGUI>().text = "Jump";
                    jumpPressed = false;
                    runButton.SetActive(false);
                    runningCamera.enabled = false;
                    jumpingCamera.enabled = true;
                    runningMeter.runMeterSlider.gameObject.SetActive(false); //hide run meter
                    player.GetComponentInChildren<Animator>().speed = 0; //make running animation stop
                    jumpMeter.jumpBar.gameObject.transform.parent.gameObject.SetActive(true); //sets the jump meter to showing
                    //jumpMeter.setToRegularSpeed(); //setting the bar speed to normal speed
                    jumpMeter.jumpBar.gameObject.transform.parent.GetComponent<Animator>().speed = 4;
                    float averageSpeed = runningMeter.getAverageSpeed();
                    if (averageSpeed > 8000 && averageSpeed < 9000)
                    {
                        jumpSparkle.startColor = Color.green;
                    }
                    else if (averageSpeed > 6000 && averageSpeed < 10500)
                    {
                        jumpSparkle.startColor = Color.yellow;
                    }
                    else
                    {
                        jumpSparkle.startColor = Color.red;
                    }
                    jumpSparkle.Play();
                    StartCoroutine(jumpMeterTimeLimit(3)); //makes a time limit of x seconds for jumping angle
                }
            }
                
        }
        if (jumpMeter.jumpBar.gameObject.transform.parent.gameObject.activeInHierarchy) //about to jump
        {
            jumpMeter.updateJumpMeter();
            if (Input.GetKeyDown(KeyCode.Space) || jumpPressed) //makes jump
            {
                if (!isFoul)
                {
                    if (playerLeftFoot.transform.position.x > jumpBoard.transform.position.x || playerRightFoot.transform.position.x > jumpBoard.transform.position.x)
                    {
                        isFoul = true; //set the foul to true
                        foulImage.gameObject.SetActive(true);
                        foulImage.GetComponent<Animator>().Play("FoulSlide");
                    }
                    jumpButton.SetActive(false); //hides button until pike
                    jumpButton.GetComponentInChildren<TextMeshProUGUI>().text = "Pike";
                    jumpPressed = false;
                    //jumpMeter.MakeJump();
                    jumpMeter.jumpBar.gameObject.transform.parent.GetComponent<Animator>().speed = 0;
                    float time = jumpMeter.jumpBar.gameObject.transform.parent.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime;
                    if (time >= 1) {
                        time = time % (int)time;
                    }

                    if (time < 0.5)
                    {
                        timeDiff = Math.Abs(time - 0.25f);
                    }
                    else
                    {
                        timeDiff = Math.Abs(time - 0.75f);
                    }
                    if (timeDiff < 0.035)
                    {
                        jumpSparkle.startColor = Color.green;
                    }
                    else if (timeDiff < 0.1)
                    {
                        jumpSparkle.startColor = Color.yellow;
                    }
                    else
                    {
                        jumpSparkle.startColor = Color.red;
                    }
                    jumpSparkle.Play();
                    StartCoroutine(jumpMeterHold(0.5f)); //calls waiting method 
                }
                
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
                player.GetComponentInChildren<Animator>().speed = 1; //making the animation match the sunning speed
                jumpButton.SetActive(false);

            }
            if (player.transform.position.y<225)
            {
                player.GetComponent<Rigidbody>().useGravity = false; //stops the jumping animation loop
                jumpButton.SetActive(false);
                StartCoroutine(legKickVelocity(0.2f)); //stop the landing from going forever
                if (player.transform.position.x < -1890 || player.transform.position.x  > -1854) //checks if the player is on the sandpit
                {
                    isFoul = true;
                    foulImage.gameObject.SetActive(true);
                    foulImage.GetComponent<Animator>().Play("FoulSlide");
                }
                else
                {
                    sandEffect.Play();
                }
                StartCoroutine(waitAfterJump());
            }
        }
        
    }

    public void changeBoard()
    {
        boardChangeCanvas.enabled = true;
        controlsCanvas.enabled = false;
        boardChangeButton.SetActive(false);
        boardChangeCamera.enabled = true;
        if (leaderboardManager.leaderBoardVisble()) //hides the leaderboard if the player clicks
        {
            leaderboardManager.hidePersonalBanner();
        }
    }

    public void changeBoardPosition(int direction)
    {
        boardDistance += direction * 12;
        if (boardDistance < 48)
        {
            boardDistance = 48;
        }
        if (boardDistance > 600) {
            boardDistance = 600;
        }
        jumpBoard.transform.position = new Vector3(startingBoardX -(boardDistance*ljSpaces),jumpBoard.transform.position.y, jumpBoard.transform.position.z);
        boardChangeText.text = "Change board to: " + boardDistance / 12 + "'";
    }

    public void finalBoard()
    {
        boardChangeCanvas.enabled = false;
        controlsCanvas.enabled = true;
        boardChangeButton.SetActive(true);
        boardChangeCamera.enabled = false;
        boardChangeButton.GetComponentInChildren<TextMeshProUGUI>().text = "Board: " + boardDistance / 12 + "'";
        PublicData.getCharactersInfo(PublicData.currentRunnerUsing).eventPrefs.longJumpBoard = boardDistance;
        ev.SetSelectedGameObject(null);
    }
    IEnumerator jumpMeterTimeLimit(float time) //waits x seconds before saying the player has taken too long with their jumping meter
    {
        yield return new WaitForSeconds(time);
        if (!isFoul) {
            if (jumpMeter.jumpBar.transform.parent.gameObject.activeInHierarchy && jumpMeter.jumpBar.gameObject.transform.parent.GetComponent<Animator>().speed != 0)
            {
                jumpMeter.jumpBar.transform.parent.gameObject.SetActive(false);
                runningCamera.enabled = true;
                runningMeter.runningSpeed = 10000;
                foulImage.gameObject.SetActive(true);
                foulImage.GetComponent<Animator>().Play("FoulSlide");
                yield return new WaitForSeconds(1.5f);
                runningCamera.enabled = false;
                updatePlayerBanner(-1000);
                afterJump(false);
            }
        }

        

    }

    IEnumerator runThroughWait(float time) //waits for the player to run through before calling a foul
    {
        yield return new WaitForSeconds(time);
        runningCamera.enabled = false; //stops the running loop
        player.GetComponentInChildren<Animator>().speed = 1; //normal playing speed instead of running speed
        updatePlayerBanner(-1000); //update the banner to foul
        afterJump(false);
    }

    IEnumerator legKickVelocity(float time) //holds the velocity of the kicking
    {
        yield return new WaitForSeconds(time);
        player.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0); //makes the player stop the kick velocity

    }

    IEnumerator waitAfterJump() //holds the player for x seconds after they jump
    {
        yield return new WaitForSeconds(1);
        bool record = false;
        //-1899.73 At 0 feet 
        //-1864.7 At 19 feet
        //-1875.03 At 16 feet
        if (isFoul) // checks for fouls
        {
            updatePlayerBanner(-1000);
        } else {
            float spacesPerInch = ljSpaces; //16 feet minus 19 feet divided by the inches in 3 feet
            //was 1930.4792068f
            float totalInches = (-1*jumpBoard.transform.position.x + player.transform.position.x) / spacesPerInch; //finds total inches that have been jumped (distance jumped divided by spaces per inch of the sand)
            /*if (currentJumpNumber == 0) //fix random bugs for jumping distance
            {
                totalInches -= 2;
            } else
            {
                totalInches += 5;
            }*/
            //totalInches += boardDistance - 96;
            if (totalInches > Int32.Parse(PublicData.gameData.leaderboardList[1][1][0])/100.0f && totalInches > PublicData.gameData.personalBests.longJump) //game record
            {
                PublicData.gameData.personalBests.longJump = totalInches;
                PublicData.getCharactersInfo(PublicData.currentRunnerUsing).characterBests.longJump = totalInches;
                leadF.SetLeaderBoardEntry(1, PublicData.gameData.playerName, (int)(leaderboardManager.roundToNearest(0.25f, totalInches) * 100), PublicData.gameData.countryCode + "," + PublicData.currentRunnerUsing);
                leadF.checkForOwnPlayer(1, 20); //checks to make sure it can stay in the top 20
                leaderboardManager.addMarkLabelToPlayer(1);
                leaderboardManager.showRecordBanner(2);
                record = true;
            }
            else if (totalInches > PublicData.gameData.personalBests.longJump) //checks for a new personal record
            {
                leadF.SetLeaderBoardEntry(1, PublicData.gameData.playerName, (int)(leaderboardManager.roundToNearest(0.25f, totalInches) * 100), PublicData.gameData.countryCode + "," + PublicData.currentRunnerUsing);
                leadF.checkForOwnPlayer(1, 20); //checks to make sure it can stay in the top 20
                leaderboardManager.showRecordBanner(1);
                PublicData.gameData.personalBests.longJump = leaderboardManager.roundToNearest(0.25f,totalInches);
                PublicData.getCharactersInfo(PublicData.currentRunnerUsing).characterBests.longJump = leaderboardManager.roundToNearest(0.25f, totalInches); ;
                leaderboardManager.addMarkLabelToPlayer(3);
                record = true;
            }
            else if (totalInches > PublicData.getCharactersInfo(PublicData.currentRunnerUsing).characterBests.longJump)
            {
                PublicData.getCharactersInfo(PublicData.currentRunnerUsing).characterBests.longJump = leaderboardManager.roundToNearest(0.25f, totalInches); ;
                leaderboardManager.addMarkLabelToPlayer(2);
                leaderboardManager.showRecordBanner(0);
                record = true;
            }
            updatePlayerBanner(leaderboardManager.roundToNearest(0.25f, totalInches));
        }
        afterJump(record);
       
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

    private void afterJump(bool record)
    {
        boardChangeButton.gameObject.SetActive(false);
        jumpButton.SetActive(false);
        leaderboardManager.showCurrentPlayerMarks(currentPlayerBanner, 3); //updates and shows the player leaderboard
        currentJumpNumber++; //inceases to the next jump
        player.GetComponentInChildren<Animator>().speed = 1;
        foulImage.gameObject.SetActive(false);
        if (isFoul) //if scratched
        {
            player.GetComponentInChildren<Animator>().Play("Upset"); //Animation for after the jump
        } else if (record) 
        {
            player.GetComponentInChildren<Animator>().Play("Exited"); //Animation for after the jump
        } else
        {
            player.GetComponentInChildren<Animator>().Play("Wave"); //Animation for after the jump
        }
        frontCamera.enabled = true;
        jumpingCamera.enabled = false;
        player.transform.position = new Vector3(-1874.80005f, 226.729996f, -369.709991f); //makes the player in the middle of the runway for show
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
        runningMeter.runMeterSlider.gameObject.SetActive(true); //shows run meter bar
        leaderboardManager.hidePersonalBanner(); //hides personal banner
        player.GetComponentsInChildren<Transform>()[1].eulerAngles = new Vector3(0, 90, 0); //reset rotation
        player.GetComponentsInChildren<Transform>()[1].localPosition = new Vector3(0, 0, 0); //reset position
        foulImage.gameObject.SetActive(false); //hides the image
        isFoul = false; //makes the jump not a foul
        runButton.SetActive(true);
        jumpButton.SetActive(true);
        leaderboardManager.showRecordBanner(-1);
        jumpButton.GetComponentInChildren<TextMeshProUGUI>().text = "Takeoff";
        boardChangeButton.gameObject.SetActive(true);
        leaderboardManager.showUpdatedLeaderboard();
    }




    IEnumerator jumpMeterHold(float time) //holds the meter forzen for time so you can she what it landed on
    {
        yield return new WaitForSeconds(time);
        jumpMeter.jumpBar.gameObject.transform.parent.gameObject.SetActive(false); //sets the jump meter to hiding
        float powerPercent = 1; //percent of max power used
        float averageSpeed = runningMeter.getAverageSpeed(); //gets average running speed
        if (averageSpeed <= 8500) //sets percentage based on distance from 0 to 8500. 8500 is considered the perfect run
        {
            powerPercent = averageSpeed / 8500;
        }
        else //sets from 0 to 4500 for the top part
        {
            powerPercent = 1 - ((averageSpeed - 8500) / 4500);
        }
        //curve formulas
        float power = (float)(8.5f + PublicData.curveValue(PublicData.getCharactersInfo(PublicData.currentRunnerUsing).strengthLevel, 6, "st"));
        power += PublicData.curveValue(PublicData.getCharactersInfo(PublicData.currentRunnerUsing).agilityLevel, 1, "ag");
        power += PublicData.curveValue(PublicData.getCharactersInfo(PublicData.currentRunnerUsing).speedLevel, 2, "sp");
        if (godMode) powerPercent = 1;
        power *= powerPercent;
        float jumpPercent = 1 - (timeDiff / 0.25f);
        if (godMode) jumpPercent = 1;
        power *= jumpPercent;
        power += 10;
        player.GetComponentInChildren<Animator>().Play("LongJump");
        player.GetComponentInChildren<Animator>().speed = 1 - power * powerToAnimationSpeedRatio;
        if (PublicData.currentRunnerUsing == 2) //check for duncan ability
        {
            player.GetComponent<Rigidbody>().velocity = new Vector3(power * (1 + (float.Parse(PublicData.charactersInfo.ElementAt(PublicData.currentRunnerOn + 1)[9]) / 100f)), (power * 0.6f)* (1-(float.Parse(PublicData.charactersInfo.ElementAt(PublicData.currentRunnerOn + 1)[10])/100f)), 0); //make charcter jump
        } else
        {
            player.GetComponent<Rigidbody>().velocity = new Vector3(power, power * 0.6f, 0); //make charcter jump
        }
        player.GetComponent<Rigidbody>().useGravity = true;
        jumpButton.SetActive(true); //allows pike

    }

    private void FixedUpdate()
    {
        if (runningCamera.enabled)
        {
            float speed = runningMeter.runningSpeed;
            if (speed > PublicData.averageSpeedDuringRun)
            {
                speed = PublicData.averageSpeedDuringRun - (speed - PublicData.averageSpeedDuringRun); //makes it so over slows you down
            }
            player.transform.Translate(new Vector3(0, 0, speed*1.3f * runningSpeedRatio)); //making character move according to run meter
            player.GetComponentInChildren<Animator>().speed = speed * animationRunningSpeedRatio; //making the animation match the sunning speed
            runningMeter.updateTimeElapsed();
        }
    }

    
        
}





//TODO determine to round the marks or not


