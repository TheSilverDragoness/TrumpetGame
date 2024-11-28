using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid
{
    public GridNode[,] grid;
    int gridSize = 0;

    public Grid (int size)
    {
        gridSize = size + 1;

        grid = new GridNode[gridSize, gridSize];
        
        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                grid[x, y] = new GridNode(x, y);
            }
        }
    }

    public Grid EditGrid(int xcoord,  int ycoord, int edit, Grid g) 
    {
        g.grid[xcoord, ycoord].type = edit;

        return g;
    }

    public List<GridNode> neighbourNodes (GridNode node)
    {
        List<GridNode> neighbours = new List<GridNode>();

        if (node.xCoord + 1 < gridSize)
            neighbours.Add(grid[node.xCoord + 1, node.yCoord]);

        if (node.xCoord - 1 >= 0)
            neighbours.Add(grid[node.xCoord - 1, node.yCoord]);

        if (node.yCoord + 1 < gridSize)
            neighbours.Add(grid[node.xCoord, node.yCoord + 1]);

        if (node.yCoord - 1 >= 0)
            neighbours.Add(grid[node.xCoord, node.yCoord - 1]);

        return neighbours;
    }
}


public class GridNode
{
    public int type = 0;

    public int xCoord;
    public int yCoord;

    public int gCost;
    public int hCost;

    public GridNode parent;

    public GridNode(int _xCoord, int _yCoord)
    {
        xCoord = _xCoord;
        yCoord = _yCoord;
    }

    public int fCost
    {
        get
        {
            return gCost + hCost;
        }
    }
}