using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.VisualScripting;
using UnityEngine;

public class PathFinder : MonoBehaviour
{
    [SerializeField] Vector2Int startCoordinates;
    public Vector2Int StartCoordinates { get { return startCoordinates; } }
    [SerializeField] Vector2Int destinationCoordinates;
    public Vector2Int DestinationCoordinates { get { return destinationCoordinates; } }

    Node startNode;
    Node destinationNode;
    Node currentSearchNode;

    Queue<Node> frontier = new Queue<Node>();
    Dictionary<Vector2Int, Node> reached = new Dictionary<Vector2Int, Node>();


    Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.right, Vector2Int.left, };
    GridManager gridManager;
    Dictionary<Vector2Int, Node> grid = new Dictionary<Vector2Int, Node>();
    Vector2Int coordinatesNewTower;
    public Vector2Int CoordinatesNewTower {get { return coordinatesNewTower; } }

    void Awake()
    {
        gridManager = FindObjectOfType<GridManager>();
        if (gridManager != null)
        {
            grid = gridManager.Grid;
            startNode = gridManager.Grid[startCoordinates];
            destinationNode = gridManager.Grid[destinationCoordinates];
            
        }
    }
    void Start()
    {
        GetNewPath();
    }

    public List<Node> GetNewPath()
    {
       return GetNewPath(startCoordinates);
    }

     public List<Node> GetNewPath(Vector2Int coordinates)
    {
        gridManager.ResetNodes();
        BredthFirstSearch(coordinates);
        return BuildPath();
    }

    void ExploreNeighbours()
    {
        List<Node> neighbours = new List<Node>();
        foreach (Vector2Int direction in directions)
        {
            Vector2Int neighbourCoords = currentSearchNode.coordinates + direction;
            if (grid.ContainsKey(neighbourCoords))
            {
                neighbours.Add(grid[neighbourCoords]);
            }

        }

        foreach (Node neighbour in neighbours)
        {
            if (!reached.ContainsKey(neighbour.coordinates) && neighbour.isWalkable)
            {
                neighbour.connectedTo = currentSearchNode;
                reached.Add(neighbour.coordinates, neighbour);
                frontier.Enqueue(neighbour);
            }
        }

    }

    void BredthFirstSearch(Vector2Int coordinates)
    {   
        startNode.isWalkable = true;
        destinationNode.isWalkable = true;

        frontier.Clear();
        reached.Clear();

        bool isRunning = true;

        frontier.Enqueue(grid[coordinates]);
        reached.Add(coordinates, grid[coordinates]);

        while (frontier.Count > 0 && isRunning)
        {
            currentSearchNode = frontier.Dequeue();
            currentSearchNode.isExplored = true;
            ExploreNeighbours();
            if (currentSearchNode.coordinates == destinationCoordinates)
            {
                isRunning = false;
            }
        }
    }

    List<Node> BuildPath()
    {
        List<Node> path = new List<Node>();
        Node currentNode = destinationNode;

        path.Add(currentNode);
        currentNode.isPath = true;

        while (currentNode.connectedTo != null)
        {
            currentNode = currentNode.connectedTo;
            path.Add(currentNode);
            currentNode.isPath = true;
        }
        path.Reverse();

        return path;
    }

    public bool WillBlockPath(Vector2Int coordinates)
    {
        if (grid.ContainsKey(coordinates))
        {
            bool previouseState = grid[coordinates].isWalkable;

            grid[coordinates].isWalkable = false;
            List<Node> newPath = GetNewPath();
            grid[coordinates].isWalkable = previouseState;

            if (newPath.Count <= 1)
            {
                GetNewPath();
                return true;
            }
        }
        return false;
    }

      public bool WillBlockPathMultiple(List<Vector2Int> List)
    {   List <bool> previouseStateList = new();
        List<Vector2Int> containsKeyList = new();
        // List<Node> newPath = new ();
       
       //sort which coordinates are within the grid
        foreach (Vector2Int coordinate in List)
        { if (grid.ContainsKey(coordinate))
            {
                containsKeyList.Add(coordinate);
            }
        }
        
        // check only coordinates that are in the grid
        if(containsKeyList.Count >0)
            {
                // temp block off all grid nodes in the provided array
                foreach (Vector2Int coordinate in containsKeyList)
                { if (grid.ContainsKey(coordinate))
                    {
                        previouseStateList.Add(grid[coordinate].isWalkable);
                        grid[coordinate].isWalkable = false;
                    }
                }
                
                //check to see if theres a path
                List<Node> newPath = GetNewPath();

                //put the grid nodes back to what they were
                for (int i = 0; i<containsKeyList.Count;i++)
                {   Vector2Int temp = containsKeyList[i];
                    grid[temp].isWalkable = previouseStateList[i];
                }
            
                //verify if path is blocked by the length of the path
                if (newPath.Count <= 1)
                        {
                            GetNewPath();
                            return true;
                        }
            }

        return false;
    }

 public void NotifyReceivers()
 {
    BroadcastMessage("RecalculatePath", false, SendMessageOptions.DontRequireReceiver);
 }

 public void UpdateCoordinatePlaceholder(Vector2Int coordinates)
 {
    coordinatesNewTower = coordinates;
 }

}
