using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileBorder : MonoBehaviour
{
    LineRenderer lineRenderer;
    Waypoint waypoint;
    [SerializeField] float squareWidth = 10f;
    [SerializeField] float lineWidth = 0.2f;
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

        squareVertices[0] = new Vector3(-squareWidth*0.5f,1.5f,squareWidth*0.5f);
        squareVertices[1] = new Vector3(squareWidth*0.5f,1.5f,squareWidth*0.5f);
        squareVertices[2] = new Vector3(squareWidth*0.5f,1.5f,-squareWidth*0.5f);
        squareVertices[3] = new Vector3(-squareWidth*0.5f,1.55f,-squareWidth*0.5f);

        lineRenderer.positionCount = squareVertices.Length;
        lineRenderer.SetPositions(squareVertices);
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
