using UnityEngine; // Importa as funcionalidades b�sicas do Unity
using System.Collections.Generic; // Importa cole��es gen�ricas como Stack e HashSet
using System.Collections; // Importa IEnumerator para corrotinas

// Classe que implementa o algoritmo Depth-First Search (DFS) com visualiza��o
public class TraeDFS : MonoBehaviour
{
    // Se��o de configura��o do DFS no Inspector do Unity
    [Header("DFS Configuration")]
    public Node startNode; // N� inicial para come�ar a busca DFS
    
    // Se��o de configura��es de visualiza��o no Inspector
    [Header("Visualization")]
    public float searchDelay = 0.1f; // Delay em segundos entre cada passo da busca para visualiza��o
    
    // Vari�vel privada para controlar se uma busca est� em andamento
    private bool isSearching = false;

    // M�todo p�blico para iniciar a busca DFS
    public void StartDFS()
    {
        // Verifica se j� n�o est� executando uma busca para evitar m�ltiplas execu��es simult�neas
        if (!isSearching)
        {
            StartCoroutine(DFSAlgorithm()); // Inicia a corrotina do algoritmo DFS
        }
    }

    // M�todo principal que executa o algoritmo DFS como corrotina para permitir visualiza��o
    private IEnumerator DFSAlgorithm()
    {
        // Marca que a busca est� em andamento
        isSearching = true;
        
        // Verifica se o n� inicial foi definido antes de come�ar
        if (startNode == null)
        {
            isSearching = false; // Reseta o flag de busca
            yield break; // Sai da corrotina se n�o h� n� inicial
        }
        
        // Cria uma pilha (Stack) para armazenar os n�s a serem visitados
        // DFS usa pilha para implementar o comportamento LIFO (Last In, First Out)
        Stack<Node> stack = new Stack<Node>();
        
        // Cria um conjunto (HashSet) para rastrear os n�s j� visitados
        // HashSet oferece verifica��o O(1) para evitar revisitar n�s
        HashSet<Node> visited = new HashSet<Node>();

        // Adiciona o n� inicial na pilha para come�ar a busca
        stack.Push(startNode);
        
        // Marca o n� inicial como visitado para evitar process�-lo novamente
        visited.Add(startNode);

        // Loop principal: continua enquanto houver n�s na pilha para processar
        while (stack.Count > 0)
        {
            // Remove e obt�m o n� do topo da pilha (comportamento LIFO do DFS)
            Node currentNode = stack.Pop();
            
            // Aplica material visual para mostrar o n� sendo processado atualmente
            // N�o altera a apar�ncia dos n�s especiais (Start e Goal)
            if (currentNode.nodeType != NodeType.Start && currentNode.nodeType != NodeType.Goal)
            {
                currentNode.GetComponent<Renderer>().material = currentNode.actualWay;
            }
            
            // Verifica se o n� atual � o objetivo da busca
            if (currentNode.nodeType == NodeType.Goal)
            {
                isSearching = false; // Marca que a busca foi conclu�da
                yield break; // Sai da corrotina pois encontrou o objetivo
            }
            
            // Aguarda um tempo configurado para permitir visualiza��o do processo
            yield return new WaitForSeconds(searchDelay);
            
            // Itera atrav�s de todos os vizinhos do n� atual
            foreach (Node neighbor in currentNode.neighbors)
            {
                // Verifica se o vizinho � v�lido: existe, n�o foi visitado e n�o � uma parede
                if (neighbor != null && !visited.Contains(neighbor) && neighbor.nodeType != NodeType.Wall)
                {
                    // Adiciona o vizinho n�o visitado na pilha para processamento futuro
                    stack.Push(neighbor);
                    
                    // Marca o vizinho como visitado para evitar process�-lo novamente
                    visited.Add(neighbor);
                    
                    // Aplica material visual para mostrar n�s que podem ser explorados
                    // N�o altera a apar�ncia dos n�s especiais (Start e Goal)
                    if (neighbor.nodeType != NodeType.Start && neighbor.nodeType != NodeType.Goal)
                    {
                        neighbor.GetComponent<Renderer>().material = neighbor.possibleWay;
                    }
                }
            }
        }
        
        // Se chegou aqui, a pilha est� vazia e n�o encontrou o objetivo
        isSearching = false; // Marca que a busca foi conclu�da sem sucesso
    }
}