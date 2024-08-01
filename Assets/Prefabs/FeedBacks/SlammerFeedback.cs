using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using UnityEngine;

public class SlammerFeedback : MonoBehaviour
{
    public MMF_Player landingFeedback;
    SlamMover slamMover;
    [SerializeField] bool checkSingal;
    void Start()
    {
        slamMover = GetComponentInParent<SlamMover>();
        
    }

    void Update()
    {   
       
    }
}
