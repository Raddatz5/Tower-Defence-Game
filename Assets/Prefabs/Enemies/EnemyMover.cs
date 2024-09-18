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
    bool imMoving = false;
    public bool ImMoving {get { return imMoving;}}
    bool imAttacking = false;
    public bool ImAttacking {get { return imAttacking;}}
    public float cornerRadius = 3f;
    private bool roundingCorner = false;
    private Vector3 cornerCenter;
    private float t = 0;
    bool isThereACorner = false;

    
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
    {   t = 0;
        isThereACorner = false;
        roundingCorner = false;
        imMoving = false;
        imAttacking = false; 
        last4Positions.Clear();
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
            imMoving = true;
            imAttacking = false; 
            Vector3 startPosition = transform.position;
            Vector3 endPosition = gridManager.GetPostitionFromCoordinates(path[i].coordinates);
            Vector3 nextNode = gridManager.GetPostitionFromCoordinates(path[i+1].coordinates);
            targetPosition = endPosition;
            currentWaypoint = i;
            distanceBetweenPoints = Vector3.Distance(startPosition, endPosition);
            Corners(endPosition, nextNode);

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
            {   if(isThereACorner)
                    {
                        HowCloseToCornerStart(endPosition);
                    }
                    //if not currently rounding corner, follow path
                if(!roundingCorner)
                {
                    travelPercent = elapsedTime/startSpeed/distanceBetweenPoints*10.10f;
                    travelPercentOnDestroy = travelPercent;
                    rb.MovePosition(Vector3.Lerp(startPosition, endPosition, travelPercent));
                    yield return new WaitForEndOfFrame();
                    elapsedTime += Time.deltaTime*speed;
                }    
                else{
                    Debug.Log("I should start rounding the corner");
                    StopAllCoroutines();
                    StartCoroutine(GoAroundCorner(endPosition,gridManager.GetPostitionFromCoordinates(path[i+1].coordinates)));
                    break;
                    }                          
        }
        }
        
        // StartCoroutine(DamageCastle());
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
        {   imMoving = false;
            imAttacking = false; 
           
            yield return new WaitForSeconds(castleHealth.CastleDamageDelay);
            imAttacking = true;
        }
        
    }

    IEnumerator KnightFight()
    {   
        while(knightTarget.activeSelf)
        {   imMoving = false;
            imAttacking = false; 

            knightTarget.GetComponent<KnightMover>().Stop();
            knightTarget.GetComponent<KnightMover>().WhoIsHittingMe(enemyHealth);
            float elapsedTime = 0;

            while(elapsedTime < attackSpeed)
            {                   
                yield return new WaitForEndOfFrame();
                elapsedTime += Time.deltaTime;
            }
            imAttacking = true;
                        

        }
        knightTarget = null;
        UpdateSpeed();
    }

    public void AttackTrigger()
    {
        if (knightTarget != null)
        {
            knightTarget.GetComponent<KnightMover>().TakeHealth(attackDamage);
        }
        else 
        {
            castleHealth.DamageCastle(enemy.RamDamage);
            enemyHealth.ApplyDamage(castleHealth.CastleDamage);
        }
    }
    IEnumerator SlowDownToStop()
    {
        yield return new WaitForSeconds(0.25f);
        speed = 0f;
        imMoving = false;
    }

    // IEnumerator TurnCorner(Vector3 directionOfTurn)
    // {
    //     Vector3 startDirection = transform.forward;
    //     Vector3 endDirection = directionOfTurn;
        
    //     float travelPercent = 0;
    //     float elapsedTime = 0;

    //     transform.LookAt(endPosition);

    //     while (travelPercent < 1)
    //     {   
    //         travelPercent = elapsedTime/startSpeed/distanceBetweenPoints*10.10f;
    //         travelPercentOnDestroy = travelPercent;
    //         rb.MovePosition(Vector3.Lerp(startPosition, endPosition, travelPercent));
    //         yield return new WaitForEndOfFrame();
    //         elapsedTime += Time.deltaTime*speed;                               
    //     }

    // The points that make up the path
    // public List<Vector3> path = new List<Vector3>();

    // Radius of the corner rounding
 

    private void Corners(Vector3 closestNode, Vector3 nextNode)
    {   // Get the current segment start and end
        Vector3 startPoint = closestNode;
        Vector3 endPoint = nextNode;

        // Calculate direction between the points
        Vector3 direction = (endPoint - startPoint).normalized;
        Debug.Log("Angle of next two waypoints " + Vector3.Angle(transform.forward,direction));
        if(Vector3.Angle(transform.forward,direction) > 5f)
        {
            isThereACorner = true;
            CalculateCorner(startPoint, endPoint);
            Debug.Log("The center of the corner is " + cornerCenter);
            // Debug.Log("Distance to next end point " + Vector3.Distance(transform.position, startPoint));
        }
        else{isThereACorner = false;}
    }
    void HowCloseToCornerStart(Vector3 closestNode)
    {
        // // Check if we're approaching the end of the segment and should round the corner
        if (!roundingCorner && Vector3.Distance(transform.position, closestNode) < cornerRadius && isThereACorner)
        {
            roundingCorner = true;           
        }
    }
    IEnumerator GoAroundCorner(Vector3 first, Vector3 second)
    {    
        t = 0;
        while(roundingCorner)
        {
        //     // Interpolate along the rounded corner
                t += Time.deltaTime * speed / cornerRadius;
                transform.position = GetRoundedCornerPosition(t);
        //     // Rotate to face the next target point
        //     Vector3 lookTarget = roundingCorner ? GetRoundedCornerPosition(t + 0.01f) : gridManager.GetPostitionFromCoordinates(path[currentWaypoint+1].coordinates);
        //     transform.LookAt(lookTarget);
        // Vector3 previousDirection = (first - transform.position).normalized;
        // Vector3 nextDirection = (second - transform.position).normalized;
        // Vector3 direction = Vector3.Lerp(previousDirection,nextDirection,t);
        // Vector3 newPostiion = transform.position + speed * Time.fixedDeltaTime * direction;
        // transform.LookAt(transform.position + direction);
        // rb.MovePosition(newPostiion);

        //     // Once we've finished rounding, move to the next segment
            if (t >= Mathf.PI / 2f)
            {   Debug.Log("DONE rounding the corner");
                roundingCorner = false;
                t = 0;
                RecalculatePath(false);
                break;
            }
            yield return new WaitForEndOfFrame();
        }
        // else
        // {
        // //     // Move along the straight line
        //     transform.position = Vector3.MoveTowards(transform.position, endPoint, speed * Time.deltaTime);
        // }
    }


    private void CalculateCorner(Vector3 startPoint, Vector3 endPoint)
    {
        // Calculate the direction to move out from the corner (bisect the angle)
        Vector3 previousDirection = (transform.position - startPoint).normalized;
        Vector3 nextDirection = (endPoint - startPoint).normalized;

        Vector3 bisector = (previousDirection + nextDirection).normalized;
        float angleBisectorToNext = Vector3.Angle(bisector,nextDirection)* Mathf.Deg2Rad;
        float hypotenous = cornerRadius/Mathf.Sin(angleBisectorToNext);
        cornerCenter = startPoint;
        // determine quadrant based off bisectors x and z
        int bisectorQuadrant;
        if(bisector.x > 0 && bisector.z > 0) { bisectorQuadrant = 1;}
        else if (bisector.x < 0 && bisector.z > 0) { bisectorQuadrant = 2;}
        else if (bisector.x < 0 && bisector.z < 0) { bisectorQuadrant = 3;}
        else    { bisectorQuadrant = 4;}

        //find x and y component of corner center
        switch(bisectorQuadrant)
        {
            case 1: cornerCenter.x += Mathf.Sin(angleBisectorToNext)*hypotenous;
                    cornerCenter.z += Mathf.Cos(angleBisectorToNext)*hypotenous; break;

            case 2: cornerCenter.x -= Mathf.Sin(angleBisectorToNext)*hypotenous;
                    cornerCenter.z += Mathf.Cos(angleBisectorToNext)*hypotenous; break;

            case 3: cornerCenter.x -= Mathf.Sin(angleBisectorToNext)*hypotenous;
                    cornerCenter.z -= Mathf.Cos(angleBisectorToNext)*hypotenous; break;    

            case 4: cornerCenter.x += Mathf.Sin(angleBisectorToNext)*hypotenous;
                    cornerCenter.z -= Mathf.Cos(angleBisectorToNext)*hypotenous; break;    
        }

        // Debug.Log("The startpoint is "+startPoint + " and the hypotenous is " +hypotenous+" and the angle is " + angleBisectorToNext+
        //            " and the bisector thing is " + bisector);
        
    }

    private Vector3 GetRoundedCornerPosition(float angle)
    {
        // Calculate the position on the circular corner using parametric equations of a circle
        float x = Mathf.Cos(angle) * cornerRadius;
        float z = Mathf.Sin(angle) * cornerRadius;

        return cornerCenter + new Vector3(x, 0, z); // Assumes a 2D XZ plane
    }

    private void OnDrawGizmos()
    {
        // Draw the path and corner radius for debugging
        Gizmos.color = Color.green;

        for (int i = 0; i < path.Count - 1; i++)
        {
            Gizmos.DrawLine(gridManager.GetPostitionFromCoordinates(path[i].coordinates), gridManager.GetPostitionFromCoordinates(path[i+1].coordinates));
        }

        // Gizmos.color = Color.red;
        // Gizmos.DrawWireSphere(transform.position, cornerRadius);
    }
}