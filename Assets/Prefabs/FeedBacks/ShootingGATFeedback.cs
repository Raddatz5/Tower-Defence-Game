using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using UnityEngine;

public class ShootingGATFeedback : MonoBehaviour
{
    public MMF_Player ShootingFeedback1;
  
   
    public void PlayTheFeedback()
    {  
            ShootingFeedback1?.PlayFeedbacks();
    }
}
