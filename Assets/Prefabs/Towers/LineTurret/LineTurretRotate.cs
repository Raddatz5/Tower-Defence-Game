using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineTurretRotate : MonoBehaviour
{
    [SerializeField] bool CW = false;
    float speed = 40f;
    int whichWay = 1;

    void Start()
    {
        if (CW)
        {
            whichWay = -1;
        }
    }
    void Update()
    {   
        transform.Rotate(0f,speed*Time.deltaTime*whichWay,0f);
    }
}
