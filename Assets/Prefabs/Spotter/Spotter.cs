using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spotter : MonoBehaviour
{
    [SerializeField] float range = 20;
    public float Range{get{return range;}}
    [SerializeField] float rangeAfterBuff;
    public float RangeAfterBuff{get { return rangeAfterBuff;}}
    Upgrade upgrade;
    float previousRangeAfterBuff;
    TowerObjectPool towerObjectPool;


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
        else {  
                float internalRangeCalc = 0;
                for(int i = 0; i< towerObjectPool.CurrentRangeBuffs.Count; i++)
                { 
                    GameObject spotter = towerObjectPool.CurrentRangeBuffs[i];
                    if(spotter != gameObject)
                    {   
                        Upgrade spotterUpgrade = spotter.GetComponent<Upgrade>();
                        float tempRangeOfBuff = Mathf.RoundToInt(spotterUpgrade.RangeAfterBuff);
                        float rangeModFromBuff = spotterUpgrade.RangeMod;

                        int distance = Mathf.RoundToInt(Vector3.Distance(transform.position, spotter.transform.position));
                        if (distance <= tempRangeOfBuff)
                        {
                            internalRangeCalc = internalRangeCalc + rangeModFromBuff * range;
                        }
                    }
                }
                rangeAfterBuff = range + internalRangeCalc;
            }
        if(previousRangeAfterBuff != rangeAfterBuff)
        {   
            previousRangeAfterBuff = rangeAfterBuff;
            upgrade.UpdateRangeAfterBuff(rangeAfterBuff);
        }           
    }
}
