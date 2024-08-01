using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using TMPro;
using System.Diagnostics.Tracing;
using Unity.VisualScripting;

[ExecuteAlways]
[RequireComponent(typeof(TextMeshPro))]
public class CoordinateLabeler : MonoBehaviour
{
    [SerializeField] Color defaultColor = Color.white;
    [SerializeField] Color blockedColor = Color.gray;
    [SerializeField] Color exploredColor = Color.yellow;
    [SerializeField] Color pathColor = new Color(1f, 0.5f, 0f);
    TextMeshPro label;
    Vector2Int localXYCoordinate = new Vector2Int();
    GridManager gridManager;


    void Awake()
    {
        gridManager = FindObjectOfType<GridManager>();
        label = GetComponent<TextMeshPro>();
        label.enabled = true;
        DisplayCoordinates();
    }

    void Update()
    {
        if (!Application.isPlaying)
        {
            DisplayCoordinates();
            UpdateObjectName();
        }

        ColorCoordinates();
        ToggleLabels();
    }

    void DisplayCoordinates()
    {
        if (gridManager == null) { return; }
        localXYCoordinate.x = Mathf.RoundToInt(transform.parent.position.x / gridManager.UnityGridSize);
        localXYCoordinate.y = Mathf.RoundToInt(transform.parent.position.z / gridManager.UnityGridSize);
        label.text = localXYCoordinate.x + "," + localXYCoordinate.y;
    }

    void UpdateObjectName()
    {
        transform.parent.name = localXYCoordinate.ToString();
    }

    void ColorCoordinates()
    {
        if (gridManager == null) { return; }

        Node node = gridManager.GetNode(localXYCoordinate);

        if (node == null) { return; }

        if (!node.isWalkable)
        {
            label.color = blockedColor;
        }
        else if (node.isPath)
        {
            label.color = pathColor;
        }
        else if (node.isExplored)
        {
            label.color = exploredColor;
        }
        else
        {
            label.color = defaultColor;
        }


    }
    void ToggleLabels()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            label.enabled = !label.IsActive();
        }
    }
}
