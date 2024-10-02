using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;

public class Upgrade : MonoBehaviour
{
    [SerializeField] float baseRange;
    [SerializeField] Transform proximatyObjectForUpgrade;
     public float BaseRange{ get { return baseRange; } }
    [SerializeField] float rangeAfterBuff;
    public float RangeAfterBuff { get { return rangeAfterBuff; } }
    [SerializeField] float rangeUpAmount;
    [SerializeField] int costOfRangeUp;
    public int CostOfRangeUp { get { return costOfRangeUp;}}
    [SerializeField] float rangeMod;
    public float RangeMod { get { return rangeMod;}}
    float initialRangeMod;
    [SerializeField] float baseDamage;
    public float BaseDamage{ get { return baseDamage;}}
    [SerializeField] float currentDamage;
    public float CurrentDamage{ get { return currentDamage;}}
    [SerializeField] float defaultBuff = 1f;
    public float DefaultBuff{ get { return defaultBuff; } }
    [SerializeField] float damageModFromBuff = 0.25f;
    public float DamageModFromBuff{get { return damageModFromBuff;} }
    float initialDamageModFromBuff;
    [SerializeField] float dmgUpAmount;
    [SerializeField] int costOfDMGUp;
    public int CostOfDMGUp {get { return costOfDMGUp;}}
    [SerializeField] float baseAttackSpeed;
    public float BaseAttackSpeed{ get { return baseAttackSpeed; } }
     [SerializeField] float attackSpeedUpAmount;
    [SerializeField] int costOfSpeedUp;

    int numberOfRangeUp = 0;
    public int NumberOfRangeUp {get { return numberOfRangeUp; } }
    int numberOfDMGUp = 0;
    public int NumberOfDMGUp { get { return numberOfDMGUp;}}
    Gold gold;
    GoldCost goldCost;
    ButtonManager buttonManager;
    int totalGoldWorth;
    public int TotalGoldWorth{ get { return totalGoldWorth;}}
    TowerObjectPool towerObjectPool;
    float previousDefaultBuff;
    float initialBaseDamage;
    [SerializeField]float initiaLBaseRange;

    float rangeModAmountFromDMGBuffs = 0f;
    float dmgModAmountFromDMGBuffs = 0;

void Awake()
{
    gold = FindFirstObjectByType<Gold>();
    goldCost = GetComponent<GoldCost>();
    buttonManager = FindAnyObjectByType<ButtonManager>();
    numberOfRangeUp = 0;
    numberOfDMGUp = 0;
    rangeAfterBuff = baseRange;
    totalGoldWorth = goldCost.HowMuch;   
    currentDamage = baseDamage;
    initialBaseDamage = baseDamage;
    initialDamageModFromBuff = damageModFromBuff;
    initiaLBaseRange = baseRange;
    initialRangeMod = rangeMod;
    defaultBuff = 1f;
    towerObjectPool = FindObjectOfType<TowerObjectPool>();
}
   void OnEnable()
   {
        numberOfRangeUp = 0;
        numberOfDMGUp = 0;
        rangeModAmountFromDMGBuffs = 0f;
        dmgModAmountFromDMGBuffs = 0;
        rangeMod = initialRangeMod;
        damageModFromBuff = initialDamageModFromBuff;
        baseRange = initiaLBaseRange;
        rangeAfterBuff = baseRange;
        baseDamage = initialBaseDamage;
        currentDamage = baseDamage;
        totalGoldWorth = goldCost.HowMuch;
   }
void Update()
{
    CheckForBuffs();
    if(gameObject.CompareTag("Buff"))
    {
        UpdateRangeMod();
        UpdateDMGMod();
    }
}
// Checking for buffs from the singer, damage related
   void CheckForBuffs()
   {
        if (towerObjectPool.CurrentAttackBuffs.Count != 0)
        {       float internalDefaultBuff = 1f;
                float internalRangeModAmountFromDMGBuffs = 0f;
                float internalDMGModFromBuff = 0;     
                for (int i = 0; i < towerObjectPool.CurrentAttackBuffs.Count; i++)
                    {  
                        GameObject singer = towerObjectPool.CurrentAttackBuffs[i];
                        Upgrade singerUpgrade = singer.GetComponent<Upgrade>();
                        float tempRangeOfBuff = Mathf.RoundToInt(singerUpgrade.RangeAfterBuff);
                        float tempDMGModFromBuff = singerUpgrade.DamageModFromBuff;
                        
                        int  distance = Mathf.RoundToInt(Vector3.Distance(proximatyObjectForUpgrade.position,singer.transform.position));

                        if (distance <= tempRangeOfBuff)
                        {
                            internalDefaultBuff += tempDMGModFromBuff;
                             if(gameObject.CompareTag("Buff"))
                             {
                                internalRangeModAmountFromDMGBuffs = internalRangeModAmountFromDMGBuffs + tempDMGModFromBuff*0.1f;
                                if(gameObject != towerObjectPool.CurrentAttackBuffs[i])
                                {  
                                    internalDMGModFromBuff = internalDMGModFromBuff + tempDMGModFromBuff*0.2f;
                                }
                             }
                            
                        }
                    }
                defaultBuff = internalDefaultBuff;
                dmgModAmountFromDMGBuffs = internalDMGModFromBuff;
                rangeModAmountFromDMGBuffs = internalRangeModAmountFromDMGBuffs;
         }
         else{defaultBuff = 1f;
                dmgModAmountFromDMGBuffs = 0f;
                rangeModAmountFromDMGBuffs = 0f;}

        if(previousDefaultBuff != defaultBuff)
        {
            previousDefaultBuff = defaultBuff;
        }
      UpdateDMGAfterBuff(defaultBuff*baseDamage);
   }

