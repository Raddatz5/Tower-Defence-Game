using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;
using Quaternion = UnityEngine.Quaternion;

public class CatapultAim : MonoBehaviour
{
    
    public LineRenderer lineRenderer;
    public Transform target;
    public Transform catapultAimer;
    [SerializeField] float vertexHeight = 20f;
    [SerializeField] float yRelease = 5f;
    bool lookingForTarget =false;
    public bool LookingForTarget {get {return lookingForTarget;}}
    bool loaded = false;
    public float reloadTimer = 5f;
    public float turnSpeed = 3f;
    public Animator  animator;
    bool imRotating = false;
    private Coroutine fireCoroutine = null;
    bool notFiring = true;
    public Transform gearRotate1;
    public Transform gearRotate2;
    float previousYRotation2;
     float previousYRotation3;
    float offset;
    List <Vector3> path = new List<Vector3>();
    public List<Vector3> Path {get {return path;}}
    BarrelManager barrelManager;
    Coroutine coroutine;

    void Start()
    {   
        RotateAimer();
        transform.rotation = Quaternion.Euler(0f,catapultAimer.eulerAngles.y,0f);
        loaded = true;
        previousYRotation2 = gearRotate1.localRotation.eulerAngles.y;
        previousYRotation3 = gearRotate2.localRotation.eulerAngles.y;
        offset = turnSpeed*21/6;
        barrelManager = FindAnyObjectByType<BarrelManager>();
    }

    public void StartCatapult()
    {   if(!lookingForTarget)
        {
        target.gameObject.SetActive(true);
        lookingForTarget = true;
        
          if (fireCoroutine != null)
        {
            StopCoroutine(fireCoroutine);
            fireCoroutine = null;
        }
        }
        else 
        {
            StopCatapult(); 
        }
    }

    public void StopCatapult()
    {   
        lookingForTarget = false;
        target.gameObject.SetActive(false);
    }

    void Update()
    {   
        RotateAimer();
        
        if(notFiring)
        { RotateCatapult();}

        if(lookingForTarget)
        {
            if(Input.GetMouseButtonDown(0))
            {   
                if(loaded)
                {   
                    UpdateLine();
                    if (fireCoroutine != null)
                    {
                        StopCoroutine(fireCoroutine);
                    }
                    fireCoroutine = StartCoroutine(Fire());
                }
                else
                {
                    Debug.Log("Theres no barrel");
                }
            }     
        }
    }

    IEnumerator Fire()
    {   
        StopCatapult();
        yield return new WaitUntil(() => !imRotating);
        notFiring = false;
        loaded = false;
        animator.SetTrigger("FireTrigger");
        Debug.Log("I set the trigger");
        StartCoroutine(Reload());
        StopCatapult();
        fireCoroutine = null;
    }

    public void FireComplete()
    {   
        StartCoroutine(barrelManager.BarrelLaunch());
        notFiring = true;
    }
    private void RotateCatapult()
    {   
            float currentYRotation1 = catapultAimer.eulerAngles.y;
            float previousXRotation = transform.eulerAngles.y;
            //get the sign for the direction and offset
            float sign = Mathf.Sign((currentYRotation1-previousXRotation+180)%360-180);
            float previousGearZRotation = gearRotate1.eulerAngles.z;
            float previousGear2ZRotation = gearRotate2.eulerAngles.z;
            
            if(Mathf.Abs(Mathf.DeltaAngle(currentYRotation1,previousXRotation))<turnSpeed)
            {   //make the rotating quicksnap so it doesnt vibrate back and forth
                transform.rotation = Quaternion.Euler(0f,currentYRotation1,0f);
                imRotating = false;
                 //rotate the side gears around local z axis
                float tempOffset = Mathf.Abs(Mathf.DeltaAngle(currentYRotation1,previousXRotation))*21/6;
                gearRotate1.localRotation = Quaternion.Euler(0f,previousYRotation2,previousGearZRotation+tempOffset*sign);
                gearRotate2.localRotation = Quaternion.Euler(0f,previousYRotation3,previousGear2ZRotation+tempOffset*sign);
            }
            else
            {
                //Rotate the entire thing around the y axis by folowing catapultaimer y axis
                transform.rotation = Quaternion.Euler(0f,previousXRotation+turnSpeed*sign,0f);
                imRotating = true;
                //rotate the side gears around local z axis
                gearRotate1.localRotation = Quaternion.Euler(0f,previousYRotation2,previousGearZRotation+offset*sign);
                gearRotate2.localRotation = Quaternion.Euler(0f,previousYRotation3,previousGear2ZRotation+offset*sign);
            }

    }

