using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Video;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] private VideoPlayer tutorialPlayer;

    private string[] videoNames = new string[]
    {
        "HundredMeterTut.mp4",
        "LongJumpTut.mp4",
        "PoleVaultTut.mp4",
        "ShotputTut.mp4",
        "JavelinTut.mp4",
        "FourHundredTut.mp4",
        "DiscusTut.mp4",
        "HighJumpTut.mp4",
        "HurdlesTut.mp4",
        "FifteenHundredTut.mp4"
    };

    // Start is called before the first frame update
    void Start()
    {
        tutorialPlayer.url = Path.Combine(Application.streamingAssetsPath, videoNames[PublicData.currentSelectedEventIndex]);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
