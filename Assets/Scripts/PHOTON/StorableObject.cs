using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StorableObject : MonoBehaviour
{
    public string actorNumber; // unique ID that PUN randomly assigns to each player
    public TextMeshPro label; // label that gets instantiated via RPC in all client's scenes
}
