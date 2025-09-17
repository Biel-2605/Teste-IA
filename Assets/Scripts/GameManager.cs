using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum searchType
{
    BFS,
    DFS
}

public class GameManager : MonoBehaviour
{
    [Header("Map Size")]
    public int height = 20;
    public int width = 20;
    public int numberOfWalls = 50;

    [Header("Node")]
    public GameObject node;
    public List<GameObject> nodes;

    public List<GameObject> walls;
    [HideInInspector]
    public bool hasStart = false;
    [HideInInspector]
    public bool hasGoal = false;

    [Header("Search Type")]
    public searchType searchType = searchType.DFS;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GridGenerator.GenerateGrid(height, width, numberOfWalls, node);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            HandleMouseClick();
        }
    }
     

    private void HandleMouseClick()
    {
        // Cria um ray da câmera para a posição do mouse
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // Verifica se o ray atingiu algum objeto
        if (Physics.Raycast(ray, out hit))
        {
            // Verifica se o objeto atingido tem o componente Node
            Node nodeComponent = hit.collider.GetComponent<Node>();
            if (nodeComponent != null)
            {
                nodeComponent.OnNodeClicked();     
            }
        }
    }
}