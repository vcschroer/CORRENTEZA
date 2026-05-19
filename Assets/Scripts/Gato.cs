using UnityEngine;
using UnityEngine.InputSystem;

public class Gato : MonoBehaviour
{
    private bool playerNearby = false;

    void Start()
    {
        if (GameManager.Instance != null && GameManager.Instance.gatoDistraido)
        {
            transform.localScale = new Vector3(1, 0.5f, 1); // Fica deitadinho/achatado de exemplo
        }
    }

    void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.gatoDistraido) return;

        if (playerNearby && Keyboard.current.eKey.wasPressedThisFrame)
        {
            InteragirComGato();
        }
    }

    void InteragirComGato()
    {
        if (GameManager.Instance.temPeixe)
        {
            GameManager.Instance.gatoDistraido = true;
            Debug.Log("<color=green>[Progresso]</color> Você entregou o peixe! O Gato está distraído. Agora você pode pegar as chaves do carro!");

        }
        else
        {
            Debug.Log("O Gato está bravo bloqueando o caminho! Você precisa de um PEIXE para distraí-lo.");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) playerNearby = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) playerNearby = false;
    }
}
