using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TowerObjectPool : MonoBehaviour
{
    [SerializeField] GameObject tower0;
    [SerializeField] GameObject tower1;
    [SerializeField] GameObject tower2;
    [SerializeField] GameObject tower3;
    [SerializeField] GameObject tower4;
    [SerializeField] GameObject tower5;
    [SerializeField] GameObject tower6;
    [SerializeField] GameObject tower7;
    [SerializeField] GameObject tower8;

    List<GameObject> towerIndex = new();
    public List<GameObject> TowerIndex {get {return towerIndex;}}
    
    [SerializeField] [Range(0,50)] int poolSize;

    GameObject[][] towerPools;
    public GameObject[][] TowerPools { get { return towerPools; } }

    Vector2Int arrayPosition;
    public Vector2Int ArrayPosition { get { return arrayPosition; } }
    List<GameObject> currentRangeBuffs = new();
    public List<GameObject> CurrentRangeBuffs { get { return currentRangeBuffs;}}
    List<GameObject> currentDMGBuffs = new();
    public List<GameObject> CurrentAttackBuffs { get { return currentDMGBuffs;}}
    bool isShowing = false;
    public bool IsShowing { get { return isShowing; } }


    private void Awake() 
    {
        CreateEnemyIndexList();
        PopulatePool();
    }

    private void CreateEnemyIndexList()
    {
        towerIndex.Add(tower0);
        towerIndex.Add(tower1);
        towerIndex.Add(tower2);
        towerIndex.Add(tower3);
        towerIndex.Add(tower4);
        towerIndex.Add(tower5);
        towerIndex.Add(tower6);
        towerIndex.Add(tower7);
        towerIndex.Add(tower8);
    }

    private void PopulatePool()
    {   
        towerPools = new GameObject[towerIndex.Count][];
                
        for(int j = 0; j < towerIndex.Count; j++)
        {   
            towerPools[j] = new GameObject[poolSize];
            for (int i = 0; i<poolSize; i++)
            {
                towerPools[j][i]= Instantiate(towerIndex[j],transform.position, Quaternion.Euler(0f,180f,0f),transform);
                towerPools[j][i].name = towerPools[j][i].name.Replace("(Clone)", "");
                towerPools[j][i].SetActive(false);
            }
        }
    }

     void Update()
    {   
        UpdateActiveRangeBuffList();
        UpdateActiveDMGBuffList();
       
        if(Input.GetKeyDown(KeyCode.R))
        {   
            isShowing = !isShowing;
            ShowRangeCircle(isShowing);
        }
    }

 void UpdateActiveRangeBuffList()
    {     currentRangeBuffs.Clear();     
        for(int i = 0; i < towerPools[7].Length; i++)
        {
            if(towerPools[7][i].activeSelf)
            {
                currentRangeBuffs.Add(towerPools[7][i]);
            }
        }
    }
    void UpdateActiveDMGBuffList()
    {    currentDMGBuffs.Clear();     
        for(int i = 0; i < towerPools[5].Length; i++)
        {
            if(towerPools[5][i].activeSelf)
            {
                currentDMGBuffs.Add(towerPools[5][i]);
            }
        }
    }
public GameObject SpawnTower(int towerIndex, Vector3 position, GameObject waypoint)
{   
    for(int i = 0; i < towerPools[towerIndex].Length; i++)
    {   
        if(!towerPools[towerIndex][i].activeSelf)
        {
            towerPools[towerIndex][i].transform.position = position;
            towerPools[towerIndex][i].GetComponent<Upgrade>().AssignWaypoint(waypoint);
            towerPools[towerIndex][i].SetActive(true);
            GameObject returnedObject = towerPools[towerIndex][i];
            return returnedObject;
        }  
    }
    return null;
}
 public void ShowRangeCircle(bool value)
    {
        BroadcastMessage("ShowCircle",isShowing,SendMessageOptions.DontRequireReceiver);
    }
}
