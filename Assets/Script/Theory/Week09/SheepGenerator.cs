using Mono.Cecil;
using System;
using System.Data;
using UnityEngine;

public class SheepGenerator : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Sheep watame = new Sheep(1);
        Sheep bobby = new Sheep(2);
        Sheep ahkba = new Sheep(4);

        Debug.Log("watame is number" + watame.AskNumber());
        Debug.Log("ahkba is number"+ahkba.AskNumber());
        Debug.Log("Total Sheep Method"+Sheep.GetallSheep());
        Debug.Log("Total Sheep Field" + Sheep.totalSheepCount);
        Debug.Log("Total Sheep Property" + Sheep.maxSheepCount);

        ahkba.SetNumber(3);
        Debug.Log("ankba is number"+ahkba.AskNumber()); 
        Sheep.RemoveSheep(1);

        //FarmUtils myFarm = new FarmUtils();
        int Wool = FarmUtils.CalculationWoolCapcity(Sheep.totalSheepCount);
        Debug.Log("MyFarm woolCapacity is " + Wool);
        Debug.Log("This month day time is" + FarmUtils.DayTime);
    }
}
