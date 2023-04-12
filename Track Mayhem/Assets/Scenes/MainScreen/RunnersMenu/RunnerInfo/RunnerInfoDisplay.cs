using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class RunnerInfoDisplay : MonoBehaviour
{
    [SerializeField] private Canvas mainCanvas;

    private void Start()
    {
        RunnerInformation info = getCharactersInfo(PublicData.currentRunnerOn); //gets the gamedata traits
        string[] att = PublicData.charactersInfo.ElementAt(info.runnerId + 1); //gets the characters.csv traits
        foreach (Transform tf in mainCanvas.transform) //sets all the text boxes to appropriate values
        {
            if (tf.name == "RunnerName") //sets runners names 
            {
                tf.GetComponent<TextMeshProUGUI>().text = att[1];
            }
            if (tf.name == "Description") //sets description for the runner
            {
                tf.GetComponent<TextMeshProUGUI>().text = att[2];
            }
            if (tf.name == "SelectButton" && !info.unlocked) //hides the select button for locked runners
            {
                tf.gameObject.SetActive(false);
            }
            if (tf.name == "Upgrades" && !info.unlocked) //hides the upgrade buttons for locked runners
            {
                foreach (Transform tff in tf.transform)
                {
                    foreach (Transform tfff in tff.transform)
                    {
                        if (tfff.name == "UpgradeButton")
                        {
                            tfff.gameObject.SetActive(false);
                        }
                    }
                }
            }
        }
    }

    private RunnerInformation getCharactersInfo(int id) //gets the current gamedata traits
    {
        List<RunnerInformation> runnerData = PublicData.gameData.allRunners;
        for (int i = 0; i < runnerData.Count; i++)
        {
            if (runnerData.ElementAt(i).runnerId == id)
            {
                return runnerData.ElementAt(i);
            }
        }
        return null;
    }
}
