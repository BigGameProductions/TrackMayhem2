using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugMenu : MonoBehaviour
{ 

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A)) //resets all runners info
        {
            foreach (RunnerInformation ri in PublicData.gameData.allRunners)
            {
                ri.speedLevel = 0;
                ri.strengthLevel = 0;
                ri.agilityLevel = 0;
                ri.flexabilityLevel = 0;

            }
        }

        if (Input.GetKeyDown(KeyCode.Z)) //maxes out best person
        {
            foreach (RunnerInformation ri in PublicData.gameData.allRunners)
            {
                if (ri.runnerId==1)
                {
                    ri.speedLevel = 10;
                    ri.strengthLevel = 10;
                    ri.agilityLevel = 10;
                    ri.flexabilityLevel = 10;
                }


                

            }
        }
    }
}
