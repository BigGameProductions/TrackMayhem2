using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeanDetector : MonoBehaviour
{
    public bool endRace = false;
    public int tfCount;
    GameObject head;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(waitForLoad());
    }

    IEnumerator waitForLoad()
    {
        yield return new WaitForSeconds(0.5f);
        head = gameObject.GetComponentsInChildren<Transform>()[tfCount].gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (head != null)
        {
            if (head.transform.position.x <= -2161.52f)
            {
                endRace = true;
            }
        }
        
    }


    
}
