using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseClicker : MonoBehaviour
{
    public LayerMask clickableLayers;
    bool lookingForSecondTower = false;
    GameObject secondTowerRef = null;

    // bool isMouseOver = false;

void Update() 
{   
    if(Input.GetMouseButtonDown(0))
    {   if(!lookingForSecondTower) 
        {   
            if(!EventSystem.current.IsPointerOverGameObject())
            {
                Ray ray1 = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray1, out RaycastHit hit1, Mathf.Infinity, clickableLayers))
                {  
                    Collider clickedCollider = hit1.collider;
                    Waypoint tile = clickedCollider.GetComponent<Waypoint>();
                    tile.CheckTile();
                }
            }
        }
        else 
        {
            if(!EventSystem.current.IsPointerOverGameObject())
            {
                Ray ray1 = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray1, out RaycastHit hit1, Mathf.Infinity, clickableLayers))
                {  
                    Collider clickedCollider = hit1.collider;
                    Waypoint tile = clickedCollider.GetComponent<Waypoint>();
                    tile.CheckTileSecondTower(secondTowerRef);
                }
            }
        }
    }
    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, clickableLayers))
    {
        Collider clickedCollider = hit.collider;
        Waypoint tile = clickedCollider.GetComponent<Waypoint>();
        tile.UpdateMouseOver();
    }
}
public void SecondTowerCheck(GameObject tower)
{
    secondTowerRef = tower;
    lookingForSecondTower = true;
}
public void FoundSecondTower()
{
    lookingForSecondTower = false;
}

//  private void OnMouseEnter()
//     {
//         isMouseOver = true;
//         // Handle mouse enter behavior (e.g., highlight the object)
//     }

//     private void OnMouseExit()
//     {
//         isMouseOver = false;
//         // Handle mouse exit behavior (e.g., remove highlight)
//     }
}
