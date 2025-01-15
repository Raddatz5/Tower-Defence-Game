using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using TMPro;
using Vector3 = UnityEngine.Vector3;
using UnityEngine;

public class EnemyWaves : MonoBehaviour
{
   [SerializeField] ObjectPoolBig2 objectPoolBig2;
   [SerializeField] Vector2Int secondaryCoordinates;
   [SerializeField] bool isDoubleLocation = false;
   [SerializeField] int waveReward = 60;
   [SerializeField] GameObject startWaveButton;
   [SerializeField] GameObject currentWave;
   [SerializeField] GameObject youWinScreen;
   TextMeshProUGUI waveText;
   TextMeshProUGUI currentWaveText;
       [Tooltip("First element is how many, second is timing between")]
   [SerializeField] float [] wave1number = new float [2];
          [Tooltip("First element is enemyBignumber (0 is basic), second is how many, third is timing between")]
      [SerializeField] float [] wave1special = new float [3];
   [SerializeField] float [] wave2number = new float [2];
      [SerializeField] float [] wave2special = new float [3];
   [SerializeField] float [] wave3number = new float [2];
      [SerializeField] float [] wave3special = new float [3];
   [SerializeField] float [] wave4number = new float [2];
      [SerializeField] float [] wave4special = new float [3];
   [SerializeField] float [] wave5number = new float [2];
      [SerializeField] float [] wave5special = new float [3];
   [SerializeField] float [] wave6number = new float [2];
      [SerializeField] float [] wave6special = new float [3];  
    [SerializeField] float [] wave7number = new float [2];
      [SerializeField] float [] wave7special = new float [3];
   [SerializeField] float [] wave8number = new float [2];
      [SerializeField] float [] wave8special = new float [3];
   [SerializeField] float [] wave9number = new float [2];
      [SerializeField] float [] wave9special = new float [3];
   [SerializeField] float [] wave10number = new float [2];
      [SerializeField] float [] wave10special = new float [3];
   int waveIndex =1;
   Gold gold;
   bool lastWave = false;
   GridManager gridManager;
   Vector3 secondLocation;

