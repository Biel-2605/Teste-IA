using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.Properties;

// Classe que implementa o algoritmo de Busca em Profundidade (Depth-First Search)
public class DFS : MonoBehaviour
{
    // M�todo est�tico que executa um passo da busca DFS
    public static void PerformSearchStep()
    {
        // Obt�m refer�ncias aos componentes necess�rios do GameManager
        GameManager gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        Data data = GameObject.Find("GameManager").GetComponent<Data>();
        
        // Pega o n� atual do topo da pilha (sem remov�-lo)
        Node currentNode = data.stackDFS.Peek();

        // Chama o m�todo para encontrar vizinhos do n� atual
        FindNeighbor(currentNode);
    }

    // M�todo est�tico que procura vizinhos v�lidos do n� atual
    static void FindNeighbor(Node currentNode)
    {
        // Obt�m o conjunto de n�s j� visitados
        HashSet<Node> visitedNodes = GameObject.Find("GameManager").GetComponent<Data>().visitedNodes;
        GameManager gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        
        // Se o n� atual n�o � o n� inicial, pinta-o com material de "caminho poss�vel"
        if(currentNode.nodeType != NodeType.Start)
            currentNode.GetComponent<Renderer>().material = currentNode.possibleWay;
            
        bool goalFound = false; // Flag para indicar se o objetivo foi encontrado
        
        // Primeiro, verifica se algum vizinho � o n� objetivo
        foreach (Node neighbor in currentNode.neighbors)
        {
            // Se encontrou o objetivo
            if (neighbor == gameManager.goal)
            {
                goalFound = true;                    // Marca que o objetivo foi encontrado
                gameManager.isSearching = false;     // Para a busca
                visitedNodes.Add(neighbor);          // Adiciona o objetivo aos visitados
                GameObject.Find("GameManager").GetComponent<Data>().stackDFS.Push(neighbor); // Adiciona � pilha
                
                // Cria o caminho final para anima��o
                List<Node> finalPath = new List<Node>();
                
                // Converte a pilha para array e depois para lista mantendo ordem correta
                Node[] stackArray = GameObject.Find("GameManager").GetComponent<Data>().stackDFS.ToArray();
                
                // Adiciona na ordem inversa (do in�cio para o fim do caminho)
                for (int i = stackArray.Length - 1; i >= 0; i--)
                {
                    finalPath.Add(stackArray[i]);
                }
                
                // Inicia a anima��o do caminho encontrado
                gameManager.SetFinalPath(finalPath);
                break; // Sai do loop pois encontrou o objetivo
            }
        }
        
        // Se o objetivo n�o foi encontrado, procura por vizinhos n�o visitados
        if (!goalFound)
        {
            bool neighborFound = false; // Flag para indicar se encontrou vizinho v�lido
            
            // Procura por vizinhos n�o visitados
            foreach (Node neighbor in currentNode.neighbors)
            {
                // Se o vizinho ainda n�o foi visitado
                if (!visitedNodes.Contains(neighbor))
                {
                    visitedNodes.Add(neighbor);     // Marca como visitado
                    GameObject.Find("GameManager").GetComponent<Data>().stackDFS.Push(neighbor); // Adiciona � pilha
                    neighborFound = true;           // Marca que encontrou um vizinho
                    break; // Sai do loop (DFS explora apenas um caminho por vez)
                }
            }
            
            // Se n�o encontrou nenhum vizinho v�lido, faz backtrack
            if (!neighborFound)
            {
                // Remove o n� atual da pilha (backtrack)
                GameObject.Find("GameManager").GetComponent<Data>().stackDFS.Pop();
                
                // Pinta o n� como "blacklisted" (caminho sem sa�da)
                currentNode.GetComponent<Renderer>().material = currentNode.blackListed;
                
                // Se a pilha ficou vazia, n�o h� caminho poss�vel
                if (GameObject.Find("GameManager").GetComponent<Data>().stackDFS.Count == 0)
                {
                    gameManager.isSearching = false; // Para a busca
                }
            }
        }
    }
}