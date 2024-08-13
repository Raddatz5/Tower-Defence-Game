using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Numerics;
using Unity.Mathematics;
using Unity.VisualScripting;
//using UnityEditor.U2D;
using UnityEngine;
using UnityEngine.EventSystems;
using Vector3 = UnityEngine.Vector3;

public class Waypoint : MonoBehaviour
{
    [SerializeField] bool isPlaceable;
    public bool IsPlaceable { get { return isPlaceable; } }
    [SerializeField] bool isWall;
    public bool IsWall { get { return isWall;}}
    GameObject[] listOfSpawns;
    public ButtonManager buttonManager;
    GameObject spawnObject;
    int index;
    Gold gold;
    int currentBalance;
    int goldCost1;
    Camera mainCamera;
    TowerObjectPool towerObjectPool;
    TileBorder tileBorder;
    bool isThereATowerOnMe = false;
    GameObject whatTowerIsOnMe = null;
    GridManager gridManager;
    PathFinder pathFinder;
    Vector2Int coordinates = new();

    void Awake()
    {
        gridManager = FindObjectOfType<GridManager>();
        pathFinder = FindObjectOfType<PathFinder>();
    }
    void Start()
    {   towerObjectPool = FindObjectOfType<TowerObjectPool>(); 
        tileBorder = GetComponent<TileBorder>();
        mainCamera = Camera.main;
        mainCamera.eventMask = 1 <<
        LayerMask.NameToLayer("Default");
        if (buttonManager == null) { return; }
        else
        {
            gold = FindAnyObjectByType<Gold>();
            buttonManager = FindAnyObjectByType<ButtonManager>();
            listOfSpawns = buttonManager.selectedObjects;
            spawnObject = listOfSpawns[0];
        }
        if(gridManager != null)
        {
            coordinates = gridManager.GetCoordinatesFromPosition(transform.position);

            if(!isPlaceable || isWall)
            {
                gridManager.BlockNode(coordinates);
            }
        }
    }
    public void UpdateMouseOver() 
    {    
       buttonManager.GhostPositionUpdate(transform.position, isPlaceable && !pathFinder.WillBlockPath(coordinates));       
    }
    public void CheckTile()
    {  
        if (spawnObject == null)
        {
            return;
        }
        else if (!EventSystem.current.IsPointerOverGameObject() && isPlaceable && !pathFinder.WillBlockPath(coordinates) && buttonManager.IsBuildMenuOpen)
        {   
                index = buttonManager.WhereOnIndex;
                spawnObject = listOfSpawns[index];
                goldCost1 = spawnObject.GetComponent<GoldCost>().HowMuch;
                currentBalance = gold.CurrentBalance;
                if(currentBalance >= goldCost1)
                {
                    whatTowerIsOnMe = towerObjectPool.SpawnTower(index, transform.position, gameObject);
                    if(this.CompareTag("Floating"))
                    {
                        whatTowerIsOnMe.transform.parent = transform; 
                    }
                    gold.Withdraw(goldCost1);
                    isPlaceable = false;
                    gridManager.BlockNode(coordinates);
                    pathFinder.NotifyReceivers();
                    tileBorder.UpdateSquare();
                    isThereATowerOnMe = true;
                }
        }
        else if(isThereATowerOnMe && !buttonManager.IsBuildMenuOpen)
        {
            whatTowerIsOnMe.GetComponent<Upgrade>().TowerUpgradeMenuFirst();
        }
        }
    public void MakePlaceable()
    {
        gridManager.UnBlockNode(coordinates);
        pathFinder.NotifyReceivers();
        isPlaceable = true;
    }
    
    public void NoMoreTower()
    {
        isThereATowerOnMe = false;
    }
    
}
