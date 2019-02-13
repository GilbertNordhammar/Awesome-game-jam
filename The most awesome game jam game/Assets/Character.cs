using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    [Space]
    public int starting_riksdaler;

    public StageSpawner spawner;
    public Animator armsAnimator;
    [Range(0, 10)] public int difficulty = 10;

    private QuickTimeTrigger currentQuicktime;
    private int svenska_riksdaler_;
    public int svenska_riksdaler {
        get {
            return svenska_riksdaler_;
        }
        set {
            MoneyDisplay.instance.SoftSetMoney(value);
            svenska_riksdaler_ = value;

            if (svenska_riksdaler_ <= 0) {
                Lose();
            }
        }
    }

    [Space]
    [FMODUnity.EventRef]
    public string slowmoEvent;

    [FMODUnity.EventRef]
    public string slapEvent;

    [FMODUnity.EventRef]
    public string stepEvent;

    [FMODUnity.EventRef]
    public string breathEvent;

    [Space]
    public float stepInterval;

    private float stepCounter;

    private Rigidbody body;

    private FMOD.Studio.EventInstance slapEventInstance;
    private FMOD.Studio.EventInstance slowmoEventInstance;
    private FMOD.Studio.EventInstance breathEventInstance;
    private FMOD.Studio.EventInstance stepEventInstance;

    private bool slowmo;
    private bool lost;

    private float startingFOV;

    void Awake() {
        body = GetComponent<Rigidbody>();
        startingFOV = Camera.main.fieldOfView;
    }

    // Start is called before the first frame update
    void Start()
    {
        svenska_riksdaler = starting_riksdaler;
        InputSystem.instance.Failure += TriggerQuicktimeEventFail;
        InputSystem.instance.Succes += TriggerQuicktimeEventSuccess;

        slowmoEventInstance = FMODUnity.RuntimeManager.CreateInstance(slowmoEvent);
        slapEventInstance = FMODUnity.RuntimeManager.CreateInstance(slapEvent);
        breathEventInstance = FMODUnity.RuntimeManager.CreateInstance(breathEvent);
        stepEventInstance = FMODUnity.RuntimeManager.CreateInstance(stepEvent);

        breathEventInstance.start();

        slowmo = false;
        lost = false;
        stepCounter = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        stepCounter += Time.deltaTime * (slowmo ? GlobalConfig.slowmoFactor * 2f : 1.0f);

        if (stepCounter > stepInterval && !lost)
        {
            stepEventInstance.start();
            stepCounter = 0f;
        }

        if (slowmo)
        {
            Camera.main.fieldOfView -= (GlobalConfig.slowmoFactor * 0.2f);
        }
    }

    void Lose()
    {
        lost = true;
        body.AddTorque(new Vector3(
            2.0f, 
            UnityEngine.Random.Range(0f, 2f) - 1f, 
            UnityEngine.Random.Range(0f, 2f) - 1f), 
            ForceMode.Impulse
            );
        MoneyDisplay.instance.SetLoseTextEnabled(true);
        spawner.move = false;
        GetComponent<CapsuleCollider>().isTrigger = false;
        body.useGravity = true;
        InputSystem.instance.PlaySessionIsOver();
    }

    void Win()
    {
        spawner.move = false;
        MoneyDisplay.instance.Win();
        InputSystem.instance.PlaySessionIsOver();
        MoneyDisplay.instance.SetWinTextEnabled(true);
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
        currentQuicktime = obj;
        InputSystem.instance.StartInput(obj.data.difficulty);
        spawner.OnQuicktimeStart();
        ActivateSlowmo();
    }

    void TriggerQuicktimeEventSuccess()
    {
        TriggerQuicktimeEventOver(true);
    }

    void TriggerQuicktimeEventFail()
    {
        TriggerQuicktimeEventOver(false);
    }

    void TriggerSlapSound()
    {
        slapEventInstance.start();
    }

    void ActivateSlowmo()
    {
        slowmo = true;
        armsAnimator.SetFloat("arm_speed", GlobalConfig.slowmoFactor);
        slowmoEventInstance.start();
    }

    void DeactivateSlowmo()
    {
        slowmo = false;
        armsAnimator.SetFloat("arm_speed", 1.0f);
        Camera.main.fieldOfView = startingFOV;
    }

    public void TriggerQuicktimeEventOver(bool success)
    {
        if (success) {
            armsAnimator.SetTrigger(currentQuicktime.data.animation);
            currentQuicktime.DestroyObj();
            Invoke("TriggerSlapSound", 0.4f);
        } else {
            currentQuicktime.data.failSoundInstance.start();

            ReceiptDisplay.instance.SetTotalCost(currentQuicktime.data.cost);
            ReceiptDisplay.instance.SetThingName(currentQuicktime.data.thingName);
            ReceiptDisplay.instance.Show(true);
            Invoke("HideReceipt", 1.2f);

            svenska_riksdaler -= currentQuicktime.data.cost;
            Destroy(currentQuicktime.gameObject);
        }
        InputSystem.instance.StopInput();
        spawner.OnQuicktimeEnd(success);
        DeactivateSlowmo();
    }

    void HideReceipt()
    {
        ReceiptDisplay.instance.Show(false);
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Trigger")) {
            TriggerQuicktimeEvent(col.transform.parent.GetComponent<QuickTimeTrigger>());
        }
        else if (col.CompareTag("EndTrigger")) {
            TriggerQuicktimeEventOver(false);
        }
        else if (col.CompareTag("Home"))
        {
            Win();
        }
    }
}
