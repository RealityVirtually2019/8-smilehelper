using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;

public class TriggerEvents : MonoBehaviour {

    public GameObject sphere = GameObject.FindGameObjectWithTag("Sphere");
    public GameObject model = GameObject.FindGameObjectWithTag("Model");
    public GameObject cube = GameObject.FindGameObjectWithTag("Cube");

    public void OnCollisionEnter(Collision obj)
    {
        if (obj.gameObject == sphere)
        {
            sphere.GetComponent<Renderer>().enabled = false;
            model.GetComponent<Renderer>().enabled = true;
            cube.GetComponent<Renderer>().enabled = true;
        }
        if (obj.gameObject == model)
        {
            sphere.GetComponent<Renderer>().enabled = true;
            model.GetComponent<Renderer>().enabled = false;
            cube.GetComponent<Renderer>().enabled = true;
        }
        if (obj.gameObject == cube)
        {
            sphere.GetComponent<Renderer>().enabled = true;
            model.GetComponent<Renderer>().enabled = true;
            cube.GetComponent<Renderer>().enabled = false;
        }
    }

}

//this code will execute the function TestMessage1 on any components on the GameObject target that implement the ICustomMessageTarget interface
//ExecuteEvents.Execute<ICustomMessageTarget>(target, null, (x,y)=>x.Message1());