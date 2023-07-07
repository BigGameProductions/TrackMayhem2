using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.Services.Core;
using System;
using Unity.Services.Core.Environments;
using Unity.Services.Authentication;

public class CharacterCreatorController : MonoBehaviour
{
    [SerializeField] private Button selectButton;
    [SerializeField] private TMP_InputField nameInput;
    [SerializeField] private Image selectedCountry;
    [SerializeField] private TextMeshProUGUI errorMessage;

    [SerializeField] private Canvas mainCanvas;
    [SerializeField] private Canvas flagSelectCanvas;

    [SerializeField] private GameObject flagImage;
    [SerializeField] ItemStorage itemStorage;
    [SerializeField] private GameObject flagGrid;


    private int totalFlags = 0;

    // Start is called before the first frame update
    public string environment = "testing";

    async void Start()
    {
        try
        {
            var options = new InitializationOptions()
                .SetEnvironmentName(environment);

            await UnityServices.InitializeAsync(options);
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
        catch (Exception exception)
        {
            // An error occurred during services initialization.
        }
    }

    public void flagSelect()
    {
        flagSelectCanvas.enabled = true;
        mainCanvas.enabled = false;
        /*totalFlags = 0;
        foreach (Sprite sp in itemStorage.flags)
        {
            GameObject newFlag = Instantiate(flagImage, flagGrid.transform);
            newFlag.GetComponent<Image>().sprite = sp;
            totalFlags++;

        }*/
        for (int i=0; i<flagGrid.GetComponentsInChildren<Image>().Length; i++)
        {
            flagGrid.GetComponentsInChildren<Button>()[i].onClick.RemoveAllListeners();
            int j = i;
            flagGrid.GetComponentsInChildren<Button>()[i].onClick.AddListener(()=>selectTheFlag(j));
        }

    }

    public void selectTheFlag(int id)
    {
        PublicData.gameData.countryCode = itemStorage.flags[id].name;
        mainCanvas.enabled = true;
        flagSelectCanvas.enabled = false;
        selectedCountry.sprite = flagGrid.GetComponentsInChildren<Image>()[id].sprite;
    }


// Update is called once per frame
void Update()
    {
        
    }

    private string checkPrefs()
    {
        string name = nameInput.text;
        string countryName = PublicData.gameData.countryCode;
        if (name == "NONAME" || name == "")
        {
            return "Please select name for your character";
        }
        if (countryName == "NOCOUNTRY" || selectedCountry.sprite == null)
        {
            return "Please select a country for your character";
        }
        return "GOOD";
    }
   
    public void makeCharacter() {
        if (checkPrefs() == "GOOD")
        {
            /*GameData gd = new GameData();
            gd.playerName = nameInput.text;
            SaveService.SaveSlotData(gd);*/
            PublicData.gameData.playerName = nameInput.text;
            PublicData.currentBoxOpening = 0;
            SceneManager.LoadScene("ChestOpening");
        } else
        {
            errorMessage.text = checkPrefs();
        }
    }

}
