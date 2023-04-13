using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class JumpingMeter : MonoBehaviour
{
    public Image jumpBar; //the bar image
    public float jumpMeterSpeed; //current value of the jump meter
    private bool jumpMeterDirection; //the direction of the meter
    private float movingBarSpeed; //the speed back and forth of the jumpm meter
    [SerializeField] public float jumpBarSpeed; //the set speed of the jump meter. Edited by character traits
    private float startingJumpBar; //holds the starting position of the jump bar
    private float jumpBarIncreasePerInteger; //the increase of the bar per speed unit
    [SerializeField] private float jumpMeterToTop;//stores the distance from the begining to the end of the jump meter

    [SerializeField] private float setScaleForMovement; //has the scale of the parent canvas that future scaling is based on from 16:9
    // Start is called before the first frame update
    void Start()
    {
        //scaling
        float changePercent = jumpBar.transform.parent.parent.GetComponent<RectTransform>().localScale.x / setScaleForMovement;
        jumpMeterToTop *= changePercent;
        //scaling

        startingJumpBar = jumpBar.transform.position.x;
        jumpBarIncreasePerInteger = jumpMeterToTop / 200f;
    }

    public void setToRegularSpeed()
    {
        movingBarSpeed = jumpBarSpeed;
    }

    public void updateJumpMeter()
    {
        jumpMeterSpeed += Time.deltaTime * movingBarSpeed * (jumpMeterDirection ? -1 : 1);
        if (jumpMeterSpeed >= 200 || jumpMeterSpeed <= 0)
        {
            if (jumpMeterSpeed >= 200) //prevents bar from going out of bounds
            {
                jumpMeterSpeed = 200;
            }
            else
            {
                jumpMeterSpeed = 0;
            }
            jumpMeterDirection = !jumpMeterDirection;
        }
        jumpBar.transform.position = new Vector3(startingJumpBar + (jumpMeterSpeed * jumpBarIncreasePerInteger), jumpBar.transform.position.y, jumpBar.transform.position.z);
    }

    public void MakeJump()
    {
        movingBarSpeed = 0;
    }
}