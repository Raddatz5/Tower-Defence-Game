using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Unity.VisualScripting;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class KnightMover : MonoBehaviour
{
    List<Transform> path = new List<Transform>();
    [SerializeField] int starthealth;
    [Tooltip("The number of enemies this knight will hit with one attack")]
    [SerializeField] int numberOfEnemyHits = 1;
    [SerializeField] float currentHealth;
    [SerializeField] float speed;
    float initialSpeed;
    [SerializeField] float damage;
    float initialDamage;
    [Tooltip("Time Between Attacks")]
    [SerializeField] float attackSpeed;
    float initialAttackSpeed;
    Rigidbody rb;
    int stopMe = 1;
    List<EnemyHealth> enemyList = new List<EnemyHealth>();
     List<EnemyHealth> enemiesToRemove = new List<EnemyHealth>();
    [SerializeField] bool amIFighting = false;


void Awake()
{
     rb = GetComponent<Rigidbody>();
     initialSpeed = speed;
     initialDamage = damage;
     initialAttackSpeed = attackSpeed;
}
    void OnEnable()
    {   amIFighting =false;
        enemyList.Clear();  
        enemiesToRemove.Clear(); 
        currentHealth = starthealth;
        damage = initialDamage;
        speed = initialSpeed;
        attackSpeed = initialAttackSpeed;
        Go();
        FindPath();
        ReturnToStart();
        StartCoroutine(FollowPath());
    }
// void OnDisable() 
//     {
//         amIFighting =false;
//         enemyList.Clear();  
//         enemiesToRemove.Clear(); 
//     }

    void Update()
    {   
        if(enemyList.Count != 0 && !amIFighting)
        {  
            amIFighting = true;
            StartCoroutine(Fighting());
        }
       
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Knight"))
        {  
            Vector3 relativeVel = rb.velocity - other.attachedRigidbody.velocity;
            float dotProduct = Vector3.Dot(relativeVel, transform.forward);
            Debug.Log(dotProduct);
            if(dotProduct > 0)
            {   
                Stop();
                StartCoroutine(ImWaiting(other.transform));
            }
        }
    }

    void FindPath()
    {   
        path.Clear();
        GameObject parent = GameObject.FindWithTag("Path");
        if (parent != null)
        {   
            foreach (Transform t in parent.transform)
            {
                path.Add(t);
            }
            path.Reverse();
        }
    }
IEnumerator Fighting()
{   
    while(enemyList.Count > 0)
    {       ClearList();
            for (int i = 0; i < enemyList.Count; i++)
            {   
                if(enemyList[i].gameObject.activeInHierarchy)
                {
                    if((i-(numberOfEnemyHits-1)) <=0)
                    {
                        enemyList[i].ApplyDamage(damage);
                    }
                }
                else if(!enemyList[i].gameObject.activeInHierarchy)
                    {
                    enemiesToRemove.Add(enemyList[i]);
                    }
            }
            foreach(EnemyHealth enemy in enemiesToRemove)
            {
                enemyList.Remove(enemy);
            }
            if(enemyList.Count <= 0)
            {   Go();
                break;                
            }
            else {   
                yield return new WaitForSeconds(attackSpeed);
                }
    }
    amIFighting = false;
    Go();
}
    void ReturnToStart()
    {   
        transform.position = path[0].transform.position;
        
    }
     IEnumerator FollowPath()
    {   
        for (int i = 0; i < path.Count; i++)
        {
            Vector3 startPosition = transform.position;
            Vector3 endPosition = path[i].transform.position;

            float travelPercent = 0;
            float elapsedTime= 0f;

            transform.LookAt(endPosition);

            while (travelPercent < 1)
            {   
                travelPercent = elapsedTime/speed;
                rb.MovePosition(Vector3.Lerp(startPosition, endPosition, travelPercent));
                yield return new WaitForEndOfFrame();
                elapsedTime += Time.deltaTime*speed*stopMe;                               
            }
        }
    }

    public void TakeHealth(float damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            gameObject.SetActive(false);
        }
    }

IEnumerator ImWaiting(Transform closestKnight)
{   Debug.Log("Im waiting");
    float distance = Vector3.Distance(transform.position,closestKnight.position);
    yield return new WaitUntil(() => (Vector3.Distance(transform.position,closestKnight.position) - distance) > 1f);
    Go();

}
    public void Stop()
    {
        stopMe =0;
    }

    public void Go()
    {
        stopMe = 1;
    }

    public void WhoIsHittingMe(EnemyHealth enemy)
    
    {   
        if(!enemyList.Contains(enemy))
            {
                enemyList.Add(enemy);
            }
    }
    void ClearList()
    {
        enemiesToRemove.Clear();
    }

}
