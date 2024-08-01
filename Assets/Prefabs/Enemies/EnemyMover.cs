using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Unity.Properties;
using Unity.VisualScripting;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;
using UnityEngine.AI;

[RequireComponent(typeof(Enemy))]
public class EnemyMover : MonoBehaviour
{
    [SerializeField] List<Waypoint> path = new List<Waypoint>();
    [Tooltip("Lower means faster")]
    [SerializeField] float startSpeed = 1.25f;
    [SerializeField] float speed = 1f;
    Enemy enemy;
    List<GameObject> colliderCounts;
    public bool fromOther;
    [SerializeField] int tempPosition;
    float tempTravelPercent = 0f;
    int currentWaypoint;
    public int CurrentWaypoint { get { return currentWaypoint; } }
    float travelPercentOnDestroy;
    public float TravelPercentOnDestroy { get { return travelPercentOnDestroy;}}
    CastleHealth castleHealth;
    EnemyHealth enemyHealth;
    Rigidbody rb;
    Vector3 targetPosition;
    public Vector3 TargetPosition { get { return targetPosition;}}
    GameObject knightTarget = null;
    [SerializeField] float attackSpeed = 1f;
    [SerializeField] float attackDamage = 10f;

    
     private void Awake()
    {
        enemy = GetComponent<Enemy>();
        colliderCounts = new();
        fromOther = false;
        rb = GetComponent<Rigidbody>();
    }

    void OnEnable()
    {   
        castleHealth = FindAnyObjectByType<CastleHealth>();
        enemyHealth = GetComponent<EnemyHealth>();
        colliderCounts.Clear();
        speed = 1f;
        FindPath();
        FindStart();
        StartCoroutine(FollowPath(tempPosition));
       }

    void Update()
    {
        UpdateList();
    }

      void FindPath()
    {
        path.Clear();
        GameObject parent = GameObject.FindGameObjectWithTag("Path");
        
        foreach (Transform child in parent.transform)
        {
            path.Add(child.GetComponent<Waypoint>());
        }
    }

 void FindStart()
 {
    if(!fromOther)
    {   
        tempPosition = 1;
        tempTravelPercent = 0f;
        ReturnToStart();
    }
    else
    {   
        FollowPath(tempPosition);
        fromOther = false;
    }
 }

 public void TempPosition(int destroyPositionWaypoint, float travelPercentDestroyed)
 {  
    fromOther = true;
    tempPosition = destroyPositionWaypoint;
    tempTravelPercent = travelPercentDestroyed;
    int tempStartPosition;

    if(tempPosition-1 < 0) {tempStartPosition = 0;}
    else{tempStartPosition = tempPosition -1;}

    transform.position = Vector3.Lerp(path[tempStartPosition].transform.position, path[tempPosition].transform.position,tempTravelPercent);
 }

    void ReturnToStart()
    {   
        transform.position = path[0].transform.position;
        
    }

    IEnumerator FollowPath(int tempPosition)
    {   
        for (int i = tempPosition; i < path.Count; i++)
        {
            Vector3 startPosition = transform.position;
            Vector3 endPosition = path[i].transform.position;
            targetPosition = endPosition;
            currentWaypoint = i;

            float travelPercent = 0;
            float elapsedTime= tempTravelPercent*startSpeed;

            transform.LookAt(endPosition);

            while (travelPercent < 1)
            {   
                travelPercent = elapsedTime/startSpeed;
                travelPercentOnDestroy = travelPercent;
                rb.MovePosition(Vector3.Lerp(startPosition, endPosition, travelPercent));
                yield return new WaitForEndOfFrame();
                elapsedTime += Time.deltaTime*speed;                               
            }
            tempTravelPercent = 0f;
           
        }
        
        StartCoroutine(DamageCastle());
    }

void OnTriggerEnter(Collider other) 
{   
    if(other.CompareTag("DeBuff"))
    {  
        colliderCounts.Add(other.gameObject);
        UpdateSpeed();
    }
      if(other.CompareTag("Knight") && knightTarget != other)
    {  
        knightTarget = other.gameObject;
        StartCoroutine(SlowDownToStop());
        StartCoroutine(KnightFight());
    }
}

void OnTriggerExit(Collider other) 
{
        if(other.CompareTag("DeBuff"))
    {  
        colliderCounts.Remove(other.gameObject);
        UpdateSpeed();
    }
    if(other.CompareTag("Knight"))
    {  
         UpdateSpeed();
    }
}
void UpdateList()
{   
    if(colliderCounts.Count != 0)
    {
        for(int i = 0;i< colliderCounts.Count;i++)
        {
            if(!colliderCounts[i].activeSelf)
            {
                colliderCounts.Remove(colliderCounts[i]);
            }
        }
        UpdateSpeed();
    }
}

void UpdateSpeed()
{ 
       if (colliderCounts.Count != 0)
        {   
            speed = 1f;  
            for(int i = 0;i< colliderCounts.Count;i++)
            {
                speed = speed*colliderCounts[i].GetComponent<Upgrade>().BaseDamage;
            }
        }
        else { speed = 1f; }
}
  IEnumerator DamageCastle()
    {   
        while(true)
        {
            castleHealth.DamageCastle(enemy.RamDamage);
            enemyHealth.ApplyDamage(castleHealth.CastleDamage);
            yield return new WaitForSeconds(castleHealth.CastleDamageDelay);
        }
        
    }

    IEnumerator KnightFight()
    {   
        while(knightTarget.activeSelf)
        {             
            knightTarget.GetComponent<KnightMover>().Stop();
            knightTarget.GetComponent<KnightMover>().WhoIsHittingMe(enemyHealth);
            float elapsedTime = 0;

            while(elapsedTime < attackSpeed)
            {                   
                yield return new WaitForEndOfFrame();
                elapsedTime += Time.deltaTime;
            }
            knightTarget.GetComponent<KnightMover>().TakeHealth(attackDamage);            

        }
        UpdateSpeed();
    }
    IEnumerator SlowDownToStop()
    {
        yield return new WaitForSeconds(0.25f);
        speed = 0f;
    }
}
