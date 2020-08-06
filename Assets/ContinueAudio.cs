using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContinueAudio : MonoBehaviour
{
    //allow for audio to be accessed from another script or scene
    // ex: ContinueAudio.Instance.GameObject.GetComponent<AudioSource>().pause();
    private static ContinueAudio instance = null;
    public static ContinueAudio Instance
    {
        get { return instance; }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void Awake()
    {
        if(instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
        //allows for game object to exist outside of scene
        DontDestroyOnLoad(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
