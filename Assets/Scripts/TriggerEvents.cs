using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;

public class TriggerEvents : ImageCapture {

    public GameObject sphere;
    public GameObject model;
    public GameObject cube;

    private void Awake()
    {
        sphere = GameObject.Find("Sphere");
        model = GameObject.Find("model_000");
        cube = GameObject.Find("Cube");
    }

    public void OnCollisionEnter(Collision obj)
    {
        //SetSoundObject(obj.gameObject);
        Debug.Log("Collision with " + obj);
    }

}