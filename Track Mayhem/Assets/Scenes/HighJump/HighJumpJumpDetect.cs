using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighJumpJumpDetect : MonoBehaviour
{
    public bool metHeight = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void resetMakeDetector()
    {
        metHeight = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Collider")
        {
            metHeight = true;
        }
    }
}
