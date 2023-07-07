using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class DownloadManager : MonoBehaviour
{
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

    private int filesLoaded = 0;
    private int filesToLoad = 10;
    [SerializeField] private Slider loadProgress;

    string dataPath;

    // Start is called before the first frame update
    void Start()
    {
        dataPath = Application.persistentDataPath;
#if UNITY_EDITOR
        dataPath = Application.streamingAssetsPath;
        #endif
        loadProgress.maxValue = filesToLoad;
        foreach (string video in videoNames)
        {
            if (File.Exists(Path.Combine(dataPath, video)))
            {
                Debug.Log(video + ": exits");
                filesLoaded++;
            } else
            {
                Debug.Log("Need to load: " + video);
                StartCoroutine(GetText(video));

            }
        }
    }

    IEnumerator GetText(string file_name)
    {
        string url = "https://biggameproductions.000webhostapp.com/Tutorials/" + file_name;
        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            yield return www.Send();
            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                string savePath = string.Format("{0}/{1}", dataPath, file_name);
                System.IO.File.WriteAllBytes(savePath, www.downloadHandler.data);
                filesLoaded++;
            }
        }
    }



    // Update is called once per frame
    void Update()
    {
        if (filesLoaded == filesToLoad)
        {
            SceneManager.LoadScene("MainScreen");
        }
        loadProgress.value = filesLoaded;
    }
}
