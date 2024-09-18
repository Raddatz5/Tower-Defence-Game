using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

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
    float timeInterval = 0f;
   [SerializeField] float particleSize;
    float particleSizeInitial;
    [SerializeField] float particleSizeUp = 0.43f;
    float subEmitProb;
    float subEmitProbInitial;
    float subEmitProbUp = 0.26f;
    ParticleSystem particleSystemA;
    ParticleSystem.MainModule mainModule;
    

void Start()
{
    lookatEnemyMirror = GetComponentInParent<LookatEnemyMirror>();
    particleSystemA = GetComponentInChildren<ParticleSystem>();
    mainModule = particleSystemA.main;
    var subEmitterModule = particleSystemA.subEmitters;
    subEmitProbInitial = subEmitterModule.GetSubEmitterEmitProbability(0);
    particleSizeInitial = mainModule.startSize.constant;
}

void OnEnable()
{    subEmitProb = subEmitProbInitial;
     particleSize = particleSizeInitial;
     upgrade = GetComponent<Upgrade>();
     rampUpTime = rampUpTimeBase;
}

void Update()
{
    CheckForDMGBuffs();
}
    public IEnumerator RampUpDamage()
    {   
        ResetParticleSizeandProb();
        float elapsedTime = 0f;
        int i = 1;
        AdjustRampUpTime();
        timeInterval = rampUpTime/4;
        imRampingUp = true;
        while (elapsedTime < rampUpTime)
        {
            t = elapsedTime / rampUpTime;
            baseDamage = Mathf.Lerp(startDamage, maxDamageCap*defaultBuff, t);
            if(breakWhileLoop){break;}

            if(elapsedTime > timeInterval*i)
            {
                IncreaseParticleSizeandProb();
                i++;
            }
        
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        imRampingUp = false;
        Debug.Log("Im ramped Up, emitProb is: " +subEmitProb + " and particlesize is: " + mainModule.startSize.constant);
        if (breakWhileLoop)
            {baseDamage = startDamage;}
        else
            {baseDamage = maxDamageCap;}
    }

void IncreaseParticleSizeandProb()
{
    subEmitProb += subEmitProbUp;
    Debug.Log(subEmitProb);
    particleSize += particleSizeUp;
    float multiplier = particleSize/particleSizeInitial;

    var subEmitterModule = particleSystemA.subEmitters;
    subEmitterModule.SetSubEmitterEmitProbability(0,subEmitProb);
    mainModule.startSizeMultiplier = multiplier;
    Debug.Log("Particle size after BUMP " +mainModule.startSize.constant);

}
void ResetParticleSizeandProb()
{
    subEmitProb = subEmitProbInitial;
    particleSize = particleSizeInitial;
    Debug.Log("The intial particle size "+particleSize);

    var subEmitterModule = particleSystemA.subEmitters;
    subEmitterModule.SetSubEmitterEmitProbability(0,subEmitProbInitial);
    mainModule.startSizeMultiplier = 1f;
    Debug.Log("Particle size after reset multi " +mainModule.startSize.constant);
    
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
