using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class Graph
{
    public List<Node> nodes;
    public List<Edge> edges;

    public Graph(List<Node> nodeList, List<Edge> edgeList) 
    {
        nodes = nodeList;
        edges = edgeList;
    }
}

public class Node
{
    public Vector3 position;
    public int xcoord;
    public int ycoord;
    public List<Node> connections;
    public List<Tuple<int, int>> neighbours1 = new List<Tuple<int, int>>();
    public List<Tuple<int, int>> neighbours2 = new List<Tuple<int, int>>();
    public bool isSpawn;

    public Node(Vector3 pos, int x, int y, bool _isSpawn)
    {
        isSpawn = _isSpawn;
        position = pos;
        xcoord = x;
        ycoord = y;
        connections = new List<Node>();
        if (!isSpawn)
        {
            neighbours1 = new List<Tuple<int, int>>()
            {
                Tuple.Create(xcoord + 1, ycoord + 1),
                Tuple.Create(xcoord - 1, ycoord + 1),
                Tuple.Create(xcoord + 1, ycoord - 1),
                Tuple.Create(xcoord - 1, ycoord - 1)
            };
            neighbours2 = new List<Tuple<int, int>>()
            {
                Tuple.Create(xcoord + 1, ycoord),
                Tuple.Create(xcoord - 1, ycoord),
                Tuple.Create(xcoord, ycoord + 1),
                Tuple.Create(xcoord, ycoord - 1)
            };
        }
        else if (isSpawn)
        {
            neighbours1 = new List<Tuple<int, int>>()
            {
                Tuple.Create(xcoord + 1, ycoord + 1),
                Tuple.Create(xcoord + 1, ycoord + 2),
                Tuple.Create(xcoord + 2, ycoord + 1),
                Tuple.Create(xcoord + 2, ycoord + 2),
                Tuple.Create(xcoord - 1, ycoord + 1),
                Tuple.Create(xcoord - 1, ycoord + 2),
                Tuple.Create(xcoord - 2, ycoord + 1),
                Tuple.Create(xcoord - 2, ycoord + 2),
                Tuple.Create(xcoord + 1, ycoord - 1),
                Tuple.Create(xcoord + 1, ycoord - 2),
                Tuple.Create(xcoord + 2, ycoord - 1),
                Tuple.Create(xcoord + 2, ycoord - 2),
                Tuple.Create(xcoord - 1, ycoord - 1),
                Tuple.Create(xcoord - 1, ycoord - 2),
                Tuple.Create(xcoord - 2, ycoord - 1),
                Tuple.Create(xcoord - 2, ycoord - 2)
            };
            neighbours2 = new List<Tuple<int, int>>()
            {
                Tuple.Create(xcoord, ycoord + 1),
                Tuple.Create(xcoord, ycoord + 2),
                Tuple.Create(xcoord, ycoord - 1),
                Tuple.Create(xcoord, ycoord - 2),
                Tuple.Create(xcoord + 1, ycoord),
                Tuple.Create(xcoord + 2, ycoord),
                Tuple.Create(xcoord - 1, ycoord),
                Tuple.Create(xcoord - 2, ycoord)
            };
        }
    }
 }

public class Edge
{
    public Node startNode;
    public Node endNode;

    public Edge(Node start, Node end)
    {
        startNode = start;
        endNode = end;
    }
}