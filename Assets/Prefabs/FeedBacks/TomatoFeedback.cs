using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using UnityEngine;

public class TomatoFeedback : MonoBehaviour
{
    public MMF_Player TomatoStand;

    void OnEnable()
{
    StartCoroutine(CountDown());
}

    IEnumerator CountDown()
    {
        yield return new WaitForSeconds(0.6f);
        TomatoStand?.PlayFeedbacks();
    }
}
    
