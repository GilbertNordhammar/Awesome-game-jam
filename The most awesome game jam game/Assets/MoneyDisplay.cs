using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoneyDisplay : MonoBehaviour
{
    public static MoneyDisplay instance;

    public Text moneyText;
    public Text loseText;

    public RectTransform winScreen;

    private int moneyTarget;
    private int currentMoney;

    void Awake()
    {
        currentMoney = 0;

        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    public void SoftSetMoney(int money)
    {
        Debug.Log(gameObject.name);
        Debug.Log("Set money target to: " + money);
        moneyTarget = money;
    }

    public void SetMoney(int money)
    {
        currentMoney = money;
        moneyTarget = money;
        SetMoneyText(money);
    }

    void SetMoneyText(int money)
    {
        moneyText.text = "" + money;
    }

    public void SetLoseTextEnabled(bool enabled)
    {
        loseText.enabled = enabled;
    }

    public void Win()
    {
        winScreen.gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (moneyTarget <= 0) {
            SetMoney(0);
        } else if (currentMoney > moneyTarget) {
            if (currentMoney - moneyTarget > 2000) {
                currentMoney -= 500;
            } else if (currentMoney - moneyTarget > 50) {
                currentMoney -= 50;
            } else {
                currentMoney--;
            }
            SetMoneyText(currentMoney);
        } else if (currentMoney < moneyTarget) {
            if (moneyTarget - currentMoney > 2000) {
                currentMoney += 500;
            } else if (moneyTarget - currentMoney > 50) {
                currentMoney += 50;
            } else {
                currentMoney++;
            }
            SetMoneyText(currentMoney);
        }
    }
}
