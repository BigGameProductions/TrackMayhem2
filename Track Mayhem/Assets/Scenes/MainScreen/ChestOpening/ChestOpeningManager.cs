using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
using UnityEngine.SceneManagement;

public class ChestOpeningManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI numText;

    private int stage = 0;

    // Start is called before the first frame update
    void Start()
    {
        nextStage();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) //changes stage when mouse is pressed
        {
            nextStage();
        }
    }

    private void nextStage()
    {
        if (stage == 0) //token stage
        {
            titleText.text = "Tokens";
            int tokenAmount = UnityEngine.Random.Range(100, 4000);
            PublicData.gameData.tokens += tokenAmount;
            numText.text = tokenAmount.ToString();
        }
        if (stage == 1 || stage == 2) //character points stage
        {
            RunnerInformation character = getRandomCharacter();
            string[] att = PublicData.charactersInfo[character.runnerId + 1];
            int upgradePoints = UnityEngine.Random.Range(50, 200);
            character.upgradePoints += upgradePoints; //adds points to character
            titleText.text = att[1] + " Points";
            numText.text = upgradePoints.ToString();
        }
        if (stage == 3) //charatcer or over stage
        {
            int chance = UnityEngine.Random.Range(0, 100);
            if (chance >=0 && chance < 15)
            {
               titleText.text = "Unlocked";
                numText.text = "Rare";

            }
            if (chance >=0 && chance < 10)
            {
                titleText.text = "Unlocked";
                numText.text = "Epic";
            }
            if (chance >= 0 && chance < 5)
            {
                titleText.text = "Unlocked";
                numText.text = "Mythic";
            }
            if (chance >= 0 && chance < 3)
            {
                titleText.text = "Unlocked";
                numText.text = "Legendary";
            }
            if (chance >= 15)
            {
                SceneManager.LoadScene("MainScreen");
            }
        }
        if (stage == 4) //over stage
        {
            SceneManager.LoadScene("MainScreen");
        }
        stage++;
    }

    private RunnerInformation getRandomCharacter() //gets a random character from the game data that the player has unlocked
    {
        RunnerInformation character = PublicData.gameData.allRunners.ElementAt(0);
        do
        {
            character = PublicData.gameData.allRunners.ElementAt(UnityEngine.Random.Range(0, PublicData.gameData.allRunners.Count));
        } while (!character.unlocked); //makes sure the player has the runner that is randomly being selected
        return character;
    }
}
