using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Prop
{
    [SerializeField] public Vector3 offsetPosition;
    [SerializeField] public int index;
    [SerializeField] public Transform prop;
}

public class StageSpawner : MonoBehaviour
{
    public Transform[] difficulty1;
    public Transform[] difficulty2;
    public Transform[] difficulty3;

    public Prop[] props;

    public Transform[] levels;
    public float intervalDistance;
    public int chunkLength;

    public float lowerEdge;
    public float upperEdge;

    public bool move;
    public bool slowmo;

    public float quicktimeTimer;

    [Space]
    public int difficultyRaise1;
    public int difficultyRaise2;

    private Transform[] quicktimeTriggers;
    private List<Transform> buffer;
    private Vector3 movementVector;
    private Vector3 quicktimeSpawnPosition;
    private Transform activeQuicktimeTrigger;
    private float quicktimeTimerInitialValue;
    private int completedQuicktimes;

    void Awake()
    {
        completedQuicktimes = 0;
        quicktimeTriggers = difficulty1;
        movementVector = new Vector3(0, 0, -GlobalConfig.StageSpeed);
        quicktimeSpawnPosition = new Vector3(0, 1, upperEdge);
        quicktimeTimerInitialValue = quicktimeTimer;
    }

    // Start is called before the first frame update
    void Start()
    {
        buffer = InitialStage();
    }

    // Update is called once per frame
    void Update()
    {
        if (move)
        {
            quicktimeTimer -= Time.deltaTime;

            if (activeQuicktimeTrigger == null && quicktimeTimer <= 0.0f)
            {
                SpawnQuickTimeTrigger();
            }
        }
    }

    // called with a fixed interval (every physics update)
    void FixedUpdate()
    {
        if (activeQuicktimeTrigger != null && move)
        {
            activeQuicktimeTrigger.position += movementVector * (slowmo ? GlobalConfig.slowmoFactor : 1.0f);
        }

        foreach (Transform c in buffer)
        {
            if (move)
            {
                c.position += movementVector * (slowmo ? GlobalConfig.slowmoFactor : 1.0f);
            }
        }
    }

    public void OnQuicktimeStart()
    {
        slowmo = true;
    }

    public void OnQuicktimeEnd(bool success)
    {
        slowmo = false;
        quicktimeTimer = quicktimeTimerInitialValue;
        activeQuicktimeTrigger = null;
        completedQuicktimes++;
        if (completedQuicktimes > difficultyRaise2)
        {
            quicktimeTriggers = difficulty3;
        } else if (completedQuicktimes > difficultyRaise1)
        {
            quicktimeTriggers = difficulty2;
        }
    }

    void SpawnQuickTimeTrigger()
    {
        Transform quicktimeTrigger = quicktimeTriggers[UnityEngine.Random.Range(0, quicktimeTriggers.Length)];
        activeQuicktimeTrigger = Instantiate(quicktimeTrigger, quicktimeSpawnPosition, new Quaternion(0, 0, 0, 0));
    }

    List<Transform> InitialStage()
    {
        List<Transform> ret = new List<Transform>();

        for (int i = 0; i < chunkLength; i++)
        {
            Vector3 chunkPos = transform.position + new Vector3(0, 0, i * intervalDistance);

            foreach (Prop p in props)
            {
                if (p.index == i)
                {
                    Transform prop = Instantiate(p.prop, chunkPos + p.offsetPosition, new Quaternion(0, 0, 0, 0));
                    ret.Add(prop);
                }
            }

            Transform chunk = Instantiate(levels[UnityEngine.Random.Range(0, levels.Length)], chunkPos, new Quaternion(0, 0, 0, 0));
            chunk.SetParent(transform);
            ret.Add(chunk);
        }

        return ret;
    }

    public Vector3 GetActiveQuicktimeTriggerPosition()
    {
        if (activeQuicktimeTrigger == null) return Vector3.left;
        return activeQuicktimeTrigger.position;
    }
}
