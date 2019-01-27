using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReceiptDisplay : MonoBehaviour
{
    public static ReceiptDisplay instance;

    public Text thingName;
    public Text totalCost;

    public RectTransform receiptParent;

    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    public void Show(bool show)
    {
        receiptParent.gameObject.SetActive(show);
    }

    public void SetThingName(string name)
    {
        thingName.text = name;
    }

    public void SetTotalCost(int cost)
    {
        totalCost.text = "" + cost;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
