using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrelFireDamage : MonoBehaviour
{
    [SerializeField] float range = 20;
    [SerializeField]float baseDamage = 8f;
    [SerializeField] float damagePeriod = 0.2f;
    float appliedDamage = 0;
    float timer = 0;
    List<GameObject> numberOfEnemies = new();
    CapsuleCollider capsuleCollider;
    void Start()
    {
        capsuleCollider = GetComponent<CapsuleCollider>();
    }

    void Update()
    {   DetermineDamageChunk();
        UpdateList();       
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
        timer += Time.deltaTime;

        if(timer >= damagePeriod)
        {
            for(int i =0; i < numberOfEnemies.Count; i++)
            {   
                float distance = Vector3.Distance(transform.position, numberOfEnemies[i].transform.position);
                EnemyHealth enemyHealth = numberOfEnemies[i].GetComponent<EnemyHealth>();
                if(distance < range)
                {       
                    enemyHealth.ApplyDamage(appliedDamage);                    
                }
            
            }
            timer = 0;
        }
    }
}

void DetermineDamageChunk()
{
    float elapsedTime = 0;
    float chunkDamage = 0;

    while (elapsedTime < damagePeriod)
    {   
        chunkDamage += Time.deltaTime * baseDamage;
        elapsedTime += Time.deltaTime;
    }
    appliedDamage = chunkDamage;
}

}
