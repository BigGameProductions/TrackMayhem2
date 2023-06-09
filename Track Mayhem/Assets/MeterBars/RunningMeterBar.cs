using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RunningMeterBar : MonoBehaviour
{
    public Image runningBar;

    public float runningSpeed; //public variable to get the running speed
    [SerializeField] public float speedPerClick; //speed added per click of the meter. Changed by character traits
    public float startingBarHeight; //hold the starting y value of the bar
    [SerializeField] private float maxSpeed;//max speed the player can go
    public float barIncreasePerSpeed; // increase of bar per speed number
    [SerializeField] public float barDecreaseSpeed; //the speed in which the bar decreases. Changed by character traits
    private float totalRunningSpeed; //total distance travelled for running
    private float timeElapsedRunning; //time that has elepsed for running
    [SerializeField] private float increaseForBarToTheTop;//stores distance from the bottom of the bar to the top

    [SerializeField] private float setScaleForMovement; //has the scale of the parent canvas that future scaling is based on from 16:9

    [SerializeField] public Slider runMeterSlider; //the slider for the run meter
    [SerializeField] private Image fillImage; //image of the slider


    // Start is called before the first frame update
    void Start()
    {
        //scaling
        float changePercent = runningBar.transform.parent.parent.GetComponent<RectTransform>().localScale.x / setScaleForMovement;
        increaseForBarToTheTop *= changePercent;
        //scaling

        runMeterSlider.maxValue = maxSpeed;

        startingBarHeight = runningBar.transform.position.y;
        barIncreasePerSpeed = increaseForBarToTheTop / maxSpeed;
        Debug.Log(runningBar.transform.parent.GetComponent<RectTransform>().localScale);
    }

    public float getAverageSpeed()
    {

        if (timeElapsedRunning == 0) return 0; //prevents NaN error
        return totalRunningSpeed / timeElapsedRunning;
    }

    public void increaseHeight() //increases the height of the bar on the run meter
    {
        runningSpeed += speedPerClick;
    }

    public void stopRunMeter() //stops the meter from being visible
    {
        //nothing
    }

    public void updateRunMeter()
    {
        if (runningSpeed <= 0) //makes sure running speed does not go below 0
        {
            runningSpeed = 0;
        }
        else
        {
            runningSpeed -= Time.deltaTime * barDecreaseSpeed; //decreses running speed
        }
        if (runningSpeed > maxSpeed) //making a max speed
        {
            runningSpeed = maxSpeed;
        }
        runningBar.transform.position = new Vector3(runningBar.transform.position.x, startingBarHeight + (runningSpeed * barIncreasePerSpeed), runningBar.transform.position.z);
        runMeterSlider.value = runningSpeed;
        if (SceneManager.GetActiveScene().name != "FifteenHundred")
        {
            if (runningSpeed < 220 || SceneManager.GetActiveScene().name != "FourHundred")
            {
                if (runningSpeed > 220)
                {
                    fillImage.color = Color.red;
                }
                else if (runningSpeed > 150)
                {
                    fillImage.color = Color.green;
                }
                else
                {
                    fillImage.color = Color.yellow;
                }
            }
        }
        
        
    }

    public void updateTimeElapsed()
    {
        if (runningSpeed == 0)
        {
            totalRunningSpeed = 0;
            timeElapsedRunning = 0;
        }
        else
        {
            totalRunningSpeed += runningSpeed;
            timeElapsedRunning += Time.deltaTime;

        }
    }

}
