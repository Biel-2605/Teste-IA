using UnityEngine;

public class Node : MonoBehaviour
{
    [Header("Materials")]
    public Material actualWay;
    public Material blackList;
    public Material finish;
    public Material floor;
    public Material possibleWay;
    public Material start;
    public Material wall;

    bool isClicked = false;

    public void OnNodeClicked()
    {
        Debug.Log($"Node clicado: {gameObject.name}");

        if (!isClicked)
        {
            GetComponent<Renderer>().material = start;
            isClicked = true;
        }
        else
        {
            GetComponent<Renderer>().material = floor;
            isClicked = false;
        }
    }
}
