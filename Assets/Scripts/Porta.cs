using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Porta : MonoBehaviour
{
    [Header("Configurações da Transição")]
    public string nomeDaCenaParaCarregar;
    public bool ehPortaDeSaida = false;

    [Header("Configurações de Spawn Fixo")]
    public float distanciaAfastamento = 1.5f;

    private bool playerNearby = false;

    void Update()
    {
        if (playerNearby && Keyboard.current.eKey.wasPressedThisFrame)
        {
            EntrarOuSair();
        }
    }

    void EntrarOuSair()
    {
        if (GameManager.Instance == null) return;

        if (!ehPortaDeSaida)
        {
            Vector3 pontoDeRetornoParaRua = transform.position + (transform.forward * distanciaAfastamento);
            pontoDeRetornoParaRua.y = transform.position.y;

            GameManager.Instance.posicaoNoMapaPrincipal = pontoDeRetornoParaRua;
        }

        GameManager.Instance.veioDeUmaCasa = true;


        SceneManager.LoadScene(nomeDaCenaParaCarregar);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = false;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Vector3 pontoVisual = transform.position + (transform.forward * distanciaAfastamento);
        pontoVisual.y = transform.position.y;
        Gizmos.DrawSphere(pontoVisual, 0.3f);
    }
}
