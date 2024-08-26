using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldRotate : MonoBehaviour
{   
    [SerializeField] float rotateSpeedInitial;
    [SerializeField] float rotateSpeedMultiplier;
    Transform center;
    Vector3 offset;
    
    
    void Start()
    {
        center = GetComponentInParent<Transform>().transform;
        offset = transform.position - center.position;
    }

    
    void FixedUpdate()
    {
        transform.Rotate(center.up,rotateSpeedInitial*Time.deltaTime*rotateSpeedMultiplier);
        // transform.position = center.position + offset;
    }
}
