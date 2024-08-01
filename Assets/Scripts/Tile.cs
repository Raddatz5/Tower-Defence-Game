using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Numerics;
using Unity.Mathematics;
using Unity.VisualScripting;
//using UnityEditor.U2D;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField] bool isPlaceable;
    GameObject[] listOfSpawns;
    public bool IsPlaceable { get { return isPlaceable; } }
    public ButtonManager buttonManager;
    GameObject spawnObject;
    int index;

    Gold gold;
    int currentBalance;
    int goldCost1;

    GridManager gridManager;
    PathFinder pathFinder;
    Vector2Int coordinates = new();

    void Awake()
    {
        gridManager = FindObjectOfType<GridManager>();
        pathFinder = FindObjectOfType<PathFinder>();
        gold = FindObjectOfType<Gold>();
        buttonManager = FindObjectOfType<ButtonManager>();
    }


    void Start()
    {
        if(gridManager != null)
        {
            coordinates = gridManager.GetCoordinatesFromPosition(transform.position);

            if(!isPlaceable)
            {
                gridManager.BlockNode(coordinates);
            }
        }
              
        if (buttonManager == null) { return; }
        else
        {
            WhatShouldSpawnFromButton();
        }
       
    }



    void Update()
    {
        if (spawnObject == null) { return; }
        else
        {
            HowMuchDidTheSpawnCost();
        }
    }

    void OnMouseDown()
    {
        if (spawnObject == null)
        {
            return;
        }
        else
             if (gridManager.GetNode(coordinates).isWalkable && !pathFinder.WillBlockPath(coordinates) && currentBalance >= goldCost1)
            {
                Instantiate(spawnObject, transform.position, quaternion.identity);
                gold.Withdraw(goldCost1);
                isPlaceable = false;
                gridManager.BlockNode(coordinates);
                pathFinder.NotifyReceivers();
            }
            
    }

    private void WhatShouldSpawnFromButton()
    {
        listOfSpawns = buttonManager.selectedObjects;
        spawnObject = listOfSpawns[0];
    }

    private void HowMuchDidTheSpawnCost()
    {
        index = buttonManager.WhereOnIndex;
        spawnObject = listOfSpawns[index];
        goldCost1 = spawnObject.GetComponent<GoldCost>().HowMuch;
        currentBalance = gold.CurrentBalance;
    }
}