    private void RotateAimer()
    {
        //Rotate to face target
        Vector3 direction = target.position - catapultAimer.position;

        // Ignore vertical differences
        direction.y = 0;

        // If direction is not zero, calculate the rotation
        if (direction != Vector3.zero)
        {
            Quaternion rotation = Quaternion.LookRotation(direction);
            catapultAimer.rotation = rotation;
        }
    }

    IEnumerator Reload()
    {   
        yield return new WaitForSeconds(reloadTimer);
        loaded = true;
        animator.ResetTrigger("FireTrigger");
        Debug.Log("Im loaded");
    }

    private void UpdateLine()
    {   
        path.Clear();
        //write out the knowns, y of the vertex and y of the point that goes through the line, which is the origin of this object
        //using the vertex form of the quadratic equation, having one root (the target) and one point on the line (this transform)
        // and the vertex height only, not its x coordinate, we can solve for the second root and the vertex x coordinate.

        float vertexY =  catapultAimer.position.y+vertexHeight - target.transform.position.y;
        float pointYOnLine = catapultAimer.position.y+yRelease  - target.transform.position.y;
        float existingRoot = target.transform.localPosition.z;
        // Debug.Log("The vertex y (b) is "+vertexY +" THe yint is "+pointYOnLine+" and the root is "+existingRoot);

        //solve for the x coordinate of the vertex since the Y is already known. This is a quadratic so 2 roots will appear, the higher one is correct. I used wolfram alpha to spit this stupid thing out.
       float h1 = (-Mathf.Sqrt(vertexY*vertexY*existingRoot*existingRoot - pointYOnLine*vertexY* existingRoot*existingRoot) + pointYOnLine* existingRoot - vertexY* existingRoot)/pointYOnLine;
       float h2 = (Mathf.Sqrt(vertexY*vertexY*existingRoot*existingRoot - pointYOnLine*vertexY* existingRoot*existingRoot) + pointYOnLine* existingRoot - vertexY* existingRoot)/pointYOnLine;
        // float h1 = (-Mathf.Sqrt((-2*existingRoot*pointYOnLine + 2*existingRoot*vertexY)*(-2*existingRoot*pointYOnLine + 2*existingRoot*vertexY) - 4*pointYOnLine*(existingRoot*existingRoot*pointYOnLine - existingRoot*existingRoot*vertexY)) + 2*existingRoot*pointYOnLine - 2*existingRoot*vertexY)/(2*pointYOnLine);
        // float h2 = (Mathf.Sqrt((-2*existingRoot*pointYOnLine + 2*existingRoot*vertexY)*(-2*existingRoot*pointYOnLine + 2*existingRoot*vertexY) - 4*pointYOnLine*(existingRoot*existingRoot*pointYOnLine - existingRoot*existingRoot*vertexY)) + 2*existingRoot*pointYOnLine - 2*existingRoot*vertexY)/(2*pointYOnLine);
        float h = Math.Max(h1, h2);
        // Debug.Log("h1 is "+h1+" The h2 is "+h2);
        //get the second root
        float x2 = existingRoot - (existingRoot-h)*2;
        
        // // now solve for a
        float a =  -vertexY/Mathf.Pow(existingRoot-h,2);
 
        // // Calculate coefficients a, b, and c
         float b = a * (x2 + existingRoot);
         float c = existingRoot * x2*a-Mathf.Abs(target.transform.localPosition.y);
         lineRenderer.useWorldSpace = false;

         // Set the number of points in the LineRenderer
         lineRenderer.positionCount = Mathf.RoundToInt(existingRoot);
        //  Debug.Log("A is "+a+" The h is "+h);

        // // Calculate the points on the parabola
        for (int i = 0; i < Mathf.RoundToInt(existingRoot); i++)
        {
            // find y using x = i;
            float y = a*i*i-i*b+c;
            //add it to the list
            Vector3 localPoint = new(0, y, i);
            Vector3 rotatedPoint = transform.position + catapultAimer.transform.rotation*localPoint;
            path.Add(rotatedPoint);
            //add it to line renderer
            lineRenderer.SetPosition(i, new Vector3(0, y, i));
        }
    }
}
