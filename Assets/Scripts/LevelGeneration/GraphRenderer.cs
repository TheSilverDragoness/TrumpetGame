using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphRenderer : MonoBehaviour
{
    [SerializeField]
    private GameObject nodeObj;
    private List<GameObject> nodeObjects = new List<GameObject>();

    [SerializeField]
    private GameObject edgeObj;
    private List<GameObject> edgeObjects = new List<GameObject>();

    private Graph graph;

    [SerializeField]
    private float connectionWidth;

    [SerializeField]
    private Material connectionMat;

    public void InitialiseGraphRender(Graph gr)
    {
        graph = gr;
        RenderGraph();
    }

    private void RenderGraph()
    {
        if (nodeObjects.Count != 0 || edgeObjects.Count != 0) 
        {
            DestroyGraph();
        }
        foreach (var node in graph.nodes)
        {
            GameObject nodeOb = Instantiate(nodeObj, node.position, Quaternion.identity);

            // change this to work using edges and not node.connections
            foreach (var connection in node.connections)
            {
                GameObject child = new GameObject("Connection");
                child.transform.parent = nodeOb.transform;
                LineRenderer lr = child.AddComponent<LineRenderer>();
                lr.positionCount = 2;
                lr.SetPosition(0, node.position);
                lr.SetPosition(1, connection.position);
                lr.endWidth = connectionWidth;
                lr.startWidth = connectionWidth;
                lr.material = connectionMat;
            }
            
            nodeObjects.Add(nodeOb);
        }
    }

    private void DestroyGraph()
    {
        Debug.Log("Node Count: " + nodeObjects.Count);
        foreach (var node in nodeObjects)
        {
            if (node != null)
            {
                Destroy(node);
                Debug.Log("Node Destroyed: " + node.name);
            }
        }
        ClearGraph();
    }

    private void ClearGraph()
    {
        nodeObjects.Clear();
        //edgeObjects.Clear();
        Debug.Log("Graph Cleared");
    }
}
