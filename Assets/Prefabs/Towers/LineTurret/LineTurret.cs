using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineTurret : MonoBehaviour
{
    Upgrade upgrade;
    [SerializeField] GameObject tower1;
    [SerializeField] GameObject tower1BuildMesh;
    [SerializeField] GameObject tower2;
    [SerializeField] Transform tower1AttachPoint;
    [SerializeField] Transform tower2AttachPoint;
    [SerializeField] float speed;
    MeshRenderer buildMesh1Renderer;
    bool tower2PositionFound = false;
    bool lookingForTower2 =false;
    public bool LookingForTower2 {get {return lookingForTower2;}}
    MouseClicker mouseClicker;
    PathFinder pathFinder;
    List <Vector2Int> coordinateList = new ();
    ButtonManager buttonManager;
    GameObject waypoint1;
    GameObject waypoint2;
    LineRenderer lineRenderer;

    void Awake()
    {
        upgrade = GetComponent<Upgrade>();
        buildMesh1Renderer = tower1BuildMesh.GetComponent<MeshRenderer>();
        mouseClicker = FindAnyObjectByType<MouseClicker>();
        pathFinder = FindAnyObjectByType<PathFinder>();
        buttonManager = FindAnyObjectByType<ButtonManager>();
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.enabled = false;
    }
    //TO DO LIST
    
    // gold.Withdraw(goldCost1);
    
    // add upgarde to line turret

    //note: upgrade has 1st waypoint assinged to it, not the 2nd
    void OnEnable()
    {   coordinateList.Clear();
        tower2PositionFound = false;
        tower1.SetActive(false);
        tower2.SetActive(false);
        buildMesh1Renderer.enabled = true;
        lineRenderer.enabled=true;
    }

    IEnumerator Setup()
    {   StartCoroutine(CloseIfBuildMenu());
        lookingForTower2 =true;
        mouseClicker.SecondTowerCheck(gameObject);
        lineRenderer.positionCount = 2;
        StartCoroutine(FollowAttachPoints());

        yield return new WaitUntil(() => tower2PositionFound);

        // Vector3 tower2Position = gridManager.GetPostitionFromCoordinates(tower2coordinates);
        buildMesh1Renderer.enabled = false;
        lineRenderer.material = lineRenderer.materials[1];

        tower1.SetActive(true);
        tower2.SetActive(true);

        mouseClicker.FoundSecondTower();
        pathFinder.NotifyReceivers();

        waypoint1.GetComponent<Waypoint>().BlockTile();
        waypoint2.GetComponent<Waypoint>().BlockTile();
        
        StopAllCoroutines();
        yield return null;
    }

//starts here when waypoint sees index 8 (line turret)
    public void StartSetup(Vector2Int coordinates, GameObject waypoint)
    {   
        waypoint1 = waypoint;
        coordinateList.Add(coordinates);
        StartCoroutine(Setup());
    }
    //called from waypoint
    public void FoundTower2(Vector3 coordinates, GameObject waypoint)
    {   waypoint2 = waypoint;
        tower2.transform.position = coordinates;
        tower2PositionFound =true;
        lookingForTower2 = false;
        StopCoroutine(CloseIfBuildMenu());
    }
    //called from waypoint before foundtower2
    public bool IsItBlocked(Vector2Int coordinates2)
    {   
        bool isItBlocked;
        coordinateList.Add(coordinates2);
        isItBlocked = pathFinder.WillBlockPathMultiple(coordinateList);
        if(isItBlocked){coordinateList.RemoveAt(1);}
        return isItBlocked;
    }


IEnumerator CloseIfBuildMenu()
{
    yield return new WaitUntil(() => !buttonManager.IsBuildMenuOpen);
     mouseClicker.FoundSecondTower();
    gameObject.SetActive(false);
    yield return null;
}   
IEnumerator FollowAttachPoints()
{
    while(lookingForTower2)
    {   tower2.transform.position = buttonManager.GhostLocation;
        lineRenderer.SetPosition(0,tower1AttachPoint.position);
        lineRenderer.SetPosition(1,tower2AttachPoint.position);
        yield return null;
    }
}

//called from buttonmanager
public void SellTower()
{   
    waypoint1.GetComponent<Waypoint>().MakePlaceable();
    waypoint1.GetComponent<TileBorder>().UpdateSoldBorder();

    waypoint2.GetComponent<Waypoint>().MakePlaceable();
    waypoint2.GetComponent<TileBorder>().UpdateSoldBorder();
}
}
