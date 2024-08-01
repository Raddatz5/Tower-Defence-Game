using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using MoreMountains.Feedbacks;
using Unity.Mathematics;
using Unity.VisualScripting;
//using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class Singer : MonoBehaviour
{
    [SerializeField] float range = 20;
    public float Range{get{return range;}}
    float rangeOfBuff;
    [SerializeField] float rangeAfterBuff;
    Upgrade upgrade;
    float previousRangeAfterBuff;
    TowerObjectPool towerObjectPool;
    float rangeModFromBuff;

    void Start()
    {
        towerObjectPool = FindObjectOfType<TowerObjectPool>();
    }

    void OnEnable()
    {
        upgrade = GetComponent<Upgrade>();
        range = upgrade.BaseRange;
        rangeAfterBuff = range;
        previousRangeAfterBuff = rangeAfterBuff;
    }

    void Update()
    {
        CheckForRangeBuffs();
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
    }
   

}
