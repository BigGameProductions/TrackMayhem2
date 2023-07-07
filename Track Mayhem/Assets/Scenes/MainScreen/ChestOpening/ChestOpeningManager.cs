using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
using UnityEngine.SceneManagement;
using System;

public class ChestOpeningManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI numText;

    [SerializeField] private ItemStorage itemStorage;
    [SerializeField] private Material chestColor;

    [SerializeField] private GameObject chest;

    [SerializeField] private Camera mainCamera;

    [SerializeField] private GameObject playerController;

    [SerializeField] private GameObject counterImage;

    [SerializeField] private TextMeshProUGUI itemCount;

    private int maxStage;

    private int stage = -2;

    private List<int> usedIDList = new List<int>();

    private bool hasChar = false; //tells if the box will unlock a new character
    private bool hasGearShard = false; // tells if box will have gear shards

    // Start is called before the first frame update
    void Start()
    {
        chestColor.color = itemStorage.chestColors[PublicData.currentBoxOpening];
        titleText.enabled = false;
        numText.enabled = false;
        maxStage = Int32.Parse(PublicData.chestInfo.ElementAt(PublicData.currentBoxOpening+1)[3]) + 1;
        counterImage.SetActive(false);
        int chance = UnityEngine.Random.Range(0, 100);
        if (getUnlockedCharacterCount() != PublicData.gameData.allRunners.Count && chance < Int32.Parse(PublicData.chestInfo.ElementAt(PublicData.currentBoxOpening + 1)[6]))
        {
            hasChar = true;
        }
        if (getUnlockedCharacterCount() == 0)
        {
            hasChar = true;
        }
        if (UnityEngine.Random.Range(0,100) < 90)
        {
            foreach (RunnerInformation ri in PublicData.gameData.allRunners)
            {
                if (ri.unlocked)
                {
                    if (checkForGearElgible(ri.runnerId))
                    {
                        hasGearShard = true;
                        break;
                    }
                }
            }
        }
        nextStage();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) //changes stage when mouse is pressed
        {
            if (stage == maxStage+1)
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

    private bool checkForGearElgible(int id)
    {
        RunnerInformation ri = PublicData.getCharactersInfo(id);
        if (Int32.Parse(PublicData.charactersInfo.ElementAt(id + 1)[3]) > ri.speedLevel)
        {
            return false;
        }
        if (Int32.Parse(PublicData.charactersInfo.ElementAt(id+ 1)[4]) > ri.strengthLevel)
        {
            return false;
        }
        if (Int32.Parse(PublicData.charactersInfo.ElementAt(id + 1)[5]) > ri.agilityLevel)
        {
            return false;
        }
        if (Int32.Parse(PublicData.charactersInfo.ElementAt(id + 1)[6]) > ri.flexabilityLevel)
        {
            return false;
        }
        return true;
    }

    private void nextStage()
    {
        itemCount.text = (maxStage - stage - 1 + (hasChar?1:0)).ToString();
        string[] currentChestData = PublicData.chestInfo.ElementAt(PublicData.currentBoxOpening+1);
        if (stage == -1)
        {
            StartCoroutine(chestTransition(0.8f));
            chest.GetComponent<Animator>().Play("Chest_Open_Close");
        }
        if (stage == 0) //token stage
        {
            counterImage.SetActive(true);
            titleText.enabled = true;
            numText.enabled = true;
            chest.SetActive(false);
            titleText.text = "Tokens";
            int tokenAmount = UnityEngine.Random.Range(Int32.Parse(currentChestData[1]), Int32.Parse(currentChestData[2]) + 1);
            PublicData.gameData.tokens += tokenAmount;
            numText.text = tokenAmount.ToString();
        }
        if (stage > 0 && stage < maxStage) //character points stage
        {
            RunnerInformation character;
            do //stops duplicates characters unless needed
            {
                character = getRandomCharacter();
            } while (usedIDList.Contains(character.runnerId) && usedIDList.Count != getUnlockedCharacterCount());
            usedIDList.Add(character.runnerId);
            if (usedIDList.Count == getUnlockedCharacterCount())
            {
                usedIDList = new List<int>();
            }
            string[] att = PublicData.charactersInfo[character.runnerId + 1];
            int upgradePoints = UnityEngine.Random.Range(Int32.Parse(currentChestData[4]), Int32.Parse(currentChestData[5]) + 1);
            character.upgradePoints += upgradePoints; //adds points to character
            titleText.text = att[1] + " Points";
            numText.text = upgradePoints.ToString();
        }
        if (stage == maxStage) //over stage
        {
            if (hasGearShard)
            {
                int shardAmount = UnityEngine.Random.Range(1, 4);
                titleText.text = " Gear Shards";
                numText.text = shardAmount.ToString();
                PublicData.gameData.gearShards += shardAmount;
                hasGearShard = false;
                stage--;
            } else
            {
                if (!hasChar) //if a runner will be unlocked
                {
                    returnBack();
                }
                else
                {
                    counterImage.SetActive(false);
                    titleText.text = "";
                    numText.text = "";
                    mainCamera.GetComponent<Animator>().Play("CharacterUnlock");
                    playerController.SetActive(false);
                    StartCoroutine(showCharacterUnlock(7));

                }
            }
        }
        if (stage == maxStage + 1) //over stage
        {
            returnBack();
        }
        stage++;
    }

    private void returnBack()
    {
        if (PublicData.fromShop)
        {
            SceneManager.LoadScene("Shop");
        } else
        {
            SceneManager.LoadScene("MainScreen");
        }
    }

    IEnumerator showCharacterUnlock(float delay)
    {
        yield return new WaitForSeconds(delay);
        playerController.SetActive(true);
        //all the chances for characters
        int commonChance = Int32.Parse(PublicData.chestInfo.ElementAt(PublicData.currentBoxOpening+1)[7]);
        int rareChance = Int32.Parse(PublicData.chestInfo.ElementAt(PublicData.currentBoxOpening + 1)[8]);
        int epicChance = Int32.Parse(PublicData.chestInfo.ElementAt(PublicData.currentBoxOpening + 1)[9]);
        int legendaryChance = Int32.Parse(PublicData.chestInfo.ElementAt(PublicData.currentBoxOpening + 1)[10]);
        int chance = UnityEngine.Random.Range(1, 101);
        int rarity = 0;
        titleText.text = "Unlocked";
        if (chance < commonChance + 1)
        {
            rarity = 0;

        }
        else if (chance > commonChance && chance < commonChance+rareChance+1)
        {
            rarity = 1;

        }
        else if (chance > commonChance+rareChance && chance < commonChance+rareChance+epicChance+1)
        {
            rarity = 2;

        }
        else if (chance > commonChance+rareChance+epicChance)
        {
            rarity = 3;

        }
        RunnerInformation ri = unlockCharacterOfRarity(rarity);
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

    private int getUnlockedCharacterCount()
    {
        int i = 0;
        foreach(RunnerInformation ri in PublicData.gameData.allRunners)
        {
            if (ri.unlocked)
            {
                i++;
            }
        }
        return i;
    }

    //1 is common
    //2 is rare
    //3 is epic
    //4 is legendary
    private int getTotalLeftOfRarity(int rarity)
    {
        List<string> rarityList = new List<string>();
        rarityList.Add("Common");
        rarityList.Add("Rare");
        rarityList.Add("Epic");
        rarityList.Add("Legendary");
        int i = 0;
        foreach (RunnerInformation ri in PublicData.gameData.allRunners)
        {
            if (!ri.unlocked)
            {
                if (rarityList.IndexOf(PublicData.charactersInfo.ElementAt(ri.runnerId+1)[7]) == rarity)
                {
                    i++;
                }
            }
        }
        return i;

    }

    private RunnerInformation unlockCharacterOfRarity(int rarity) //unlocks the character of a specific rarity
    {
        if (getTotalLeftOfRarity(rarity) == 0)
        {
            if (rarity == 0 || ((rarity == 1 || rarity == 2) && getTotalLeftOfRarity(rarity-1)==0)) //makes the flow of unlocking work if all of one rarity is unlocked
            {
                return unlockCharacterOfRarity(rarity + 1);
            } else
            {
                return unlockCharacterOfRarity(rarity - 1);
            }
        } else
        {
            List<string> rarityList = new List<string>();
            rarityList.Add("Common");
            rarityList.Add("Rare");
            rarityList.Add("Epic");
            rarityList.Add("Legendary");
            List<RunnerInformation> chanceList = new List<RunnerInformation>();
            foreach (RunnerInformation ri in PublicData.gameData.allRunners)
            {
                if (!ri.unlocked && rarityList.IndexOf(PublicData.charactersInfo.ElementAt(ri.runnerId+1)[7]) == rarity)
                {
                    chanceList.Add(ri);
                }

            }
            RunnerInformation unlockedRunner = chanceList.ElementAt(UnityEngine.Random.Range(0, chanceList.Count)); //get random character in the rarity
            unlockedRunner.unlocked = true;
            return unlockedRunner;
        }
    }
}
