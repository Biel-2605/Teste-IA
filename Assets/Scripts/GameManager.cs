using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    [Header("Map Size")]
    public int height = 20;
    public int width = 20;
    public int numberOfWalls = 50;

    [Header("Node")]
    public GameObject node;
    public List<GameObject> nodes;


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
            Debug.Log("Clicou");
        }
    }

    private void HandleMouseClick()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            Node nodeComponent = hit.collider.GetComponent<Node>();
            if (nodeComponent != null)
            {
                nodeComponent.OnNodeClicked();
            }
        }
    }
}
