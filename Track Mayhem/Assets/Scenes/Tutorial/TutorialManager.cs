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

    string dataPath;

    // Start is called before the first frame update
    void Start()
    {
        dataPath = Application.persistentDataPath;
#if UNITY_EDITOR
        dataPath = Application.streamingAssetsPath;
#endif
        if (true)
        {
            tutorialPlayer.url = Path.Combine(dataPath, videoNames[PublicData.currentSelectedEventIndex]);
            tutorialPlayer.source = VideoSource.Url;
            tutorialPlayer.Prepare();
            tutorialPlayer.Play();
            Debug.Log("this is the link:" + tutorialPlayer.url);
            Debug.Log(File.Exists(tutorialPlayer.url));
        }
        else
        {
            var myLoadedAssetBundle
            = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath,"sceneassetbundle"));
            if (myLoadedAssetBundle == null)
            {
                Debug.Log("Failed to load AssetBundle!");
                return;
            }
            Debug.Log("did it");
            string[] prefab = myLoadedAssetBundle.GetAllAssetNames();
            foreach (string st in prefab)
            {
                Debug.Log(st);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
