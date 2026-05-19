using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement; 

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Configurações do Jogador")]
    public GameObject playerPrefab;
    [HideInInspector] public GameObject playerInstance;

    [Header("Transição de Cenas")]
    public Vector3 posicaoNoMapaPrincipal;
    public bool veioDeUmaCasa = false;

    [Header("Configuração de Design")]
    public int quantidadeObjetosNecessarios = 7;

    [Header("Progresso Atual")]
    public bool temPeixe = false;
    public bool gatoDistraido = false;
    public bool temChavesDoCarro = false;
    public bool carroAberto = false;
    public bool temBoia = false;
    public bool peixeJaFoiColetado = false;
    public bool chaveJaFoiColetada = false;

    private HashSet<string> itensFarejados = new HashSet<string>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        if (SceneManager.GetActiveScene().name == "teste menu") return;
    }

    private void OnEnable() { SceneManager.sceneLoaded += OnSceneLoaded; }
    private void OnDisable() { SceneManager.sceneLoaded -= OnSceneLoaded; }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "teste menu") return;

        SpawnOrPositionPlayer();
    }

    void SpawnOrPositionPlayer()
    {
        if (SceneManager.GetActiveScene().name == "teste menu") return;

        if (playerInstance == null)
        {
            playerInstance = Instantiate(playerPrefab);
        }

        if (veioDeUmaCasa)
        {
            Rigidbody rb = playerInstance.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }

            playerInstance.transform.position = posicaoNoMapaPrincipal;
            veioDeUmaCasa = false; 
        }
        else
        {
            GameObject spawnPoint = GameObject.Find("DefaultSpawnPoint");
            if (spawnPoint != null)
            {
                Rigidbody rb = playerInstance.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.linearVelocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                }
                playerInstance.transform.position = spawnPoint.transform.position;
            }
            else
            {
                Debug.LogWarning("Nenhum 'DefaultSpawnPoint' foi encontrado nesta cena de jogo!");
            }
        }
    }

    public void RegistrarItemChave(string id)
    {
        if (!itensFarejados.Contains(id))
        {
            itensFarejados.Add(id);
        }
    }

    public bool JaInteragiuComItem(string id) { return itensFarejados.Contains(id); }
    public bool ForamTodosOsObjetosChave() { return itensFarejados.Count >= quantidadeObjetosNecessarios; }
    public int QuantidadeDeItensAchados() { return itensFarejados.Count; }
}