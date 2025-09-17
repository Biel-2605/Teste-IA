using UnityEngine;
using System.Collections.Generic;
using System.Diagnostics;

public enum NodeType
{
    Floor,
    Wall,
    Start,
    Goal
}

public class Node : MonoBehaviour
{
    [Header("Materials")]
    public Material actualWay;
    public Material blackListed;
    public Material finish;
    public Material floor;
    public Material possibleWay;
    public Material start;
    public Material wall;

    [Header("Node Properties")]
    public List<Node> neighbors = new List<Node>();
    public NodeType nodeType = NodeType.Floor;

    bool isClicked = false;

    public void OnNodeClicked()
    {
        if (nodeType != NodeType.Wall)
        {
            if (nodeType == NodeType.Start)
            {
                GetComponent<Renderer>().material = floor;
                nodeType = NodeType.Floor;
                GameObject.Find("GameManager").GetComponent<GameManager>().hasStart = false;
                
            }
            else if (nodeType == NodeType.Goal)
            {
                GetComponent<Renderer>().material = floor;
                nodeType = NodeType.Floor;
                GameObject.Find("GameManager").GetComponent<GameManager>().hasGoal = false;
            }
            else
            {
                if (!GameObject.Find("GameManager").GetComponent<GameManager>().hasStart)
                {
                    GetComponent<Renderer>().material = start;
                    nodeType = NodeType.Start;
                    GameObject.Find("GameManager").GetComponent<GameManager>().hasStart = true;
                }
                else if (!GameObject.Find("GameManager").GetComponent<GameManager>().hasGoal)
                {
                    GetComponent<Renderer>().material = finish;
                    nodeType = NodeType.Goal;
                    GameObject.Find("GameManager").GetComponent<GameManager>().hasGoal = true;
                }
            }
        }

        /*
        if ( !isClicked )
        {
            GetComponent<Renderer>().material = start;
            isClicked = true;
        }
        else
        {
            GetComponent<Renderer>().material = floor;
            isClicked = false;
        }
        */
    }
}
