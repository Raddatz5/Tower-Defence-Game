using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldRotate : MonoBehaviour
{   
    [SerializeField] float rotateSpeedInitial;
    [SerializeField] float rotateSpeedMultiplier;
    Transform rotateAroundMe;
    Quaternion offsetRotation;
    Quaternion initialRotation;
    Vector3 offset;
    
    public void AssignRotation(Transform rotationAxis)
    {
        rotateAroundMe = rotationAxis;
        offset = transform.position - rotateAroundMe.position;
        initialRotation = transform.rotation; 
        offsetRotation = initialRotation;
    }
     
    void Update()
    {
        Orbit();

    }

    private void Orbit()
    {
        transform.position = rotateAroundMe.position + offset;
        transform.rotation = offsetRotation;
        float angle = rotateSpeedInitial * Time.deltaTime * rotateSpeedMultiplier;
        transform.RotateAround(rotateAroundMe.position, rotateAroundMe.up, angle);
        offset = transform.position - rotateAroundMe.position;
        offsetRotation = transform.rotation;
    }
}
