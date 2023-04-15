using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemStorage : MonoBehaviour
{
    public List<GameObject> characterList; //holds all the characters models

    public Avatar mainCharacterAvatar; //holds the avatar that all peoplle are based on

    public RuntimeAnimatorController characterAnimator; //holds the animator for the new character
    //TODO add flags later

    public GameObject initRunner(int id, Transform tf) //class used by all scenes to init the current runner and return in in case of any other changes
    {
        GameObject childPlayer = Instantiate(characterList[id], tf.transform); //copies the current runner into the scene
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