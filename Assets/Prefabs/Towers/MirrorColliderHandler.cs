using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MirrorColliderHandler : MonoBehaviour
{      
    LookatEnemyMirror lookatEnemyMirror;
    GameObject targetEnemy;
    ParticleSystem particleSystemA;

    void Start()
    {
        lookatEnemyMirror = GetComponent<LookatEnemyMirror>();
        particleSystemA = GetComponent<ParticleSystem>();
    }
       void OnParticleCollision(GameObject other) 
    {   
        targetEnemy = lookatEnemyMirror.Target;
        if(other != targetEnemy)
        {
        
        }
        else 
        {    Debug.Log("Im hitting the right target");
            var mainModule = particleSystemA.main;
            mainModule.startLifetime = 0;
        }
        // else { int numCollision = particleSystemA.GetCollisionEvents(other,collisionEvent)}
    }
}
