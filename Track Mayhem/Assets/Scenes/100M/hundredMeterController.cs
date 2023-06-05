using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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

    bool isRunning = false;
    bool runPressed = false;
    bool started = false; //if the player has gone out of the blocks

    // Start is called before the first frame update
    void Start()
    {
        itemStorage.initRunner(PublicData.currentRunnerUsing, player.transform); //inits the runner into the current scene
        setText.enabled = false;
        prImage.enabled = false;
        foulImage.enabled = false;

    }

    // Update is called once per frame
    void Update()
    {
        if (!leaderboardManager.cinematicCamera.gameObject.activeInHierarchy)
        {
            runningMeter.updateRunMeter();
            if ((Input.GetKeyDown(KeyCode.Space) || runPressed) && runningMeter.runningBar.transform.parent.gameObject.activeInHierarchy) //updating speed on click
            {
                if (!isRunning)
                {
                    foulImage.enabled = true;
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
        setText.text = "Set";
        player.GetComponentInChildren<Animator>().Play("BlockStartUp");
        StartCoroutine(showGo(UnityEngine.Random.Range(1.0f, 3.0f)));
    }

    IEnumerator showGo(float delay)
    {
        yield return new WaitForSeconds(delay);
        setText.text = "GO";
        isRunning = true;
    }

    private void FixedUpdate()
    {
        if (playerCamera.enabled && isRunning)
        {
            player.transform.Translate(new Vector3(0, 0, runningMeter.runningSpeed * runningSpeedRatio)); //making character move according to run meter
            player.GetComponentInChildren<Animator>().speed = runningMeter.runningSpeed * animationRunningSpeedRatio; //making the animation match the sunning speed
            runningMeter.updateTimeElapsed();
        }
    }
}
