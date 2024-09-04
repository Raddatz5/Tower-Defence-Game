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


public class LookatEnemy : MonoBehaviour
{
    [SerializeField] Transform weapon;
    [SerializeField] GameObject target = null;
    [SerializeField] float attackTimer = 1f;
    public GameObject Target{get{return target;}}
    GameObject closestEnemy = null;
    float range;
    [SerializeField] float rangeAfterBuff;
    public float RangeAfterBuff { get { return rangeAfterBuff; } }
     ParticleSystem[] particleSystemA;
    float rangeOfBuff;
    float rangeModFromBuff;
    List<GameObject> numberOfEnemies = new();
    List<GameObject> enemiesToRemove = new();
    Vector3 lastTargetPosition;
    [SerializeField] float targetChangeSpeed = 3f;
    bool coroutineIsWaitingPlaying = false;
    [SerializeField]bool coroutineIsLerping = false;
    Quaternion currentRotation;
    Quaternion lastTargetRotation;
    Quaternion targetRotation;
    bool coroutineShooting = false;
    ShootingFeedback shootingFeedbackl;
    int activeParticleCount = 0;
    TowerObjectPool towerObjectPool;
    CapsuleCollider capsuleCollider;
    bool boolEnemyInRange = false;
    Upgrade upgrade;
    float previousRangeAfterBuff;
    ArrowManager arrowManager;
    bool isTimerRunning = false;
    float currentTimer;

    void Start()
    {   arrowManager = GetComponentInChildren<ArrowManager>();
        // particleSystemA = GetComponentsInChildren<ParticleSystem>();
        // shootingFeedbackl = GetComponentInChildren<ShootingFeedback>();
        towerObjectPool = FindObjectOfType<TowerObjectPool>();
        capsuleCollider = GetComponent<CapsuleCollider>();
    }

void OnEnable()
{
    lastTargetPosition = new Vector3(transform.position.x,transform.position.y,transform.position.z + 10f);
    upgrade = GetComponent<Upgrade>();
    range = upgrade.BaseRange;
    rangeAfterBuff = range;
    previousRangeAfterBuff = rangeAfterBuff;
    currentTimer = attackTimer;
}
    void Update()
    {   
        UpdateTimer();
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
        RemoveInactiveEnemies();

        if (numberOfEnemies.Count != 0)
        {
            boolEnemyInRange = true;
            if (target == null)
            {
                Debug.Log("Gettingnew target");
                closestEnemy = numberOfEnemies.OrderBy(enemy => Vector3.Distance(transform.position, enemy.transform.position)).FirstOrDefault();
                target = closestEnemy;
                ChangeTargetRotation(lastTargetPosition, target.transform);
                // StartCoroutine(AimWeapon());
            }
            else if (target.activeSelf)
            {
                if (!numberOfEnemies.Contains(target))
                {
                    Debug.Log("I lost the target");
                    target = null;
                }
                else if (!coroutineIsLerping)
                {
                    weapon.LookAt(target.transform.position + new Vector3(0, 3f, 0));
                    lastTargetPosition = target.transform.position;
                    Attack(true);
                }
            }
            else
            {
                target = null;
            }


            // else if(target != closestEnemy)
            //     {
            //         KeepTrackOfTarget();
            //     }
        }
        else
        {
            boolEnemyInRange = false;
            target = null;
            weapon.LookAt(lastTargetPosition, Vector3.up * 3);
            float eulerAngleOffset = -8f;
            weapon.Rotate(Vector3.right, eulerAngleOffset, Space.Self);
            Attack(false);
        }
    }

    private void RemoveInactiveEnemies()
    {
        enemiesToRemove.Clear();
        for (int i = 0; i < numberOfEnemies.Count; i++)
        {
            if (!numberOfEnemies[i].activeSelf)
            {
                enemiesToRemove.Add(numberOfEnemies[i]);
            }
        }
        foreach (var enemy in enemiesToRemove)
        {
            numberOfEnemies.Remove(enemy);
        }
    }

    // private void KeepTrackOfTarget()
    // {                    
    //         if(!coroutineIsWaitingPlaying)
    //         {   if(!target.activeSelf)
    //             {StartCoroutine(HoldTarget());}
    //             else {StartCoroutine(HoldTarget());}
    //         }
    //         else if(coroutineIsWaitingPlaying && !target.activeSelf)
    //         {          
    //             StartCoroutine(HoldTarget());
    //         }              
    // }

    //   IEnumerator AimWeapon()
    //     {      
    //         while(boolEnemyInRange)
    //             {       
    //                 if(!coroutineIsLerping)
    //                     {
    //                         weapon.LookAt(target.transform, Vector3.up*1);
    //                         float eulerAngleOffset = -7f; 
    //                         weapon.Rotate(Vector3.right, eulerAngleOffset, Space.Self);
    //                     }
    //                     Attack(true);
    //                     yield return null; 
    //             }

    //     }


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
    void Attack(bool isActive)
    {   
        if(isActive && boolEnemyInRange && !isTimerRunning && !coroutineIsLerping)
            {   
                isTimerRunning = true;
                arrowManager.Fire(target);
                Debug.Log("Fire! at "+Time.time);
                // else if(CheckParticleCount() == 0 )
                //     {StartCoroutine(Shooting());}
            }
        
        
    }

// IEnumerator Shooting()
// {  
//     coroutineShooting = true;
//     // foreach (ParticleSystem partSys in particleSystemA)
//     // {
//     //     partSys.Play();
//     // }
//     // bool hasPlayedOnce = false;
//     // if(!hasPlayedOnce)
//     // {
//     //     shootingFeedbackl.ShootFeedback();
//     //     hasPlayedOnce = true;
//     // }
//     arrowManager.Fire(target);
//      yield return new WaitForSeconds(1f);
//      Debug.Log("Fire! at "+Time.time);
//      coroutineShooting = false;
           
// }
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
        else { rangeAfterBuff = range;
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
//     IEnumerator HoldTarget()
// {   
//     coroutineIsWaitingPlaying = true;
//     yield return null;
//     target = closestEnemy;
//     ChangeTargetRotation(lastTargetPosition,target.transform);
//     StartCoroutine(AimWeapon());
//     coroutineIsWaitingPlaying = false;
// }
// int CheckParticleCount()
//  {          
//         activeParticleCount = 0;
//         foreach(ParticleSystem ps in particleSystemA)
//         {
//             activeParticleCount += ps.particleCount;
//         }
//         return activeParticleCount;
//  }

 void UpdateTimer()
 {
    if(isTimerRunning)
    {
        currentTimer -= Time.deltaTime;
        if(currentTimer <= 0f)
        {   
            isTimerRunning = false;
            currentTimer = attackTimer;
        }
    }
 }
}

