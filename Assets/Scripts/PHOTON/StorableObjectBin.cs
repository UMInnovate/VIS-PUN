using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class StorableObjectBin : MonoBehaviour
{
    public List<StorableObject> StorableObjectList = new List<StorableObject>(); // the list containing all content instantiated via RPC
    [SerializeField]
    private PhotonRoom photonRoomRef;

    [SerializeField]
    public myPlayer myPlayerRef;

    public void AddStorableObject(StorableObject _object)
    {
        StorableObjectList.Add(_object);
        Debug.Log("A new object was added to the bin:");
        Debug.Log("Label name: " + _object.label.name + " ID: " + _object.actorNumber);
    }

    public void RemoveStorableObject_RPC(string _actorNumber) // check, clear, destroy
    {
        Debug.Log("Going to remove " + _actorNumber + " objects");
        foreach(StorableObject _object in StorableObjectList)
        {
            if(_object.actorNumber == _actorNumber)
            {
                Debug.Log("found an object to remove!");
                _object.label.GetComponent<MeshRenderer>().enabled = false; // hides the label
                
                //Destroy(_object.gameObject); // destroy
                //StorableObjectList.Remove(_object); // remove from list

            }

            else
            {
                return;
            }
        }
    }
}
