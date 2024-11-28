using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GenerateAdjacencyMatrix : MonoBehaviour
{
    private Graph graph;

    private GraphRenderer graphRenderer;
    private GenerateLayout layoutGenerator;
    private GenerateMesh meshGenerator;

    private int seed;

    [SerializeField]
    private int minNodes;
    [SerializeField]
    private int maxNodes;
    [SerializeField]
    private int minConnections;
    [SerializeField]
    private int maxConnections;
    [SerializeField]
    private int base1 = 2; 
    [SerializeField]
    private int base2 = 3;
    [SerializeField]
    private int gridSize = 18;
    [SerializeField]
    private GameController gameController;
    [SerializeField]
    private GameObject pcgUI;

    private Vector3 spawnCoords;

    private int matrixSize;
    private List<List<int>> adjMatrix;

    private string inputText;
    [SerializeField]
    private TMP_Text[] seedText;

    List<int> seq1 = new List<int>();
    List<int> seq2 = new List<int>();

    private List<Vector3> positions = new List<Vector3>();

    int attempts = 0;
    int maxAttempts = 100;

    public void Awake()
    {
        pcgUI.SetActive(true);
        graphRenderer = GetComponent<GraphRenderer>();
        layoutGenerator = GetComponent<GenerateLayout>();
        meshGenerator = GetComponent<GenerateMesh>();
    }
    public void InputGrabber(string str)
    {
        Debug.Log("Input Grabber String: " + str);
        inputText = str;
        Debug.Log("Input Seed Set to " + inputText);
    }

    private int GetSeed()
    {
        int randSeed = (int)System.DateTime.Now.Ticks;
        return randSeed;
    }

    private void CheckSeed()
    {
        if (!string.IsNullOrEmpty(inputText))
        {
            if (int.TryParse(inputText, out seed))
            {
                Debug.Log("parsing successful, seed: " + seed);
            }
            else
            {
                Debug.LogError("Input string was not in correct format");
            }
        }
        else
        {
            seed = GetSeed();
        }
    }

    private void DisplaySeed()
    {
        Debug.Log("Matrix seed: " + seed);
        foreach (TMP_Text outputText in seedText)
        {
            outputText.text = seed.ToString();
        }
    }

    public void GenerateMatrix()
    {
        CheckSeed();
        DisplaySeed();
        InitialiseAdjMatrix();
    }

    private void InitialiseAdjMatrix()
    {
        Debug.Log("Initialising Adjacency Matrix");

        UnityEngine.Random.InitState(seed);

        matrixSize = UnityEngine.Random.Range(minNodes, maxNodes + 1);
        Debug.Log("Matrix size: " + matrixSize);

        adjMatrix = new List<List<int>>();

        for (int i = 0; i < matrixSize; i++)
        {
            adjMatrix.Add(new List<int>());
            for (int j = 0; j < matrixSize; j++)
            {
                adjMatrix[i].Add(0);
            }
        }

        PopulateMatrix();
    }

    private void PopulateMatrix()
    {
        for (int i = 0; i < matrixSize; i++)
        {
            for (int k = i + 1; k < matrixSize; k++)
            {
                int j = UnityEngine.Random.Range(0, 2);
                adjMatrix[i][k] = j;
                adjMatrix[k][i] = j;
            }
        }

        DisplayMatrix();
        CreateGraph();
    }

    private void DisplayMatrix()
    {
        Debug.Log("Generated Matrix:");
        for (int i = 0; i < matrixSize; i++)
        {
            Debug.Log(string.Join(" ", adjMatrix[i]));
        }
    }

    private void CreatePositions()
    {
        positions.Clear();
        seq1.Clear();
        seq2.Clear();
        HaltonSequence hseq = new HaltonSequence();
        seq1 = hseq.HaltonSeq(matrixSize, base1, gridSize);
        seq2 = hseq.HaltonSeq(matrixSize, base2, gridSize);

        
        for (int i = 0; i < seq1.Count; i++)
        {
            positions.Add(new Vector3(seq1[i], seq2[i], 0));
        }
    }
    
    private void GenerateValidPositions()
    {
        bool isValid1 = false;
        bool isValid2 = false;
        attempts = 0;

        while ((!isValid1 || !isValid2) && attempts < maxAttempts)
        {
            attempts++;
            CreatePositions();
            isValid1 = CheckSequence1(positions);
            isValid2 = CheckSequence2(positions);
        }
        if (!isValid2 && !isValid1 && attempts >= maxAttempts)
        {
            Debug.LogError("MAX ATTEMPTS EXCEEDED");
        }
    }

    private void CreateGraph()
    {
        Debug.Log("Creating Graph");
        List<Node> nodes = new List<Node>();
        List<Edge> edges = new List<Edge>();
        
        GenerateValidPositions();
        //CreatePositions();

        Debug.Log("Positions Count: " + positions.Count);
        Debug.Log("Sequence 1 Count: " + seq1.Count);
        Debug.Log("Sequence 2 Count: " + seq2.Count);

        for (int i = 0; i < matrixSize; i++)
        {
            bool isSpawn = (positions[i] == spawnCoords);
            nodes.Add(new Node(positions[i], seq1[i], seq2[i], isSpawn));
        }
        
        if (nodes.Count != matrixSize)
        {
            Debug.LogError("Node list count does not match matrix size");
            Debug.LogError("Node list = " +  nodes.Count);
            Debug.LogError("matrix size = " + matrixSize);
        }
        else
        {
            Debug.Log("node list successfuly created");
        }

        for (int i = 0; i < matrixSize; i++)
        {
            for (int k = 0; k < matrixSize; k++)
            {
                if (adjMatrix[i][k] == 1)
                {
                    nodes[i].connections.Add(nodes[k]);
                }
            }
        }

        ApplyRule(nodes);

        List<Tuple<Node, Node>> pairList = new List<Tuple<Node, Node>>();

        foreach (Node a in nodes)
        {
            foreach (Node b in a.connections)
            {
                if (!ContainsNodePair(a, b, pairList))
                {
                    pairList.Add(Tuple.Create(a, b));
                    edges.Add(new Edge(a, b));
                }
            }
        }

        foreach (Edge edge in edges)
        {
            Debug.Log("Edge between " + edge.startNode + " and " + edge.endNode);
        }
        graph = new Graph(nodes, edges);
        Debug.Log("Graph Created");
        //graphRenderer.InitialiseGraphRender(graph);
        layoutGenerator.InitialiseGrid(gridSize);
        layoutGenerator.AddNodeNeighbours1ToGrid(nodes);
        layoutGenerator.AddPaths(edges);
        layoutGenerator.ReturnGrid();
        layoutGenerator.AddNodesToGrid(nodes);
        layoutGenerator.AddNodeNeighbours2ToGrid(nodes);
        layoutGenerator.DisplayGrid();
        //layoutGenerator.RenderGrid();
        Grid grid = layoutGenerator.grid;
        meshGenerator.GenerateWalls(grid);
        meshGenerator.CreatePlayerSpawn((int)spawnCoords.x, (int)spawnCoords.y, gameController);
        meshGenerator.CreateSpawnRoomObstacle((int)spawnCoords.x, (int)spawnCoords.y);
        meshGenerator.CreateNavMesh(gridSize);
        meshGenerator.SpawnPlayer(gameController);
        pcgUI.SetActive(false);
        gameController.GameStart();
    }

    private void ApplyRule(List<Node> nl)
    {
        if (minConnections == 0 || maxConnections == 0)
        {
            return;
        }

        foreach (Node node in nl)
        {
            for (int i = node.connections.Count - 1; i >= 0; i--)
            {
                Node connectedNode = node.connections[i];

                if (node.connections.Count !> maxConnections && connectedNode.connections.Count !> minConnections)
                {
                    continue;
                }

                node.connections.Remove(connectedNode);
                connectedNode.connections.Remove(node);

                if (node.connections.Count <= maxConnections)
                {
                    break;
                }
            }
        }
        foreach (Node node in nl)
        {
            while (node.connections.Count < minConnections)
            {
                for (int i = 0; i < nl.Count; i++)
                {
                    Node potentialConnection = nl[i];
                  
                    if (node.connections.Contains(potentialConnection) || node == potentialConnection)
                    {
                        continue;
                    }
                    
                    node.connections.Add(potentialConnection);
                    potentialConnection.connections.Add(node);

                    Debug.Log("Connections: " + node.connections.Count);

                    if (node.connections.Count >= minConnections)
                    {
                        break;
                    }
                    
                }
            }
        }

        foreach (Node node in nl)
        {
            Debug.Log(node + " has connections: " + node.connections.Count);
        }
    }

    bool ContainsNodePair(Node node1, Node node2, List<Tuple<Node, Node>> list)
    {
        bool containsNodes = false;
        Tuple<Node, Node> t1 = Tuple.Create(node1, node2);
        Tuple<Node, Node> t2 = Tuple.Create(node2, node1);

        if (list.Contains(t1) || list.Contains(t2))
        {
            containsNodes = true;
        }

        return containsNodes;
    }

    public bool CheckSequence1(List<Vector3> sequence)
    {
        bool isValid = true;
        for (int i = 0; i < sequence.Count; i++)
        {
            for (int j = 0; j < sequence.Count; j++)
            {
                if (i == j)
                {
                    continue;
                }
                if (!CheckDistance(sequence[i], sequence[j], 5))
                {
                    isValid = false;
                    break;
                }
            }
            if (isValid)
            {
                spawnCoords = sequence[i];
                Debug.Log("Spawn Coords : " + sequence[i]);
                return true;
            }
        }
        return false;
    }

    public bool CheckSequence2(List<Vector3> sequence)
    {
        bool isValid = true;
        for (int i = 0; i < sequence.Count; i++)
        {
            for (int j = i + 1; j < sequence.Count; j++)
            {
                if (!CheckDistance(sequence[i], sequence[j], 4))
                {
                    isValid = false;
                }
            }
        }
        return isValid;
    }

    private bool CheckDistance(Vector3 position1, Vector3 position2, int minDistance)
    {
        float xDist = Mathf.Abs(position1.x - position2.x);
        float yDist = Mathf.Abs(position1.y - position2.y);

        return (xDist >= minDistance || yDist >= minDistance);
    }
}
