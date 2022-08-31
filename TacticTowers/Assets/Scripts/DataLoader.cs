using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataLoader : MonoBehaviour
{

    void Start()
    {
        Technologies.BaseHpMultiplier = float.Parse(LoadString("baseHpMultiplier", "1"));
        Technologies.DmgMultiplier = float.Parse(LoadString("dmgMultiplier", "1"));
        Technologies.ShootAngleMultiplier = float.Parse(LoadString("shootAngleMultiplier", "1"));
        Technologies.MoneyMultiplier = float.Parse(LoadString("moneyMultiplier", "1"));
        
        Technologies.IsFrostGunUnlocked = Convert.ToBoolean(LoadInt("isFrostGunUnlocked", 0));
        Technologies.IsFlamethrowerUnlocked = Convert.ToBoolean(LoadInt("isFlamethrowerUnlocked", 0));
        Technologies.IsRailgunUnlocked = Convert.ToBoolean(LoadInt("isRailgunUnlocked", 0));
        Technologies.IsTeslaUnlocked = Convert.ToBoolean(LoadInt("isTeslaUnlocked", 0));

        Credits.credits = int.Parse(LoadString("Credits", "0"));
        Credits.CreditsInTotal = int.Parse(LoadString("CreditsInTotal",  Credits.credits.ToString()));

        Technologies.MinUpgradePrice = LoadInt("minUpgradePrice", 10);

        BaseSelectManager.SelectedBaseIndex = LoadInt("selectedBaseIndex", 0);
        
        
        YandexSDK.Instance.Authenticate();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            Credits.AddCredits(100);
        }

        if (Input.GetKeyDown(KeyCode.U))
        {
            PlayerPrefs.DeleteAll();
        }
    }

    public static void SaveInt(string variableName, int isUnlocked)
    {
        PlayerPrefs.SetInt(variableName, Convert.ToInt16(isUnlocked));
    }

    public static void SaveInt(string variableName, bool isUnlocked)
    {
        PlayerPrefs.SetInt(variableName, Convert.ToInt16(isUnlocked));
    }

    public static void SaveString(string variableName, string value)
    {
        PlayerPrefs.SetString(variableName, value);
    }

    public static string LoadString(string variableName, string defaultValue)
    {
        return PlayerPrefs.GetString(variableName, defaultValue);
    }
    
    public static int LoadInt(string variableName, int defaultValue)
    {
        return PlayerPrefs.GetInt(variableName, defaultValue);
    }
}
