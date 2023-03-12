using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PathManager : MonoBehaviour
{
    [SerializeField] int gridWith;
    [SerializeField] int gridHeight;
    [SerializeField] float cellSize;

    public Pathfinding pathfinding;
    public Grid grid;

    // When a obstacle is settled on the grid, this event is invoked.
    public UnityEvent obstacleEvent;
    bool isObstacleInvoked;

    [SerializeField] bool showGrid;

    private void Awake()
    {
        ReferansHolder.instance.pathManager = this;
        // This code instantiates a new pathfinding.
        pathfinding = new Pathfinding(gridWith, gridHeight, cellSize, transform.position);
        grid = pathfinding.GetGrid();
    }

    // This function invoke obstacle event and start 'ObstacleInvekeEnum' coroutine.
    public void InvokeObstacleEvent()
    {
        if (isObstacleInvoked == false)
        { 
            isObstacleInvoked = true;
            StartCoroutine(ObstacleInvekeEnum());
            obstacleEvent.Invoke();
        }
    }

    // This coroutine set 'isObstacleInvoked' to true.
    IEnumerator ObstacleInvekeEnum()
    {
        yield return null;
        isObstacleInvoked = false;
    }

    // This function return cell size.
    public float GetCellSize()
    {
        return cellSize;
    }

    // This function return grid with.
    public float GetGridWith()
    {
        return gridWith;
    }

    // This function return grid height.
    public float GetGridHeight()
    {
        return gridHeight;
    }

    // This functinon set path.
    List<GridNode> SetPath(Vector2 currentPos, Vector2 targetPos)
    {
        List<GridNode> path = pathfinding.FindClosestReachablePos(currentPos, targetPos);

        if (path != null)
        {
            for (int i = 0; i < path.Count - 1; i++)
            {
                Debug.DrawLine(transform.position + new Vector3(path[i].x, path[i].y) * 10f + Vector3.one * 5f, transform.position + new Vector3(path[i + 1].x, path[i + 1].y) * 10f + Vector3.one * 5f, Color.green, 5f);
            }
        }

        return path;
    }

    //This function creates a Vector3 list based on the world coordinates of the incoming start and end positions. 
    public List<Vector3> GetWayPoints(Vector2 currentPos, Vector2 targetPos)
    {
        List<GridNode> path = pathfinding.FindClosestReachablePos(currentPos, targetPos);

        List<Vector3> waypoints = new List<Vector3>();
        if (path != null)
        {
            for (int i = 0; i < path.Count; i++)
            {
                if (i == 0)
                    continue;
                waypoints.Add(grid.GridToWorldPosition(path[i].x, path[i].y, true));
            }
        }
        return waypoints;
    }

    // This function checks whether the grid is reachable or not.
    public bool IsReachable(Vector2 currentPos, Vector2 targetPos)
    {
        List<GridNode> path = SetPath(currentPos, targetPos);
        bool isReachable = false;
        if (path != null)
            isReachable = true;

        return isReachable;
    }

    private void OnDrawGizmos()
    {
        if (!showGrid)
            return;

        Gizmos.color = Color.blue;
        Vector2 transformPos = transform.position;
        for (int x = 0; x < gridWith; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {

                Gizmos.DrawLine(new Vector2(x, y) * cellSize + transformPos, new Vector2(x + 1, y) * cellSize + transformPos);
                Gizmos.DrawLine(new Vector2(x, y) * cellSize + transformPos, new Vector2(x, y + 1) * cellSize + transformPos);
            }
        }

        Gizmos.DrawLine(new Vector2(0, gridHeight) * cellSize + transformPos, new Vector2(gridWith, gridHeight) * cellSize + transformPos);
        Gizmos.DrawLine(new Vector2(gridWith, 0) * cellSize + transformPos, new Vector2(gridWith, gridHeight) * cellSize + transformPos);

        if (grid == null)
            return;
        for (int x = 0; x < grid.gridNodes.GetLength(0); x++)
        {
            for (int y = 0; y < grid.gridNodes.GetLength(1); y++)
            {
                if (grid.gridNodes[x, y].isFull)
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawCube(grid.GridToWorldPosition(x, y, true), Vector3.one * 16);
                }
            }
        }
    }
}
