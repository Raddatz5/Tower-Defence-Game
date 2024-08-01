using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MirrorDamage : MonoBehaviour
{
    [SerializeField] float maxDamageCap = 40f;
    public float MaxDamageCap { get { return maxDamageCap; } }
    [SerializeField] float maxDamageWithBuff;
    public float MaxDamageWithBuff { get { return maxDamageWithBuff;}}
    [SerializeField] float rampUpTime;
    public float RampUpTime { get { return rampUpTime;}}
    [SerializeField] float rampUpTimeBase = 3f;
    [SerializeField] float baseDamage;
    public float BaseMirrorDamage { get { return baseDamage; } }
    bool imRampingUp = false;
    public bool ImRampingUp { get { return imRampingUp; } }
    float startDamage = 0f;
    float t = 0f;
    LookatEnemyMirror lookatEnemyMirror;

    int rangeBuffsInRange;
    bool breakWhileLoop = false;
    Upgrade upgrade;
    float defaultBuff = 1f;

void Start()
{
    lookatEnemyMirror = GetComponentInParent<LookatEnemyMirror>();
}

void OnEnable()
{
     upgrade = GetComponent<Upgrade>();
     rampUpTime = rampUpTimeBase;
}

void Update()
{
    CheckForDMGBuffs();
}
    public IEnumerator RampUpDamage()
    {   
        float elapsedTime = 0f;
        AdjustRampUpTime();
        imRampingUp = true;
        while (elapsedTime < rampUpTime)
        {
            t = elapsedTime / rampUpTime;
            baseDamage = Mathf.Lerp(startDamage, maxDamageCap*defaultBuff, t);
            if(breakWhileLoop){
            break;}
            yield return null;

            elapsedTime += Time.deltaTime;
        }
        imRampingUp = false;
        if (breakWhileLoop)
            {baseDamage = startDamage;}
        else
            {baseDamage = maxDamageCap;}
    }

    public void ResetDamage()
    {   StartCoroutine(BreakWhileLoop());
        baseDamage = startDamage;
    }

    IEnumerator BreakWhileLoop()
    {
        breakWhileLoop = true;
        yield return new WaitForSeconds(0.1f);
        breakWhileLoop = false;
    }

    void AdjustRampUpTime()
    { 
        rampUpTime = rampUpTimeBase + rampUpTimeBase * (rangeBuffsInRange+upgrade.NumberOfRangeUp)*0.3f;
    }
    public void UpdateRangeBuffsInRangeAmount(int number)
    {
        rangeBuffsInRange = number;
        AdjustRampUpTime();
    }

       void CheckForDMGBuffs()
    {  
        defaultBuff = upgrade.DefaultBuff;
        maxDamageCap = upgrade.BaseDamage;
        maxDamageWithBuff = maxDamageCap*defaultBuff;
    }
}
