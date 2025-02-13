using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.SceneManagement;
using Vector3 = UnityEngine.Vector3;
using Quaternion = UnityEngine.Quaternion;
using Unity.VisualScripting;
using Vector2 = UnityEngine.Vector2;
using System;
using TMPro;
using Gaia;

public class ButtonManager : MonoBehaviour
{
    [SerializeField] public GameObject[] selectedObjects;
    [SerializeField] GameObject[] selectedObjectsGhosts;
    GameObject[] ghostPool;
    GameObject spawnObject;
    GameObject spawnObjectGhost;
    [SerializeField] int spawnObjectIndex = 0;
    [SerializeField] GameObject buildMenu;
    bool isBuildMenuOpen = false;
    public bool IsBuildMenuOpen {get { return isBuildMenuOpen;}}
    [SerializeField] GameObject upgradeMenu;
     bool isUpgradeMenuOpen = false;
    public bool IsUpgradeMenuOpen {get { return isUpgradeMenuOpen;}}
     public int WhereOnIndex{get{return spawnObjectIndex;}}
     bool isPaused = false;
     public bool IsPaused{get{return isPaused;}}
     float originalTimeScale;
     CastleHealth castleHealth;
     Vector3 ghostLocation = new();
     public Vector3 GhostLocation{get{ return ghostLocation;}}
     bool showGhostIfPlaceable = false;
     Camera cameraMain;
     GameObject towerToUpgrade = null;
     public GameObject TowerToUpgrade {get { return towerToUpgrade;}}    
     Gold gold;
     [Tooltip("Percent that the towers total cost will sell for")]
     [SerializeField] float percentToSell = 0.8f;
     GameObject towerWaypoint;
     bool isBorderShowing = false;
     ShowTileBorder showTileBorder;
     TowerObjectPool towerObjectPool;
     GameObject targetTowerCheck;
     RectTransform upgradeMenuTransform;
    Canvas canvas;
    [SerializeField] TextMeshProUGUI statsText;
    [SerializeField] GameObject background;
    [SerializeField] RectTransform upgradeRangeButton;
    TextMeshProUGUI rangeUpgradeText;
    [SerializeField] RectTransform upgradeDMGButton;
    TextMeshProUGUI dmgUpgradeText;
    [SerializeField] RectTransform sellButton;
     TextMeshProUGUI sellText;
    float whatButtonIsLongest = 0;
    [SerializeField] GameObject pauseMenu;
    [SerializeField] GameObject controlsPic;
    [SerializeField] GameObject towerInfoPic;
    CatapultAim catapultAim;
    CameraController cameraController;

    

    void Awake()
    {   
        spawnObjectIndex = 0;
        GameObject findMainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        cameraMain = findMainCamera.GetComponent<Camera>();
        spawnObject = selectedObjects[spawnObjectIndex];
        originalTimeScale = Time.timeScale;
        castleHealth = FindAnyObjectByType<CastleHealth>();
        isBuildMenuOpen = false;
        isUpgradeMenuOpen = false;
        buildMenu.SetActive(false);
        upgradeMenu.SetActive(true);
        gold = FindAnyObjectByType<Gold>();
        showTileBorder = FindAnyObjectByType<ShowTileBorder>();
        towerObjectPool = FindAnyObjectByType<TowerObjectPool>();
        upgradeMenuTransform = upgradeMenu.GetComponent<RectTransform>();
        canvas = upgradeMenuTransform.GetComponentInParent<Canvas>();
        SpawnGhostPool();
        rangeUpgradeText = upgradeRangeButton.GetComponentInChildren<TextMeshProUGUI>();
        dmgUpgradeText = upgradeDMGButton.GetComponentInChildren<TextMeshProUGUI>();
        sellText = sellButton.GetComponentInChildren<TextMeshProUGUI>();
        catapultAim = FindAnyObjectByType<CatapultAim>();
    }
void Start()
{   
    statsText.text = $" Level: \nRange: \nDamage: "; 
    pauseMenu.SetActive(false);
    controlsPic.SetActive(false);
    towerInfoPic.SetActive(false);
    cameraController = FindAnyObjectByType<CameraController>();
}
    void Update()
    {  
        CheckForInput();
        UpdateGhost();
    }

