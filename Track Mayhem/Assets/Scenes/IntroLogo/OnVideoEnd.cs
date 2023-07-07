using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using System.IO;
using System.Net;

public class OnVideoEnd : MonoBehaviour
{
    public float timer;
    public VideoPlayer videoPlayer;
    public string nextScene;

    private void Start()
    {

        /*SaveSystem.LoadPlayer();
        videoPlayer.source = VideoSource.Url;
        /*if (!RemoteURLExists(videoPlayer.url))
        {
            videoPlayer.source = VideoSource.VideoClip;
        }*/
        //videoPlayer.url = Path.Combine(new string[] {Application.streamingAssetsPath, "Big Game Productions Logo Animation.mp4" });
        //videoPlayer.Play();
        StartCoroutine(waitForIntro());

    }

    IEnumerator waitForIntro()
    {
        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene("DownloadAssets");
    }

    // Update is called once per frame
   /* void Update()
    {
        timer -= Time.deltaTime;
        if (timer < 0)
        {
            SceneManager.LoadScene(nextScene);   
        }
    }

    bool RemoteURLExists(string url)
    {
         try
        {
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            request.Method = "HEAD";
            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
            response.Close();
            return (response.StatusCode == HttpStatusCode.OK);
        } catch
        {
            return false;
        }
        
    }*/

   
  

}
