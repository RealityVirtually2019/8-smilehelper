using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buddy : MonoBehaviour {

    public static Buddy instance;

    private void Awake()
    {
        instance = this;
    }
    
    public void PositionBuddy()
    {
        //TODO set position based on person's position
    }

    public void playSound(Emotion emotion)
    {
        //buddyObject.GetComponent<AudioSource>().Play();
    }
}