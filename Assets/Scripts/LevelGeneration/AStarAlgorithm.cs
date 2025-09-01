using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarAlgorithm
{
    Grid grid;

    public AStarAlgorithm(Grid g)
    {
        grid = g;
    }

    public void FindPath(Node snode, Node tnode)
    {
        GridNode startNode = grid.grid[snode.xcoord, snode.ycoord];
        GridNode targetNode = grid.grid[tnode.xcoord, tnode.ycoord];

        List<GridNode> openSet = new List<GridNode>();
        HashSet<GridNode> closedSet = new HashSet<GridNode>();

        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            GridNode currentNode = openSet[0];
            for (int i = 1; i < openSet.Count; i++) 
            {
                if (openSet[i].fCost < currentNode.fCost || openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost)
                {
                    currentNode = openSet[i];
                }
            }
            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            if (currentNode == targetNode)
            {
                GetPath(startNode, targetNode);
                Debug.Log("Finished Generating path");
            }

            foreach (GridNode neighbour in grid.neighbourNodes(currentNode))
            {
                //Debug.Log("(" + currentNode.xCoord + "," +  currentNode.yCoord + ") + has neighbour at (" + neighbour.xCoord + "," + neighbour.yCoord + ")");
                if (closedSet.Contains(neighbour) || neighbour.type > 1)
                {
                    continue;
                }

                int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);
                if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                {
                    neighbour.gCost = newMovementCostToNeighbour;
                    neighbour.hCost = GetDistance(neighbour, targetNode);
                    neighbour.parent = currentNode;

                    if (!openSet.Contains(neighbour))
                    {
                        openSet.Add(neighbour);
                    }
                }
            }
        }
    }

    void GetPath(GridNode startNode, GridNode endNode)
    {
        List<GridNode> path = new List<GridNode>();
        GridNode currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        path.Add(startNode);

        foreach (GridNode n in path)
        {
            if (n.type == 0)
            {
                n.type = 1;
            }
        }

        Debug.Log("Generated Path between nodes at " + startNode.xCoord + ", " + startNode.yCoord + " and " + endNode.xCoord + ", " + endNode.yCoord);
    }

    int GetDistance(GridNode nodeA, GridNode nodeB)
    {
        int xDist = Mathf.Abs(nodeA.xCoord - nodeB.xCoord);
        int yDist = Mathf.Abs(nodeA.yCoord - nodeB.yCoord);

        return  xDist + yDist;
    }

    public Grid GetGrid()
    {
        return grid;
    }
}