    private void CheckForInput()
    {   
        if (Input.GetKeyDown(KeyCode.Escape) && !castleHealth.CastleDeath)
        {   if(isBuildMenuOpen)
            {
                OpenBuildMenu(false);
                pauseMenu.SetActive(false);
                catapultAim.StopCatapult();
            }
            else
            {
                PauseGame(isPaused);
                
            }
        }
        if(Input.GetKeyDown(KeyCode.B))
        {   if(!isBuildMenuOpen)
            {       
            OpenBuildMenu(true);
            catapultAim.StopCatapult();
            }
            else if(isBuildMenuOpen)
            {
                OpenBuildMenu(false);
            }
        }
        if(Input.GetKeyDown(KeyCode.C))
        {   
            isBorderShowing = !isBorderShowing;
            showTileBorder.ShowBorderA(isBorderShowing);
        }
        if(Input.GetMouseButtonDown(1))
        {    if(isBuildMenuOpen)
            {
              OpenBuildMenu(false);
            }
            catapultAim.StopCatapult();
        }
        if(Input.GetKeyDown(KeyCode.Q))
        {
            OpenBuildMenu(false);
            catapultAim.StartCatapult();
        }

    }
void UpdateGhost()
{   
    if(showGhostIfPlaceable && isBuildMenuOpen)
            {
                spawnObjectGhost.SetActive(true);
            }
            else{spawnObjectGhost.SetActive(false);}
    if(showGhostIfPlaceable)
        {   if(spawnObjectGhost.GetComponent<LineRenderer>() != null)
            {spawnObjectGhost.GetComponent<LineRenderer>().enabled = true;} 
            foreach(MeshRenderer mesh in spawnObjectGhost.GetComponentsInChildren<MeshRenderer>())
                {mesh.enabled = true;}
        }
    else
        {   if(spawnObjectGhost.GetComponent<LineRenderer>() != null)
            {spawnObjectGhost.GetComponent<LineRenderer>().enabled = false;}
            foreach(MeshRenderer mesh in spawnObjectGhost.GetComponentsInChildren<MeshRenderer>())
                  {mesh.enabled = false;}
        }
    spawnObjectGhost.transform.position = ghostLocation;
}
//called from Upgrade script on tower
public void OpenUpgradeMenu(GameObject targetTower, GameObject waypoint, Vector3 position)
{       
        if(targetTower.name == "Cannon")
        {
            cameraController.FreezeCamera(true);
        }            
        Vector3 adjustedPosition = cameraMain.WorldToScreenPoint(position);
        adjustedPosition.x *= canvas.GetComponent<RectTransform>().rect.width / cameraMain.pixelWidth;
        adjustedPosition.x += 60;
        adjustedPosition.y *= canvas.GetComponent<RectTransform>().rect.height / cameraMain.pixelHeight;
        adjustedPosition.y += 20;
        upgradeMenuTransform.anchoredPosition = adjustedPosition;
       
        towerToUpgrade = targetTower;
        if(towerToUpgrade != targetTowerCheck && targetTowerCheck != null)
        {   if(towerToUpgrade.name != "LineTurret")
            {
                targetTowerCheck.GetComponent<LineRenderer>().enabled = towerObjectPool.IsShowing;
            }
        }
        towerWaypoint = waypoint;
        isUpgradeMenuOpen = true;
        upgradeMenu.SetActive(true);

        towerToUpgrade.GetComponent<LineRenderer>().enabled = true;

        UpdateUpgradeMenu();
        IsFullyVisibleOnScreen(upgradeMenuTransform, cameraMain, upgradeMenuTransform.anchoredPosition);

        StartCoroutine(CloseUpgradeMenu(position));
}
public void UpdateUpgradeMenu()
{   
    Upgrade towerUpgrade = towerToUpgrade.GetComponent<Upgrade>();
    string name = towerToUpgrade.name;
    int level = towerUpgrade.NumberOfRangeUp + towerUpgrade.NumberOfDMGUp;
    float range = towerUpgrade.RangeAfterBuff;
    float damage = towerUpgrade.CurrentDamage;
    int sellPrice = Mathf.RoundToInt(towerUpgrade.TotalGoldWorth*percentToSell);
    int rangeUpgradePrice = towerUpgrade.CostOfRangeUp;
    int dmgUpPrice = towerUpgrade.CostOfDMGUp;
    whatButtonIsLongest = 0f; //to adjust the upgrade menu's total border based on the longest button or stats screen

    switch(name)
    {
        case "Ballista": 
            statsText.text = $"{name} Level: {level}\nRange: {Mathf.RoundToInt(range)}\nDamage: {Mathf.RoundToInt(damage)}";
                break;
        case "Flamethrower":  
            statsText.text = $"{name} Level: {level}\nRange: {Mathf.RoundToInt(range)}\nDamage: {Mathf.RoundToInt(damage)}";
                break;
        case "Gatling Ballista":  
            statsText.text = $"{name} Level: {level}\nRange: {Mathf.RoundToInt(range)}\nDamage: {Mathf.RoundToInt(damage)}";
                break;        
        case "Mirror": 
            float rampupTime = towerToUpgrade.GetComponent<MirrorDamage>().RampUpTime;
            statsText.text = $"{name} Level: {level}\nRange: {Mathf.RoundToInt(range)}\nMax Damage Cap: {Mathf.RoundToInt(damage)}\nRamp Up Time: {rampupTime}";
                break;
        case "Slammer":  
            statsText.text = $"{name} Level: {level}\nRange: {Mathf.RoundToInt(range)}\nDamage: {Mathf.RoundToInt(damage)}";
                break;
        case "Singer":  
            statsText.text = $"{name} Level: {level}\nRange: {Mathf.RoundToInt(range)}\nDamage Boost: {Mathf.RoundToInt(towerUpgrade.DamageModFromBuff*100)}%";
                break;            
        case "TomatoStand":  
            statsText.text = $"{name} Level: {level}\nRange: {Mathf.RoundToInt(range)}\nSlow: {Mathf.RoundToInt(100 -(towerUpgrade.BaseDamage*100))}%";
                break;
        case "Spotter":  
            statsText.text = $"{name} Level: {level}\nRange: {Mathf.RoundToInt(range)}\nRange Boost: {Mathf.RoundToInt(towerUpgrade.RangeMod*100)}%";
                break;
        case "LineTurret":  
            statsText.text = $"{name} Level: {level}";
                break;         
    }
    Rect existingRect = statsText.rectTransform.rect;
    RectTransform backgroundRect = background.GetComponent<RectTransform>();
    backgroundRect.sizeDelta = new Vector2(existingRect.width+30,existingRect.height+30);
    whatButtonIsLongest = backgroundRect.rect.width;

    rangeUpgradeText.text = $"Upgrade Range: {rangeUpgradePrice} Gold";
    upgradeRangeButton.sizeDelta = new Vector2(rangeUpgradeText.rectTransform.rect.width+30, rangeUpgradeText.rectTransform.rect.height+15);
    upgradeRangeButton.anchoredPosition = new Vector2(-15,-existingRect.height-15);
    if(upgradeRangeButton.rect.width >=  whatButtonIsLongest)
    {
        whatButtonIsLongest = upgradeRangeButton.rect.width;
    }
    
    dmgUpgradeText.text = $"Upgrade Damage: {dmgUpPrice} Gold";
    upgradeDMGButton.sizeDelta = new Vector2(dmgUpgradeText.rectTransform.rect.width+30, dmgUpgradeText.rectTransform.rect.height+15);
    upgradeDMGButton.anchoredPosition = new Vector2(-15,upgradeRangeButton.anchoredPosition.y-upgradeRangeButton.rect.height-10);
    if(upgradeDMGButton.rect.width >=  whatButtonIsLongest)
    {
        whatButtonIsLongest = upgradeDMGButton.rect.width;
    }


    sellText.text = $"Sell: {sellPrice} Gold";
    sellButton.sizeDelta = new Vector2(sellText.rectTransform.rect.width+30, sellText.rectTransform.rect.height+15);
    sellButton.anchoredPosition = new Vector2(0-15, upgradeDMGButton.anchoredPosition.y-upgradeDMGButton.rect.height-10);
    if(sellButton.rect.width >=  whatButtonIsLongest)
    {
        whatButtonIsLongest = sellButton.rect.width;
    }

    //Update upgrade menu width for the screen check
    upgradeMenuTransform.sizeDelta = new Vector2(whatButtonIsLongest, Mathf.Abs(sellButton.anchoredPosition.y-sellButton.rect.height-40));
    IsFullyVisibleOnScreen(upgradeMenuTransform, cameraMain, upgradeMenuTransform.anchoredPosition);

}

IEnumerator CloseUpgradeMenu(Vector3 screenPos) 
{   bool localUpgradeMenuOpen = true;
    targetTowerCheck = towerToUpgrade;
    while(localUpgradeMenuOpen)
    {   
        if(Input.GetKeyUp(KeyCode.Mouse1))
        {
            localUpgradeMenuOpen = false;
        }
      
        if(Vector3.Distance(Input.mousePosition, cameraMain.WorldToScreenPoint(screenPos)) > 420)
         {
            localUpgradeMenuOpen = false;
        }
        yield return null;
    }
    if(towerToUpgrade.name != "LineTurret")
    {towerToUpgrade.GetComponent<LineRenderer>().enabled = towerObjectPool.IsShowing;}
    upgradeMenu.SetActive(false);
    if(towerToUpgrade.name == "Cannon")
        {
            cameraController.FreezeCamera(false);
        }       
}

