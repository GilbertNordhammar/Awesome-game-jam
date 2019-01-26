using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class QuicktimeData : ScriptableObject
{
    public string animation;
    public int cost;
    public float destructionDelay;
    public Vector3 deathForce;
}
