using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.MagicLeap;
using UnityEngine.UI;

public class Keypad : MonoBehaviour
{
    // module 2 only
    [SerializeField, Tooltip("Text field of button for Handedness selection")]
    Text screenText = null;

    Button b; 

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void PINButtonClicked(GameObject btn)
    {
        Text btnText = btn.GetComponentInChildren<Text>();
        screenText = btnText;
        Debug.Log("Button Clicked: " + btnText);
        if (btn.gameObject.name == "Button Send")
            GLOBALS.stage++;
    }
}
