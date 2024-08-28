using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Debugging : MonoBehaviour
{
    float originalTimeScale;
    int speedIndex = 0;
    
    void Start()
    {
        originalTimeScale = Time.timeScale;
    }
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.P))
        {   
            switch(speedIndex)
            {
                case 0: originalTimeScale = Time.timeScale;
                        Time.timeScale = Time.timeScale*0.05f;
                        speedIndex++;
                        break;

                case 1: Time.timeScale = originalTimeScale;
                        originalTimeScale = Time.timeScale;
                        Time.timeScale = Time.timeScale*0.2f;
                        speedIndex++;
                        break;

                case 2: Time.timeScale = originalTimeScale;
                        speedIndex =0;
                        break;
            }
                
        }
    }

    void OnDisable()
    {
        Time.timeScale = originalTimeScale;
    }
}
