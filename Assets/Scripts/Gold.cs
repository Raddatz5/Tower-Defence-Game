using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using TMPro;



public class Gold : MonoBehaviour
{
    [SerializeField] int startingBalance = 150;
    [SerializeField] int currentBalance;
    public int CurrentBalance{get{return currentBalance;}}
    [SerializeField] TextMeshProUGUI displauBalance;

void Awake()
{
    currentBalance = startingBalance;
    UpdateDisplay();
}
private void Update() 
{
    if(currentBalance < 1)
    {
        currentBalance = 0;
    }
}
    public void AddToGold(int goldAmount)
    {
        currentBalance += Mathf.Abs(goldAmount);
        UpdateDisplay();
    }

    public void Withdraw(int goldAmount)
    {
        currentBalance -= Mathf.Abs(goldAmount);
        UpdateDisplay();
    }

    void UpdateDisplay()
    {
        displauBalance.text = "Gold: " + currentBalance;
    }
}
