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



// Update is called once per frame
void Update()
    {
        
    }

    private bool checkPrefs()
    {
        return true;
    }
   
    public void makeCharacter() {
        if (checkPrefs())
        {
            /*GameData gd = new GameData();
            gd.playerName = nameInput.text;
            SaveService.SaveSlotData(gd);*/
            PublicData.gameData.playerName = nameInput.text;
            SceneManager.LoadScene("MainScreen");
        }
    }

}
