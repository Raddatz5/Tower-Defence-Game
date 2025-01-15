using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageLine : MonoBehaviour
{
   float baseDamage;
    EnemyHealth enemyHealth;
    Upgrade upgrade;

void Awake()
{
   upgrade = GetComponentInParent<Upgrade>();
   baseDamage = upgrade.BaseDamage;
}
void OnEnable()
{
            transform.SetPositionAndRotation(transform.parent.position, Quaternion.identity);
}

   void OnTriggerEnter(Collider other) 
   {    
        if (other.CompareTag("Enemy"))
        {   baseDamage = upgrade.CurrentDamage;
            enemyHealth = other.GetComponent<EnemyHealth>();
            if(enemyHealth.CurrentEnemyHealth != 0 && other != null)
               {
                  if(baseDamage > enemyHealth.CurrentEnemyHealth)
                  {
                     float appliedDamage = enemyHealth.CurrentEnemyHealth;
                     enemyHealth.ApplyDamage(appliedDamage);
                  }
                  else {enemyHealth.ApplyDamage(baseDamage);}              
               }
        }
    }
}
