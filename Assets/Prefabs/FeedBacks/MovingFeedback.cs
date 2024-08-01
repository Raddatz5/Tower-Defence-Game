using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using UnityEngine;

public class MovingFeedback : MonoBehaviour
{
    public MMF_Player movementFeedback;
    // Start is called before the first frame update
    void Start()
    {
        movementFeedback?.PlayFeedbacks();
    }
    
}
