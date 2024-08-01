using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Floating : MonoBehaviour
{
    [SerializeField] float forwardDistance;
    [SerializeField] float speed;
    
    Vector3 startPosition;
     Vector3 endPosition;

    void Start()
    {
        
        startPosition = transform.position;
        endPosition = startPosition + transform.forward*forwardDistance;
        StartCoroutine(FollowPath());
    }

    IEnumerator FollowPath()
    {   
        while(true)
        {         
            float travelPercent = 0;

            while (travelPercent < 1)
            {   
               travelPercent += Time.deltaTime*speed; 
                transform.position = Vector3.Lerp(startPosition, endPosition, travelPercent);
                yield return new WaitForEndOfFrame();
                                              
            }
            Vector3 tempPos = startPosition;
            startPosition = endPosition;
            endPosition = tempPos;
           
            yield return new WaitForSeconds(1f);

        }
    }       
}

