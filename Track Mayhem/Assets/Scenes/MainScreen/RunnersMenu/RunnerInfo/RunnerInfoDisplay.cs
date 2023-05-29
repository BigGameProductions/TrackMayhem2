using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using System;

public class RunnerInfoDisplay : MonoBehaviour
{
    [SerializeField] private Canvas mainCanvas;
    [SerializeField] private GameObject player;
    [SerializeField] private ItemStorage itemStorage;

    [SerializeField] private GameObject popup;
    [SerializeField] private TextMeshProUGUI resultText;

    [SerializeField] private TextMeshProUGUI coinsText;
    [SerializeField] private TextMeshProUGUI upgradePointsText;
    [SerializeField] private TextMeshProUGUI TCText;

    [SerializeField] private Image TCImage;

    [SerializeField] private Button buyButton;



    private void Start()
    {
        itemStorage.initRunner(PublicData.currentRunnerOn, player.transform);
        updateBoxes();
    }

    private void updateBoxes()
    {
        RunnerInformation info = PublicData.getCharactersInfo(PublicData.currentRunnerOn); //gets the gamedata traits
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
            if (tf.name == "Upgrades") //hides the upgrade buttons for locked runners
            {
                int count = 0; //keeps track of the trait index that is being edited
                foreach (Transform tff in tf.transform)
                {
                    foreach (Transform tfff in tff.transform)
                    {
                        if (tfff.name == "Upgrades")
                        {
                            if (count == 0) //counts is the trait that is being edited
                            {
                                setUpgradeButons(tfff, Int32.Parse(att[3]), false); //sets the upgrade buttons for the current trait
                                setUpgradeButons(tfff, info.speedLevel, true); //sets the upgrade buttons for the current trait by real level
                            }
                            if (count == 1)
                            {
                                setUpgradeButons(tfff, Int32.Parse(att[4]), false);
                                setUpgradeButons(tfff, info.strengthLevel, true); //sets the upgrade buttons for the current trait by real level
                            }
                            if (count == 2)
                            {
                                setUpgradeButons(tfff, Int32.Parse(att[5]), false);
                                setUpgradeButons(tfff, info.agilityLevel, true); //sets the upgrade buttons for the current trait by real level
                            }
                            if (count == 3)
                            {
                                setUpgradeButons(tfff, Int32.Parse(att[6]), false);
                                setUpgradeButons(tfff, info.flexabilityLevel, true); //sets the upgrade buttons for the current trait by real level
                            }
                        }
                        if (tfff.name == "UpgradeButton")
                        {
                            if (info.unlocked)
                            {
                                int level = 0; //sets the gold amount
                                switch (count)
                                {
                                    case 0:
                                        level = info.speedLevel;
                                        break;
                                    case 1:
                                        level = info.strengthLevel;
                                        break;
                                    case 2:
                                        level = info.agilityLevel;
                                        break;
                                    case 3:
                                        level = info.flexabilityLevel;
                                        break;

                                }
                                int upgradeAmount = PublicData.upgradeLevelForTrait(count, info, att);
                                tfff.GetComponent<Button>().enabled = true; //disables the user from clicking max level
                                if (level == 10)
                                {
                                    tfff.GetComponent<Button>().enabled = false; //disables the user from clicking max level
                                    setButtonIcon(2, tfff);
                                }
                                else if (info.upgradePoints < upgradeAmount || (PublicData.usesTrainingCard(count, info, att) && PublicData.gameData.trainingCards < 1) || PublicData.gameData.tokens < PublicData.getGoldAmountForLevel(level))
                                {
                                    setButtonIcon(0, tfff);
                                } else
                                {
                                    setButtonIcon(1, tfff);
                                }
                                //tfff.GetComponentInChildren<TextMeshProUGUI>().text = PublicData.getGoldAmountForLevel(level).ToString();


                                Slider sl = tfff.GetComponentInChildren<Slider>();
                                sl.maxValue = upgradeAmount; //temp as "max" will throw an error //TODO
                                sl.value = info.upgradePoints;
                                if (upgradeAmount == -1) { // making sure the text shows that it is max level
                                    sl.GetComponentInChildren<TextMeshProUGUI>().text = "Max"; //setting the max value for slider
                                } else
                                {
                                    sl.GetComponentInChildren<TextMeshProUGUI>().text = info.upgradePoints + "/" + upgradeAmount; //second value is parsed from reuturn string

                                }
                            } else
                            {
                                tfff.gameObject.SetActive(false); //hides the button for locked characters
                            }
                        }
                    }
                    count++;
                }
            }
        }
    }

    public void hidePopup()
    {
        popup.gameObject.SetActive(false);
    }
    
    //0 is plus
    //1 is arrow
    //2 is max
    private void setButtonIcon(int status, Transform tf) //makes the icon for the buy button show
    {
        foreach (Transform tff in tf)
        {
            if (tff.name == "UpgradePrice")
            {
                tff.gameObject.SetActive(status == 2);
            }
            if (tff.name == "Arrow")
            {
                tff.gameObject.SetActive(status == 1);
            }
            if (tff.name == "Plus")
            {
                tff.gameObject.SetActive(status == 0);
            }
        }
    }


    public void upgradeCharacter(int num) //num is the index of the trait that is being upgraded
    {
        RunnerInformation info = PublicData.getCharactersInfo(PublicData.currentRunnerOn); //gets the gamedata traits
        string[] att = PublicData.charactersInfo.ElementAt(info.runnerId + 1); //gets the characters.csv traits
        int upgradeNumber = PublicData.upgradeLevelForTrait(num, info, att); //gets price of the current trait in points
        popup.gameObject.SetActive(true); //shows popup
        int level = 0; //level of upgrade
        switch (num)
        {
            case 0:
                level = info.speedLevel;
                break;
            case 1:
                level = info.strengthLevel;
                break;
            case 2:
                level = info.agilityLevel;
                break;
            case 3:
                level = info.flexabilityLevel;
                break;

        }
        string name = "Speed"; //sets the gold amount
        switch (num)
        {
            case 1:
                name = "Strength";
                break;
            case 2:
                name = "Agility";
                break;
            case 3:
                name = "Flexability";
                break;

        }
        resultText.text = name + " Level " + (level + 1);
        coinsText.text = PublicData.getGoldAmountForLevel(num).ToString(); //shows coins needed
        upgradePointsText.text = PublicData.upgradeLevelForTrait(num, info, att).ToString(); //shows upgrade point amount
        if (PublicData.usesTrainingCard(num, info, att)) //shows training cards if needed
        {
            TCImage.enabled = true;
            TCText.enabled = true;
        } else
        {
            TCImage.enabled = false;
            TCText.enabled = false;
        }
        buyButton.interactable = true;
        //check if it can be used
        if (info.upgradePoints < upgradeNumber || (PublicData.usesTrainingCard(num, info, att) && PublicData.gameData.trainingCards<1) || PublicData.gameData.tokens < PublicData.getGoldAmountForLevel(level))
        {
            buyButton.interactable = false;
        }
        buyButton.onClick.AddListener(() =>  //sets up the buying button to buy things
        {
            hidePopup();
            if (info.upgradePoints >= upgradeNumber)
            {
                if (PublicData.usesTrainingCard(num, info, att)) //subtracts trining cards if used
                {
                    PublicData.gameData.trainingCards--;
                }
                info.upgradePoints -= upgradeNumber;
                if (num == 0)
                {
                    PublicData.getCharactersInfo(PublicData.currentRunnerOn).speedLevel++;
                }
                if (num == 1)
                {
                    PublicData.getCharactersInfo(PublicData.currentRunnerOn).strengthLevel++;
                }
                if (num == 2)
                {
                    PublicData.getCharactersInfo(PublicData.currentRunnerOn).agilityLevel++;
                }
                if (num == 3)
                {
                    PublicData.getCharactersInfo(PublicData.currentRunnerOn).flexabilityLevel++;
                }
                updateBoxes();
            }
        });
        
        
    }

    

    private void setUpgradeButons(Transform tf, int amount, bool level) //takes a transform and loops through all the upgrade boxes to decide what is highlighted. Level decides if the highlight is for the current level
    {
        for (int i=0; i<tf.childCount; i++) //loops through all boxes
        {
            if (i<amount)
            {
                Image item = tf.GetComponentsInChildren<RectTransform>()[i+1].GetComponent<Image>(); //gets the image in the loop
                item.color = level ? Color.blue:Color.red; //makes color red for temp and blue for the other depending on the level
            }
        }
    }
}
