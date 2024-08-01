using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using UnityEngine;

public class ShootingGATFeedback : MonoBehaviour
{
    public MMF_Player ShootingFeedback1;
        new ParticleSystem particleSystem;
   
        [SerializeField] float timeDelayStart = 1f;
        [SerializeField] float timeDelay;
   
         
    void Start()
    {
        particleSystem = GetComponentInChildren<ParticleSystem>();
        timeDelay = timeDelayStart;
    }

 

    void Update()
    {   if(timeDelay>0f && particleSystem.emission.enabled)
        { timeDelay -= Time.deltaTime;
        }
         else if(particleSystem.emission.enabled)
        {  
            ShootingFeedback1?.PlayFeedbacks();
        }
        else {timeDelay = timeDelayStart;}
    }
}
