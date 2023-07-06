using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class RunnersDisplay : MonoBehaviour
{
    [SerializeField] GridLayoutGroup unlockedGrid; //the grid showing all unlocked runners
    [SerializeField] GridLayoutGroup lockedGrid; //this grid showing all the locked runners
    [SerializeField] GameObject runnerObject; //the object to be instaintiated from
    [SerializeField] GameObject lockedItems; //holds the locked items title and grid 

    [SerializeField] float normalCanvasSize;

    [SerializeField] GameObject contentView;

    [SerializeField] private ItemStorage itemStorage;

    [SerializeField] GameObject backButton;

    private float lockedItemsMovementPerRow; //stores the amount of pixels locked items must move per each new row 


    // Start is called before the first frame update
    void Start()
    {
        contentView.transform.localPosition = new Vector3(0, PublicData.runnerMenuPosition, 0);
        lockedItemsMovementPerRow = (unlockedGrid.cellSize.y + unlockedGrid.spacing.y) * (unlockedGrid.transform.parent.parent.parent.parent.GetComponent<RectTransform>().localScale.y); //scales the distance to runners and the locked title

        for (int i = 0; i < PublicData.charactersInfo.Count; i++) //finds all characters in characters.csv
        {
            if (i != 0)
            {
                makeCharacterDisplay(PublicData.charactersInfo.ElementAt(i)); //sets the character detials
            }
        }
        float movementAmount = lockedItemsMovementPerRow * (((unlockedGrid.transform.childCount % 3) == 0 ? 0 : 1) + (int)(unlockedGrid.transform.childCount/3));
        lockedItems.transform.position -= new Vector3(0, movementAmount, 0);
        if (PublicData.selectingCharacter)
        {
            backButton.SetActive(false);
        }


    }

    private void makeCharacterDisplay(string[] att)
    {
        RunnerInformation info = PublicData.getCharactersInfo(Int32.Parse(att[0])); //gets the current information about the runner
        GameObject runnerIcon; //makes a runner object to be added to the grid
        if (info.unlocked)
        {
            runnerIcon = Instantiate(runnerObject, unlockedGrid.transform); //copies the runner object for the unlocked grid
        } else 
        {
            runnerIcon = Instantiate(runnerObject, lockedGrid.transform); //copies the runner object for the locked grid
        }
        foreach (Transform tf in runnerIcon.transform)
        {
            if (tf.name == "Name")
            {
                tf.gameObject.GetComponent<TextMeshProUGUI>().text = att[1]; //setting name according to csv
            }
            if (tf.name == "PointsTotal")
            {
                tf.gameObject.GetComponent<TextMeshProUGUI>().text = info.points.ToString();
                if(!info.unlocked)
                {
                    tf.gameObject.SetActive(false);
                }
            }
            if (tf.name == "Upgrades")
            {
                foreach (Transform tff in tf) //setting all the traits to current levels
                {
                    if (tff.name == "Speed")
                    {
                        setUpdateIcon(tff, info.speedLevel, info);
                    }
                    if (tff.name == "Strength")
                    {
                        setUpdateIcon(tff, info.strengthLevel, info);
                    }
                    if (tff.name == "Agility")
                    {
                        setUpdateIcon(tff, info.agilityLevel, info);
                    }
                    if (tff.name == "Flexability")
                    {
                        setUpdateIcon(tff, info.flexabilityLevel, info);
                    }
                    if (!info.unlocked)
                    {
                        tff.gameObject.SetActive(false);
                    }
                }
            }
            if (tf.name == "RunnerImage")
            {
                tf.GetComponent<Image>().sprite = itemStorage.runnerImages[info.runnerId];
            }
        }


    }

   
    

    private void setUpdateIcon(Transform tf, int num, RunnerInformation info) //sets the upgrade level of a trait to a specific level
    {
        string[] att = PublicData.charactersInfo.ElementAt(info.runnerId + 1); //gets the characters.csv traits
        foreach (Transform tff in tf)
        {
            if (tff.name == "UpgradeLevel")
            {
                tff.GetComponent<TextMeshProUGUI>().text = num.ToString();
            }
            if (tff.name == "UpgradeImage")
            {
                int count = 0;
                switch (tf.name)
                {
                    case "Strength":
                        count = 1;
                        break;
                    case "Agility":
                        count = 2;
                        break;
                    case "Flexability":
                        count = 3;
                        break;
                }
                Color setColor = Color.white;
                if (info.upgradePoints >= PublicData.upgradeLevelForTrait(count, info, att))
                {
                    setColor = Color.cyan;
                    if (PublicData.gameData.tokens >= PublicData.getGoldAmountForLevel(num))
                    {
                        setColor = Color.green;
                    }
                }
                if (num == 10)
                {
                    setColor = Color.yellow;
                }
                tff.GetComponent<Image>().color = setColor;
            }
        }
    }

   
}
