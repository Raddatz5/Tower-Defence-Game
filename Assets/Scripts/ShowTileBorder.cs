using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowTileBorder : MonoBehaviour
{   
    public void ShowBorderA(bool value)
    {
        BroadcastMessage("ShowBorder",value,SendMessageOptions.DontRequireReceiver);
    }
}
