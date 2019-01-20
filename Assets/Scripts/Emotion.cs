using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Emotion
{
    public string emotion;
    public double level;
    public AudioClip audio;

    public Emotion(string s, double l)
    {
        emotion = s;
        level = l;
        
    }

    public string GetEmotion()
    {
        return emotion;
    }
}