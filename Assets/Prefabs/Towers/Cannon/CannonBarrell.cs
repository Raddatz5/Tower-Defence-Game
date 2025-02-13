using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;
using Quaternion = UnityEngine.Quaternion;
using UnityEngine.UI;

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
    [SerializeField] Transform rotateMe;
    public Transform gearRotate1;
    public Transform gearRotate2;
    float previousYROtation;
    [SerializeField] float turnSpeed = 2f;


    // TO DO: make an upgrade panel, have it so it can only be changed when its not in a round, have it so it can rotate and show destination when selected

      void Awake()
    {
        PopulatePool();
        tempParent = FindAnyObjectByType<Parent>().transform;
    }

    void Start()
    {
        buttonManager = FindAnyObjectByType<ButtonManager>();
    }

    void OnEnable()
    {   lineRenderer = GetComponent<LineRenderer>();
        animator = GetComponent<Animator>();
        maxLineLength = ballSpeed * (fuseTime+fuseTimeOffset); 
        startTransform = rotateMe.transform;
        previousYROtation = rotateMe.transform.eulerAngles.y;
        // StartCoroutine(Fire());
        // ShowBallLocation();
        
    }


    void Update()
    {
        DrawReflectingLine();
        RotateController();
        RotateCannon();
        
    }
    // void ShowBallLocation()
    // {
        

    // }

   void RotateController()
   {    
        previousYROtation = rotateMe.transform.eulerAngles.y;
        if (Input.GetKey(KeyCode.A))
        {   
            rotateMe.transform.Rotate(0f,turnSpeed*Time.deltaTime,0f);
        }
        if (Input.GetKey(KeyCode.D))
        {
            rotateMe.transform.Rotate(0f,-turnSpeed*Time.deltaTime,0f);
        }
   }
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
            float currentYRotation1 = rotateMe.transform.eulerAngles.y;
            float tempOffset = Mathf.DeltaAngle(currentYRotation1,previousYROtation)*5/3*-1;
            
            gearRotate1.Rotate(tempOffset,0f,0f);
            gearRotate2.Rotate(tempOffset*-1f,0f,0f);
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
