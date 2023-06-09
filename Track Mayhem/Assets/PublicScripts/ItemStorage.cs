using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemStorage : MonoBehaviour
{
    public List<GameObject> characterList; //holds all the characters models

    public Avatar mainCharacterAvatar; //holds the avatar that all peoplle are based on

    public RuntimeAnimatorController characterAnimator; //holds the animator for the new character

    public Sprite[] flags; //this is all the sprites for all the flags

    public Color[] chestColors; //this is the colors for all the chest rarities

    public Sprite[] labelSprites; //list of labels for the player banner
    //0 is wr

    public string[] eventNames; //stores the list of the string names of the events
    public Sprite[] pictogramSprites; //sprites of all the events pictograms
    public Vector2[] pictogramSizes; //sizes of the rect transform of the pictogram

    public Color[] bannerColors;
    //0 is character best
    //1 is personal record
    //2 is game record

    public Vector3[] decathalonCalculations; //numbers to multiply the scores by

    public float[] decathalonMeterConversions; //number for conversions for each event

    public Sprite[] runnerImages; //all the profile icons for the runners

    public Color[] rarityColors; //color for the backround of the rarities

    public AudioClip[] audios; //list of sound effects in the game


    public int findFlagIndexOfCountry(string code) //find the index of flags given the country code
    {
        for (int i = 0; i < flags.Length; i++)
        {
            if (flags[i].name == code.ToLower())
            {
                return i;
            }
        }
        return 0;
    }

    public GameObject initRunner(int id, Transform tf, GameObject basePlayer=null) //class used by all scenes to init the current runner and return in in case of any other changes
    {
        GameObject childPlayer = Instantiate(characterList[id], tf.transform); //copies the current runner into the scene

        if (basePlayer != null)
        {
            List<string> colliderList = new List<string>();
            colliderList.Add("Head");
            colliderList.Add("Spine");
            colliderList.Add("LeftLeg");
            colliderList.Add("LeftUpLeg");
            colliderList.Add("RightUpLeg");
            colliderList.Add("RightLeg");
            colliderList.Add("LeftArm");
            colliderList.Add("RightArm");
            colliderList.Add("LeftForeArm");
            colliderList.Add("RightForeArm");

            PublicData.addColldiersToRunner(basePlayer, childPlayer, "mixamorig1:", colliderList);
        }
        

        childPlayer.transform.SetAsFirstSibling();
        Animator childAnimator;
        if (childPlayer.GetComponent<Animator>() == null)
        {
            childAnimator = childPlayer.AddComponent<Animator>(); //adds the animator to the runner

        } else
        {
            childAnimator = childPlayer.GetComponent<Animator>(); //gets current animator if it already has one
        }
        childAnimator.avatar = mainCharacterAvatar; //sets the avator layout for animation
        childAnimator.runtimeAnimatorController = characterAnimator; //sets the animator controller for animations
        childAnimator.applyRootMotion = true; //setting root motion to true for animation to work
        childAnimator.cullingMode = AnimatorCullingMode.CullUpdateTransforms; //setting the culling mode to make the animation smooth
        return childPlayer;
    }
}
