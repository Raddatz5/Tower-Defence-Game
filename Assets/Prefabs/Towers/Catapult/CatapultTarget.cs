using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatapultTarget : MonoBehaviour
{   
    public LayerMask clickableLayers;
    CatapultAim catapultAim;
    Vector3 lasPoint;

    void Awake()
    {   
        catapultAim = FindAnyObjectByType<CatapultAim>();
        lasPoint = transform.parent.position + new Vector3(50f, -10f,0);
        transform.position = lasPoint;
    }
    void Start()
    {
        gameObject.SetActive(false);
    }
    
    // Update is called once per frame
    void Update()
    {   
        if(catapultAim.LookingForTarget)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, clickableLayers))
            {
                transform.position = hit.point;
                lasPoint = transform.position;
            }
        }
        
    }
    void OnDisable()
    {
        transform.position = lasPoint;
    }
}
