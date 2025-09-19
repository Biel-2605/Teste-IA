using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

public class DFS : MonoBehaviour
{
    public static void PerformSearchStep()
    {
        UnityEngine.Debug.Log("Buscando DFS");

        GameManager gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        Data data = GameObject.Find("GameManager").GetComponent<Data>();
        Node currentNode = data.stackDFS.Peek();

        FindNeighbor(currentNode);
    }

    static void FindNeighbor(Node currentNode)
    {
        HashSet<Node> visitedNodes = GameObject.Find("GameManager").GetComponent<Data>().visitedNodes;
        bool goalFound = false;
        foreach (Node neighbor in currentNode.neighbors)
        {
            if(neighbor == GameObject.Find("GameManager").GetComponent<GameManager>().goal)
            {
                goalFound = true;
                GameObject.Find("GameManager").GetComponent<GameManager>().isSearching = false;
                visitedNodes.Add(neighbor);
                GameObject.Find("GameManager").GetComponent<Data>().stackDFS.Push(neighbor);
                break;
            }
        }
        if (!goalFound)
        {
            bool neighborFound = false;
            foreach(Node neighbor in currentNode.neighbors)
            {

                if (!visitedNodes.Contains(neighbor))
                {
                    visitedNodes.Add(neighbor);
                    GameObject.Find("GameManager").GetComponent<Data>().stackDFS.Push(neighbor);
                    neighborFound = true;
                    currentNode.GetComponent<Renderer>().material. = current.possibleWay;
                }
            }
        }
    }
}