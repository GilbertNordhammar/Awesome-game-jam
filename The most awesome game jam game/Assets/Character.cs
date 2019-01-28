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

    private Rigidbody body;

    void Awake() {
        body = GetComponent<Rigidbody>();
    }

    // Start is called before the first frame update
    void Start()
    {
        svenska_riksdaler = starting_riksdaler;
        InputSystem.instance.Failure += TriggerQuicktimeEventFail;
        InputSystem.instance.Succes += TriggerQuicktimeEventSuccess;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void Lose()
    {
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
    }

    void TriggerQuicktimeEventSuccess()
    {
        TriggerQuicktimeEventOver(true);
    }

    void TriggerQuicktimeEventFail()
    {
        TriggerQuicktimeEventOver(false);
    }

    public void TriggerQuicktimeEventOver(bool success)
    {
        if (success) {
            armsAnimator.SetTrigger(currentQuicktime.data.animation);
            currentQuicktime.DestroyObj();
        } else {
            var sound = FMODUnity.RuntimeManager.CreateInstance(currentQuicktime.data.failSound);
            sound.start();

            ReceiptDisplay.instance.SetTotalCost(currentQuicktime.data.cost);
            ReceiptDisplay.instance.SetThingName(currentQuicktime.data.thingName);
            ReceiptDisplay.instance.Show(true);
            Invoke("HideReceipt", 1.2f);

            svenska_riksdaler -= currentQuicktime.data.cost;
            Destroy(currentQuicktime.gameObject);
        }
        InputSystem.instance.StopInput();
        spawner.OnQuicktimeEnd(success);
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
