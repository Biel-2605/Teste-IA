using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.InputSystem;

// Enumera��o que define os tipos de algoritmos de busca dispon�veis
public enum searchType
{
    BFS, // Busca em Largura (Breadth-First Search)
    DFS  // Busca em Profundidade (Depth-First Search)
}

// Classe principal que gerencia o jogo e coordena as buscas
public class GameManager : MonoBehaviour
{
    // Se��o de configura��es do mapa
    [Header("Map Size")]
    public int height = 20;        // Altura do grid em n�s
    public int width = 20;         // Largura do grid em n�s
    public int numberOfWalls = 50; // N�mero de paredes a serem geradas aleatoriamente

    // Se��o de configura��es dos n�s
    [Header("Node")]
    public GameObject node;           // Prefab do n� a ser instanciado
    public List<GameObject> nodes;    // Lista de todos os n�s no grid

    public List<GameObject> walls;    // Lista espec�fica de n�s que s�o paredes
    
    // Vari�veis de controle de estado do jogo (ocultas no inspector)
    [HideInInspector]
    public bool hasStart = false;     // Indica se h� um n� inicial definido
    [HideInInspector]
    public Node start;                // Refer�ncia ao n� inicial
    [HideInInspector]
    public bool hasGoal = false;      // Indica se h� um n� objetivo definido
    [HideInInspector]
    public Node goal;                 // Refer�ncia ao n� objetivo

    // Se��o de configura��es da busca
    [Header("Search")]
    public searchType searchType = searchType.DFS; // Tipo de algoritmo de busca selecionado
    public Data data;                               // Componente que armazena estruturas de dados da busca
    public float searchDelay = 1f;                  // Atraso em segundos entre cada passo da busca
    float searchTimer = 0f;                         // Timer interno para controlar o atraso

    // Vari�veis de controle do estado da busca (ocultas no inspector)
    [HideInInspector]
    public bool isSearching = false;  // Indica se uma busca est� em andamento
    [HideInInspector]
    public bool searchDone = false;   // Indica se a busca foi conclu�da
    
    // Se��o de configura��es da anima��o do caminho
    [Header("Path Animation")]
    public float animationSpeed = 0.1f;  // Velocidade da anima��o (tempo entre mudan�as)
    public int lightTrailLength = 3;     // Quantos n�s ficam "acesos" simultaneamente
    
    // Vari�veis privadas para controle da anima��o
    private List<Node> finalPath = new List<Node>(); // Caminho final encontrado pela busca
    private float animationTimer = 0f;               // Timer para controlar a velocidade da anima��o
    private int currentAnimationIndex = 0;           // �ndice atual na anima��o do caminho

    // M�todo chamado uma vez antes da primeira execu��o do Update
    void Start()
    {
        // Gera o grid inicial com as dimens�es e n�mero de paredes especificados
        GridGenerator.GenerateGrid(height, width, numberOfWalls, node);
    }

    // M�todo chamado a cada frame
    void Update()
    {
        // Verifica se o bot�o esquerdo do mouse foi pressionado
        if (Input.GetMouseButtonDown(0))
        {
            HandleMouseClick(); // Processa o clique do mouse
        }

        // Se uma busca est� em andamento, controla os passos da busca
        if (isSearching)
        {
            searchTimer += Time.deltaTime; // Incrementa o timer com o tempo decorrido
            
            // Se o tempo de atraso foi atingido, executa o pr�ximo passo
            if (searchTimer >= searchDelay)
            {
                searchTimer = 0f; // Reseta o timer
                
                // Executa o passo apropriado baseado no tipo de busca selecionado
                if (searchType == searchType.DFS)
                {
                    DFS.PerformSearchStep(); // Executa um passo do DFS
                }
                else if (searchType == searchType.BFS)
                {
                    BFS.PerformSearchStep(); // Executa um passo do BFS
                }
            }
        }
        
        // Chama a anima��o do caminho se a busca foi conclu�da
        AnimatePath();
    }  

    // M�todo privado para processar cliques do mouse no grid
    private void HandleMouseClick()
    {
        // Cria um ray da c�mera principal para a posi��o atual do mouse
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit; // Vari�vel para armazenar informa��es do objeto atingido

        // Verifica se o ray atingiu algum objeto com collider
        if (Physics.Raycast(ray, out hit))
        {
            // Tenta obter o componente Node do objeto atingido
            Node nodeComponent = hit.collider.GetComponent<Node>();
            
            // Se o objeto tem um componente Node, processa o clique
            if (nodeComponent != null)
            {
                nodeComponent.OnNodeClicked(); // Chama o m�todo de clique do n�
            }
        }
    }

    // M�todo privado para animar o caminho encontrado
    private void AnimatePath()
    {
        // S� executa a anima��o se a busca foi conclu�da e h� um caminho v�lido
        if (!searchDone || finalPath.Count == 0)
            return;

        // Controla o tempo da anima��o baseado na velocidade configurada
        animationTimer += Time.deltaTime;
        
        // Se o tempo da anima��o foi atingido, atualiza a visualiza��o
        if (animationTimer >= animationSpeed)
        {
            animationTimer = 0f; // Reseta o timer da anima��o
            
            // Remove o efeito de "luz" de todos os n�s do caminho
            for (int i = 0; i < finalPath.Count; i++)
            {
                if (finalPath[i] != null)
                {
                    finalPath[i].SetPathAnimation(false); // Desativa anima��o do n�
                }
            }
            
            // Aplica o efeito de "luz" nos n�s atuais (baseado no comprimento da trilha)
            for (int i = 0; i < lightTrailLength; i++)
            {
                // Calcula o �ndice do n� considerando o movimento circular
                int nodeIndex = (currentAnimationIndex + i) % finalPath.Count;
                
                // Se o n� existe, ativa sua anima��o
                if (finalPath[nodeIndex] != null)
                {
                    finalPath[nodeIndex].SetPathAnimation(true); // Ativa anima��o do n�
                }
            }
            
            // Avan�a para o pr�ximo n� na sequ�ncia (movimento circular)
            currentAnimationIndex = (currentAnimationIndex + 1) % finalPath.Count;
        }
    }
    
    // M�todo p�blico para definir o caminho final e iniciar a anima��o
    public void SetFinalPath(List<Node> path)
    {
        finalPath = new List<Node>(path); // Cria uma c�pia da lista do caminho
        currentAnimationIndex = 0;        // Reseta o �ndice da anima��o
        animationTimer = 0f;              // Reseta o timer da anima��o
        searchDone = true;                // Marca a busca como conclu�da
    }
}