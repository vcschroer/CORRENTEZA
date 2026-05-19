using UnityEngine;
using UnityEngine.InputSystem;

public class InteractableObject : MonoBehaviour
{
    [Header("Configurações de ID único")]
    [Tooltip("Dê um nome único para cada um dos objetos-chave (ex: item_01, item_02...)")]
    public string idUnico;

    [Header("Configurações da Pista")]
    public GameObject planePrefab;
    public float spawnHeight = 2f;
    public float destroyTime = 2f;

    public Transform target; 

    private Renderer rend;
    private Color originalColor;
    public Color highlightColor = Color.white;

    private bool playerNearby = false;

    void Start()
    {
        rend = GetComponent<Renderer>();
        originalColor = rend.material.color;

    }

    void Update()
    {
        if (playerNearby && Keyboard.current.eKey.wasPressedThisFrame)
        {
            Interact();
        }
    }

    void Interact()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.RegistrarItemChave(idUnico);
        }

        Vector3 spawnPosition = transform.position + Vector3.up * spawnHeight;

        Vector3 direction = target.position - spawnPosition;
        direction.y = 0;

        Quaternion rotation = Quaternion.identity;

        if (direction != Vector3.zero)
        {
            rotation = Quaternion.LookRotation(direction);
        }

        GameObject plane = Instantiate(planePrefab, spawnPosition, rotation);
        Destroy(plane, destroyTime);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = true;
            rend.material.color = highlightColor;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = false;
            rend.material.color = originalColor;
        }
    }
}