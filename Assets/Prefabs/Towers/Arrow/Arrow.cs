using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    [SerializeField]GameObject target;
    [SerializeField] float timer = 6f;
    float elapsedtime;
    float speed;
    ArrowManager arrowManager;
    Vector3 initialPosition;
    Transform refenceParent;

    void Awake()
    {   
        refenceParent = gameObject.transform.parent;
        arrowManager = GetComponentInParent<ArrowManager>();
    }
    void OnEnable()
    {   
        if(arrowManager!=null)
        {
        elapsedtime = 0f; 
        speed = arrowManager.Speed;
        target = arrowManager.TempTarget;
        transform.position = arrowManager.transform.position;
        }
      
    }

      void Update()
        {   elapsedtime += Time.deltaTime;  
            if(elapsedtime >= timer)
                {
                    gameObject.SetActive(false);
                }
            initialPosition = transform.position;
            if(target != null)
            {
                transform.LookAt(target.transform.position + new Vector3(0,3f,0));
            }
            transform.position = initialPosition + transform.forward * speed * Time.deltaTime;        
    }

    public void WhatsMyTarget(GameObject targetRef)
    {
        target = targetRef;
    }
    public void ReparentAndDisable()
    {   
        if(transform.parent != refenceParent)
        {
        gameObject.transform.SetParent(refenceParent, false);
        }
        gameObject.SetActive(false);
    }
  }
