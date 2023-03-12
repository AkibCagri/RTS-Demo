using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding
{
    // Movement Cost according to A* algorithm.
    const int SRAIGHT_COST = 10;
    const int DIAGONAL_COST = 14;

    Grid grid;

    // searching list
    List<GridNode> openList = new List<GridNode>();

    // searched list
    HashSet<GridNode> closeList = new HashSet<GridNode>();

    [SerializeField] int closeSearchCount = 3;

    public Pathfinding(int _width, int _height, float _cellSize, Vector2 _originPosition)
    {
        // This code instantiates a new grid.
        grid = new Grid(_width, _height, _cellSize, _originPosition);
    }

    public Grid GetGrid()
    {
        return grid;
    }

    // This function find closest reachable path using find path funtion.
    public List<GridNode> FindClosestReachablePos(Vector2 startPos, Vector2 endPos)
    {
        Vector2 gridPos = grid.WorldToGridPosition(endPos);
        int posX = (int)gridPos.x;
        int posY = (int)gridPos.y;
        int searchRange = 0;

        while (searchRange <= closeSearchCount)
        {
            List<GridNode> currentPath = null;
            for (int x = posX - searchRange; x < posX + searchRange + 1; x++)
            {
                for (int y = posY - searchRange; y < posY + searchRange + 1; y++)
                {
                    // If x and y are out of borders, this code return.
                    bool outX = x - 1 < -1 || x + 1 >= grid.gridNodes.GetLength(0) + 1;
                    bool outY = y - 1 < -1 || y + 1 >= grid.gridNodes.GetLength(1) + 1;

                    if (outX && outY)
                        return null;

                    // If x or y are out of borders, this code continue to loop.
                    if (outX)
                        continue;
                    if (outY)
                        continue;

                    // If a grid has been searched, this code prevents it from being searched again,
                    if (x > posX - searchRange && x < posX + searchRange && y > posY - searchRange && y < posY + searchRange)
                        continue;

                    Vector2 currentEndPos = grid.GridToWorldPosition(x, y, true);

                    // If grid is empty, this code search a path.
                    if (grid.CheckValue(currentEndPos) == false)
                        currentPath = FindPath(startPos, currentEndPos);

                    //If current path is not null, this code return current path.
                    if (currentPath != null)
                    {
                        return currentPath;
                    }
                }
            }

            // End of for loops search range is increased.
            searchRange++;
        }
        return null;
    }

    // This function find shortest path.
    public List<GridNode> FindPath(Vector2 startPos, Vector2 endPos)
    {
        // Setting first values.
        startPos = grid.WorldToGridPosition(startPos);
        endPos = grid.WorldToGridPosition(endPos);
        int startX = (int)startPos.x;
        int startY = (int)startPos.y;
        int endX = (int)endPos.x;
        int endY = (int)endPos.y;
        GridNode startNode = grid.gridNodes[startX, startY];
        GridNode endNode = grid.gridNodes[endX, endY];
        openList = new List<GridNode> { startNode };
        closeList = new HashSet<GridNode>();

        for (int x = 0; x < grid.gridNodes.GetLength(0); x++)
        {
            for (int y = 0; y < grid.gridNodes.GetLength(1); y++)
            {
                GridNode gridNode = grid.gridNodes[x, y];
                gridNode.gCost = int.MaxValue;
                gridNode.CalculateFCost();
                gridNode.cameFromNode = null;
            }
        }

        startNode.gCost = 0;
        startNode.hCost = CalculateDistanceCost(startNode, endNode);
        startNode.CalculateFCost();

        // Searching loop
        while (openList.Count > 0)
        {
            // Finding path node which have lowest F cost to define next path point.
            GridNode currentNode = GetLowestFCostNode(openList);
            if (currentNode == endNode)
            {
                // if there is a reachable path, the path is calculated.
                return CalculatePath(endNode);
            }

            // The node which has lowest F cost remove form open list and add to close list.
            openList.Remove(currentNode);
            closeList.Add(currentNode);

            // "Finding a new current node from one of the neighbours of the current node
            foreach (GridNode neighbourNode in GetNeighbourList(currentNode))
            {
                // if close list has the node, node are eliminated.
                if (closeList.Contains(neighbourNode))
                    continue;

                // Full nodes are eliminated.
                if (neighbourNode.isFull)
                {
                    closeList.Add(neighbourNode);
                    continue;
                }

                // G cost of neighbour node is calculated accarding to current node.
                int temporaryGCost = currentNode.gCost + CalculateDistanceCost(currentNode, neighbourNode);
                // if new G cost lower than old G cost, neighbour node is added to open list.
                if (temporaryGCost < neighbourNode.gCost)
                {
                    neighbourNode.cameFromNode = currentNode;
                    neighbourNode.gCost = temporaryGCost;
                    neighbourNode.hCost = CalculateDistanceCost(neighbourNode, endNode);
                    neighbourNode.CalculateFCost();

                    if (!openList.Contains(neighbourNode))
                    {
                        openList.Add(neighbourNode);
                    }
                }
            }
        }
        return null;
    }

    // This function find neighburs of current node.
    List<GridNode> GetNeighbourList(GridNode currentNode)
    {
        List<GridNode> neighbourList = new List<GridNode>();
        if (currentNode.x - 1 >= 0)
        {
            neighbourList.Add(grid.gridNodes[currentNode.x - 1, currentNode.y]);
            if (currentNode.y - 1 >= 0)
                neighbourList.Add(grid.gridNodes[currentNode.x - 1, currentNode.y - 1]);
            if (currentNode.y + 1 < grid.gridNodes.GetLength(1))
                neighbourList.Add(grid.gridNodes[currentNode.x - 1, currentNode.y + 1]);
        }

        if (currentNode.x + 1 < grid.gridNodes.GetLength(0))
        {
            neighbourList.Add(grid.gridNodes[currentNode.x + 1, currentNode.y]);
            if (currentNode.y - 1 >= 0)
                neighbourList.Add(grid.gridNodes[currentNode.x + 1, currentNode.y - 1]);
            if (currentNode.y + 1 < grid.gridNodes.GetLength(1))
                neighbourList.Add(grid.gridNodes[currentNode.x + 1, currentNode.y + 1]);
        }

        if (currentNode.y - 1 >= 0)
            neighbourList.Add(grid.gridNodes[currentNode.x, currentNode.y - 1]);

        if (currentNode.y + 1 < grid.gridNodes.GetLength(1))
            neighbourList.Add(grid.gridNodes[currentNode.x, currentNode.y + 1]);

        return neighbourList;
    }

    // This function calculate according to data.
    List<GridNode> CalculatePath(GridNode endNode)
    {
        List<GridNode> path = new List<GridNode>();
        path.Add(endNode);
        GridNode currentNode = endNode;
        while (currentNode.cameFromNode != null)
        {
            path.Add(currentNode.cameFromNode);
            currentNode = currentNode.cameFromNode;
        }
        path.Reverse();
        return path;
    }

    // This function calculate calculate H cost according to A* algorithm.
    int CalculateDistanceCost(GridNode a, GridNode b)
    {
        int distanceX = Mathf.Abs(a.x - b.x);
        int distanceY = Mathf.Abs(a.y - b.y);
        int remaining = Mathf.Abs(distanceX - distanceY);
        return Mathf.Min(distanceX, distanceY) * DIAGONAL_COST + remaining * SRAIGHT_COST;
    }

    // This function finds the node which has the lowest F cost in the list passed as a parameter.
    GridNode GetLowestFCostNode(List<GridNode> gridNodeList)
    {
        GridNode LowestFCostNode = gridNodeList[0];
        for (int i = 0; i < gridNodeList.Count; i++)
        {
            if (gridNodeList[i].fCost < LowestFCostNode.fCost)
            {
                LowestFCostNode = gridNodeList[i];
            }
        }
        return LowestFCostNode;
    }


}
