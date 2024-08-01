using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using UnityEngine;

public class RamMovementFeedback : MonoBehaviour
{
    public MMF_Player ramMovement;
    void OnEnable()
    {
        ramMovement?.PlayFeedbacks();
    }

    
}
