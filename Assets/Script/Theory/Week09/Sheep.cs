using System;
using UnityEngine;

public class Sheep 
{
    public int SheepNumber;
    public static int totalSheepCount;

    public static readonly int _initialPopulation;
    static Sheep()
    {
        _initialPopulation = 5;
        totalSheepCount = _initialPopulation;
    }
    public static int maxSheepCount
    {
        get { return totalSheepCount; }
    }

    public Sheep(int i)
    {
        this.SheepNumber = i;
        totalSheepCount++;
        Debug.Log("SheepNumber" + i + "has been created");
    }
    public void SetNumber(int i)
    {
        SheepNumber = i;    
    }
    public int AskNumber()
    {
        return SheepNumber;
    }
    public static int GetallSheep() 
    {
        return totalSheepCount;
    }

    public static void RemoveSheep(int count)
    {
        totalSheepCount -= count;
        Debug.Log("RemoveSheep totalSheep is " + totalSheepCount);
    }
    public void Jump()
    {
        Debug.Log("Sheep jump Sheep got gravity" + FarmUtils.Gravity);
    }
}
