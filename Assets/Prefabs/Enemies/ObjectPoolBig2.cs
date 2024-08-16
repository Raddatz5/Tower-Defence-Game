using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ObjectPoolBig2 : MonoBehaviour
{
    [SerializeField] GameObject enemy0;
    [SerializeField] GameObject enemy1;
    [SerializeField] GameObject enemy2;
    [SerializeField] GameObject enemy3;
    [SerializeField] GameObject enemy4;
    [SerializeField] GameObject enemy5;

    List<GameObject> enemyIndex = new();
    
    [SerializeField] [Range(0,500)] int poolSize;

    GameObject[][] enemyPools;
    public GameObject[][] EnemyPools { get { return enemyPools; } }
    [SerializeField] int numberOfMini = 4;

    EnemyMover enemyMover;
    Vector2Int arrayPosition;
    public Vector2Int ArrayPosition { get { return arrayPosition; } }
    List<GameObject> currentActiveEnemies = new();
    public List<GameObject> CurrentActiveEnemies { get { return currentActiveEnemies;}}


    private void Awake() 
    {
        CreateEnemyIndexList();
        PopulatePool();
    }

    private void CreateEnemyIndexList()
    {
        enemyIndex.Add(enemy0);
        enemyIndex.Add(enemy1);
        enemyIndex.Add(enemy2);
        enemyIndex.Add(enemy3);
        enemyIndex.Add(enemy4);
        enemyIndex.Add(enemy5);
    }

    private void PopulatePool()
    {   
        enemyPools = new GameObject[enemyIndex.Count][];
                
        for(int j = 0; j < enemyIndex.Count; j++)
        {   
            enemyPools[j] = new GameObject[poolSize];
            for (int i = 0; i<poolSize; i++)
            {
                enemyPools[j][i]= Instantiate(enemyIndex[j],transform);
                enemyPools[j][i].SetActive(false);
                arrayPosition = new Vector2Int(j,i);
            }
        }
    }

void Update()
{
    UpdateActiveEnemyList();
}
    void UpdateActiveEnemyList()
    {                
        foreach(var enemyArray in enemyPools)
        {
            foreach(var enemy in enemyArray)
            {
                if(!enemy.activeSelf)
                {
                    currentActiveEnemies.Remove(enemy);
                }
            }
        }
    }

  public void SpawnEnemy(int enemyBigNumber)
    {   
          for(int i = 0; i < poolSize; i++)
            {
                if(!enemyPools[enemyBigNumber][i].activeInHierarchy)
                {
                    enemyPools[enemyBigNumber][i].SetActive(true);
                    currentActiveEnemies.Add(enemyPools[enemyBigNumber][i]);                    
                    break;  
                }                  
        }        
    }

    public void SpawnEnemyMini(int enemyBigNumber, List<Vector3> destroyedObjectWaypoint, float destroyedObjectTravel)
    {
        int miniCount = 0;
        
        for(int i = 0; i < poolSize; i++)
            {
                if(!enemyPools[enemyBigNumber-1][i].activeInHierarchy && miniCount<numberOfMini)
                {
                    enemyMover = enemyPools[enemyBigNumber-1][i].GetComponent<EnemyMover>();
                        switch(miniCount)
                        {   case 0: 
                            Vector3 spawnLocation = Vector3.Lerp(destroyedObjectWaypoint[1], destroyedObjectWaypoint[0], destroyedObjectTravel);
                            enemyMover.TempPosition(spawnLocation);
                            miniCount++;
                            enemyPools[enemyBigNumber-1][i].SetActive(true);                            
                            break;

                            case 1:
                            Vector3 spawnLocation2 = Vector3.Lerp(destroyedObjectWaypoint[2], destroyedObjectWaypoint[1], destroyedObjectTravel);
                            enemyMover.TempPosition(spawnLocation2);
                            miniCount++;
                            enemyPools[enemyBigNumber-1][i].SetActive(true);                            
                            break;
                        }
                }
            }
    }
}
