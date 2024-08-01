using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MoreMountains.Feedbacks;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class LookAtEnemyGatling : MonoBehaviour
{
    [SerializeField] Transform weapon;
    [SerializeField] GameObject target = null;
    [SerializeField] GameObject rotateMe;
    GameObject closestEnemy = null;
    float range = 20;
    ParticleSystem particleSystemA;
    bool imShooting;
    [SerializeField] float rotationSpeed = 5f;
    float rangeOfBuff;
    [SerializeField] float rangeAfterBuff;
    public float RangeAfterBuff{get{return rangeAfterBuff;}}
    float rangeModFromBuff;
    List<GameObject> numberOfEnemies = new();
     Vector3 lastTargetPosition;
    [SerializeField] float targetChangeSpeed = 3f;
    bool coroutineIsWaitingPlaying = false;
    [SerializeField]bool coroutineIsLerping = false;
    Quaternion currentRotation;
    Quaternion lastTargetRotation;
    Quaternion targetRotation;
    TowerObjectPool towerObjectPool;
    CapsuleCollider capsuleCollider;
    bool boolEnemyInRange = false;
    bool imRotating = false;
    Upgrade upgrade;
    float previousRangeAfterBuff;

    void Start()
    {
        particleSystemA = GetComponentInChildren<ParticleSystem>();
        lastTargetPosition = new Vector3(transform.position.x,transform.position.y,transform.position.z + 10f);
        towerObjectPool = FindObjectOfType<TowerObjectPool>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        range = upgrade.BaseRange;
        rangeAfterBuff = range;
        previousRangeAfterBuff = rangeAfterBuff;
        Attack(false);
    }
void OnEnable()
{
    lastTargetPosition = new Vector3(transform.position.x,transform.position.y,transform.position.z + 10f);
    upgrade = GetComponentInChildren<Upgrade>();
    range = upgrade.BaseRange;
    rangeAfterBuff = range;
    previousRangeAfterBuff = rangeAfterBuff;
}

    void Update()
    {   
        CheckForBuffs();
        UpdateList();
    }

void OnTriggerEnter(Collider other) 
{   
    numberOfEnemies.Add(other.gameObject);    
}
void OnTriggerExit(Collider other)
{
    numberOfEnemies.Remove(other.gameObject);
}

void UpdateList()
{   
    for(int i = 0;i< numberOfEnemies.Count;i++)
    {
        if(!numberOfEnemies[i].activeSelf)
        {
            numberOfEnemies.Remove(numberOfEnemies[i]);
        }
    }
   
    if(numberOfEnemies.Count != 0)
        {   boolEnemyInRange = true;
            closestEnemy = numberOfEnemies.OrderBy(enemy => Vector3.Distance(transform.position, enemy.transform.position)).FirstOrDefault();
            if(target == null)
                {
                    target = closestEnemy;
                    StartCoroutine(AimWeapon());
                 }
            if(closestEnemy == target)
            {   if(target.activeSelf)
                {
                    lastTargetPosition = target.transform.position;
                }
            }
            else if(target != closestEnemy)
                {
                    KeepTrackOfTarget();
                }
        }
    else {  boolEnemyInRange = false;
            target = null;
            Attack(false);
            weapon.LookAt(lastTargetPosition);
            }
}

     private void KeepTrackOfTarget()
    {                    
            if(!coroutineIsWaitingPlaying)
            {   if(!target.activeSelf)
                {StartCoroutine(HoldTarget());}
                else {StartCoroutine(HoldTarget());}
            }
            else if(coroutineIsWaitingPlaying && !target.activeSelf)
            {          
                StartCoroutine(HoldTarget());
            }              
    }

 IEnumerator AimWeapon()
    {   
        Attack(true);   
        while(boolEnemyInRange)
            {       if(!coroutineIsLerping)
                    {
                        weapon.LookAt(target.transform);
                    }
                    
                    yield return null; 
            }
            
           
    }
   
    void Attack(bool isActive)
    {
        var emissionModule = particleSystemA.emission;
        emissionModule.enabled = isActive;
        imShooting = isActive;
        if(!imRotating)
        {StartCoroutine(RotateGatling());}
    }

   IEnumerator RotateGatling()
    {   imRotating = true;
        while (imShooting)
        {
            rotateMe.transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime);
            yield return null;        
        }
        imRotating = false;

    }

    void CheckForBuffs()
    {   
        capsuleCollider.radius = rangeAfterBuff;
        capsuleCollider.height = capsuleCollider.radius*2 + 10;
        range = upgrade.BaseRange;
        if (towerObjectPool.CurrentRangeBuffs.Count == 0)
        {
            rangeAfterBuff = range;
            upgrade.UpdateRangeAfterBuff(rangeAfterBuff);
            return;
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
                if(previousRangeAfterBuff != rangeAfterBuff)
                {
                    previousRangeAfterBuff = rangeAfterBuff;
                    upgrade.UpdateRangeAfterBuff(rangeAfterBuff);
                }
            }
    }

   void ChangeTargetRotation(Vector3 lastTarget, Transform currentTarget)
    {
        Vector3 directionToTarget = currentTarget.transform.position - transform.position;
        targetRotation = Quaternion.LookRotation(directionToTarget);
        
        Vector3 directionToLastTarget = lastTarget - transform.position;
        lastTargetRotation = Quaternion.LookRotation(directionToLastTarget);

           if(!coroutineIsLerping)
                {
                    StartCoroutine(LerpBetweenTargets(lastTargetRotation,targetRotation,1,true));
                }
                else{StartCoroutine(LerpBetweenTargets(currentRotation, targetRotation,2,false));} 
    }
     IEnumerator LerpBetweenTargets(Quaternion initialTargetRotation, Quaternion targetRotation, int targetChangeSPeedBuff, bool brakWhileLoop)
    {   
        coroutineIsLerping = true;
        
        Attack(false);
     
        float travelPercent = Mathf.Clamp01(0f);
        while(travelPercent < 1)
        {
            if(brakWhileLoop){travelPercent = 0;brakWhileLoop = false;}
        weapon.transform.rotation = Quaternion.Lerp(initialTargetRotation,targetRotation,travelPercent);
        currentRotation = weapon.transform.rotation;
        travelPercent += Time.deltaTime*targetChangeSpeed*targetChangeSPeedBuff;
        yield return new WaitForEndOfFrame();
        }
        if(travelPercent >= 1f)                    
        {coroutineIsLerping = false;}
        
    }
        IEnumerator HoldTarget()
    {   
        coroutineIsWaitingPlaying = true;
        yield return null;
        target = closestEnemy;
        ChangeTargetRotation(lastTargetPosition,target.transform);
        StartCoroutine(AimWeapon());
        coroutineIsWaitingPlaying = false;
    }
}
