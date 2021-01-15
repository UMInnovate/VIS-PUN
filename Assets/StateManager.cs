using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StateManager : MonoBehaviour
{
    #region Input Manager, State Pointer, Controllers
    [Header("Observes")]
   // [SerializeField] InputManager _input = null;
    [SerializeField] CollissionManager _collissions = null;
    [SerializeField] GameObject UI = null; //point to content root
    #endregion

    private void Start()
    {

    }
}
