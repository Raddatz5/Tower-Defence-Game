using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatapultTarget : MonoBehaviour
{   
    public LayerMask clickableLayers;
    
    // Update is called once per frame
    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, clickableLayers))
    {
        transform.position = hit.point;
    }
        
    }
}
