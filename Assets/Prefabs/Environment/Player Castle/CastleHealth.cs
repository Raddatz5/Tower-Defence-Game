using System.Collections;
using System.Collections.Generic;
using MoreMountains.Tools;
// using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.UI;

public class CastleHealth : MonoBehaviour
{
    [SerializeField] float maxCastleHealth = 1000f;
    public float MaxCastleHealth {get { return maxCastleHealth; }}
    [SerializeField] float currentCastleHealth;
    public float CurrentCastleHealth { get { return currentCastleHealth; }}
    [SerializeField] GameObject gameOverScreen;
    protected MMHealthBar _targetHealthBar;
    ButtonManager buttonManager;
    bool castleDeath = false;
    public bool CastleDeath {get {return castleDeath;}}
    [SerializeField] float castleDamage = 15f;
    public float CastleDamage {get {return castleDamage;}}
    [SerializeField] float castleDamageDelay = 3f;
    public float CastleDamageDelay {get {return castleDamageDelay;}}
    CastleHealthBar castleHealthBar;
  

    void Start()
    {
        buttonManager = FindAnyObjectByType<ButtonManager>();
        currentCastleHealth = maxCastleHealth;
        castleHealthBar = FindObjectOfType<CastleHealthBar>();
    }

    
   public void DamageCastle(float castleDamage)
   {    
        currentCastleHealth -= castleDamage;
        castleHealthBar.UpdateBar(currentCastleHealth);

        if (currentCastleHealth <= 0)
        {   
            castleDeath = true;
            buttonManager.PauseGame(false);
            gameOverScreen.SetActive(true);

        }
   }

//    public virtual void UpdateHealth()
//     {
//         if (_targetHealthBar != null)
//         {
//             _targetHealthBar.UpdateBar(currentCastleHealth, 0f,maxCastleHealth, true);    
//         }
//     }
}
