using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Animations;
using System.Globalization;

public class TomatoStand : MonoBehaviour
{
 
    [SerializeField] float range = 30f;
    [SerializeField] GameObject tomatoSplatParent;

    float rangeOfBuff;
    float rangeModFromBuff;
    [SerializeField] float rangeAfterBuff;
     public float RangeAfterBuff{get { return rangeAfterBuff;}}
    CapsuleCollider capsuleCollider;

    [SerializeField] GameObject spritePrefab; 
     [SerializeField] GameObject tomatoSplat;
    [SerializeField] float spawnRadius = 5f;
    TowerObjectPool towerObjectPool;
    int tomatoPoolCount = 15;
    GameObject[] tomatoPool;
    GameObject[] tomatoSplatPool;
    Upgrade upgrade;
    float previousRangeAfterBuff;

    void Start()
    {   
        capsuleCollider = GetComponent<CapsuleCollider>();
        towerObjectPool = FindObjectOfType<TowerObjectPool>();
        PopulatePool();
        PopulateSplatPool();
        ThrowTomatoes();
    }
    void OnEnable()
    {   
        upgrade = GetComponent<Upgrade>();
        range = upgrade.BaseRange;
        rangeAfterBuff = range;
        previousRangeAfterBuff = rangeAfterBuff;
          if(tomatoSplatPool != null)
        {
            foreach(GameObject tomato in tomatoSplatPool)
            {   if(tomato != null)
                {
                 tomato.SetActive(false);
                }
                
            }
        }
        if(tomatoPool != null)
        {
          ThrowTomatoes();
        }        
    }
    void OnDisable() 
    {   
        if(tomatoPool != null)
        {
            foreach(GameObject tomato in tomatoPool)
            {
                tomato.SetActive(false);
            }
        }
    }
     void PopulatePool()
   {
         tomatoPool = new GameObject[tomatoPoolCount];
                
        for(int j = 0; j < tomatoPoolCount; j++)
        {   
                tomatoPool[j]= Instantiate(spritePrefab,transform);
                tomatoPool[j].SetActive(false);            
        }
   }
        void PopulateSplatPool()
   {
         tomatoSplatPool = new GameObject[8];
                
        for(int j = 0; j < 4; j++)
        {   
                tomatoSplatPool[j]= Instantiate(tomatoSplat,transform.position, Quaternion.Euler(-90f,0,0), tomatoSplatParent.transform);
                tomatoSplatPool[j].SetActive(false);            
        }
   }

    void Update()
    {   
        CheckForRangeBuffs();
    }

    void ThrowTomatoes()
    {      
        StartCoroutine(SpawnTomato());                      
    }

    IEnumerator SpawnTomato()
    {   
        spawnRadius = rangeAfterBuff;
        yield return new WaitForSeconds(1f);
        for(int i = 0; i < tomatoPoolCount; i++)
        {   if(!tomatoPool[i].activeInHierarchy)
                {
                    Vector2 randomOffset = Random.insideUnitCircle * spawnRadius;
                    int randomAngle = Random.Range(0,361);
                    Vector3 spawnPosition = transform.position + new Vector3(randomOffset.x, 0.2f, randomOffset.y);
                    tomatoPool[i].transform.position = spawnPosition;
                    tomatoPool[i].transform.rotation = Quaternion.Euler(90f, randomAngle, 0f);
                    tomatoPool[i].SetActive(true);
                    SpawnSplat(spawnPosition);
                    break;
                }
        }
        ThrowTomatoes();
    }    
void SpawnSplat(Vector3 spawnLocation)
{  
     for(int j = 0; j < 4; j++)
    {   if(!tomatoSplatPool[j].activeInHierarchy)
        {
            tomatoSplatPool[j].transform.position = spawnLocation;
            tomatoSplatPool[j].SetActive(true);
            break;
        }
    }
}
   void CheckForRangeBuffs()
    { 
        range = upgrade.BaseRange;
        if (towerObjectPool.CurrentRangeBuffs.Count == 0)
        {
            rangeAfterBuff = range;
            upgrade.UpdateRangeAfterBuff(rangeAfterBuff);           
        }
        else {rangeAfterBuff = range;
                for(int i = 0; i< towerObjectPool.CurrentRangeBuffs.Count; i++)
                { 
                    GameObject spotter = towerObjectPool.CurrentRangeBuffs[i];
                    Upgrade spotterUpgrade = spotter.GetComponent<Upgrade>();
                    rangeOfBuff = Mathf.RoundToInt(spotterUpgrade.RangeAfterBuff);
                    rangeModFromBuff = spotterUpgrade.RangeMod;

                    int distance = Mathf.RoundToInt(Vector3.Distance(transform.position, spotter.transform.position));
                    if (distance <= rangeOfBuff)
                    {
                        rangeAfterBuff = rangeAfterBuff + rangeModFromBuff * range;
                    }
                }
                
            }
        if(previousRangeAfterBuff != rangeAfterBuff)
        {   
            previousRangeAfterBuff = rangeAfterBuff;
            upgrade.UpdateRangeAfterBuff(rangeAfterBuff);
        }
        capsuleCollider.radius = rangeAfterBuff;
        capsuleCollider.height = capsuleCollider.radius * 2 + 30;

    }
}
