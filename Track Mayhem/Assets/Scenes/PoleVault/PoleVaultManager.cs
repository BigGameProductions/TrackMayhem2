using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class PoleVaultManager : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject basePlayer;

    [SerializeField] private ItemStorage itemStorage;
    [SerializeField] private JumpingMeter jumpMeter;
    [SerializeField]private RunningMeterBar runMeter;
    [SerializeField] private LeaderboardManager leaderboardManager;
    [SerializeField] private Image foulImage; //is the image that appears when you foul or don't land in the sand
    [SerializeField] private Image prImage; //is the image that appears when you pr
    [SerializeField] private GameObject poleVaultPole; //the main pole that is used for the event
    [SerializeField] private GameObject poleGrip; //part of the pole that is being held onto

    private LeaderboardFunctions leadF = new LeaderboardFunctions();

    [SerializeField] private GameObject runButton;
    [SerializeField] private GameObject jumpButton;

    private bool runPressed;
    private bool jumpPressed;

    [SerializeField] private float runningSpeedRatio; //stores the ratio for running compared to runningSpeed
    [SerializeField] private float animationRunningSpeedRatio; //stores the ratio for animation speed compared to runningSpeed

    private bool passedBar = false; //if the player has passed the current height
    private float maxPlayerHeight = 0; //max height the player jumping

    Vector3 tempPos = new Vector3(3.6730001f, -1.17499995f, -0.213f);
    Vector3 temprot = new Vector3(57.1805916f, 299.280121f, 330.898041f);

    PlayerBanner currentPlayerBanner; //stores the current player data in a banner class

    [SerializeField] private Camera playerCamera; //is the main camera of the character
    [SerializeField] private Camera jumpingCamera; //this is the camera for the jumping part of the vault
    [SerializeField] private Camera frontCamera; //front facing camera for after the jump

    [SerializeField] private GameObject barRaise; //bar to raise and lower
    [SerializeField] private GameObject bar; //bar to fall

    private Vector3 startingBarHeight; //height of the bar to determine if it falls

    private int rightHandTransformPosition = 55;

    private float openingHeight = 120;
    private float currentBarHeight = 0; //sets the starting height to x inches

    private Vector3 startingPosition = new Vector3(-2255.1f, 226.73f, -73.9f); //starting position of player
    private Vector3 startingCameraPosition = new Vector3(0.150004253f, 2.09000158f, -1.94002295f); //position of player camera
    private Vector3 startingCameraRotation = new Vector3(19.372f, 0, 0); //rotation of player camera

    //private Vector3 polePosition = new Vector3(-3.83200002f, -0.86500001f, 0.143999994f); // position relative to hand (local)
    //private Vector3 poleRotaion = new Vector3(288.128937f, 348.744415f, 233.137482f); //local pole rotaion

    private Vector3 playerLaunchPosition = new Vector3(-0.049f, -0.0249f, 0.008f);
    private Vector3 playerLaunchPositionStand = new Vector3(-0.0359001607f, -0.0293995682f, -0.0177000742f);
    private Vector3 playerLaunchRotation = new Vector3(1.43247366f, 179.064468f, 65.4714813f);

    private Vector3 playerFinalPosition = new Vector3(0.00280068698f, 0.00170000177f, 0.0182998832f);
    private Vector3 playerFinalRotation = new Vector3(355.625885f, 178.877747f, 14.6459017f);

    private Vector3 playerPikeRotation = new Vector3(-3, 0, 10);

    private int currentJumpNumber = 0;


    bool inCinematic = true; //changes to false once cinematic is over
    bool isRunning = true; //shows if the player is in running form
    bool isPlanting = false; //shows if the player is planting the pole

    [SerializeField] private Canvas controlsCanvas;

    // Start is called before the first frame update
    void Start()
    {
        controlsCanvas.enabled = false;
        jumpButton.GetComponentInChildren<TextMeshProUGUI>().text = "Plant";
        currentBarHeight = openingHeight;
        itemStorage.initRunner(PublicData.currentRunnerUsing, player.transform, basePlayer); //inits the runner into the current scene
        
        poleVaultPole.transform.SetParent(player.GetComponentsInChildren<Transform>()[rightHandTransformPosition]); //sets the parent of the pole to the right hand of the player
        //poleVaultPole.transform.localPosition = polePosition; //sets local position of pole
        //poleVaultPole.transform.localEulerAngles = poleRotaion; //sets local rotation of pole


        jumpMeter.jumpBar.gameObject.transform.parent.gameObject.SetActive(false); //hides the jump meter
        player.GetComponentInChildren<Animator>().Play("PoleRunning");
        leaderboardManager.cinematicCamera.GetComponent<Animator>().SetInteger("event", 2); //sets animator to the current event


        player.transform.position = startingPosition; //puts player in starting position

        startingBarHeight = bar.transform.position;

        updateBarRaiseHeight(); //sets bar to starting height

        foulImage.enabled = false; //hide the foul icon
        prImage.enabled = false; //hides the pr image
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
        if (isPlanting)
        {
            jumpingCamera.transform.position = new Vector3(jumpingCamera.transform.position.x, player.transform.position.y+10, jumpingCamera.transform.position.z);
        }
        if (inCinematic)
        {
            if (!leaderboardManager.cinematicCamera.GetComponent<Animator>().enabled)
            {
                inCinematic = false;
                leaderboardManager.cinematicCamera.gameObject.SetActive(false);
                playerCamera.enabled = true;
            }
        } else
        {
            if (controlsCanvas.enabled == false)
            {
                controlsCanvas.enabled = true;
            }
        }
        if (poleVaultPole.GetComponent<Animator>().GetBool("Launched")) //when the player is launched in the air
        {
            if (player.transform.position.y > maxPlayerHeight)
            {
                maxPlayerHeight = player.transform.position.y;
            }
            float speedRatio = (PublicData.getCharactersInfo(PublicData.currentRunnerUsing).agilityLevel + 2 * PublicData.getCharactersInfo(PublicData.currentRunnerUsing).agilityLevel) / 30;
            if ((Input.GetKeyDown(KeyCode.Space) || jumpPressed) && player.GetComponentInChildren<Animator>().GetBool("Pike"))
            {
                jumpPressed = false;
                player.GetComponentInChildren<Animator>().speed = (float)(0.75 + (1.25*speedRatio)); //anaimation speed
                player.GetComponentInChildren<Animator>().Play("Pike");
                jumpButton.SetActive(false);
            } else if (Input.GetKeyDown(KeyCode.Space) || jumpPressed)
            {
                jumpButton.GetComponentInChildren<TextMeshProUGUI>().text = "Pike";
                jumpPressed = false;
                player.GetComponentInChildren<Animator>().speed = (float)(0.75 + (1.25 * speedRatio)); //anaimation speed
                player.GetComponentInChildren<Animator>().Play("LegBack");
                //player.transform.localEulerAngles = playerPikeRotation;
               // player.GetComponentInChildren<Transform>().localEulerAngles = new Vector3(0, 270, 0);
                //player.transform.Translate(0, 0, -3); //testing
                player.GetComponentInChildren<Animator>().SetBool("Pike", true);
            }
            if (player.transform.position.y < 232) { //end jump
                poleVaultPole.GetComponent<Animator>().SetBool("Launched", false);
                StartCoroutine(waitForFall(0.5f));


            }

        }
       
        IEnumerator waitForFall(float delay)
        {
            yield return new WaitForSeconds(delay);
            passedBar = !(startingBarHeight.y - bar.transform.position.y > 1.5 || barStraight(bar, 1) || maxPlayerHeight+2.5 < bar.transform.position.y);
            if (!passedBar)
            {
                Debug.Log(startingBarHeight.y + "" + bar.transform.position.y);
                updatePlayerBanner(-10000); //code for a miss
            }
            else
            {
                updatePlayerBanner(-10); //code for a make        
            }
            afterJump();
        }

        if (jumpMeter.jumpBar.gameObject.transform.parent.gameObject.activeInHierarchy) //about to jump and updates jumping meter
        {
            jumpMeter.updateJumpMeter();
            if (Input.GetKeyDown(KeyCode.Space) || jumpPressed) //makes jump
            {
                jumpButton.SetActive(false); //hides button until curl and pike
                jumpButton.GetComponentInChildren<TextMeshProUGUI>().text = "Curl";
                jumpPressed = false;
                jumpMeter.MakeJump();
                //float jumpMeterSpeed = jumpMeter.jumpMeterSpeed;
                StartCoroutine(startPlant(0.5f)); //calls planting method 

            }
        }

        if (playerCamera.enabled && !leaderboardManager.cinematicCamera.gameObject.activeInHierarchy && isRunning) //runs when the player is in the running stage
        {
            runMeter.updateRunMeter();
            if ((Input.GetKeyDown(KeyCode.Space) || runPressed) && runMeter.runningBar.transform.parent.gameObject.activeInHierarchy) //updating speed on click
            {
                runPressed = false;
                runMeter.increaseHeight();
                if (leaderboardManager.leaderBoardVisble()) //hides the leaderboard if the player clicks
                {
                    leaderboardManager.hidePersonalBanner();
                }
            }
            if (player.transform.position.z < -194)
            {
                isRunning = false; //stops running speed
                isPlanting = true; //makes planting fail
                foulImage.enabled = true; //show foul
                runMeter.runningBar.transform.parent.gameObject.SetActive(false); //hide run meter
                StartCoroutine(foulRun(1)); //wait x seconds then end jump
                
            } else  {
                if (Input.GetKeyDown(KeyCode.P) || jumpPressed) //if the player presses the jump button
                {
                    jumpButton.GetComponentInChildren<TextMeshProUGUI>().text = "Jump";
                    jumpPressed = false;
                    runButton.SetActive(false);
                    jumpMeter.jumpBar.transform.parent.gameObject.SetActive(true);
                    isRunning = false; //shows the player is not longer running
                    runMeter.runningBar.transform.parent.gameObject.SetActive(false); //hides running meter
                    player.GetComponentInChildren<Animator>().speed = 0; //stops the player animations
                    jumpMeter.setToRegularSpeed(); //setting the bar speed to normal speed
                    /*runningCamera.enabled = false;
                    jumpingCamera.enabled = true;
                    runningMeter.runningBar.transform.parent.gameObject.SetActive(false); //hide run meter
                    player.GetComponentInChildren<Animator>().speed = 0; //make running animation stop
                    jumpMeter.jumpBar.gameObject.transform.parent.gameObject.SetActive(true); //sets the jump meter to showing
                    jumpMeter.setToRegularSpeed(); //setting the bar speed to normal  speed
                    float averageSpeed = runningMeter.getAverageSpeed();
                    if (averageSpeed > 7500 && averageSpeed < 9500)
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
                    StartCoroutine(jumpMeterTimeLimit(3)); //makes a time limit of x seconds for jumping angle*/
                }
            }

            
            
        }
    }

    IEnumerator foulRun(float delay)
    {
        yield return new WaitForSeconds(delay);
        poleVaultPole.transform.parent = null;
        updatePlayerBanner(-10000);
        afterJump();
        
    }

    private bool barStraight(GameObject bar, float range)
    {
        if (bar.transform.eulerAngles.x < (0-range) || bar.transform.eulerAngles.x > range)
        {
            return false;
        }
        if (bar.transform.eulerAngles.y < (0 - range) || bar.transform.eulerAngles.y > range)
        {
            return false;
        }
        if (bar.transform.eulerAngles.z < (0 - range) || bar.transform.eulerAngles.z > range)
        {
            return false;
        }
        return true;
    }

    private void updatePlayerBanner(float mark)
    {
        currentPlayerBanner = leaderboardManager.getPlayerBanner();

        if (currentJumpNumber == 0)
        {
            currentPlayerBanner.mark1 = mark;
        }
        else if (currentJumpNumber == 1)
        {
            currentPlayerBanner.mark2 = mark;
        }
        else if (currentJumpNumber == 2)
        {
            currentPlayerBanner.mark3 = mark;
        }

    }

    IEnumerator startPlant(float delay) //starts the planting process
    {
        yield return new WaitForSeconds(delay);
        jumpMeter.jumpBar.transform.parent.gameObject.SetActive(false);
        isPlanting = true; //showing the player is planting the pole
        player.GetComponentInChildren<Animator>().Play("PolePlanting"); //player plant animation
        poleVaultPole.GetComponent<Animator>().enabled = false; //temp way to make the animation work
        player.GetComponentInChildren<Animator>().speed = 1;
        poleVaultPole.transform.localPosition = tempPos; //sets local position of pole
        poleVaultPole.transform.localEulerAngles = temprot; //sets local rotation of pole
        StartCoroutine(startVault(1));

    }

    IEnumerator startVault(float delay) //starts the initial pole bend
    {
        yield return new WaitForSeconds(delay);
        if (player.transform.position.z > -189)
        {
            foulImage.enabled = true;
            poleVaultPole.transform.parent = null;
            updatePlayerBanner(-10000);
            afterJump();
        } else
        {
            poleVaultPole.GetComponent<Animator>().enabled = true;
            poleVaultPole.transform.parent = null; //makes the parent root of scene so it moves independently
            player.transform.parent = poleGrip.transform; //makes the player follow the pole grip
            playerCamera.enabled = false; //makes the running camera not enabled
            jumpingCamera.enabled = true; //makes the vaulting camera enabled
            player.transform.localPosition = playerLaunchPosition; //makes the player in the right position
            player.transform.localEulerAngles = playerLaunchRotation; //makes the player in the right rotation
            poleVaultPole.GetComponent<Animator>().speed = 1;
            poleVaultPole.GetComponent<Animator>().Play("VaultStage1"); //makes the pole in the right animation
            StartCoroutine(stage1Vault(1.5f));
        }
        
    }

    IEnumerator stage1Vault(float delay) //stage on of the vault and waits for player input
    {
        yield return new WaitForSeconds(delay);
        player.transform.localPosition = playerLaunchPositionStand; //makes the player in the right curl up position
        player.GetComponentInChildren<Animator>().Play("PoleRunnerStage1"); //makes the player curl up
        StartCoroutine(launchPlayer(0.5f)); //makes next stage temp


    }

    IEnumerator launchPlayer(float delay)
    {
        yield return new WaitForSeconds(delay);
        poleVaultPole.GetComponent<Animator>().SetBool("Launched", true); //sets animator parameter to show player has been launched
        poleVaultPole.GetComponent<Animator>().speed = 1; //makes the pole normal speed and resumes it
        player.GetComponentInChildren<Animator>().Play("PoleRunnerStage2"); //makes the player in the final upright launching position
        player.transform.localPosition = playerFinalPosition; //gets right launch position
        player.transform.localEulerAngles = playerFinalRotation; //gets right launch rotation
        StartCoroutine(addUpForce(0.5f)); //wait for form then launch up


    }

    IEnumerator addUpForce(float delay) //adds delay to when the player in lauched up
    {
        yield return new WaitForSeconds(delay);
        jumpButton.SetActive(true); //allows curl to happen
        player.GetComponent<Rigidbody>().useGravity = true; //makes it so that player can fall
        float jumpPowerScale = 0.4f; //scale to balence the jumping power
        float runPowerScale = 1;
        float runningPower = runPowerScale*PublicData.getCharactersInfo(PublicData.currentRunnerUsing).speedLevel; //power of running temp 
        float runPowerPercentage; //defualt to max power
        float averageSpeed = runMeter.getAverageSpeed(); //gets average running speed
        float inPitSpeed = -5; //default for the pit
        float maxPitChange = 3; //max change from default on each side
        if (averageSpeed <= 8500) //sets percentage based on distance from 0 to 8500. 8500 is considered the perfect run
        {
            runPowerPercentage = averageSpeed / 8500;
            inPitSpeed += maxPitChange * (1 - runPowerPercentage); //less in the pit
        }
        else //sets from 0 to 4500 for the top part
        {
            runPowerPercentage = 1 - ((averageSpeed - 8500) / 4500);
            //inPitSpeed -= maxPitChange * (1 - runPowerPercentage); //more in the pit
        }
        runningPower *= runPowerPercentage; //max 20
        float jumpingPower = jumpPowerScale*(PublicData.getCharactersInfo(PublicData.currentRunnerUsing).agilityLevel + PublicData.getCharactersInfo(PublicData.currentRunnerUsing).flexabilityLevel + PublicData.getCharactersInfo(PublicData.currentRunnerUsing).strengthLevel);
        //max 15
        float jumpPercentage = 1 - (Math.Abs(100 - jumpMeter.jumpMeterSpeed) / 100);
        float power = (runningPower + ((jumpingPower*jumpPercentage)*2))/3; //jump is 2/3 jump and 1/3 run  
        power += 1; //default power
        player.GetComponent<Rigidbody>().velocity = new Vector3(0, power, inPitSpeed); //makes player launch up


    }

   

    private void afterJump()
    {
        jumpButton.SetActive(false); //hides jump button for after jump
        leaderboardManager.showCurrentPlayerMarks(currentPlayerBanner, 3); //updates and shows the player leaderboard
        poleVaultPole.GetComponent<Animator>().SetBool("Launched", false);
        currentJumpNumber++; //inceases to the next jump
        if (!passedBar) //if scratched
        {
            player.GetComponentInChildren<Animator>().Play("Upset"); //Animation for after the jump
        }
        else if (currentBarHeight > PublicData.gameData.personalBests.polevault) //if got a personal record
        {

            leadF.SetLeaderBoardEntry(2, PublicData.gameData.playerName, (int)(currentBarHeight * 100), PublicData.gameData.countryCode + "," + PublicData.currentRunnerUsing);
            leadF.checkForOwnPlayer(2, 20); //checks to make sure it can stay in the top 20
            prImage.enabled = true;
            PublicData.gameData.personalBests.polevault = currentBarHeight;
            player.GetComponentInChildren<Animator>().Play("Exited"); //Animation for after the jump
            currentJumpNumber = 0;
        }
        else
        {
            player.GetComponentInChildren<Animator>().Play("Wave"); //Animation for after the jump
            currentJumpNumber = 0;
        }
        if (currentJumpNumber == 0) //if it needs to move up
        {
            //move up 6 inches
            currentBarHeight += 6;
            updateBarRaiseHeight();
        }
        frontCamera.enabled = true;
        jumpingCamera.enabled = false;
        player.transform.parent = null;
        player.transform.position = new Vector3(-2255.1001f, 230.100006f, -242.100006f); //sets the after jump position
        player.transform.eulerAngles = new Vector3(0, 180, 0);
        runMeter.runningSpeed = 0; //resets running speed
        jumpButton.GetComponentInChildren<TextMeshProUGUI>().text = "Plant";
        StartCoroutine(waitAfterPersonalBanner(3));
    }

    private void updateBarRaiseHeight() //updates the height of the bar and resets all rotation
    {
        bar.transform.localEulerAngles = new Vector3(0, 0, 0); //reset bar rotation
        bar.transform.localPosition = new Vector3(0, 29.2f, 0);
        //set the height to the current height
        startingBarHeight = bar.transform.position;
        barRaise.transform.localPosition = new Vector3(0, 0, (float)(-226.9 + currentBarHeight * PublicData.spacesPerInch*3));
        leaderboardManager.updateCurrentBarHeight(currentBarHeight, openingHeight); //updates the height and if it is opening height
    }

    IEnumerator waitAfterPersonalBanner(int time)
    {
        yield return new WaitForSeconds(time);
        if (currentJumpNumber == 3)
        {
            leaderboardManager.simRemainingJumps();
            SceneManager.LoadScene("EndScreen");
        }
        player.transform.position = startingPosition;
        player.GetComponentInChildren<Animator>().Play("PoleRunning");
        poleVaultPole.GetComponent<Animator>().Play("RunningPosition");
        poleVaultPole.transform.SetParent(player.GetComponentsInChildren<Transform>()[rightHandTransformPosition]); //sets the parent of the pole to the right hand of the player
        playerCamera.enabled = true; //shows running camera
        jumpingCamera.enabled = false; //hides jumping camera
        frontCamera.enabled = false; //hides the front camera
        runMeter.runningBar.transform.parent.gameObject.SetActive(true); //shows run meter bar
        leaderboardManager.hidePersonalBanner(); //hides personal banner
        isRunning = true; //updates state
        isPlanting = false; //updates state
        player.GetComponentsInChildren<Transform>()[1].localEulerAngles = new Vector3(0, 0, 0); //reset rotation
        player.GetComponentsInChildren<Transform>()[1].localPosition = new Vector3(0, 0, 0); //reset position
        playerCamera.transform.localPosition = startingCameraPosition; //set the camera in the right position
        playerCamera.transform.localEulerAngles = startingCameraRotation; //set the camera in the right angle
        player.GetComponent<Rigidbody>().useGravity = false; //stop player from falling dowm
        player.GetComponentInChildren<Animator>().SetBool("Pike", false); //reset pike var
        foulImage.enabled = false; //hides the image
        updateBarRaiseHeight();
        prImage.enabled = false;// hides the image
        passedBar = false;
        maxPlayerHeight = 0;
        runButton.SetActive(true);
        jumpButton.SetActive(true);
        //isFoul = false; //makes the jump not a foul
        leaderboardManager.showUpdatedLeaderboard();

    }
    private void FixedUpdate() //fixed for speed and running
    {
        if (playerCamera.enabled && (isRunning || isPlanting)) //if running or planting
        {
            if (isPlanting) //slowly lowers speed if in the plant
            {
                runMeter.updateRunMeter();
            }
            player.transform.Translate(new Vector3(0, 0, runMeter.runningSpeed * runningSpeedRatio)); //making character move according to run meter
           if (isRunning) player.GetComponentInChildren<Animator>().speed = runMeter.runningSpeed * animationRunningSpeedRatio; //making the animation match the sunning speed
           if (isRunning)
            {
                runMeter.updateTimeElapsed();

            }
        }
    }
}
