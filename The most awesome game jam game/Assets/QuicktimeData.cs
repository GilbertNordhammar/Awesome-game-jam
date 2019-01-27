using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class QuicktimeData : ScriptableObject
{
    public string thingName;
    public string animation;
    public int cost;
    public int difficulty;
    public float destructionDelay;
    public Vector3 deathForce;

    [FMODUnity.EventRef]
    public string failSound;
}
