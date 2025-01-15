using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using MoreMountains.Feedbacks;
using MoreMountains.Feel;
using Unity.Mathematics;
using Unity.VisualScripting;
//using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;
using System.Threading.Tasks;
using Quaternion = UnityEngine.Quaternion;

public class SlamMover : MonoBehaviour
{
    [SerializeField] Transform weapon;
    float speedDown = 7;
    float speedUp = 2;
    float delay = 0.5f;
    [SerializeField] float range = 20;
    float initialRange;
    bool imMoving = false;
    Vector3 startPosition;
    Vector3 endPosition;
    Vector3 bottomPosition;
    bool movingDown = true;
    int stopMe = 0;
    float baseDamage = 8f;
    float rangeOfBuff;
    [SerializeField] float rangeAfterBuff;
    public float RangeAfterBuff{get { return rangeAfterBuff;}}
    float rangeModFromBuff;
    [SerializeField] float defaultBuff = 1f;
    [SerializeField] float damagePeriod = 0.2f;
    float appliedDamage = 0;
    float timer = 0;
    List<GameObject> numberOfEnemies = new();
      CapsuleCollider capsuleCollider;
     TowerObjectPool towerObjectPool;
     [SerializeField] GameObject dustCloud;
     GameObject[] dustCloudPool;
     [Tooltip("Particle system lifetime")]
     [SerializeField] float dustCLoudRadius = 0.5f;
     float previousRangeAfterBuff;
     Upgrade upgrade;
     Animator animator;
     bool isPlaying = false;
 
 void Awake()
 {
    PopulatePool();
 }
    void Start()
    {
        towerObjectPool = FindObjectOfType<TowerObjectPool>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        animator = GetComponentInChildren<Animator>();
        initialRange = range;
        StartCoroutine(MoveSlammer());
        
    }
    void OnEnable()
    {   
        startPosition = weapon.transform.position;
        endPosition = startPosition + Vector3.down*3f;
        bottomPosition = endPosition;
        upgrade = GetComponent<Upgrade>();
        range = upgrade.BaseRange;
        rangeAfterBuff = range;
        previousRangeAfterBuff = rangeAfterBuff;
        numberOfEnemies.Clear();

         if(dustCloudPool != null)
        {
            foreach(GameObject tomato in dustCloudPool)
            {   if(tomato != null)
                {
                 tomato.SetActive(false);
                }
                
            }
        }
    }
    void Update()
    {   
        CheckForDMGBuffs();
        CheckForRangeBuffs();
        UpdateList();          
        if (imMoving)
        {   
            if(!isPlaying)
            {
                animator.SetBool("StartMove", true);
            }
        }
        else {isPlaying = false;}
    }
void OnTriggerEnter(Collider other) 
{   
    if(other.CompareTag("Enemy")){numberOfEnemies.Add(other.gameObject);}    
}
void OnTriggerExit(Collider other)
{
   if(other.CompareTag("Enemy")){numberOfEnemies.Remove(other.gameObject);}
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
    {   
        Attack(true);
        timer += Time.deltaTime;

        if(timer >= damagePeriod)
        {
            for(int i =0; i < numberOfEnemies.Count; i++)
            {   
                float distance = Vector3.Distance(transform.position, numberOfEnemies[i].transform.position);
                EnemyHealth enemyHealth = numberOfEnemies[i].GetComponent<EnemyHealth>();
                if(distance < rangeAfterBuff)
                {       
                    enemyHealth.ApplyDamage(appliedDamage);                    
                }
            
            }
            timer = 0;
        }
    }
    else 
      {
        animator.SetBool("StartMove", false);
        Attack(false);
        isPlaying = false;
      }
}

void DetermineDamageChunk()
{
    float elapsedTime = 0;
    float chunkDamage = 0;

    while (elapsedTime < damagePeriod)
    {   
        chunkDamage += Time.deltaTime * defaultBuff * baseDamage;
        elapsedTime += Time.deltaTime;
    }
    appliedDamage = chunkDamage;
}


    void Attack(bool isActive)
    {
        imMoving = isActive;
    }

    IEnumerator MoveSlammer()
    {   
        while (true)
        {   
            float speed = movingDown ? speedDown : speedUp;
            float travelPercent = 0f;

            while (travelPercent < 1f)
            {
                travelPercent += Time.deltaTime * speed * stopMe;
                weapon.transform.position = Vector3.Lerp(startPosition, endPosition, travelPercent);
               
                yield return null;
            }
            Vector3 temp = startPosition;
            startPosition = endPosition;
            endPosition = temp;
            yield return new WaitForSeconds(delay);

            movingDown = !movingDown;
           
        }
    }
 void CheckForDMGBuffs()
    {
        baseDamage = upgrade.CurrentDamage;
        defaultBuff = upgrade.DefaultBuff;
        DetermineDamageChunk();
    }
    void CheckForRangeBuffs()
    {   
        capsuleCollider.radius = rangeAfterBuff;
        capsuleCollider.height = capsuleCollider.radius * 2 + 30;
        range = upgrade.BaseRange;
        
        if (towerObjectPool.CurrentRangeBuffs.Count == 0)
        {
            rangeAfterBuff = range;
            upgrade.UpdateRangeAfterBuff(rangeAfterBuff);           
        }
        else {   rangeAfterBuff = range;
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
            UpdateDustCLoud();
        }           
    }
  public void StartDustCLoud()
    {
        for(int i = 0; i < dustCloudPool.Length; i++)
        {
            if(!dustCloudPool[i].activeInHierarchy)
            {
                dustCloudPool[i].SetActive(true);
                break;
            }
        }
    }
    public void ChangeBool()
    {
        isPlaying = false;
    }
void UpdateDustCLoud()
{   if (dustCloudPool != null)
{
    foreach(GameObject dust in dustCloudPool)
    {
        ParticleSystem dustPart = dust.GetComponent<ParticleSystem>();
        if (dustPart != null)
        {
        ParticleSystem.MainModule mainModule = dustPart.main;
        dustCLoudRadius = 0.5f*(rangeAfterBuff/initialRange);
        mainModule.startLifetime = dustCLoudRadius;
        }
    }
}
}
void PopulatePool()
{   
    dustCloudPool = new GameObject[8];

    for(int i = 0;i < 5; i++)
    {
        dustCloudPool[i] = Instantiate(dustCloud,transform);
        dustCloudPool[i].SetActive(false);

    }
}

}
