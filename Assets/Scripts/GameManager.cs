using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Configurações do Jogador")]
    public GameObject playerPrefab;
    [HideInInspector] public GameObject playerInstance;

    [Header("Transição de Cenas Inteligente")]
    [HideInInspector] public string idDaPortaDeDestino;
    [HideInInspector] public bool veioDeUmaPorta = false;

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

        if (SceneManager.GetActiveScene().name == "testeMenu") return;
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "testeMenu") return;

        SpawnOrPositionPlayer();
    }

    void SpawnOrPositionPlayer()
    {
        if (SceneManager.GetActiveScene().name == "testeMenu") return;

        if (playerInstance == null)
        {
            playerInstance = Instantiate(playerPrefab);
            playerInstance.SetActive(false);
        }

        Rigidbody rb = playerInstance.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        GameObject spawnPoint = GameObject.Find("DefaultSpawnPoint");

        if (spawnPoint == null)
        {
            Debug.LogWarning("DefaultSpawnPoint não encontrado!");
            return;
        }

        if (veioDeUmaPorta && !string.IsNullOrEmpty(idDaPortaDeDestino))
        {
            Porta[] todasAsPortas = FindObjectsByType<Porta>(FindObjectsSortMode.None);

            foreach (Porta p in todasAsPortas)
            {
                if (p.idDestaPorta == idDaPortaDeDestino)
                {
                    Vector3 pos = p.ObterPontoDeSpawn();

                    StartCoroutine(ForcePositionAfterDelay(pos));

                    spawnPoint.transform.position = pos;

                    veioDeUmaPorta = false;
                    idDaPortaDeDestino = "";

                    return;
                }
            }
        }

        StartCoroutine(ForcePositionAfterDelay(spawnPoint.transform.position));

        veioDeUmaPorta = false;
        idDaPortaDeDestino = "";
    }

    IEnumerator ForcePositionAfterDelay(Vector3 pos)
    {
        yield return null;
        yield return null;
        yield return null;

        if (playerInstance != null)
        {
            playerInstance.transform.position = pos;

            Rigidbody rb = playerInstance.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }

            playerInstance.SetActive(true);

            Debug.Log(
                "POSIÇÃO FORÇADA -> X=" + pos.x +
                " Y=" + pos.y +
                " Z=" + pos.z
            );
        }
    }

    public void RegistrarItemChave(string id)
    {
        if (!itensFarejados.Contains(id))
        {
            itensFarejados.Add(id);
        }
    }

    public bool JaInteragiuComItem(string id)
    {
        return itensFarejados.Contains(id);
    }

    public bool ForamTodosOsObjetosChave()
    {
        return itensFarejados.Count >= quantidadeObjetosNecessarios;
    }

    public int QuantidadeDeItensAchados()
    {
        return itensFarejados.Count;
    }
}