using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HurdleCollisionDetector : MonoBehaviour
{
    private List<string> hurdleList = new List<string>();

    public int hitCount;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name[0] == 'i' && !hurdleList.Contains(collision.gameObject.name))
        {
            hitCount++;
            hurdleList.Add(collision.gameObject.name);
        }
    }
}