   void Start()
   {
   //  objectPoolBig2 = FindAnyObjectByType<ObjectPoolBig2>();
    startWaveButton.SetActive(true);
    waveText = startWaveButton.GetComponentInChildren<TextMeshProUGUI>();
    waveText.text  = "Start Wave: "+waveIndex;
    currentWaveText = currentWave.GetComponentInChildren<TextMeshProUGUI>();
    currentWave.SetActive(false);
    gold = FindAnyObjectByType<Gold>();
    gridManager = FindObjectOfType<GridManager>();
    secondLocation = gridManager.GetPostitionFromCoordinates(secondaryCoordinates);
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
            StartCoroutine(SpawnEnemies(wave1number)); 
            StartCoroutine(SpawnSpecialEnemies1(wave1special));
            if(isDoubleLocation)
            {
               StartCoroutine(SpawnEnemiesSecond(wave1number));
               StartCoroutine(SpawnSpecialEnemiesSecond(wave1special));
            }      
            break;

            case 2: 
            // startWaveButton.SetActive(false);
            StartCoroutine(SpawnEnemies(wave2number));   
            StartCoroutine(SpawnSpecialEnemies1(wave2special));
              if(isDoubleLocation)
            {
               StartCoroutine(SpawnEnemiesSecond(wave2number));
               StartCoroutine(SpawnSpecialEnemiesSecond(wave2special));
            }            
            break;

            case 3: 
            // startWaveButton.SetActive(false);
            StartCoroutine(SpawnEnemies(wave3number));    
            StartCoroutine(SpawnSpecialEnemies1(wave3special));
              if(isDoubleLocation)
            {
               StartCoroutine(SpawnEnemiesSecond(wave3number));
               StartCoroutine(SpawnSpecialEnemiesSecond(wave3special));
            }          
            break;

            case 4: 
            // startWaveButton.SetActive(false);
            StartCoroutine(SpawnEnemies(wave4number));
            StartCoroutine(SpawnSpecialEnemies1(wave4special));
               if(isDoubleLocation)
            {
               StartCoroutine(SpawnEnemiesSecond(wave4number));
               StartCoroutine(SpawnSpecialEnemiesSecond(wave4special));
            }           
            break;

            case 5: 
            // startWaveButton.SetActive(false);
            StartCoroutine(SpawnEnemies(wave5number));
            StartCoroutine(SpawnSpecialEnemies1(wave5special));
               if(isDoubleLocation)
            {
               StartCoroutine(SpawnEnemiesSecond(wave5number));
               StartCoroutine(SpawnSpecialEnemiesSecond(wave5special));
            }           
            break;

            case 6: 
            // startWaveButton.SetActive(false);
            StartCoroutine(SpawnEnemies(wave6number));
            StartCoroutine(SpawnSpecialEnemies1(wave6special));
               if(isDoubleLocation)
            {
               StartCoroutine(SpawnEnemiesSecond(wave6number));
               StartCoroutine(SpawnSpecialEnemiesSecond(wave6special));
            }           
            break;

            case 7: 
            // startWaveButton.SetActive(false);
            StartCoroutine(SpawnEnemies(wave7number));  
            StartCoroutine(SpawnSpecialEnemies1(wave7special));
               if(isDoubleLocation)
            {
               StartCoroutine(SpawnEnemiesSecond(wave7number));
               StartCoroutine(SpawnSpecialEnemiesSecond(wave7special));
            }             
            break;

            case 8: 
            // startWaveButton.SetActive(false);
            StartCoroutine(SpawnEnemies(wave8number)); 
            StartCoroutine(SpawnSpecialEnemies1(wave8special));
               if(isDoubleLocation)
            {
               StartCoroutine(SpawnEnemiesSecond(wave8number));
               StartCoroutine(SpawnSpecialEnemiesSecond(wave8special));
            }           
            break;

            case 9: 
            // startWaveButton.SetActive(false);
            StartCoroutine(SpawnEnemies(wave9number));   
            StartCoroutine(SpawnSpecialEnemies1(wave9special));
               if(isDoubleLocation)
            {
               StartCoroutine(SpawnEnemiesSecond(wave9number));
               StartCoroutine(SpawnSpecialEnemiesSecond(wave9special));
            }           
            break;

            case 10: 
            // startWaveButton.SetActive(false);
            StartCoroutine(SpawnEnemies(wave10number));   
            StartCoroutine(SpawnSpecialEnemies1(wave10special)); 
               if(isDoubleLocation)
            {
               StartCoroutine(SpawnEnemiesSecond(wave10number));
               StartCoroutine(SpawnSpecialEnemiesSecond(wave10special));
            }           
            break;
        }
   }

   IEnumerator SpawnEnemies(float [] values)
   {    
        startWaveButton.SetActive(false);
        currentWave.SetActive(true);
        currentWaveText.text  = "Wave: "+ waveIndex;
        waveIndex++;
        yield return new WaitForSeconds(1f);
       
            for (int i = 0; i < values[0];i++)
                {
                    objectPoolBig2.SpawnEnemy(0);
                    yield return new WaitForSeconds(values[1]);
                }
             StartCoroutine(WaitForEndOfWave());
           
            if(waveIndex ==11) 
            {
                lastWave = true;
            }         
   }
   IEnumerator WaitForEndOfWave()
   {
      yield return new WaitUntil(() =>objectPoolBig2.CurrentActiveEnemies.Count == 0);
      gold.AddToGold(waveReward);
      waveText.text  = "Start Wave: "+waveIndex;
      startWaveButton.SetActive(true);
      currentWave.SetActive(false);  
   }
   IEnumerator SpawnEnemiesSecond(float [] values)
   {     float teleportWaitTime = 0.05f;
         yield return new WaitForSeconds(1f);
       
            for (int i = 0; i < values [0];i++)
                { GameObject teleportMe = objectPoolBig2.SpawnEnemySecond(0);
                  yield return new WaitForSeconds(teleportWaitTime);
                  teleportMe.transform.position = secondLocation;
                  EnemyMover enemyMover;
                  enemyMover = teleportMe.GetComponent<EnemyMover>();
                  enemyMover.fromOther = true;
                  enemyMover.RecalculatePath(false);
                  yield return new WaitForSeconds(values[1]);
                }   
   }
    IEnumerator SpawnSpecialEnemies1(float [] values)
   {    
        yield return new WaitForSeconds(1f);
                
                for (int i = 0; i < values[1];i++)
                {
                    objectPoolBig2.SpawnEnemy(Mathf.RoundToInt(values[0]));
                    yield return new WaitForSeconds(values[2]);
                }
              
   }
   IEnumerator SpawnSpecialEnemiesSecond(float [] values)
   {     float teleportWaitTime = 0.05f;
         yield return new WaitForSeconds(1f);
       
            for (int i = 0; i < values[1];i++)
                { GameObject teleportMe = objectPoolBig2.SpawnEnemySecond(Mathf.RoundToInt(values[0]));
                  yield return new WaitForSeconds(teleportWaitTime);
                  teleportMe.transform.position = secondLocation;
                  EnemyMover enemyMover;
                  enemyMover = teleportMe.GetComponent<EnemyMover>();
                  enemyMover.fromOther = true;
                  enemyMover.RecalculatePath(false);
                  yield return new WaitForSeconds(values[2]);
                }   
   }

}