   void UpdateRangeMod()
   {
        rangeMod = initialRangeMod + baseDamage + rangeModAmountFromDMGBuffs;
   }
//Called from Waypoint when its clicked and build menu isnt open
 public void TowerUpgradeMenuFirst(GameObject waypointThatCalled) 
    { 
        buttonManager.OpenUpgradeMenu(gameObject, waypointThatCalled, waypointThatCalled.transform.position);            
    }

public void UpdateRangeAfterBuff(float newRange)
{
    rangeAfterBuff = newRange;
    if(buttonManager.IsUpgradeMenuOpen && gameObject == buttonManager.TowerToUpgrade)
    {
        buttonManager.UpdateUpgradeMenu();
    }
}
public void UpdateDMGAfterBuff(float newCurrentDamage)
{
    currentDamage = newCurrentDamage;
    if(buttonManager.IsUpgradeMenuOpen && gameObject == buttonManager.TowerToUpgrade)
    {
        buttonManager.UpdateUpgradeMenu();
    }
}
public void UpgradeRange()
{
    if(gold.CurrentBalance >= costOfDMGUp)
        {
            baseRange += rangeUpAmount;
            numberOfRangeUp++;
            gold.Withdraw(costOfRangeUp);
            totalGoldWorth += costOfRangeUp;

            switch(numberOfRangeUp)
            {
                case 1: costOfRangeUp+= 10;
                break;

                case 2: costOfRangeUp+= 20;
                break;

                case 3: costOfRangeUp+= 30;
                break;

                case 4: costOfRangeUp+= 60;
                break;

                case 5: costOfRangeUp+= 90;
                break;
                case 7: costOfRangeUp+= 130;
                break;
                case 8: costOfRangeUp+= 200;
                break;
                case 9: costOfRangeUp+= 400;
                break;
            }
        }
}
public void UpgradeDMG()
{   if(gold.CurrentBalance >= costOfDMGUp)
        {   
            if(this.name == "TomatoStand")
            {
                baseDamage = baseDamage*dmgUpAmount*(1-(dmgUpAmount*dmgModAmountFromDMGBuffs*0.15f));
            }
            else if(this.name == "Spotter")
            {
                baseDamage += baseDamage*(dmgUpAmount + dmgModAmountFromDMGBuffs);
            }
            else if(this.name == "Singer")
            {
                baseDamage = baseDamage*dmgUpAmount*(1+(dmgUpAmount*dmgModAmountFromDMGBuffs*0.15f));
            }
            else
            {
                baseDamage += dmgUpAmount;
            }
            numberOfDMGUp ++;
            gold.Withdraw(costOfDMGUp);
            totalGoldWorth += costOfDMGUp;

            switch(numberOfDMGUp)
            {
                case 1: costOfDMGUp+= 15;
                break;

                case 3: costOfDMGUp+= 30;
                break;

                case 4: costOfDMGUp+= 45;
                break;

                case 5: costOfDMGUp+= 60;
                break;

                case 6: costOfDMGUp+= 120;
                break;
                case 7: costOfDMGUp+= 220;
                break;
                case 8: costOfDMGUp+= 350;
                break;
            }
        }
}
void  UpdateDMGMod()
{
    damageModFromBuff = baseDamage+ dmgModAmountFromDMGBuffs;
}
}
