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

    [SerializeField] private ItemStorage itemStorage;
    [SerializeField] private Material chestColor;

    [SerializeField] private GameObject chest;

    [SerializeField] private Camera mainCamera;

    [SerializeField] private GameObject playerController;

    private int stage = -2;

    // Start is called before the first frame update
    void Start()
    {
        chestColor.color = itemStorage.chestColors[PublicData.currentBoxOpening];
        titleText.enabled = false;
        numText.enabled = false;
        nextStage();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) //changes stage when mouse is pressed
        {
            if (stage == 4)
            {
                if (mainCamera.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("CharacterUnlock3"))
                {
                    nextStage();
                }

            } else
            {
                nextStage();
            }
        }
    }

    IEnumerator chestTransition(float delay) //makes the delay automatic is nothing is pressed
    {
        yield return new WaitForSeconds(delay);
        if (chest.activeInHierarchy)
        {
            nextStage();
        }
    }

    private void nextStage()
    {
        if (stage == -1)
        {
            StartCoroutine(chestTransition(0.8f));
            chest.GetComponent<Animator>().Play("Chest_Open_Close");
        }
        if (stage == 0) //token stage
        {
            titleText.enabled = true;
            numText.enabled = true;
            chest.SetActive(false);
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
            if (chance >= 15)
            {
                SceneManager.LoadScene("MainScreen");
            } else
            {
                titleText.text = "";
                numText.text = "";
                mainCamera.GetComponent<Animator>().Play("CharacterUnlock");
                playerController.SetActive(false);
                List<RunnerInformation> chanceList = new List<RunnerInformation>();
                foreach (RunnerInformation ri in PublicData.gameData.allRunners)
                {
                    if (!ri.unlocked)
                    {
                        chanceList.Add(ri);
                    }

                }
                RunnerInformation unlockedRunner = chanceList.ElementAt(UnityEngine.Random.Range(0, chanceList.Count));//temp adding character
                unlockedRunner.unlocked = true;
                StartCoroutine(showCharacterUnlock(7, chance, unlockedRunner));

            }
        }
        if (stage == 4) //over stage
        {
            SceneManager.LoadScene("MainScreen");
        }
        stage++;
    }

    IEnumerator showCharacterUnlock(float delay, int chance, RunnerInformation ri)
    {
        yield return new WaitForSeconds(delay);
        playerController.SetActive(true);
        if (chance >= 0 && chance < 15)
        {
            titleText.text = "Unlocked";
            numText.text = "Rare";

        }
        if (chance >= 0 && chance < 10)
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
        numText.text = PublicData.charactersInfo.ElementAt(ri.runnerId + 1)[1];
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
