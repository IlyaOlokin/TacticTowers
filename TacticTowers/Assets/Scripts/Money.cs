using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Money : MonoBehaviour
{
    private static float money;
    private static Text text;
    private static Animation anim;

    private void Start()
    {
        text = GetComponent<Text>();
        text.text = money.ToString();
        SetMoney(0);
        anim = GetComponent<Animation>();

    }
    public static void AddMoney(float income)
    {
        float oldMoney = money;
        money += income * GlobalBaseEffects.TempMoneyMultiplier;
        if (Mathf.Floor(money) > Mathf.Floor(oldMoney)) {
            PlayAnimation();
        }
        WriteMoney();
    }

    private static void PlayAnimation()
    {
        anim.Stop("MoneyAnimation");
        anim.Play("MoneyAnimation");
    }

    public static void TakeMoney(int cost)
    {
        money -= cost;
        WriteMoney();
    }

    public static float GetMoney()
    {
        return money;
    }

    private static void SetMoney(int _money)
    {
        money = _money;
        WriteMoney();

    }
    
    private static void WriteMoney()
    {
        text.text = Mathf.Floor(money).ToString();
    }
}
