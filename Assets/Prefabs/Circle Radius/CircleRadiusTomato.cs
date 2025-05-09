using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleRadiusTomato : MonoBehaviour
{
   [SerializeField] float radius = 30f;
    int segments = 50;
    LineRenderer lineRenderer;
    TomatoStand tomatoStand;
   
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = segments ;
        lineRenderer.useWorldSpace = false; 
        lineRenderer.enabled = false;
        tomatoStand = GetComponentInParent<TomatoStand> ();

    }
    void Update()
    {
        DrawCircle();
    }

    void DrawCircle()
    {   
        radius = tomatoStand.RangeAfterBuff*0.5f;
        float angleIncrement = 360f/ segments;
        for(int i = 0; i < segments; i++)
        {
            float angle = i*angleIncrement;
            float x = Mathf.Sin(Mathf.Deg2Rad*angle)*radius;
            float z = Mathf.Cos(Mathf.Deg2Rad*angle)*radius;
            lineRenderer.SetPosition(i, new Vector3(x,1.5f,z));
        }   
    }    
    public void ShowCircle(bool isShowing)
    {   
        lineRenderer.enabled = isShowing;
    }
}
