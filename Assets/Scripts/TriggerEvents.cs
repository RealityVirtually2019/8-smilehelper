using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;

public class TriggerEvents : MonoBehaviour {

    public interface ICustomMessageTarget : IEventSystemHandler
    {
        // functions that can be called via the messaging system
        void TestMessage1();
    }


    //anything extending ICustomMessageTarget can implement custom behavior upon receiving message.
   
    public class MessageTarget1 : MonoBehaviour, ICustomMessageTarget
    {
        public void TestMessage1()
        {
            Debug.Log("Message Received");
        }
    }
}

//this code will execute the function TestMessage1 on any components on the GameObject target that implement the ICustomMessageTarget interface
//ExecuteEvents.Execute<ICustomMessageTarget>(target, null, (x,y)=>x.Message1());