    void OpenBuildMenu(bool isBuildMenuOpenA)
    {
        if(isBuildMenuOpenA)
        {   
            isBuildMenuOpen = true;
            buildMenu.SetActive(true);
            upgradeMenu.SetActive(false);
            if(towerToUpgrade != null)
            {   if(towerToUpgrade.name != "LineTurret")
                {
                towerToUpgrade.GetComponent<LineRenderer>().enabled = towerObjectPool.IsShowing;
                }
            }
            if(showGhostIfPlaceable)
            {
                spawnObjectGhost.SetActive(true);
            }
            else{spawnObjectGhost.SetActive(false);}
        }
        else
        {   
            isBuildMenuOpen = false;
            buildMenu.SetActive(false);
            spawnObjectGhost.SetActive(false);
        }
    }
    public void PauseGame(bool isPauseda)
     {
        if(!isPauseda)
        {
            isPaused = true;
            originalTimeScale = Time.timeScale;
            Time.timeScale = 0f;
            if(!castleHealth.CastleDeath)
            {
                pauseMenu.SetActive(true);
            }
        }
        else
        {
            isPaused = false;
            pauseMenu.SetActive(false);
            Time.timeScale = originalTimeScale;
        }
     }
public void ButtonTowerSell()
{   
    int totalGoldWorth = towerToUpgrade.GetComponent<Upgrade>().TotalGoldWorth;
    int goldFromSell = Mathf.RoundToInt(totalGoldWorth*percentToSell);
    gold.AddToGold(goldFromSell);
    towerToUpgrade.SetActive(false);
    towerToUpgrade.transform.parent = towerObjectPool.transform;
    if(towerToUpgrade.name == "LineTurret")
    {
        towerToUpgrade.GetComponent<LineTurret>().SellTower();
    }
    else
    {
        towerWaypoint.GetComponent<Waypoint>().MakePlaceable();
        towerWaypoint.GetComponent<TileBorder>().UpdateSoldBorder();
    }    
    upgradeMenu.SetActive(false);
}
public void ButtonTowerRangeUpgrade()
{
    towerToUpgrade.GetComponent<Upgrade>().UpgradeRange();
   
}
public void ButtonTowerDMGUpgrade()
{
    towerToUpgrade.GetComponent<Upgrade>().UpgradeDMG();
   
}
    public void ButtonClickRestart()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex);
        Time.timeScale = originalTimeScale;
    }
    public void ButtonBallista()
    {
        spawnObjectIndex = 0;
        NextAfterButtonClick();
    }
    public void ButtonFlame()
    {  
        spawnObjectIndex = 1;
        NextAfterButtonClick();
    }
     public void ButtonGatling()
    {   
        spawnObjectIndex = 2;
         NextAfterButtonClick();
    }

         public void ButtonMirror()
    {   
        spawnObjectIndex = 3;
          NextAfterButtonClick();
    }

         public void ButtonSlammer()
    {   
        spawnObjectIndex = 4;
         NextAfterButtonClick();
    }

         public void ButtonSinger()
    {   
        spawnObjectIndex = 5;
         NextAfterButtonClick();
    }

           public void ButtonTomato()
    {   
        spawnObjectIndex = 6;
         NextAfterButtonClick();
    }

           public void ButtonSpotter()
    {   
        spawnObjectIndex = 7;
         NextAfterButtonClick();
    }
        public void ButtonLineTurret()
    {   
        spawnObjectIndex = 8;
         NextAfterButtonClick();
    }
        public void CannonTurret()
    {   
        spawnObjectIndex = 9;
         NextAfterButtonClick();
    }
