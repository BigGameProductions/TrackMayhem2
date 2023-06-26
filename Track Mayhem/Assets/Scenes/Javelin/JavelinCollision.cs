using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JavelinCollision : MonoBehaviour
{
    public bool hitGround = false;

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
        if (collision.gameObject.name == "ID375")
        {
            hitGround = true;
        }
    }
}
