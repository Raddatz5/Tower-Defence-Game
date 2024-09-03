using System;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using UnityEngine;
using MoreMountains.Tools;
using Unity.VisualScripting;
using UnityEngine.Animations;


[RequireComponent(typeof(Enemy))]
public class EnemyHealth : MonoBehaviour
{
[SerializeField] float maxEnemyHealth = 100f;
[SerializeField] int enemyBigNumber;
public int EnemyBigNumber { get { return enemyBigNumber; } }
[SerializeField] float currentEnemyHealth;
public float CurrentEnemyHealth{get{return currentEnemyHealth;}}
protected MMHealthBar _targetHealthBar;
Enemy enemy;
EnemyMover enemyMover;
ObjectPoolBig2 objectPoolBig2;
[SerializeField] float healthBarVerticalOffset = 10f;
ExplosionPool explosionPool;
List<Vector3> last4Positions = new();
GridManager gridManager;

      
    void OnEnable()
    {
        currentEnemyHealth = maxEnemyHealth;
        _targetHealthBar = this.gameObject.GetComponent<MMHealthBar>();
        _targetHealthBar.HealthBarOffset = new Vector3(0f, healthBarVerticalOffset,UnityEngine.Random.Range(-0.5f,0.5f));
        UpdateHealth();
    }
void Awake()
{
    gridManager = FindObjectOfType<GridManager>();
}
void Start() 
{
    enemy = GetComponent<Enemy>();
    enemyMover = GetComponent<EnemyMover>();
    objectPoolBig2 = FindAnyObjectByType<ObjectPoolBig2>();
    explosionPool = FindAnyObjectByType<ExplosionPool>();
}
 public void ApplyDamage(float baseDamage1)
    {   
        float damageMe = Mathf.Abs(baseDamage1);
        currentEnemyHealth -= damageMe;
        UpdateHealth();
        if (currentEnemyHealth <= 0 && this != null)
        {
            DestroyEnemy();
        }
    }

void DestroyEnemy()
    {   
        enemy.RewardGold();
        explosionPool?.SpawnExplosion(enemyBigNumber,transform.position);
    if(enemyBigNumber != 0 && enemyBigNumber < 6)
    {   last4Positions = enemyMover.Last4Positions;
            string list = string.Join(", ", last4Positions.ToArray());
            Debug.Log($"{transform.position}, {enemyMover.TravelPercentOnDestroy},{list}");
        objectPoolBig2.SpawnEnemyMini(enemyBigNumber, last4Positions, enemyMover.TravelPercentOnDestroy);  
    }
    gameObject.SetActive(false);
    transform.position = transform.parent.position;
     }

    public virtual void UpdateHealth()
    {
        if (_targetHealthBar != null)
        {
            _targetHealthBar.UpdateBar(currentEnemyHealth, 0f,maxEnemyHealth, true);    
        }
    }
}
