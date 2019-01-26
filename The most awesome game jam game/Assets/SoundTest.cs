using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundTest : MonoBehaviour
{
    [FMODUnity.EventRef]
    public string soundEvent;

    // Start is called before the first frame update
    void Start()
    {
        var sound = FMODUnity.RuntimeManager.CreateInstance(soundEvent);
        sound.start();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
