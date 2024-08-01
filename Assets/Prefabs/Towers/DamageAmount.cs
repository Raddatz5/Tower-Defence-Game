using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Numerics;
using Vector3 = UnityEngine.Vector3;

public class DamageAmount : MonoBehaviour
{
    [SerializeField] float baseDamage;
    EnemyHealth enemyHealth;

    TowerObjectPool towerObjectPool;
    Upgrade upgrade;

void Start()
{
   towerObjectPool = FindObjectOfType<TowerObjectPool>();
   upgrade = GetComponentInParent<Upgrade>();
   baseDamage = upgrade.BaseDamage;
}

   void OnParticleCollision(GameObject other) 
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
