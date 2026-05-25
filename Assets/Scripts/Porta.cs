using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Porta : MonoBehaviour
{
    [Header("Configurações da Transição")]
    public string nomeDaCenaParaCarregar;

    [Tooltip("Se marcado, o player não precisa apertar 'E'. A transição acontece assim que ele encostar no colisor.")]
    public bool portaInvisivelAutomatica = false;

    [Header("Sistema de IDs (Multi-Portas)")]
    [Tooltip("Dê um nome único para ESTA porta da cena atual (Ex: PortaRua_Casa1)")]
    public string idDestaPorta;

    [Tooltip("ID da porta onde o player vai brotar na PRÓXIMA cena. Deixe EM BRANCO para usar o DefaultSpawnPoint.")]
    public string idDaPortaDeDestino;

    [Header("Configurações de Spawn")]
    [Tooltip("Distância à frente da porta (seguindo a seta azul Z do objeto) onde o player vai surgir")]
    public float distanciaAfastamento = 1.5f;

    private bool playerNearby = false;

    void Update()
    {
        if (portaInvisivelAutomatica) return;

        if (playerNearby && Keyboard.current.eKey.wasPressedThisFrame)
        {
            EntrarOuSair();
        }
    }

    void EntrarOuSair()
    {
        if (GameManager.Instance == null) return;

        GameManager.Instance.idDaPortaDeDestino = idDaPortaDeDestino;
        GameManager.Instance.veioDeUmaPorta = true;

        SceneManager.LoadScene(nomeDaCenaParaCarregar);
    }

    public Vector3 ObterPontoDeSpawn()
    {
        Vector3 ponto = transform.position + (transform.forward * distanciaAfastamento);
        ponto.y = transform.position.y; 
        return ponto;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = true;

            if (portaInvisivelAutomatica)
            {
                EntrarOuSair();
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

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(ObterPontoDeSpawn(), 0.3f);
    }
}