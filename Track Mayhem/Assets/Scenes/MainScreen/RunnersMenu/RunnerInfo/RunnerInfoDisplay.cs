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
                        if (tfff.name == "UpgradeButton" && !info.unlocked)
                        {
                            tfff.gameObject.SetActive(false);
                        }
                    }
                    count++;
                }
            }
        }
    }

    public void upgradeCharacter(int num) //num is the index of the trait that is being upgraded
    {
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
