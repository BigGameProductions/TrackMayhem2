using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ScreenshotScript : MonoBehaviour
{
    [SerializeField] GameObject player;
    [SerializeField] ItemStorage itemStorage;

    [SerializeField] int runnerNumber;
    [SerializeField] int rarityNumber;

    [SerializeField] private Material backgroundColor;

    [SerializeField] Color[] rarityColors;

    private GameObject lastRunner;

    // Start is called before the first frame update
    void Start()
    {
        updateChar();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void updateChar()
    {
        if (lastRunner != null)
        {
            Destroy(lastRunner);
        }
        itemStorage.initRunner(runnerNumber, player.transform);
        lastRunner = player.GetComponentsInChildren<Transform>()[1].gameObject;
        lastRunner.GetComponent<Animator>().speed = 0;
        string rarityName = PublicData.charactersInfo.ElementAt(runnerNumber + 1)[7];
        int rarityIndex = 3;
        if (rarityName == "Common")
        {
            rarityIndex = 0;
        } else if (rarityName == "Rare")
        {
            rarityIndex = 1;
        } else if (rarityName == "Epic")
        {
            rarityIndex = 2;
        }
        backgroundColor.color = rarityColors[rarityIndex];
    }
}
