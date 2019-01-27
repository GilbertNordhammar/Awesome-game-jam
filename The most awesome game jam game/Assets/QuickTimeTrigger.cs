using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuickTimeTrigger : MonoBehaviour
{
    private Rigidbody body;

    public BoxCollider trigger;
    public BoxCollider endTrigger;
    public Transform graphicModel;

    public QuicktimeData data;

    public float triggerDistance;

    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody>();
        trigger.transform.localPosition = new Vector3(0, 0, -triggerDistance);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DestroyObj()
    {
        if (data.deathForce != Vector3.zero)
        {
            Invoke("ThrowOff", 0.4f);
        }

        if (data.destructionDelay == 0f)
        {
            SelfDestruct();
        }
        else
        {
            Invoke("SelfDestruct", data.destructionDelay);
        }
    }

    void ThrowOff()
    {
        Vector3 v = new Vector3(Random.Range(0f, 2f), Random.Range(0f, 2f), Random.Range(0f, 2f));
        body.AddForce(data.deathForce + v, ForceMode.Impulse);
        body.AddTorque(data.deathForce + v, ForceMode.Impulse);
    }

    void SelfDestruct()
    {
        Destroy(gameObject);
    }
}
