using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldRotate : MonoBehaviour
{   
    [SerializeField] float rotateSpeedInitial;
    [SerializeField] float rotateSpeedMultiplier;
    [SerializeField] Transform rotateAroundMeFucker;
    Quaternion offsetRotation;
    Quaternion initialRotation;
    Vector3 offset;
    
    
    void Start()
    {
        // center = GetComponentInParent<Transform>().transform;
        offset = transform.position - rotateAroundMeFucker.position;
        initialRotation = transform.rotation; 
        offsetRotation = initialRotation;      
    }

    
    void Update()
    {
        Orbit();

    }

    private void Orbit()
    {
        transform.position = rotateAroundMeFucker.position + offset;
        transform.rotation = offsetRotation;
        float angle = rotateSpeedInitial * Time.deltaTime * rotateSpeedMultiplier;
        transform.RotateAround(rotateAroundMeFucker.position, rotateAroundMeFucker.up, angle);
        offset = transform.position - rotateAroundMeFucker.position;
        offsetRotation = transform.rotation;
    }
}