private void NextAfterButtonClick()
    {
        if (spawnObjectGhost != ghostPool[spawnObjectIndex])
        { spawnObjectGhost.SetActive(false); }
        spawnObject = selectedObjects[spawnObjectIndex];
        spawnObjectGhost = ghostPool[spawnObjectIndex];
    }
    void SpawnGhostPool()
        {   
            ghostPool = new GameObject[selectedObjectsGhosts.Length];

            for(int i = 0; i < selectedObjectsGhosts.Length;i++)
            {
                ghostPool[i] = Instantiate(selectedObjectsGhosts[i], transform.position, Quaternion.identity,transform);
                ghostPool[i].SetActive(false);
            }
            spawnObjectGhost = ghostPool[0];
          
        }
    public void GhostPositionUpdate(Vector3 ghostCoordinates, bool canBePlaced)
    {   
        ghostLocation = ghostCoordinates;
        showGhostIfPlaceable = canBePlaced;
    }

    void IsFullyVisibleOnScreen(RectTransform rectTransform, Camera camera, Vector2 position)
    {   
        //Checks the upgrade menus rectangle to see if its in bounds of the screen and adjusts as required
        Vector3[] objectCorners = new Vector3[4];
        rectTransform.GetWorldCorners(objectCorners);

        Rect screenBounds = new Rect(0, 0, Screen.width, Screen.height);

        float xAdjust = 0f;
        float yAdjust = 0f;

        for(int i = 0; i < 4; i++)
        {
            Vector3 screenSpaceCorner = camera.WorldToScreenPoint(objectCorners[i]);
            if(!screenBounds.Contains(screenSpaceCorner))
            {  
                float xDif = Screen.width - screenSpaceCorner.x;
                float yDif = Screen.height - screenSpaceCorner.y;
                      
              if(xDif < 0 || xDif > Screen.width) //Checking if the difference is to left or right of the screen
              { 
                if(xDif > Screen.width)
                    {xDif -= Screen.width;}
                if(Mathf.Abs(xDif) > xAdjust)
                    {xAdjust = xDif;}
              }
              if(yDif < 0 || yDif > Screen.height)//Checking if the difference is to higher or lower than the screen
              {  
                if(yDif > Screen.height)
                    {yDif -= Screen.height;}
                if(Mathf.Abs(yDif) > yAdjust)
                {yAdjust = yDif;}
              }
            }
        
        }
        xAdjust *= canvas.GetComponent<RectTransform>().rect.width / cameraMain.pixelWidth; //ratio of the actual screen to the play screen
        yAdjust *= canvas.GetComponent<RectTransform>().rect.height / cameraMain.pixelHeight;
        
        upgradeMenuTransform.anchoredPosition = new Vector2(position.x+xAdjust, position.y+yAdjust);        
    }
    public void NextLevel()
    {
        int scenIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(scenIndex+1);
        Time.timeScale = originalTimeScale;
    }

    public void MainMenu()
    {
        SceneManager.LoadScene(0);
        Time.timeScale = originalTimeScale;
    }
    public void QuitGame()
    {
        Application.Quit();
    }
    public void Controls()
    {
        controlsPic.SetActive(true);
    }
    public void GoBack()
    {
        controlsPic.SetActive(false);
    }

    public void TowerInfo()
    {
        towerInfoPic.SetActive(true);
    }

    public void goBackTowerInfo()
    {
        towerInfoPic.SetActive(false);
    }
}
