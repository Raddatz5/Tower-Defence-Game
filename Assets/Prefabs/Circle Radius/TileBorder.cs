using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileBorder : MonoBehaviour
{
    LineRenderer lineRenderer;
    Waypoint waypoint;
    float squareWidth = 9f;
    float lineWidth = 0.45f;
    bool whatsTheSignal = false;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        waypoint = GetComponent<Waypoint>();
        lineRenderer.enabled = false;
        UpdateSquare();
    }

   public void UpdateSquare()
    {
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;

        if(!waypoint.IsPlaceable)
            {
                lineRenderer.enabled = false;
            }
        else{
       
        Vector3[] squareVertices = new Vector3[4];

        squareVertices[0] = new Vector3(-squareWidth*0.5f+transform.position.x,1.5f,squareWidth*0.5f+transform.position.z);
        squareVertices[1] = new Vector3(squareWidth*0.5f+transform.position.x,1.5f,squareWidth*0.5f+transform.position.z);
        squareVertices[2] = new Vector3(squareWidth*0.5f+transform.position.x,1.5f,-squareWidth*0.5f+transform.position.z);
        squareVertices[3] = new Vector3(-squareWidth*0.5f+transform.position.x,1.55f,-squareWidth*0.5f+transform.position.z);

        lineRenderer.positionCount = squareVertices.Length;
        lineRenderer.SetPositions(squareVertices);
        lineRenderer.loop = true;
        }
    }

   public void ShowBorder(bool isShowing)
    {       
        lineRenderer.enabled = isShowing;
        whatsTheSignal = isShowing;
        UpdateSquare();
    }
    public void UpdateSoldBorder()
    {
        lineRenderer.enabled = whatsTheSignal;
    }

    
}
