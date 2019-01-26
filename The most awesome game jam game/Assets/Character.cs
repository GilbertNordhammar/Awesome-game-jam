using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public StageSpawner spawner;

    private float startTimer = 10.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float d = DistanceToQuicktimeEnd();
        if (d > -1.0f)
        {
            Debug.Log(d);
        }
    }

    // returns -1.0f if there is no active quicktime event approaching the player.
    public float DistanceToQuicktimeEnd()
    {
        Vector3 pos = spawner.GetActiveQuicktimeTriggerPosition();
        if (pos == Vector3.left) return -1.0f;

        pos = new Vector3(pos.x, transform.position.y, pos.z);
        return Vector3.Distance(transform.position, pos) - 1.0f;
    }

    void TriggerQuicktimeEvent(QuickTimeTrigger obj)
    {
        spawner.OnQuicktimeStart();
    }

    public void TriggerQuicktimeEventOver(bool success)
    {
        Debug.Log("Quicktime event ended");
        spawner.OnQuicktimeEnd(success);
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Trigger")) {
            TriggerQuicktimeEvent(col.transform.parent.GetComponent<QuickTimeTrigger>());
        }
        else if (col.CompareTag("EndTrigger")) {
            TriggerQuicktimeEventOver(false);
        }
    }
}
