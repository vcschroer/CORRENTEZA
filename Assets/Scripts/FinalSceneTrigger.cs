using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class CutsceneTrigger : MonoBehaviour
{
    [Header("Configurações da Cutscene")]
    [Tooltip("Nome exato da cena que será carregada")]
    public string nomeDaCena = "Cutscene";

    [Tooltip("Se marcado, a cena troca automaticamente ao encostar (sem precisar apertar E)")]
    public bool automaticoAoEncostar = false;

    private bool playerNearby = false;
    private bool podeUsar = false;

    void Start()
    {
        Invoke(nameof(LiberarUso), 0.8f);
    }

    void Update()
    {
        if (automaticoAoEncostar) return;

        if (playerNearby && Keyboard.current.eKey.wasPressedThisFrame)
        {
            CarregarCutscene();
        }
    }

    void CarregarCutscene()
    {
        if (!podeUsar) return;

        Debug.Log($"Preparando transição para cutscene: {nomeDaCena}");

        // Destrói todos os objetos DontDestroyOnLoad
        DestruirDontDestroyOnLoad();

        // Carrega a cena
        SceneManager.LoadScene(nomeDaCena);
    }

    private void DestruirDontDestroyOnLoad()
    {
        GameObject[] allObjects = FindObjectsByType<GameObject>(FindObjectsSortMode.None);

        int destroyedCount = 0;

        foreach (GameObject obj in allObjects)
        {
            // Objetos DontDestroyOnLoad pertencem à cena "DontDestroyOnLoad"
            if (obj.scene.name == "DontDestroyOnLoad")
            {
                Destroy(obj);
                destroyedCount++;
            }
        }

        Debug.Log($"Foram destruídos {destroyedCount} objetos DontDestroyOnLoad.");
    }

    void OnTriggerEnter(Collider other)
    {
        if (!podeUsar) return;

        if (other.CompareTag("Player"))
        {
            playerNearby = true;

            if (automaticoAoEncostar)
            {
                CarregarCutscene();
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = false;
        }
    }

    private void LiberarUso()
    {
        podeUsar = true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireCube(transform.position, transform.localScale * 1.1f);
    }
}