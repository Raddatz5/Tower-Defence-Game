using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    GameObject mainCamera;
    Transform parentTransform;

    void Start()
    {
        mainCamera = GameObject.FindWithTag("MainCamera");
        parentTransform = GetComponentInParent<Transform>();
    }

    
    void LateUpdate()
    {
       
        transform.LookAt(mainCamera.transform);
    }
}
