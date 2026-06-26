using UnityEngine;
using UnityEngine.Audio;
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

    [Header("Som de Interação")]
    public AudioClip interactSound;           // ← Arraste o som aqui
    public AudioMixerGroup sfxMixerGroup;    // ← Arraste o grupo SFX do Mixer (mesmo do pulo)

    private Renderer rend;
    private Color originalColor;
    public Color highlightColor = Color.white;
    private bool playerNearby = false;

    private AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.playOnAwake = false;
        audioSource.loop = false;
        audioSource.spatialBlend = 0f;

        if (sfxMixerGroup != null)
        {
            audioSource.outputAudioMixerGroup = sfxMixerGroup;
        }
        else
        {
            Debug.LogWarning($"SFX Mixer Group não atribuído no objeto: {gameObject.name}");
        }
    }

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
        if (interactSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(interactSound);
        }

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