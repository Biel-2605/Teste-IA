using UnityEngine;
using System.Collections.Generic;
using System.Diagnostics;

// Enumera��o que define os tipos de n�s poss�veis no grid
public enum NodeType
{
    Floor,  // N� de ch�o (caminh�vel)
    Wall,   // N� de parede (obst�culo)
    Start,  // N� inicial da busca
    Goal    // N� objetivo da busca
}

// Classe que representa um n� individual no grid de busca
public class Node : MonoBehaviour
{
    // Se��o de materiais visuais para diferentes estados do n�
    [Header("Materials")]
    public Material actualWay;    // Material para n�s que fazem parte do caminho final
    public Material blackListed;  // Material para n�s visitados que n�o fazem parte do caminho
    public Material finish;       // Material para o n� objetivo
    public Material floor;        // Material padr�o para n�s de ch�o
    public Material possibleWay;  // Material para n�s sendo explorados
    public Material start;        // Material para o n� inicial
    public Material wall;         // Material para n�s de parede

    // Se��o de propriedades do n�
    [Header("Node Properties")]
    public List<Node> neighbors = new List<Node>(); // Lista de n�s vizinhos conectados
    public NodeType nodeType = NodeType.Floor;      // Tipo atual do n�

    // Vari�veis privadas para controle de anima��o
    private bool isAnimating = false;     // Controla se o n� est� sendo animado
    private Material originalMaterial;    // Armazena o material original antes da anima��o

