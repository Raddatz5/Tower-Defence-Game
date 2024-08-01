using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckUIPanelBoundry : MonoBehaviour
{
    public bool IsFullyVisibleOnScreen(RectTransform rectTransform, Camera camera)
    {
        Vector3[] objectCorners = new Vector3[4];
        rectTransform.GetWorldCorners(objectCorners);

        Rect screenBounds = new Rect(0, 0, Screen.width, Screen.height);

        foreach(Vector3 corner in objectCorners)
        {
            Vector3 screenSpaceCorner = camera.WorldToScreenPoint(corner);
            if(!screenBounds.Contains(screenSpaceCorner))
            {
                return false;
            }
        }
        return true;
    }
}
