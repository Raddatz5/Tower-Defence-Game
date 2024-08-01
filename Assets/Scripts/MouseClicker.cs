using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseClicker : MonoBehaviour
{
    public LayerMask clickableLayers;

    // bool isMouseOver = false;

void Update() 
{   
    if(Input.GetMouseButtonDown(0))
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
    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, clickableLayers))
    {
        Collider clickedCollider = hit.collider;
        Waypoint tile = clickedCollider.GetComponent<Waypoint>();
        tile.UpdateMouseOver();
    }
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
