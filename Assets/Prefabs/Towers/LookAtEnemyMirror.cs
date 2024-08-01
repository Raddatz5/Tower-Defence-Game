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
    GameObject closestEnemy = null;
    [SerializeField] float range = 20f;
    ParticleSystem[] particleSystemA;
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
    [SerializeField] float targetChangeSpeed = 3f;
    bool coroutineIsLerping = false;
    Vector3 lastTargetPosition;
    Quaternion currentRotation;
    Quaternion lastTargetRotation;
    Quaternion targetRotation;
    TowerObjectPool towerObjectPool;
    CapsuleCollider capsuleCollider;
    bool attack = false;
    bool changingTarget = false;
    Upgrade upgrade;
    float previousRangeAfterBuff;
    GameObject previousTarget;

    void Start()
    {
        particleSystemA = GetComponentsInChildren<ParticleSystem>();
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
}


    void Update()
    {
        CheckForRangeBuffs();
        UpdateList();
        UpdateTarget();
    }

    void OnTriggerEnter(Collider other)
    {
        numberOfEnemies.Add(other.gameObject);
    }
    void OnTriggerExit(Collider other)
    {
        numberOfEnemies.Remove(other.gameObject);
    }

    private void UpdateList()
    {
        for (int i = 0; i < numberOfEnemies.Count; i++)
        {
            if (!numberOfEnemies[i].activeSelf)
            {
                numberOfEnemies.Remove(numberOfEnemies[i]);
            }
        }
    }
    void UpdateTarget()
    {
        if (numberOfEnemies.Count != 0)
        {
            closestEnemy = numberOfEnemies.OrderBy(enemy => enemy.GetComponent<EnemyHealth>().CurrentEnemyHealth).Last();
            if (target == null)
            {
                target = closestEnemy;
                ChangeTargetLayer(target);
                if (!coroutineIsLerping && !changingTarget)
                {
                    StartCoroutine(ChangeTargetRotation(lastTargetPosition, target.transform));
                }
            }
            else {
                lastTargetPosition = target.GetComponent<EnemyMover>().TargetPosition;
                previousTarget = target;
                AimWeapon();}
        }

        else
        {
            target = null;
            attack = false;
            weapon.LookAt(lastTargetPosition);
            Attack(false);
        }
    }

void ChangeTargetLayer(GameObject target)
{
    target.layer = 8;
    if(previousTarget != null)
    {
        previousTarget.layer = 6;
    }
}
    void AimWeapon()
    {
        if (attack)
        {
            float targetHealth = target.GetComponent<EnemyHealth>().CurrentEnemyHealth;
            baseDamage = mirrorDamage.BaseMirrorDamage;
            float distance = Vector3.Distance(target.transform.position, transform.position);
            if (targetHealth > 0 && distance <= rangeAfterBuff+2f && target != null)
            {
                weapon.LookAt(target.transform);
                Attack(true);
                EnemyHealth enemyHealth = target.GetComponent<EnemyHealth>();
                if (!mirrorDamage.ImRampingUp && baseDamage < mirrorDamage.MaxDamageCap)
                {
                    StartCoroutine(mirrorDamage.RampUpDamage());
                }

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
            else
            {
                weapon.LookAt(lastTargetPosition);
                Attack(false);
                attack = false;
                target = null;
                mirrorDamage.ResetDamage();
                chunkDamage = 0;
            }
        }
    }


    void Attack(bool isActive)
    {
        foreach (ParticleSystem partSys in particleSystemA)
        {
            var emissionModule = partSys.emission;
            emissionModule.enabled = isActive;
        }
    }
  
    IEnumerator ChangeTargetRotation(Vector3 lastTarget, Transform currentTarget)
    {   
        mirrorDamage.ResetDamage();
        changingTarget = true;
        Attack(false);
        attack = false;
        Vector3 directionToTarget = currentTarget.transform.position - transform.position;
        targetRotation = Quaternion.LookRotation(directionToTarget);

        Vector3 directionToLastTarget = lastTarget - transform.position;
        lastTargetRotation = Quaternion.LookRotation(directionToLastTarget);

        if (!coroutineIsLerping)
        {
            StartCoroutine(LerpBetweenTargets(lastTargetRotation, targetRotation, 1, true));
        }
        else { StartCoroutine(LerpBetweenTargets(currentRotation, targetRotation, 2, false)); }
        yield return null;
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
        changingTarget = false;
        attack = true;

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
}
