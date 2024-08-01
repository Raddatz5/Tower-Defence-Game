using System;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.Tools;
using UnityEngine;

public class CastleHealthBar : MonoBehaviour
{
    public MMProgressBar CastleHealthBarUpdate;
    CastleHealth castleHealth;
    // [Range(0f, 100f)] public float Value;

    void Start()
    {
        castleHealth = FindObjectOfType<CastleHealth>();
    }

  
    public void UpdateBar(float inverseCurrentHealth)
    {
        CastleHealthBarUpdate.UpdateBar(inverseCurrentHealth,0f,castleHealth.MaxCastleHealth);
    }
}
