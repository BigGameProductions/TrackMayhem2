using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShotputManager : MonoBehaviour
{
    [SerializeField] GameObject shotput;
    [SerializeField] GameObject player;
    [SerializeField] private GameObject basePlayer;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private Button infoButton;

    [SerializeField] private LeaderboardManager leaderboardManager;
    [SerializeField] private RunningMeterBar runMeter;

    [SerializeField] ItemStorage itemStorage;

    private int rightHandTransformPosition = 55;
    private bool isRunning = true;

    // Start is called before the first frame update
    void Start()
    {
        itemStorage.initRunner(PublicData.currentRunnerUsing, player.transform, basePlayer); //inits the runner into the current scene
        shotput.transform.SetParent(player.GetComponentsInChildren<Transform>()[rightHandTransformPosition]); //sets the parent of the pole to 
        shotput.transform.localPosition = new Vector3(-0.00200000009f, 0.140000001f, 0.0329999998f); //alligns shot to player hand
        player.GetComponentInChildren<Animator>().Play("ShotputThrow");
        player.GetComponentInChildren<Animator>().speed = 0;
        player.GetComponentsInChildren<Transform>()[1].eulerAngles = new Vector3(0, 90, 0);
    }



    // Update is called once per frame
    void Update()
    {
        if (playerCamera.enabled && !leaderboardManager.cinematicCamera.gameObject.activeInHierarchy && isRunning) //runs when the player is in the running stage
        {
            runMeter.updateRunMeter();
            if ((Input.GetKeyDown(KeyCode.Space)) && runMeter.runMeterSlider.gameObject.activeInHierarchy) //updating speed on click
            {
                infoButton.gameObject.SetActive(false);
                runMeter.increaseHeight();
                if (leaderboardManager.leaderBoardVisble()) //hides the leaderboard if the player clicks
                {
                    leaderboardManager.hidePersonalBanner();
                }
            }
            else
            {
              //nothing yet
            }



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
            player.GetComponentInChildren<Animator>().speed = (float)(0.5 * (speed/PublicData.averageSpeedDuringRun));
            runMeter.updateTimeElapsed();

        }
    }
}

