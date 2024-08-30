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
     List<Node> path = new List<Node>();
     public List<Node> Path { get { return path; } }
    [Tooltip("Lower means faster")]
    [SerializeField] float startSpeed = 1.25f;
    [SerializeField] float speed = 1f;
    Enemy enemy;
    List<GameObject> colliderCounts;
    public bool fromOther;
    Vector3 tempPosition;
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
    PathFinder pathFinder;
    GridManager gridManager;
    float distanceBetweenPoints = 0f;
    List<Vector3> last4Positions = new();
    public List<Vector3> Last4Positions { get { return last4Positions;}}

    
     private void Awake()
    {
        enemy = GetComponent<Enemy>();
        colliderCounts = new();
        fromOther = false;
        rb = GetComponent<Rigidbody>();
        pathFinder = FindObjectOfType<PathFinder>();
        gridManager = FindObjectOfType<GridManager>();
    }

    void OnEnable()
    {   last4Positions.Clear();
        castleHealth = FindAnyObjectByType<CastleHealth>();
        enemyHealth = GetComponent<EnemyHealth>();
        colliderCounts.Clear();
        speed = 1f;
        FindStart();
       }

    void Update()
    {
        UpdateList();
    }
void RecalculatePath(bool resetPath)
    {   if(gridManager.GetCoordinatesFromPosition(transform.position) != pathFinder.DestinationCoordinates)
    {   
        Vector2Int coordinates = new Vector2Int();
        int startSpot = 1;
           
        if (resetPath)
        {
            coordinates = pathFinder.StartCoordinates;
        }
        else 
        {    // Checks if the node the enemy is heading to is where the tower just got placed and if so to go on a diagonal around it. If not just continue on. 
            if(pathFinder.CoordinatesNewTower == path[currentWaypoint].coordinates)
            {
                    coordinates = gridManager.GetCoordinatesFromPosition(transform.position);
                    startSpot = 2; 
                    // Debug.Log("Thats my spot");
            }
            else
            {   if(fromOther)
                {
                    coordinates = gridManager.GetCoordinatesFromPosition(transform.position);
                }
                else
                {
                    coordinates = path[currentWaypoint].coordinates;
                    startSpot = 0;
                }
            }
            fromOther = false;         
        }
        StopAllCoroutines();
        path.Clear();
        path = pathFinder.GetNewPath(coordinates);
        StartCoroutine(FollowPath(startSpot));
    }
    }
    
 void FindStart()
 {
    if(!fromOther)
    {   
        ReturnToStart();
        RecalculatePath(true);
    }
    else
    {   
        transform.position = tempPosition;
        RecalculatePath(false);
    }
 }

 public void TempPosition(Vector3 spawnLocation)
 {  
    fromOther = true;
    tempPosition = spawnLocation;
 }

  void ReturnToStart()
    {
        transform.position = gridManager.GetPostitionFromCoordinates(pathFinder.StartCoordinates);
    }

    IEnumerator FollowPath(int start)
    {   
        for (int i = start; i < path.Count; i++)
        {
            Vector3 startPosition = transform.position;
            Vector3 endPosition = gridManager.GetPostitionFromCoordinates(path[i].coordinates);
            targetPosition = endPosition;
            currentWaypoint = i;
            distanceBetweenPoints = Vector3.Distance(startPosition, endPosition);

            // add the next postiion to the list to track where its been, for mini spawns
            if(!last4Positions.Contains(gridManager.GetPostitionFromCoordinates(path[i].coordinates)))
            {
                last4Positions.Add(gridManager.GetPostitionFromCoordinates(path[i].coordinates));
            }
            if(last4Positions.Count > 3)
            {
                last4Positions.RemoveAt(0);
            }

            float travelPercent = 0;
            float elapsedTime = 0;

            transform.LookAt(endPosition);

            while (travelPercent < 1)
            {   
                travelPercent = elapsedTime/startSpeed/distanceBetweenPoints*10.10f;
                travelPercentOnDestroy = travelPercent;
                rb.MovePosition(Vector3.Lerp(startPosition, endPosition, travelPercent));
                yield return new WaitForEndOfFrame();
                elapsedTime += Time.deltaTime*speed;                               
            }
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

//     int CheckNextTwoTiles()
//     {
//         Vector3 target1 = (gridManager.GetPostitionFromCoordinates(path[1].coordinates) - transform.position).normalized;
//         Vector3 target2 = (gridManager.GetPostitionFromCoordinates(path[2].coordinates) - transform.position).normalized;
//         float angle1 = Vector3.Angle(target1, transform.forward);
//         float angle2 = Vector3.Angle(target2, transform.forward);

//         if(angle1 < angle2)
//         {
//             return 2;
//         }
//         else
//         {
//             return 1;
//         }
//     }
}
