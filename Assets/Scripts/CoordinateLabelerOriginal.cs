using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using TMPro;
using System.Diagnostics.Tracing;

[ExecuteAlways]
[RequireComponent(typeof(TextMeshPro))]
public class CoordinateLabelerOriginal : MonoBehaviour
{   
    [SerializeField] Color defaultColor = Color.white;
    [SerializeField] Color blockedColor = Color.gray;
    TextMeshPro label;
    Vector2Int localXYCoordinate = new Vector2Int();
    Waypoint waypoint;

    void Awake()
    {
        label = GetComponent<TextMeshPro>();
        label.enabled = false;
        waypoint = GetComponentInParent<Waypoint>();
        DisplayCoordinates();
    }

        void Update()
    {
        if(!Application.isPlaying)
        {
            DisplayCoordinates();
            UpdateObjectName();
        }

        ColorCoordinates();
        ToggleLabels();
    }

    void DisplayCoordinates()
    {
        localXYCoordinate.x = Mathf.RoundToInt(transform.parent.position.x / 10);
        localXYCoordinate.y = Mathf.RoundToInt(transform.parent.position.z / 10);
        label.text = localXYCoordinate.x + "," + localXYCoordinate.y;
    }

    void UpdateObjectName()
    {
        transform.parent.name = localXYCoordinate.ToString();
    }

    void ColorCoordinates()
    {
        if (waypoint.IsPlaceable)
        {
            label.color = defaultColor;
        }
        else {
            label.color = blockedColor;
        }
    }
    void ToggleLabels()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            label.enabled =!label.IsActive();
        }
    }
}
