using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class BarrelFire : MonoBehaviour
{   
    float speed = 10f;
    CatapultAim catapultAim;
    BarrelManager barrelManager;
    List <Vector3> path = new();
    Coroutine coroutine= null;
    public Vector2 rotationRange = new Vector2(10, 20); // Range for rotation angles
    public Vector2 rotationSpeedRange = new Vector2(1, 3); // Range for rotation speeds
    private Vector3 randomRotation;
    private Vector3 rotationSpeed;
    bool imWaiting = true;
    [SerializeField] GameObject fireDamage;
    [SerializeField] GameObject explosion;
    Vector3 targetRef;

    void Awake()
    {
        catapultAim = FindAnyObjectByType<CatapultAim>();
        barrelManager = FindAnyObjectByType<BarrelManager>();
    }
   void OnEnable()
   {    
        StartCoroutine(Prepare());
   }
   void Update()
   {
    if(imWaiting)
    {  
        transform.SetPositionAndRotation(barrelManager.barrelMount.position, barrelManager.barrelMount.rotation*Quaternion.Euler(-120f,0f,0f));
    }
    else
    {
        RotateRandomly();
    }
   }

  IEnumerator Prepare()
  {
     imWaiting = true;
    //wait for the catapult trigger to go
    yield return new WaitUntil (() => catapultAim.TriggerDone);
    targetRef = catapultAim.target.position+new Vector3(0f, 0.5f,0f); 
    // Debug.Log(targetRef);
    imWaiting = false;
    GenerateRandomRotation();
        
    if(path == null || path.Count == 0)
    {
        path = new List<Vector3>(catapultAim.Path);
    }
    speed = barrelManager.Speed;
    if(catapultAim.Path != null)
    {
        coroutine ??= StartCoroutine(FollowPath());
    }
  }
   IEnumerator FollowPath()
   {    transform.Rotate(randomRotation);
        for(int i = 0; i < path.Count-1;i++)
        {
            Vector3 startPosition = transform.position;
            Vector3 endPosition = path[i+1];

            float travelPercent = 0;
            float elapsedTime = 0;

            while (travelPercent < 1)
            {   
                // RotateRandomly();
                travelPercent = elapsedTime;
                transform.position = Vector3.Lerp(startPosition, endPosition, travelPercent);
                yield return new WaitForEndOfFrame();
                elapsedTime += Time.deltaTime*speed;
            }
        }
        coroutine = null;
        path = null;
        Instantiate(explosion,targetRef, Quaternion.Euler(-90f,0f,0f));
        yield return new WaitForSeconds(0.2f);
        Instantiate(fireDamage,targetRef, Quaternion.identity);
        gameObject.SetActive(false);
   }
    void RotateRandomly()
    {
        transform.Rotate(rotationSpeed * Time.deltaTime);
    }

    void GenerateRandomRotation()
    {
        randomRotation = new Vector3(
            Random.Range(rotationRange.x, rotationRange.y),
            Random.Range(rotationRange.x, rotationRange.y),
            Random.Range(rotationRange.x, rotationRange.y)
        );

        rotationSpeed = new Vector3(
            Random.Range(rotationSpeedRange.x, rotationSpeedRange.y),
            Random.Range(rotationSpeedRange.x, rotationSpeedRange.y),
            Random.Range(rotationSpeedRange.x, rotationSpeedRange.y)
        );
    }
}
