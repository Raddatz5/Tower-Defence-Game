using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class BarrelFire : MonoBehaviour
{   
    float speed = 10f;
    CatapultAim catapultAim;
    BarrelManager barrelManager;
    List <Vector3> path = new();
    Coroutine coroutine= null;

    void Awake()
    {
        catapultAim = FindAnyObjectByType<CatapultAim>();
        barrelManager = FindAnyObjectByType<BarrelManager>();
    }
   void OnEnable()
   {   
       if(path == null || path.Count == 0)
       {
        path = new List<Vector3>(catapultAim.Path);
       }
        speed = barrelManager.Speed;
        if(catapultAim.Path != null)
        {
            coroutine ??= StartCoroutine(FollowPath());
        }
   }
   IEnumerator FollowPath()
   {   
        for(int i = 0; i < path.Count-1;i++)
        {
            Vector3 startPosition = transform.position;
            Vector3 endPosition = path[i+1];

            float travelPercent = 0;
            float elapsedTime = 0;

            while (travelPercent < 1)
            {   
                travelPercent = elapsedTime;
                transform.position = Vector3.Lerp(startPosition, endPosition, travelPercent);
                yield return new WaitForEndOfFrame();
                elapsedTime += Time.deltaTime*speed;
            }
        }
        coroutine = null;
        path = null;
        gameObject.SetActive(false);
   }
}
