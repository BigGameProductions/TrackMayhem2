using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EventSelectionManager : MonoBehaviour
{
    [SerializeField] private Button eventButton;
    [SerializeField] private Button meetButton;

    [SerializeField] private GameObject[] eventButtons;
    // Start is called before the first frame update
    void Start()
    {
        meetButton.interactable = true; //starts to player on the event screen
        eventButton.interactable = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void showMeets(bool show) //true if the meets should be shown
    { 
        foreach (GameObject go in eventButtons)
        {
            go.SetActive(!show);
        }
        meetButton.interactable = !show;
        eventButton.interactable = show;
    }
}
