using UnityEngine;
using UnityEngine.InputSystem;

public class LiberaBoia : MonoBehaviour
{
    private bool playerNearby = false;
    private PlayerController player;

    [Header("Water Settings")]
    public Renderer waterRenderer; // 🔥 arrastar o Plane Water aqui
    public Color waterNewColor = Color.cyan;

    private Color originalWaterColor;
    private bool alreadyUsed = false;

    void Start()
    {
        if (waterRenderer != null)
        {
            originalWaterColor = waterRenderer.material.color;
        }
    }

    void Update()
    {
        if (playerNearby && Keyboard.current.eKey.wasPressedThisFrame)
        {
            ActivateAbility();
        }
    }

    void ActivateAbility()
    {
        if (player != null && !alreadyUsed)
        {
            // ativa habilidade
            player.boia = true;

            // 🔥 muda cor da água
            if (waterRenderer != null)
            {
                waterRenderer.material.color = waterNewColor;
            }

            alreadyUsed = true;

            Debug.Log("Boia ativada + água alterada!");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = true;
            player = other.GetComponent<PlayerController>();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = false;
            player = null;
        }
    }
}