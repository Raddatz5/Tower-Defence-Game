using System.Collections;
using System.Collections.Generic;
// using UnityEditor.EditorTools;
using UnityEngine;

public class GridManager : MonoBehaviour
{   
    [SerializeField] Vector2Int gridSize;
    Dictionary<Vector2Int, Node> grid = new Dictionary<Vector2Int, Node>();
    public Dictionary<Vector2Int, Node> Grid{get{return grid;}}
    [Tooltip("World Grid Size - Should match unity editor snap settings")]
    [SerializeField] int unityGridSize = 10;
    public int UnityGridSize {get{return unityGridSize;}}    
    void Awake()
    {
        CreateGrid();
    }

    public Node GetNode(Vector2Int coordinates)
    {   if(grid.ContainsKey(coordinates))
         { 
            return grid[coordinates]; 
         }
        return null;
    }

    public void BlockNode(Vector2Int coordinates)
    {
        if(grid.ContainsKey(coordinates))
        {
            grid[coordinates].isWalkable = false;
        }
    }
      public void UnBlockNode(Vector2Int coordinates)
    {
        if(grid.ContainsKey(coordinates))
        {
            grid[coordinates].isWalkable = true;
        }
    }

    public void ResetNodes()
    {
        foreach(KeyValuePair<Vector2Int, Node> entry in grid)
        {
            entry.Value.connectedTo = null;
            entry.Value.isExplored = false;
            entry.Value.isPath = false;
        }
    }

    public Vector2Int GetCoordinatesFromPosition(Vector3 position)
    {
        Vector2Int localXYCoordinate = new()
        {
            x = Mathf.RoundToInt(position.x / unityGridSize),
            y = Mathf.RoundToInt(position.z / unityGridSize)
        };

        return localXYCoordinate;
    }

     public Vector3 GetPostitionFromCoordinates(Vector2Int coordinates)
    {
        Vector3 position = new()
        {
            x = coordinates.x * unityGridSize,
            z = coordinates.y * unityGridSize,
        };

        return position;
    }

    void CreateGrid()
    {
        for(int x = 0; x < gridSize.x; x++)
        {
            for(int y = 0; y < gridSize.y; y++)
            {
                Vector2Int coordinates = new Vector2Int(x, y);
                grid.Add(coordinates, new Node(coordinates, true));
                
            }
        }
    }
}
