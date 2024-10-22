using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatapultAim : MonoBehaviour
{
    
    public LineRenderer lineRenderer;
    public Transform target;
    float existingRoot;
    float x2;
    float y3;
    float yRelease;
     Transform previousPos;
     float previousYRotation2;

    void Start()
    {
       existingRoot = transform.localPosition.z;
       x2 = existingRoot + Vector3.Distance(transform.position,target.position);
       y3 = 20f;
       yRelease = 5f;
       lineRenderer.enabled = true;
       previousPos = transform;
       UpdateLine();
    }

    void Update()
    {   
        UpdateLine();
        
    }

    private void UpdateLine()
    {   
        //Rotate to face target
            Vector3 direction = target.position - transform.position;

            // Ignore vertical differences
            direction.y = 0;

            // If direction is not zero, calculate the rotation
            if (direction != Vector3.zero)
            {
                Quaternion rotation = Quaternion.LookRotation(direction);
                transform.rotation = rotation;
            }

        //write out the knowns, y of the vertex and y of the point that goes through the line, which is the origin of this object
        //using the vertex form of the quadratic equation, having one root (the target) and one point on the line (this transform)
        // and the vertex height only, not its x coordinate, we can solve for the second root and the vertex x coordinate.

        float vertexY =  transform.position.y+y3 - target.transform.position.y;
        float pointYOnLine = transform.position.y+yRelease  - target.transform.position.y;
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
            // float y = a * (i - existingRoot) * (i - x2);
            float y = a*i*i-i*b+c;
            lineRenderer.SetPosition(i, new Vector3(0, y, i));
        }
    }
}
