using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid
{
    int width, height;
    public GridNode[,] gridNodes;
    float cellSize;
    Vector2 originPosition;

    public Grid(int _width, int _height, float _cellSize, Vector2 _originPosition)
    {
        width = _width;
        height = _height;
        cellSize = _cellSize;
        originPosition = _originPosition;

        gridNodes = new GridNode[width, height];
        CreateNode();
    }

    // This function creates a new grid node for each point in the gridNodes.
    void CreateNode()
    {
        for (int x = 0; x < gridNodes.GetLength(0); x++)
        {
            for (int y = 0; y < gridNodes.GetLength(1); y++)
            {
                gridNodes[x, y] = new GridNode(this, x, y);
            }
        }
    }

    // This function checks whether the cell is empty or not.
    public bool CheckValue(Vector2 worldPos)
    {
        bool isFull = false;
        Vector2 gridPos = WorldToGridPosition(worldPos);
        if (gridPos.x >= 0 && gridPos.y >= 0 && gridPos.x < width && gridPos.y < height)
        {
            isFull = gridNodes[(int)gridPos.x, (int)gridPos.y].isFull;
        }
        else
        {
            isFull = true;
        }
        return isFull;
    }

    // This function sets grid true or false. 
    public void SetValue(Vector2 worldPos, bool isFull)
    {
        Vector2 gridPos = WorldToGridPosition(worldPos);
        if (gridPos.x >= 0 && gridPos.y >= 0 && gridPos.x < width && gridPos.y < height)
        {
            gridNodes[(int)gridPos.x, (int)gridPos.y].isFull = isFull;
        }
    }

    // This function converts grid cordinate to world cordinate values.
    public Vector2 GridToWorldPosition(int x, int y, bool isCenter)
    {
        Vector2 pos = new Vector2(x, y) * cellSize + originPosition;
        if (isCenter)
            pos += Vector2.one * cellSize * .5f;
        return pos;
    }

    // This function converts world cordinate to grid cordinate values.
    public Vector2 WorldToGridPosition(Vector2 worldPos)
    {
        worldPos -= originPosition;
        int posX = Mathf.FloorToInt(worldPos.x / cellSize);
        int posY = Mathf.FloorToInt(worldPos.y / cellSize);
        return new Vector2(posX, posY);
    }
}
