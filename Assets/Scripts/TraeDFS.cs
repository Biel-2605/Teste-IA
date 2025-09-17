using UnityEngine; // Importa as funcionalidades b�sicas do Unity
using System.Collections.Generic; // Importa cole��es gen�ricas como Stack e HashSet
using System.Collections; // Importa IEnumerator para corrotinas

// Classe que implementa o algoritmo Depth-First Search (DFS)
public class TraeDFS : MonoBehaviour
{
    [Header("DFS Configuration")] // Cria uma se��o no Inspector do Unity
    public Node startNode; // N� inicial para come�ar a busca DFS
    
    [Header("Visualization")] // Se��o para configura��es de visualiza��o
    public float searchDelay = 0.1f; // Delay entre cada passo da busca para visualiza��o
    
    private bool isSearching = false; // Flag para controlar se uma busca est� em andamento

    // M�todo p�blico para iniciar a busca DFS
    public void StartDFS()
    {
        // Verifica se j� n�o est� executando uma busca
        if (!isSearching)
        {
            StartCoroutine(DFSAlgorithm());
        }
    }

    // M�todo principal que executa o algoritmo DFS como corrotina
    private IEnumerator DFSAlgorithm()
    {
        // Marca que a busca est� em andamento
        isSearching = true;
        
        // Verifica se o n� inicial foi definido
        if (startNode == null)
        {
            Debug.LogError("N� inicial n�o foi definido!");
            isSearching = false;
            yield break;
        }
        
        // Cria uma pilha (Stack) para armazenar os n�s a serem visitados
        // DFS usa pilha para implementar o comportamento LIFO (Last In, First Out)
        Stack<Node> stack = new Stack<Node>();
        
        // Cria um conjunto (HashSet) para rastrear os n�s j� visitados
        // HashSet oferece verifica��o O(1) para evitar revisitar n�s
        HashSet<Node> visited = new HashSet<Node>();

        // Adiciona o n� inicial na pilha para come�ar a busca
        stack.Push(startNode);
        
        // Marca o n� inicial como visitado
        visited.Add(startNode);

        // Loop principal: continua enquanto houver n�s na pilha
        while (stack.Count > 0)
        {
            // Remove e obt�m o n� do topo da pilha (comportamento LIFO)
            Node currentNode = stack.Pop();
            
            // Aplica material visual para mostrar o n� sendo processado
            if (currentNode.nodeType != NodeType.Start && currentNode.nodeType != NodeType.Goal)
            {
                currentNode.GetComponent<Renderer>().material = currentNode.actualWay;
            }
            
            // Verifica se encontrou o objetivo
            if (currentNode.nodeType == NodeType.Goal)
            {
                Debug.Log("Objetivo encontrado!");
                isSearching = false;
                yield break;
            }
            
            // Aguarda um tempo para visualiza��o
            yield return new WaitForSeconds(searchDelay);
            
            // Itera atrav�s de todos os vizinhos do n� atual
            foreach (Node neighbor in currentNode.neighbors)
            {
                // Verifica se o vizinho existe, n�o foi visitado e n�o � uma parede
                if (neighbor != null && !visited.Contains(neighbor) && neighbor.nodeType != NodeType.Wall)
                {
                    // Adiciona o vizinho n�o visitado na pilha
                    stack.Push(neighbor);
                    
                    // Marca o vizinho como visitado
                    visited.Add(neighbor);
                    
                    // Aplica material visual para mostrar n�s poss�veis
                    if (neighbor.nodeType != NodeType.Start && neighbor.nodeType != NodeType.Goal)
                    {
                        neighbor.GetComponent<Renderer>().material = neighbor.possibleWay;
                    }
                }
            }
        }
        
        // Se chegou aqui, n�o encontrou o objetivo
        Debug.Log("Objetivo n�o encontrado!");
        isSearching = false;
    }
}