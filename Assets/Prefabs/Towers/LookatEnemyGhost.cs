using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using MoreMountains.Feedbacks;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;
using Quaternion = UnityEngine.Quaternion;
using UnityEngine.SocialPlatforms;


public class LookatEnemyGhost : MonoBehaviour
{


    [SerializeField] float range = 20;
    [SerializeField] float rangeAfterBuff;
    public float RangeAfterBuff { get { return rangeAfterBuff; } }
    TowerObjectPool towerObjectPool;


  
void Start()
{     
    towerObjectPool = FindObjectOfType<TowerObjectPool>();
}
 void OnEnable()
    {   
        rangeAfterBuff = range;
    }

    void Update()
    {
        CheckForRangeBuffs();
    
    }
      void CheckForRangeBuffs()
    {    
    
        if (towerObjectPool.CurrentRangeBuffs.Count == 0)
        {
            rangeAfterBuff = range;       
        }
        else { rangeAfterBuff = range;
                for(int i = 0; i< towerObjectPool.CurrentRangeBuffs.Count; i++)
                { 
                    GameObject spotter = towerObjectPool.CurrentRangeBuffs[i];
                    Upgrade spotterUpgrade = spotter.GetComponent<Upgrade>();
                    float rangeOfBuff = Mathf.RoundToInt(spotterUpgrade.RangeAfterBuff);
                    float rangeModFromBuff = spotterUpgrade.RangeMod;

                    int distance = Mathf.RoundToInt(Vector3.Distance(transform.position, spotter.transform.position));
                    if (distance <= rangeOfBuff)
                    {   
                        rangeAfterBuff = rangeAfterBuff + rangeModFromBuff * range;
                    }
                
                } 
            }

        }
    }


