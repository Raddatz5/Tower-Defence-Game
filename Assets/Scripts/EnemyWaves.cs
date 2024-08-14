using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EnemyWaves : MonoBehaviour
{
   ObjectPoolBig2 objectPoolBig2;
   [SerializeField] int waveReward = 60;
   [SerializeField] GameObject startWaveButton;
   [SerializeField] GameObject currentWave;
   [SerializeField] GameObject youWinScreen;
   TextMeshProUGUI waveText;
   TextMeshProUGUI currentWaveText;
   [SerializeField] int wave1number;
      [SerializeField] int wave1special;
   [SerializeField] int wave2number;
      [SerializeField] int wave2special;
   [SerializeField] int wave3number;
      [SerializeField] int wave3special;
   [SerializeField] int wave4number;
      [SerializeField] int wave4special;
   [SerializeField] int wave5number;
      [SerializeField] int wave5special;
   [SerializeField] int wave6number;
      [SerializeField] int wave6special;
   [SerializeField] int wave7number;
      [SerializeField] int wave7special;
    [SerializeField] int wave8number;
       [SerializeField] int wave8special;
    [SerializeField] int wave9number;
       [SerializeField] int wave9special;
    [SerializeField] int wave10number;
       [SerializeField] int wave10special;
   int waveIndex =1;
   Gold gold;
   bool lastWave = false;



   void Start()
   {
    objectPoolBig2 = FindAnyObjectByType<ObjectPoolBig2>();
    startWaveButton.SetActive(true);
    waveText = startWaveButton.GetComponentInChildren<TextMeshProUGUI>();
    waveText.text  = "Start Wave: "+waveIndex;
    currentWaveText = currentWave.GetComponentInChildren<TextMeshProUGUI>();
    currentWave.SetActive(false);
    gold = FindAnyObjectByType<Gold>();
   }

   void Update()
   {
        if(objectPoolBig2.CurrentActiveEnemies.Count == 0 && lastWave)
             {
                currentWaveText.text  = "Woo!";
                youWinScreen.SetActive(true);
                startWaveButton.SetActive(false);
             }
   }

   public void StarNextWave()
   {
        switch (waveIndex)
        {
            case 1: 
            // startWaveButton.SetActive(false);
            StartCoroutine(SpawnEnemies(wave1number,2f)); 
            StartCoroutine(SpawnSpecialEnemies1(1,wave1special,0));      
            break;

            case 2: 
            // startWaveButton.SetActive(false);
            StartCoroutine(SpawnEnemies(wave2number,1.5f));   
            StartCoroutine(SpawnSpecialEnemies1(1,wave2special,1f));         
            break;

            case 3: 
            // startWaveButton.SetActive(false);
            StartCoroutine(SpawnEnemies(wave3number,1f));    
            StartCoroutine(SpawnSpecialEnemies1(1,wave3special,3f));       
            break;

            case 4: 
            // startWaveButton.SetActive(false);
            StartCoroutine(SpawnEnemies(wave4number,0.9F));
            StartCoroutine(SpawnSpecialEnemies1(1,wave4special,2.5f));      
            break;

            case 5: 
            // startWaveButton.SetActive(false);
            StartCoroutine(SpawnEnemies(wave5number,0.7f));
            StartCoroutine(SpawnSpecialEnemies1(1,wave5special,2.5f));      
            break;

            case 6: 
            // startWaveButton.SetActive(false);
            StartCoroutine(SpawnEnemies(wave6number,0.6f));
            StartCoroutine(SpawnSpecialEnemies1(1,wave6special,2.5f));      
            break;

            case 7: 
            // startWaveButton.SetActive(false);
            StartCoroutine(SpawnEnemies(wave7number,0.5f));  
            StartCoroutine(SpawnSpecialEnemies1(1,wave7special,2.2f));        
            break;

            case 8: 
            // startWaveButton.SetActive(false);
            StartCoroutine(SpawnEnemies(wave8number,0.5f)); 
            StartCoroutine(SpawnSpecialEnemies1(1,wave8special,2f));      
            break;

            case 9: 
            // startWaveButton.SetActive(false);
            StartCoroutine(SpawnEnemies(wave9number,0.5f));   
            StartCoroutine(SpawnSpecialEnemies1(2,wave9special,3f));      
            break;

            case 10: 
            // startWaveButton.SetActive(false);
            StartCoroutine(SpawnEnemies(wave10number,0.2f));   
            StartCoroutine(SpawnSpecialEnemies1(2,wave10special,1.5f));       
            break;
        }
   }

   IEnumerator SpawnEnemies(int numberOfEnemies, float spawnTime)
   {    
        startWaveButton.SetActive(false);
        currentWave.SetActive(true);
        currentWaveText.text  = "Wave: "+ waveIndex;
        waveIndex++;
        yield return new WaitForSeconds(1f);
       
            for (int i = 0; i < numberOfEnemies;i++)
                {
                    objectPoolBig2.SpawnEnemy(0);
                    yield return new WaitForSeconds(spawnTime);
                }
             
            gold.AddToGold(waveReward);
            waveText.text  = "Start Wave: "+waveIndex;
            startWaveButton.SetActive(true);
            currentWave.SetActive(false); 
            if(waveIndex ==11) 
            {
                lastWave = true;
            }         
   }
    IEnumerator SpawnSpecialEnemies1(int bigNumber, int numberOfSpecials, float specialSpawnTime)
   {    
        startWaveButton.SetActive(false);
        currentWave.SetActive(true);
        yield return new WaitForSeconds(1f);
       
         
                for (int i = 0; i < numberOfSpecials;i++)
                {
                    objectPoolBig2.SpawnEnemy(bigNumber);
                    yield return new WaitForSeconds(specialSpawnTime);
                }
              
   }

}
