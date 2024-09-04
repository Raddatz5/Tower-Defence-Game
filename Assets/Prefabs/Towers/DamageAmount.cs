using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Numerics;
using Vector3 = UnityEngine.Vector3;
using Unity.VisualScripting;

public class DamageAmount : MonoBehaviour
{
    [SerializeField] float baseDamage;
    EnemyHealth enemyHealth;

    TowerObjectPool towerObjectPool;
    Upgrade upgrade;

    Arrow arrow;

void Awake()
{
   towerObjectPool = FindObjectOfType<TowerObjectPool>();
   upgrade = GetComponentInParent<Upgrade>();
   baseDamage = upgrade.BaseDamage;
   arrow = GetComponent<Arrow>();
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
        //TO DO: spawn bolt particle effect
        arrow.ReparentAndDisable();
   }
}
