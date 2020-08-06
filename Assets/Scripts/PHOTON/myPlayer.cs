using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class myPlayer : MonoBehaviour
{
    [HideInInspector]
    public string myPlayerActorNumber;

    public void GetMyPlayerActorNumber(string _actorNumber) // returns the player's actor number
    {
        myPlayerActorNumber = _actorNumber;
    }
}
