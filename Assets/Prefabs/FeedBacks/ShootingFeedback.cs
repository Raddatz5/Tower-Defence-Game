using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using UnityEngine;

public class ShootingFeedback : MonoBehaviour
{
        public MMF_Player ShootingFeedback1;

    public void ShootFeedback()
    {   
        ShootingFeedback1?.PlayFeedbacks();
        
    }
}