    // M�todo chamado quando o n� � clicado pelo usu�rio
    public void OnNodeClicked()
    {
        // Verifica se o n� n�o � uma parede (paredes n�o podem ser modificadas)
        if (nodeType != NodeType.Wall)
        {
            // Se o n� clicado � o n� inicial, remove-o
            if (nodeType == NodeType.Start)
            {
                GetComponent<Renderer>().material = floor; // Volta ao material de ch�o
                GameObject.Find("GameManager").GetComponent<GameManager>().start = null; // Remove refer�ncia do start
                nodeType = NodeType.Floor; // Muda tipo para ch�o
                GameObject.Find("GameManager").GetComponent<GameManager>().hasStart = false; // Marca que n�o h� start
                GameObject.Find("GameManager").GetComponent<GameManager>().isSearching = false; // Para a busca
                GameObject.Find("GameManager").GetComponent<Data>().stackDFS.Clear(); // Limpa pilha do DFS
                GameObject.Find("GameManager").GetComponent<Data>().visitedNodes.Clear(); // Limpa n�s visitados
            }
            // Se o n� clicado � o n� objetivo, remove-o
            else if (nodeType == NodeType.Goal)
            {
                GetComponent<Renderer>().material = floor; // Volta ao material de ch�o
                GameObject.Find("GameManager").GetComponent<GameManager>().goal = null; // Remove refer�ncia do goal
                nodeType = NodeType.Floor; // Muda tipo para ch�o
                GameObject.Find("GameManager").GetComponent<GameManager>().hasGoal = false; // Marca que n�o h� goal
                GameObject.Find("GameManager").GetComponent<GameManager>().isSearching = false; // Para a busca
                GameObject.Find("GameManager").GetComponent<Data>().stackDFS.Clear(); // Limpa pilha do DFS
                GameObject.Find("GameManager").GetComponent<Data>().visitedNodes.Clear(); // Limpa n�s visitados
            }
            // Se o n� � um ch�o normal, define como start ou goal conforme necess�rio
            else
            {
                // Se ainda n�o h� n� inicial, define este como start
                if (!GameObject.Find("GameManager").GetComponent<GameManager>().hasStart)
                {
                    GetComponent<Renderer>().material = start; // Aplica material de start
                    GameObject.Find("GameManager").GetComponent<GameManager>().start = this; // Define refer�ncia do start
                    nodeType = NodeType.Start; // Muda tipo para start
                    
                    // Limpa estruturas de dados de buscas anteriores
                    GameObject.Find("GameManager").GetComponent<Data>().visitedNodes.Clear();
                    GameObject.Find("GameManager").GetComponent<Data>().stackDFS.Clear();
                    GameObject.Find("GameManager").GetComponent<Data>().queueBFS.Clear();
                    BFS.ClearParentMap(); // Limpa mapa de pais do BFS
                    
                    // Adiciona o n� inicial na estrutura de dados correta baseada no tipo de busca
                    if (GameObject.Find("GameManager").GetComponent<GameManager>().searchType == searchType.DFS)
                    {
                        GameObject.Find("GameManager").GetComponent<Data>().stackDFS.Push(this); // Adiciona na pilha do DFS
                    }
                    else if (GameObject.Find("GameManager").GetComponent<GameManager>().searchType == searchType.BFS)
                    {
                        GameObject.Find("GameManager").GetComponent<Data>().queueBFS.Enqueue(this); // Adiciona na fila do BFS
                    }
                    
                    GameObject.Find("GameManager").GetComponent<Data>().visitedNodes.Add(this); // Marca como visitado
                    GameObject.Find("GameManager").GetComponent<GameManager>().hasStart = true; // Marca que h� start
                    
                    // Se j� h� goal definido, inicia a busca
                    if (GameObject.Find("GameManager").GetComponent<GameManager>().hasGoal)
                    {
                        GameObject.Find("GameManager").GetComponent<GameManager>().isSearching = true;
                    }
                }
                // Se j� h� start mas n�o h� goal, define este como goal
                else if (!GameObject.Find("GameManager").GetComponent<GameManager>().hasGoal)
                {
                    GetComponent<Renderer>().material = finish; // Aplica material de goal
                    GameObject.Find("GameManager").GetComponent<GameManager>().goal = this; // Define refer�ncia do goal
                    nodeType = NodeType.Goal; // Muda tipo para goal
                    GameObject.Find("GameManager").GetComponent<GameManager>().hasGoal = true; // Marca que h� goal
                    
                    // Se j� h� start definido, prepara para iniciar a busca
                    if (GameObject.Find("GameManager").GetComponent<GameManager>().hasStart)
                    {
                        GameObject.Find("GameManager").GetComponent<GameManager>().isSearching = true; // Inicia busca
                        
                        // Limpa estruturas de dados de buscas anteriores
                        GameObject.Find("GameManager").GetComponent<Data>().visitedNodes.Clear();
                        GameObject.Find("GameManager").GetComponent<Data>().stackDFS.Clear();
                        GameObject.Find("GameManager").GetComponent<Data>().queueBFS.Clear();
                        BFS.ClearParentMap(); // Limpa mapa de pais do BFS
                        
                        // Encontra o n� start e o adiciona na estrutura de dados correta
                        foreach (GameObject n in GameObject.Find("GameManager").GetComponent<GameManager>().nodes)
                        {
                            if(n.GetComponent<Node>().nodeType == NodeType.Start)
                            {
                                // Adiciona o n� inicial na estrutura correta baseada no tipo de busca
                                if (GameObject.Find("GameManager").GetComponent<GameManager>().searchType == searchType.DFS)
                                {
                                    GameObject.Find("GameManager").GetComponent<Data>().stackDFS.Push(n.GetComponent<Node>());
                                }
                                else if (GameObject.Find("GameManager").GetComponent<GameManager>().searchType == searchType.BFS)
                                {
                                    GameObject.Find("GameManager").GetComponent<Data>().queueBFS.Enqueue(n.GetComponent<Node>());
                                }
                                break; // Para o loop ap�s encontrar o start
                            }
                        }
                    }
                }
            }
        }
    }
    
    // M�todo para controlar a anima��o visual do caminho encontrado
    public void SetPathAnimation(bool animate)
    {
        // Se deve animar e ainda n�o est� animando
        if (animate && !isAnimating)
        {
            // Salva o material original antes de aplicar a anima��o
            originalMaterial = GetComponent<Renderer>().material;
            GetComponent<Renderer>().material = actualWay; // Aplica material de caminho atual
            isAnimating = true; // Marca como animando
        }
        // Se deve parar de animar e est� animando
        else if (!animate && isAnimating)
        {
            // Restaura o material original se existir
            if (originalMaterial != null)
            {
                GetComponent<Renderer>().material = originalMaterial;
            }
            isAnimating = false; // Marca como n�o animando
        }
    }
}

