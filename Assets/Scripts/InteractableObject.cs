using UnityEngine;
using UnityEngine.InputSystem;

public class InteractableObject : MonoBehaviour
{
    public GameObject planePrefab;
    public float spawnHeight = 2f;
    public float destroyTime = 2f;

    public Transform target; // 🔥 alvo que a seta vai apontar

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
        Vector3 spawnPosition = transform.position + Vector3.up * spawnHeight;

        // 🔥 calcula direção ignorando altura (Y)
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