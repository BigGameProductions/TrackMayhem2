using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Video;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] private VideoPlayer tutorialPlayer;

    // Start is called before the first frame update
    void Start()
    {
        tutorialPlayer.url = Path.Combine(Application.streamingAssetsPath, "HundredMeterTut.mp4");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
