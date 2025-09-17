using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class GridGenerator : MonoBehaviour
{
	static List<GameObject> nodes = new List<GameObject>();

	public static void GenerateGrid(int h, int w, int wall, GameObject node)
	{
		for (int i = 0; i < w; i++)
		{
			for (int j = 0; j < h; j++)
			{
				GameManager gameManager = FindFirstObjectByType<GameManager>();
				GameObject n = Instantiate(node, new Vector3(i - w/2, 0, j - h/2), Quaternion.identity);
				gameManager.nodes.Add(n);
				nodes.Add(n);
			}
			
		}
		RandomizeWalls(wall);
	}

	public static void RandomizeWalls(int n)
	{
		for (int i = 0; i < n; i++)
		{
			int index = Random.Range(0, nodes.Count);
			nodes[index].transform.Translate(Vector3.up);
			nodes[index].GetComponent<Renderer>().material = nodes[index].GetComponent<Node>().wall;
			nodes.Remove(nodes[index]);
		}
	}

}
