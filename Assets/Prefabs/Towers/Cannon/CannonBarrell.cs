using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;
using Quaternion = UnityEngine.Quaternion;

public class CannonBarrell : MonoBehaviour
{
    [SerializeField] float ballSpeed = 10f;
    public float BallSpeed { get{return ballSpeed;}}
    [SerializeField] float fuseTime = 3f;
    public float FuseTime { get{return fuseTime;}}
     [SerializeField] float ballRadius = 1f;
    public float BallRadius { get{return ballRadius;}}
    [SerializeField] float fuseTimeOffset = 0.02f;
    [SerializeField] int poolSize = 5;
    GameObject [] pool;
    [SerializeField] GameObject cannonBall;
    Rigidbody rb;
    LineRenderer lineRenderer;
    bool move = true;
    Vector3 direction;
    ButtonManager buttonManager;
    Transform startTransform;
    float maxLineLength;
    [SerializeField] LayerMask collisionLayer;
    [SerializeField] GameObject target;
    Transform tempParent;
    Animator animator;
    public Transform gearRotate1;
    public Transform gearRotate2;
    float previousYRotation2;
    float previousYRotation3;
    float previousXRotation;

      void Awake()
    {
        PopulatePool();
        tempParent = FindAnyObjectByType<Parent>().transform;
    }

    void Start()
    {   lineRenderer = GetComponent<LineRenderer>();
        previousYRotation2 = gearRotate1.localRotation.eulerAngles.y;
        previousYRotation3 = gearRotate2.localRotation.eulerAngles.y;
        animator = GetComponent<Animator>();
        maxLineLength = ballSpeed * (fuseTime+fuseTimeOffset); 
        startTransform = transform;
        previousXRotation = transform.eulerAngles.y;
        StartCoroutine(Fire());
        // ShowBallLocation();
        
    }


    void Update()
    {
        DrawReflectingLine();
        RotateCannon();
        
    }
    // void ShowBallLocation()
    // {
        

    // }
   
   IEnumerator Fire()
    {   
        animator.SetTrigger("CannonFire");
        StartCoroutine(Reset());
        yield return new WaitForSeconds(1f);
        for (int i = 0; i < pool.Length; i++)
        {
            if(!pool[i].activeSelf)
            {   
                
                pool[i].transform.SetParent(tempParent,true);
                pool[i].SetActive(true);
                pool[i].transform.rotation = transform.rotation;
                pool[i].transform.position = transform.position+new Vector3(0f,-5.5f+ballRadius,0f);
    
                break;
            }
        }
    }
     private void RotateCannon()
    {   
            float currentYRotation1 = transform.eulerAngles.y;
            
            // //get the sign for the direction and offset
            float sign = Mathf.Sign((currentYRotation1-previousXRotation+180)%360-180);
            float previousGearZRotation = gearRotate1.eulerAngles.x;
            float previousGear2ZRotation = gearRotate2.eulerAngles.x;
            
            // if(Mathf.Abs(Mathf.DeltaAngle(currentYRotation1,previousXRotation))<turnSpeed)
            // {   //make the rotating quicksnap so it doesnt vibrate back and forth
                // transform.rotation = Quaternion.Euler(0f,currentYRotation1,0f);
            //     imRotating = false;
                 //rotate the side gears around local z axis
                float tempOffset = Mathf.DeltaAngle(currentYRotation1,previousXRotation)*5/3*-1;
                Quaternion targetRotation = transform.rotation;
                Vector3 targetEulerAngles = targetRotation.eulerAngles;
                gearRotate1.localRotation = Quaternion.Euler(previousGearZRotation+tempOffset*sign,targetEulerAngles.y,0f);
                gearRotate2.localRotation = Quaternion.Euler(previousGear2ZRotation+tempOffset*sign,targetEulerAngles.y,0f);
            // }
            // else
            // {
            // //     //Rotate the entire thing around the y axis by folowing catapultaimer y axis
            //     transform.rotation = Quaternion.Euler(0f,previousXRotation+turnSpeed*sign,0f);
            // //     imRotating = true;
            // //     //rotate the side gears around local z axis
            //     gearRotate1.localRotation = Quaternion.Euler(0f,targetEulerAngles,previousGearZRotation+offset);
            //     gearRotate2.localRotation = Quaternion.Euler(0f,-targetEulerAngles,previousGear2ZRotation+offset);
            // }
            previousYRotation2 = gearRotate1.localRotation.eulerAngles.y;
            previousYRotation3 = gearRotate2.localRotation.eulerAngles.y;
            previousXRotation = transform.eulerAngles.y;

    }
    void DrawReflectingLine()
    {
        Vector3 currentPosition = startTransform.position;
        Vector3 direction = startTransform.forward;
        float remainingLength = maxLineLength;
   
        // Get the opposite side of the right angle triangle which is the radius of the cannonball. then use it to shorten the reflect point of the line renderer, else it looks dumb.
        float oppositeSide = ballRadius;

        // Initialize LineRenderer
        lineRenderer.positionCount = 1;
        lineRenderer.SetPosition(0, currentPosition);
        int maxIterations = 100; // Safety limit to prevent infinite loops 
        int iteration = 0;

        while (remainingLength > 0)
        {
            if (iteration++ > maxIterations) { Debug.LogError("Exceeded max iterations, possible infinite loop detected."); break; }
            RaycastHit hit;
            if (Physics.Raycast(currentPosition, direction, out hit, remainingLength, collisionLayer))
            {      
                // Reflect the direction and update the remaining length
                Vector3 newDirection = Vector3.Reflect(direction, hit.normal);
                float reflectAngle = Vector3 .Angle(direction, newDirection);
                float alphaAngle = reflectAngle*0.5f;
                // soh cah toa MOFO
                float magnitude = oppositeSide/Mathf.Sin(alphaAngle*Mathf.Deg2Rad);
                // Debug.Log("The radius is "+ oppositeSide+ " and the magnitude is "+magnitude+" and the angle is "+alphaAngle+" and the reflectAngle is "+reflectAngle);
                Vector3 backtrackVector = -direction.normalized*magnitude;
                
                remainingLength -= Vector3.Distance(currentPosition, hit.point);
                remainingLength += magnitude;
                currentPosition = hit.point+backtrackVector;
                // Debug.Log("Backtrack vector is "+backtrackVector+" and hit point is "+hit.point+" and the new position is "+currentPosition);

                // Update the line renderer
                lineRenderer.positionCount++;
                lineRenderer.SetPosition(lineRenderer.positionCount - 1, currentPosition);

                direction = newDirection;
            }
            else
            {
                // No collision, terminate the line at the remaining length
                lineRenderer.positionCount++;
                lineRenderer.SetPosition(lineRenderer.positionCount - 1, currentPosition + direction * remainingLength);
                break;
            }
        }
        target.transform.position = lineRenderer.GetPosition(lineRenderer.positionCount - 1);
    }
        private void PopulatePool()
    {
        pool = new GameObject[poolSize];

        for (int i = 0; i < pool.Length; i++)
        {
            pool[i] = Instantiate(cannonBall,transform);
            pool[i].SetActive(false);
        }
    }
        IEnumerator Reset()
    {   
        yield return new WaitForSeconds(0.05f);
        animator.ResetTrigger("CannonFire");
    }
}
