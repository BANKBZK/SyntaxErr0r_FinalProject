using System;
using UnityEngine;

public static class FarmUtils 
{
    public const int WoolCapacity = 2;
    public const float Gravity = 9.8f;

    public static readonly float DayTime = (DateTime.Now.Month>=4)? 10:8;
    public static readonly float NightTime = (DateTime.Now.Month >= 4) ? 4:6;

    public static int CalculationWoolCapcity(int Amount)
    {
        return WoolCapacity * Amount;
    }
}
