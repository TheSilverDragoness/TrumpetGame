using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateLayout : MonoBehaviour
{
    [SerializeField]
    private GameObject cube;

    public Grid grid;

    private AStarAlgorithm aStarAlgorithm;

    List<GameObject> cubeList = new List<GameObject>();
    public void InitialiseGrid(int gridSize)
    {
        Debug.Log("initialising grid...");
        grid = new Grid(gridSize);
        aStarAlgorithm = new AStarAlgorithm(grid);
    }

    public void AddNodesToGrid(List<Node> nodeList)
    {
        Debug.Log("Adding nodes to grid");
        foreach (Node node in nodeList)
        {
            grid = grid.EditGrid(node.xcoord, node.ycoord, 2, grid);
            Debug.Log("Node added");
        }
    }

    public void AddNodeNeighbours1ToGrid(List<Node> nodeList)
    {
        Debug.Log("Adding Node Neighbours to List");
        foreach (Node node in nodeList)
        {
            foreach (Tuple<int, int> coord in node.neighbours1)
            {
                grid = grid.EditGrid(coord.Item1, coord.Item2, 3, grid);
            }
        }
    }

    public void AddNodeNeighbours2ToGrid(List<Node> nodeList)
    {
        Debug.Log("Adding Node Neighbours to List");
        foreach (Node node in nodeList)
        {
            foreach (Tuple<int, int> coord in node.neighbours2)
            {
                grid = grid.EditGrid(coord.Item1, coord.Item2, 2, grid);
            }
        }
    }

    public void AddPaths(List<Edge> edgeList)
    {
        Debug.Log("Adding Paths");
        foreach (Edge edge in edgeList)
        {
            aStarAlgorithm.FindPath(edge.startNode, edge.endNode);
        }
    }

    public void ReturnGrid()
    {
        grid = aStarAlgorithm.GetGrid();
        Debug.Log("Grid Returned");
    }

    public void DisplayGrid()
    {
        int rows = grid.grid.GetLength(0);
        int cols = grid.grid.GetLength(1);

        for (int i = 0; i < rows; i++)
        {
            string row = "";
            for (int j = 0; j < cols; j++)
            {
                row += grid.grid[i, j].type + "  ";
            }
            Debug.Log(row);
        }
    }

    public void RenderGrid()
    {
        if (cubeList.Count > 0)
        {
            for (int i = cubeList.Count - 1; i >= 0; i--)
            {
                Destroy(cubeList[i]);
            }
            cubeList.Clear();
        }

        foreach (GridNode node in grid.grid)
        {
            Vector3 spawnLocation = new Vector3(node.xCoord - 25, 0, node.yCoord);
            GameObject spawnedCube = Instantiate(cube, spawnLocation, Quaternion.identity);
            cubeList.Add(spawnedCube);

            if (node.type == 1)
            {
                spawnedCube.GetComponent<MeshRenderer>().material.color = Color.green;
            }
            if (node.type == 2)
            {
                spawnedCube.GetComponent<MeshRenderer>().material.color = Color.red;
            }
            if (node.type == 3)
            {
                spawnedCube.GetComponent<MeshRenderer>().material.color = Color.blue;
            }
        }
    }
}
