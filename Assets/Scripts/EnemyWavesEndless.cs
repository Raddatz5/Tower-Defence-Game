using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EnemyWavesEndless : MonoBehaviour
{
   ObjectPoolBig2 objectPoolBig2;
   [SerializeField] int waveReward = 60;
   [SerializeField] GameObject startWaveButton;
   [SerializeField] GameObject currentWave;
   [SerializeField] float timeBetweenWaves = 20f;
   float time;
   [SerializeField] GameObject timerUI;
      TextMeshProUGUI timerText;
   TextMeshProUGUI waveText;
   TextMeshProUGUI currentWaveText;
 int enemy0number =0;
   [SerializeField] float enemy0timing;
 int enemy1number =0;
   [SerializeField] float enemy1timing;
int enemy2number = 0;
   [SerializeField] float enemy2timing;
 int enemy3number= 0;
   [SerializeField] float enemy3timing;
 int enemy4number = 0;
   [SerializeField] float enemy4timing;
 int enemy5number = 0;
   [SerializeField] float enemy5timing;
   int waveIndex =2;
   Gold gold;
   bool startTimer = false;
   [SerializeField]bool co1 = true;
   [SerializeField]bool co2 = true;
   [SerializeField]bool co3 = true;
   [SerializeField]bool co4 = true;
   [SerializeField]bool co5 = true;
   [SerializeField]bool co6 = true;
   [SerializeField]bool isWaveHappening = true;
   [SerializeField]bool isWaveTimerOn = true;


   

   void Start()
   {
    objectPoolBig2 = FindAnyObjectByType<ObjectPoolBig2>();
    startWaveButton.SetActive(true);
    waveText = startWaveButton.GetComponentInChildren<TextMeshProUGUI>();
    waveText.text  = "Start Wave: "+(waveIndex-1);
    currentWaveText = currentWave.GetComponentInChildren<TextMeshProUGUI>();
    timerText= timerUI.GetComponentInChildren<TextMeshProUGUI>();
    currentWave.SetActive(false);
    gold = FindAnyObjectByType<Gold>();
   }

   void Update()
   {  

      // difficultyLevel = 2 + Mathf.FloorToInt(time/timeChunkForDiffculty);   
      if(startTimer)
      {
         time += Time.deltaTime;
         timerText.text = Mathf.FloorToInt(time/60f) +" : "+ Mathf.Round(time*100-Mathf.FloorToInt(time/60f)*60f*100)/100.0;
      }
     ManageWaves();
       
   }
   void ManageWaves()
   {
       if(!isWaveTimerOn)
       {
         isWaveTimerOn = true;
         StartCoroutine(WaitTimer());
       } 
       else if(!isWaveHappening)
       {
         isWaveHappening = true;
         StartCoroutine(StartNextWave());
       }
   }
public void StartFirstWave()
{
   startTimer = true;
   isWaveHappening =false;
   startWaveButton.SetActive(false);
   currentWave.SetActive(true);
}

IEnumerator WaitTimer()
{
         Debug.Log("Starting Wave Timer");
         gold.AddToGold(waveReward);
         float elapsedTime = 0f;
         currentWaveText.text  = "Wave: "+ (waveIndex);
         
         while(elapsedTime < timeBetweenWaves)
         {               
             elapsedTime += Time.deltaTime;
             yield return null;
         }         
         isWaveHappening = false;
         waveIndex ++;
         currentWaveText.text  = "Wave: "+ (waveIndex);
         Debug.Log("Finish Wave Timer");
 
}

IEnumerator StartNextWave()
  {            
            enemy0number = 6 + Mathf.RoundToInt(waveIndex*waveIndex*0.3f);
            enemy0timing = enemy0timing*0.9f;

            if(waveIndex>3)
               {
               enemy1number = 1 + Mathf.RoundToInt((waveIndex-2)*(waveIndex-2)*0.19f);
               enemy1timing = enemy1timing*0.91f;
               enemy0number -= 4;
               }

            if(waveIndex>6)
               {
               enemy2number = 1 + Mathf.RoundToInt((waveIndex-4)*(waveIndex-4)*0.07f);
               enemy2timing = enemy2timing*0.93f;
               enemy1number -= 4;
               }

            if(waveIndex>10)
               {
               enemy3number = 1 + Mathf.RoundToInt((waveIndex-6)*(waveIndex-6)*0.05f);
               enemy3timing = enemy3timing*0.96f;
               enemy2number -= 4;
               enemy0number = 19;
               enemy0timing = 2f;
               }

            if(waveIndex>13)
               {
               enemy4number = 1 + Mathf.RoundToInt((waveIndex-9)*(waveIndex-9)*0.04f);
               enemy4timing = enemy4timing*0.92f;
               enemy3number -= 4;
               enemy1number = 13;
               enemy1timing = 2.7f;
               }
            if(waveIndex>15)
               {
               enemy5number = 1 + Mathf.RoundToInt((waveIndex-13)*(waveIndex-13)*0.04f);
               enemy5timing = enemy5timing*0.93f;
               enemy4number -= 4;
               enemy2number = 11;
               enemy2timing = 3.5f;
               }

            StartCoroutine(SpawnEnemies0(enemy0number,enemy0timing)); 
            StartCoroutine(SpawnEnemies1(enemy1number,enemy1timing));
            StartCoroutine(SpawnEnemies2(enemy2number,enemy2timing));    
            StartCoroutine(SpawnEnemies3(enemy3number,enemy3timing));    
            StartCoroutine(SpawnEnemies4(enemy4number,enemy4timing));    
            StartCoroutine(SpawnEnemies5(enemy5number,enemy5timing));  
            yield return null;
              
   }

   IEnumerator SpawnEnemies0(int numberOfEnemies, float spawnTime)
   {    co1 = true;
        yield return new WaitForSeconds(1f);
       int j = 0;
            for (int i = 0; i < numberOfEnemies;i++)
                {
                    objectPoolBig2.SpawnEnemy(0);
                    j = i;
                    yield return new WaitForSeconds(spawnTime);
                   
                }
                 if(j == numberOfEnemies-1)
                {Debug.Log("Ive reached Max spawns");
                     co1 = false;  
                     CheckCoroutine();
                }
                
                
            
   }
    IEnumerator SpawnEnemies1(int numberOfEnemies, float spawnTime)
   {     co2 = true;
         yield return new WaitForSeconds(2f);
         if(numberOfEnemies != 0)
         {
                int j = 0;
                for (int i = 0; i < numberOfEnemies;i++)
                {
                    objectPoolBig2.SpawnEnemy(1);
                    j=i;
                    yield return new WaitForSeconds(spawnTime);
                  
                }
                  if(j == numberOfEnemies-1)
                {
                     co2 = false; 
                     CheckCoroutine(); 
                }
         }
         else
         {
            co2 = false; 
            CheckCoroutine(); 
         }
                
         
              
   }
    IEnumerator SpawnEnemies2(int numberOfEnemies, float spawnTime)
   {    co3 = true;
         yield return new WaitForSeconds(2.5f);
         if(numberOfEnemies != 0)
         {
         int j = 0;
                
                for (int i = 0; i < numberOfEnemies;i++)
                {
                    objectPoolBig2.SpawnEnemy(2);
                    j = i;
                    yield return new WaitForSeconds(spawnTime);
                }
                  if(j == numberOfEnemies-1)
                {
                     co3 = false; 
                      CheckCoroutine(); 
                }
         }
                 else
         {
            co3 = false; 
            CheckCoroutine(); 
         }
               
             
   }
    IEnumerator SpawnEnemies3(int numberOfEnemies, float spawnTime)
   {    co4 = true;
         yield return new WaitForSeconds(3f);
         if(numberOfEnemies != 0)
         {
                int j = 0;
                for (int i = 0; i < numberOfEnemies;i++)
                {
                    objectPoolBig2.SpawnEnemy(3);
                    j = i;
                    yield return new WaitForSeconds(spawnTime);
                    
                }
                if(j == numberOfEnemies-1)
                {
                     co4 = false;  
                      CheckCoroutine();
                }
         }
                 else
         {
            co4 = false; 
            CheckCoroutine(); 
         }
              
           
              
   }
    IEnumerator SpawnEnemies4(int numberOfEnemies, float spawnTime)
   {    co5 = true;
         yield return new WaitForSeconds(4f);
         if(numberOfEnemies != 0)
         {
                int j = 0;
                for (int i = 0; i < numberOfEnemies;i++)
                {
                    objectPoolBig2.SpawnEnemy(4);
                    j = i;
                    yield return new WaitForSeconds(spawnTime);
                   
                }
                 if(j >= numberOfEnemies-1)
                {
                     co5 = false;
                     CheckCoroutine();  
                }
         }
                 
         else
         {
            co5 = false; 
            CheckCoroutine(); 
         }
                
            
              
   }
    IEnumerator SpawnEnemies5(int numberOfEnemies, float spawnTime)
   {    co6 = true;
         yield return new WaitForSeconds(5f);
         if(numberOfEnemies != 0)
         {
                int j = 0;
                for (int i = 0; i < numberOfEnemies;i++)
                {
                    objectPoolBig2.SpawnEnemy(5);
                    j = i;
                    yield return new WaitForSeconds(spawnTime);
                    
                }
                if(j >= numberOfEnemies-1)
                {
                     co6 = false; 
                     CheckCoroutine(); 
                }
         }
                 else
         {
            co6 = false; 
            CheckCoroutine(); 
         }
   }
   void CheckCoroutine()
   {
      if(!co1 && !co2 && !co3 && !co4 && !co5 && !co6)
      {
         isWaveTimerOn = false;
         Debug.Log("Says the wave is over");
      }
   }

}
