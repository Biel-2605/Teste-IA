using UnityEngine; // Importa funcionalidades b�sicas do Unity
using System.Collections; // Importa cole��es b�sicas do C#
using System.Collections.Generic; // Importa cole��es gen�ricas como Dictionary, List, HashSet
using System.Diagnostics; // Importa funcionalidades de diagn�stico
using Unity.Properties; // Importa sistema de propriedades do Unity

// Classe respons�vel pela implementa��o do algoritmo BFS (Breadth-First Search)
public class BFS : MonoBehaviour
{
    // M�todo est�tico que executa um passo da busca BFS
    public static void PerformSearchStep()
    {
        // Obt�m refer�ncia ao GameManager atrav�s da busca por GameObject
        GameManager gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        // Obt�m refer�ncia ao componente Data que cont�m as estruturas de dados
        Data data = GameObject.Find("GameManager").GetComponent<Data>();
        
        // Prote��o contra loops infinitos: limita o n�mero m�ximo de n�s na queue
        if (data.queueBFS.Count > 10000)
        {
            // Para a busca se a queue ficar muito grande
            gameManager.isSearching = false;
            // Retorna sem continuar a execu��o
            return;
        }
        
        // Verifica se a queue est� vazia (n�o h� mais n�s para processar)
        if (data.queueBFS.Count == 0)
        {
            // Para a busca pois n�o h� caminho poss�vel
            gameManager.isSearching = false;
            // Retorna sem continuar a execu��o
            return;
        }
        
        // Remove o primeiro n� da queue (FIFO - First In, First Out)
        Node currentNode = data.queueBFS.Dequeue();
        // Chama o m�todo para encontrar vizinhos do n� atual
        FindNeighbor(currentNode);
    }

    // M�todo est�tico que processa os vizinhos do n� atual
    static void FindNeighbor(Node currentNode)
    {
        // Obt�m refer�ncia ao HashSet de n�s visitados
        HashSet<Node> visitedNodes = GameObject.Find("GameManager").GetComponent<Data>().visitedNodes;
        // Obt�m refer�ncia ao GameManager
        GameManager gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        // Obt�m refer�ncia ao componente Data
        Data data = GameObject.Find("GameManager").GetComponent<Data>();
        
        // Marca o n� atual como visitado ANTES de processar vizinhos (corre��o importante)
        if (!visitedNodes.Contains(currentNode))
        {
            // Adiciona o n� atual ao conjunto de visitados
            visitedNodes.Add(currentNode);
        }
        
        // Se o n� atual n�o � o Start, muda sua cor para "possibleWay"
        if(currentNode.nodeType != NodeType.Start)
            // Aplica o material que indica caminho poss�vel
            currentNode.GetComponent<Renderer>().material = currentNode.possibleWay;
        
        // Flag para indicar se o objetivo foi encontrado
        bool goalFound = false;
        // Percorre todos os vizinhos do n� atual
        foreach (Node neighbor in currentNode.neighbors)
        {
            // Verifica se o vizinho � o n� objetivo
            if (neighbor == gameManager.goal)
            {
                // Marca que o objetivo foi encontrado
                goalFound = true;
                // Para a busca
                gameManager.isSearching = false;
                // Adiciona o n� objetivo aos visitados
                visitedNodes.Add(neighbor);
                
                // Reconstr�i o caminho do start at� o goal
                List<Node> finalPath = ReconstructPath(neighbor, currentNode);
                
                // Pinta todos os n�s visitados que n�o fazem parte do caminho final
                PaintVisitedNodesNotInPath(finalPath, visitedNodes);
                
                // Define o caminho final no GameManager para iniciar a anima��o
                gameManager.SetFinalPath(finalPath);
                // Retorna pois o objetivo foi encontrado
                return;
            }
        }
        
        // Se o objetivo n�o foi encontrado, processa os vizinhos normalmente
        if (!goalFound)
        {
            // Percorre todos os vizinhos do n� atual
            foreach (Node neighbor in currentNode.neighbors)
            {
                // Verifica se o vizinho n�o foi visitado E n�o � uma parede
                if (!visitedNodes.Contains(neighbor) && neighbor.nodeType != NodeType.Wall)
                {
                    // Marca o vizinho como visitado
                    visitedNodes.Add(neighbor);
                    // Adiciona o vizinho � queue para processamento futuro
                    data.queueBFS.Enqueue(neighbor);
                    
                    // Armazena o pai do vizinho para reconstru��o do caminho
                    if (!parentMap.ContainsKey(neighbor))
                    {
                        // Define o n� atual como pai do vizinho
                        parentMap[neighbor] = currentNode;
                    }
                }
            }
        }
    }
    
    // Dicion�rio est�tico para rastrear os pais dos n�s (necess�rio para reconstru��o do caminho)
    private static Dictionary<Node, Node> parentMap = new Dictionary<Node, Node>();
    
    // M�todo est�tico para reconstruir o caminho do goal at� o start
    static List<Node> ReconstructPath(Node goal, Node lastParent)
    {
        // Cria uma nova lista para armazenar o caminho
        List<Node> path = new List<Node>();
        
        // Adiciona o n� objetivo ao caminho
        path.Add(goal);
        
        // Define o �ltimo pai como n� atual para reconstru��o
        Node current = lastParent;
        // Adiciona o �ltimo pai ao caminho
        path.Add(current);
        
        // Reconstr�i o caminho seguindo os pais at� chegar ao start
        while (parentMap.ContainsKey(current))
        {
            // Obt�m o pai do n� atual
            current = parentMap[current];
            // Adiciona o pai ao caminho
            path.Add(current);
        }
        
        // Inverte a lista para ter o caminho do start para o goal
        path.Reverse();
        
        // Retorna o caminho completo
        return path;
    }
    
    // M�todo p�blico est�tico para limpar o mapa de pais quando uma nova busca come�ar
    public static void ClearParentMap()
    {
        // Remove todos os elementos do dicion�rio de pais
        parentMap.Clear();
    }
    
    // M�todo est�tico para pintar n�s visitados que n�o fazem parte do caminho final
    static void PaintVisitedNodesNotInPath(List<Node> finalPath, HashSet<Node> visitedNodes)
    {
        // Converte o caminho final para HashSet para busca mais eficiente O(1)
        HashSet<Node> pathNodes = new HashSet<Node>(finalPath);
        
        // Percorre todos os n�s visitados durante a busca
        foreach (Node visitedNode in visitedNodes)
        {
            // Verifica se o n� visitado n�o est� no caminho final e n�o � Start nem Goal
            if (!pathNodes.Contains(visitedNode) && 
                visitedNode.nodeType != NodeType.Start && 
                visitedNode.nodeType != NodeType.Goal)
            {
                // Aplica o material blackListed para indicar n� explorado mas n�o usado
                visitedNode.GetComponent<Renderer>().material = visitedNode.blackListed;
            }
        }
    }
}