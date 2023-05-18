using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PoleVaultManager : MonoBehaviour
{
    [SerializeField] private GameObject player;

    [SerializeField] private ItemStorage itemStorage;
    [SerializeField] private JumpingMeter jumpMeter;
    [SerializeField]private RunningMeterBar runMeter;
    [SerializeField] private LeaderboardManager leaderboardManager;
    [SerializeField] private Image foulImage; //is the image that appears when you foul or don't land in the sand
    [SerializeField] private Image prImage; //is the image that appears when you pr
    [SerializeField] private GameObject poleVaultPole; //the main pole that is used for the event
    [SerializeField] private GameObject poleGrip; //part of the pole that is being held onto

    [SerializeField] private float runningSpeedRatio; //stores the ratio for running compared to runningSpeed
    [SerializeField] private float animationRunningSpeedRatio; //stores the ratio for animation speed compared to runningSpeed

    Vector3 tempPos = new Vector3(3.6730001f, -1.17499995f, -0.213f);
    Vector3 temprot = new Vector3(57.1805916f, 299.280121f, 330.898041f);

    [SerializeField] private Camera playerCamera; //is the main camera of the character
    [SerializeField] private Camera jumpingCamera; //this is the camera for the jumping part of the vault

    private int rightHandTransformPosition = 55;

    private Vector3 startingPosition = new Vector3(-2255.1f, 226.73f, -73.9f); //starting position of player

    //private Vector3 polePosition = new Vector3(-3.83200002f, -0.86500001f, 0.143999994f); // position relative to hand (local)
    //private Vector3 poleRotaion = new Vector3(288.128937f, 348.744415f, 233.137482f); //local pole rotaion

    private Vector3 playerLaunchPosition = new Vector3(-0.049f, -0.0249f, 0.008f);
    private Vector3 playerLaunchPositionStand = new Vector3(-0.0359001607f, -0.0293995682f, -0.0177000742f);
    private Vector3 playerLaunchRotation = new Vector3(1.43247366f, 179.064468f, 65.4714813f);

    private Vector3 playerFinalPosition = new Vector3(0.00280068698f, 0.00170000177f, 0.0182998832f);
    private Vector3 playerFinalRotation = new Vector3(355.625885f, 178.877747f, 14.6459017f);

    private Vector3 playerPikeRotation = new Vector3(354.592529f, 300.05777f, 15.161294f);


    bool inCinematic = true; //changes to false once cinematic is over
    bool isRunning = true; //shows if the player is in running form
    bool isPlanting = false; //shows if the player is planting the pole

    // Start is called before the first frame update
    void Start()
    {

        itemStorage.initRunner(PublicData.currentRunnerUsing, player.transform); //inits the runner into the current scene
        Debug.Log(player.GetComponentsInChildren<Transform>()[1].GetComponentsInChildren<Transform>()[1].gameObject.name);
        Debug.Log(player.GetComponentsInChildren<Transform>()[rightHandTransformPosition].name);
        poleVaultPole.transform.SetParent(player.GetComponentsInChildren<Transform>()[rightHandTransformPosition]); //sets the parent of the pole to the right hand of the player
        //poleVaultPole.transform.localPosition = polePosition; //sets local position of pole
        //poleVaultPole.transform.localEulerAngles = poleRotaion; //sets local rotation of pole


        jumpMeter.jumpBar.gameObject.transform.parent.gameObject.SetActive(false); //hides the jump meter
        player.GetComponentInChildren<Animator>().Play("PoleRunning");
        leaderboardManager.cinematicCamera.GetComponent<Animator>().SetInteger("event", 2); //sets animator to the current event


        player.transform.position = startingPosition; //puts player in starting position

        foulImage.enabled = false; //hide the foul icon
        prImage.enabled = false; //hides the pr image
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
                runMeter.runningBar.transform.parent.gameObject.SetActive(true);

            }
        }
        if (poleVaultPole.GetComponent<Animator>().GetBool("Launched"))
        {
            if (Input.GetKeyDown(KeyCode.Space) && player.GetComponentInChildren<Animator>().GetBool("Pike"))
            {
                player.GetComponentInChildren<Animator>().Play("Pike");
            } else if (Input.GetKeyDown(KeyCode.Space))
            {
                player.GetComponentInChildren<Animator>().Play("LegBack");
                player.transform.localEulerAngles = playerPikeRotation;
                player.GetComponentInChildren<Animator>().SetBool("Pike", true);
            }
        }


        if (playerCamera.enabled && !leaderboardManager.cinematicCamera.gameObject.activeInHierarchy && isRunning)
        {
            runMeter.updateRunMeter();
            if (Input.GetKeyDown(KeyCode.Space) && runMeter.runningBar.transform.parent.gameObject.activeInHierarchy) //updating speed on click
            {
                runMeter.increaseHeight();
                if (leaderboardManager.leaderBoardVisble()) //hides the leaderboard if the player clicks
                {
                    leaderboardManager.hidePersonalBanner();
                }
            }
            
            if (Input.GetKeyDown(KeyCode.P)) //if the player presses the jump button
            {
                jumpMeter.jumpBar.transform.parent.gameObject.SetActive(true);
                isRunning = false; //shows the player is not longer running
                runMeter.runningBar.transform.parent.gameObject.SetActive(false); //hides running meter
                player.GetComponentInChildren<Animator>().speed = 0; //stops the player animations
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

    private void startPlant() //starts the planting process
    {
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
        poleVaultPole.GetComponent<Animator>().enabled = true;
        poleVaultPole.transform.parent = null; //makes the parent root of scene so it moves independently
        player.transform.parent = poleGrip.transform; //makes the player follow the pole grip
        playerCamera.enabled = false; //makes the running camera not enabled
        jumpingCamera.enabled = true; //makes the vaulting camera enabled
        player.transform.localPosition = playerLaunchPosition; //makes the player in the right position
        player.transform.localEulerAngles = playerLaunchRotation; //makes the player in the right rotation
        poleVaultPole.GetComponent<Animator>().Play("VaultStage1"); //makes the pole in the right animation
        StartCoroutine(stage1Vault(1.5f));
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
        player.GetComponent<Rigidbody>().useGravity = true; //makes it so that player can fall
        player.GetComponent<Rigidbody>().velocity = new Vector3(0, 10, -3); //makes player launch up
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
            runMeter.updateTimeElapsed();
        }
    }
}
