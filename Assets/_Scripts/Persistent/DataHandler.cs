using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DataHandler 
{
    public enum Flag
    {
        totalDamage,
        totalKills,
        totalTime,
        totalSpent,
        showDamageNumbers,
        totalWon,
    }

    public static string totalDamage = "totalDamage";
    public static string totalKills = "totalKills";
    public static string totalTime = "totalTime";
    public static string totalSpent = "totalSpent";
    public static string showDamageNumbers = "showDamageNumbers";
    public static string totalWon = "totalWon";

    public static void SetFlag(Flag flag, int value)
    {
        string key = GetFlagKey(flag);
        if (key != null)
        {

            
            PlayerPrefs.SetInt(key, value);
            

        }
    }

    public static bool IncrementFlag(Flag flag, int increment = 1)
    {
        if (intFlags.Contains(flag))
        {

            string key = GetFlagKey(flag);
            int oldVal = GetFlagInt(flag);
            SetFlag(flag, oldVal + increment);
            return true;
        }
        return false;
    }

    public static int GetFlagInt(Flag flag)
    {
        string key = GetFlagKey(flag);

        if (intFlags.Contains(flag))
        {
            return PlayerPrefs.GetInt(key, 0);
        }

        return -1;
        
    }


     static string GetFlagKey(Flag flag)
    {
        switch (flag)
        {
            case Flag.totalDamage: return totalDamage;
            case Flag.totalKills: return totalKills;
            case Flag.totalTime: return totalTime;
            case Flag.totalSpent: return totalSpent;
            case Flag.showDamageNumbers: return showDamageNumbers;
            case Flag.totalWon: return totalWon;
            default: return null;
        }
    }


    static List<Flag> intFlags = new List<Flag>()
    {
        Flag.totalDamage,
        Flag.totalKills,
        Flag.totalTime,
        Flag.totalSpent,
        Flag.showDamageNumbers,
        Flag.totalWon,



    };


}

