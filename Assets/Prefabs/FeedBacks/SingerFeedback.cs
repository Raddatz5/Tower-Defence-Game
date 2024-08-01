using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Feedbacks;

public class SingerFeedback : MonoBehaviour
{
    public MMF_Player SingerMovement;
    
    void Start()
    {
        SingerMovement?.PlayFeedbacks();
    }

}
