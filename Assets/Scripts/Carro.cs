using UnityEngine;
using UnityEngine.InputSystem;

public class Carro : MonoBehaviour
{
    private bool playerNearby = false;


    void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.carroAberto) return;

        if (playerNearby && Keyboard.current.eKey.wasPressedThisFrame)
        {
            TentarAbrirCarro();
        }
    }

    void TentarAbrirCarro()
    {
        if (GameManager.Instance == null) return;

        if (!GameManager.Instance.temChavesDoCarro)
        {
            Debug.Log("O carro está trancado. Você precisa encontrar as CHAVES DO CARRO.");
            return;
        }

        if (!GameManager.Instance.ForamTodosOsObjetosChave())
        {
            Debug.Log($"Você tem a chave, mas o cachorro ainda não farejou objetos suficientes! Faltam: {GameManager.Instance.quantidadeObjetosNecessarios - GameManager.Instance.QuantidadeDeItensAchados()}");
            return;
        }

        GameManager.Instance.carroAberto = true;
        GameManager.Instance.temBoia = true; 

        Debug.Log("<color=gold>[SUCESSO]</color> Carro aberto! O estepe caiu na água servindo de plataforma e você conseguiu a BOIA! Agora você pode nadar.");

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
