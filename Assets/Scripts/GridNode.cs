using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// This class represents a single cell of a grid, which stores its own data.
public class GridNode
{
    Grid grid;
    public int x, y;
    public int gCost, hCost, fCost;
    public bool isFull;
    public GridNode cameFromNode;

    public GridNode(Grid _grid, int _x, int _y)
    {
        grid = _grid;
        x = _x;
        y = _y;
    }

    public void CalculateFCost()
    {
        fCost = gCost + hCost;
        
    }
}
