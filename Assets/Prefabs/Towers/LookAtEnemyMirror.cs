using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MoreMountains.Feedbacks;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class LookatEnemyMirror : MonoBehaviour
{
    [SerializeField] Transform weapon;
    [SerializeField] GameObject target = null;
    public GameObject Target {get { return target;}}
    GameObject highestHealthEnemy = null;
    [SerializeField] float range = 20f;
    ParticleSystem particleSystemA;
    MirrorDamage mirrorDamage;
    float baseDamage;
    List<GameObject> rangeBuffsInRange = new();
    float rangeOfBuff;
    [SerializeField] float rangeAfterBuff;
    public float RangeAfterBuff { get { return rangeAfterBuff; } }
    float rangeModFromBuff;
[Tooltip("Time period that chunks the damage together")]
    [SerializeField] float damagPeriod = 0.2f;
    float elapsedTime = 0;
    [SerializeField] float chunkDamage = 0;
    List<GameObject> numberOfEnemies = new();
    List<GameObject> enemiesToRemove = new();
    [SerializeField] float targetChangeSpeed = 3f;
    bool coroutineIsLerping = false;
    Vector3 lastTargetPosition;
    Quaternion currentRotation;
    Quaternion lastTargetRotation;
    Quaternion targetRotation;
    TowerObjectPool towerObjectPool;
    CapsuleCollider capsuleCollider;
    bool boolEnemyInRange = false;
    Upgrade upgrade;
    float previousRangeAfterBuff;
    public LayerMask obstacleLayer;
    EnemyHealth enemyHealth;

    void Start()
    {
        particleSystemA = GetComponentInChildren<ParticleSystem>();
        mirrorDamage = GetComponentInChildren<MirrorDamage>();
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
    numberOfEnemies.Clear();
    enemiesToRemove.Clear();
}


    void Update()
    {
        CheckForRangeBuffs();
        RemoveInactiveEnemies();
        UpdateTarget();
    }

void OnTriggerEnter(Collider other) 
{   
    if(other.CompareTag("Enemy")){numberOfEnemies.Add(other.gameObject);}
}
void OnTriggerExit(Collider other)
{ 
    if(other.CompareTag("Enemy")){numberOfEnemies.Remove(other.gameObject);}
}

    void UpdateTarget()
    {
        if (numberOfEnemies.Count != 0)
        {   boolEnemyInRange = true;
        
            if (target == null)
            {   highestHealthEnemy = numberOfEnemies.OrderBy(enemy => enemy.GetComponent<EnemyHealth>().CurrentEnemyHealth).First();
                target = highestHealthEnemy;
                enemyHealth = target.GetComponent<EnemyHealth>();
                ChangeTargetRotation(lastTargetPosition, target.transform);
                
            }
            else if (target.activeSelf)
            {   
                if (!numberOfEnemies.Contains(target))
                {
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
        }

        else
        {   boolEnemyInRange = false;
            target = null;
            weapon.LookAt(lastTargetPosition+ new Vector3(0, 3f, 0));
            Attack(false);
        }
    }

    void Attack(bool isActive)
    {   
        var emissionModule = particleSystemA.emission;
        emissionModule.enabled = isActive;
       
        if(isActive && boolEnemyInRange)
        {    
            Vector3 direction = (target.transform.position + new Vector3(0,2.75f,0) - (transform.position + new Vector3(0,4f,0))).normalized;
            float distance = Vector3.Distance(target.transform.position,transform.position);
            RaycastHit hit;
            // Start ramping up damage regardless if hitting target
            baseDamage = mirrorDamage.BaseMirrorDamage;
            if (!mirrorDamage.ImRampingUp && baseDamage < mirrorDamage.MaxDamageCap)
            {
                StartCoroutine(mirrorDamage.RampUpDamage());
            }

            //Hitting an obstacle
            if(Physics.Raycast(transform.position + new Vector3(0,4f,0), direction, out hit, distance, obstacleLayer))
            {   
                var mainModule = particleSystemA.main;
                float distance2 = Vector3.Distance(transform.position,hit.point);
                mainModule.startLifetime = distance2/mainModule.startSpeed.constant;
                particleSystemA.transform.LookAt(hit.point);
            }
            //Its hitting the target
            else
            {   
                DamageTarget();
                var mainModule = particleSystemA.main;
                mainModule.startLifetime = distance/mainModule.startSpeed.constant;
                particleSystemA.transform.LookAt(target.transform.position + new Vector3(0,2.75f,0));
            }
        }
    }
    void DamageTarget()
    {   
        if (elapsedTime < damagPeriod)
        {
            chunkDamage += baseDamage * Time.deltaTime;
            elapsedTime += Time.deltaTime;
        }

        else
        {   
            if (chunkDamage >= enemyHealth.CurrentEnemyHealth)
            {
                chunkDamage = enemyHealth.CurrentEnemyHealth;
                enemyHealth.ApplyDamage(chunkDamage);
            }
            else
            {
                enemyHealth.ApplyDamage(chunkDamage);
            }

            elapsedTime = 0;
            chunkDamage = 0;
        }
    }
    void ChangeTargetRotation(Vector3 lastTarget, Transform currentTarget)
    {   
        mirrorDamage.ResetDamage();
        Attack(false);
        Vector3 directionToTarget = currentTarget.transform.position - transform.position;
        targetRotation = Quaternion.LookRotation(directionToTarget);

        Vector3 directionToLastTarget = lastTarget - transform.position;
        lastTargetRotation = Quaternion.LookRotation(directionToLastTarget);

        if (!coroutineIsLerping)
        {
            StartCoroutine(LerpBetweenTargets(lastTargetRotation, targetRotation, 1, true));
        }
        else { StartCoroutine(LerpBetweenTargets(currentRotation, targetRotation, 2, false)); }
   
    }
    IEnumerator LerpBetweenTargets(Quaternion initialTargetRotation, Quaternion targetRotation, int targetChangeSPeedBuff, bool brakWhileLoop)
    {
        coroutineIsLerping = true;
        float travelPercent = Mathf.Clamp01(0f);
        while (travelPercent < 1)
        {
            if (brakWhileLoop) { travelPercent = 0; brakWhileLoop = false; }
            weapon.transform.rotation = Quaternion.Lerp(initialTargetRotation, targetRotation, travelPercent);
            currentRotation = weapon.transform.rotation;
            travelPercent += Time.deltaTime * targetChangeSpeed * targetChangeSPeedBuff;
            yield return new WaitForEndOfFrame();
        }
        if (travelPercent >= 1f)
        { coroutineIsLerping = false; }
    }

      void CheckForRangeBuffs()
    {   
        rangeBuffsInRange.Clear();
        capsuleCollider.radius = rangeAfterBuff;
        capsuleCollider.height = capsuleCollider.radius * 2 + 30;
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
                        rangeBuffsInRange.Add(spotter);
                        rangeAfterBuff = rangeAfterBuff + rangeModFromBuff * range;
                    }
                }
            }
        if(previousRangeAfterBuff != rangeAfterBuff)
        {   
            previousRangeAfterBuff = rangeAfterBuff;
            upgrade.UpdateRangeAfterBuff(rangeAfterBuff);
        }
        mirrorDamage.UpdateRangeBuffsInRangeAmount(rangeBuffsInRange.Count);
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
}
