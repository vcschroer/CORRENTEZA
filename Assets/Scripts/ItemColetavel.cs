using UnityEngine;
using UnityEngine.InputSystem;

public class ItemColetavel : MonoBehaviour
{
    public enum TipoDeItem { Peixe, ChaveDoCarro }
    [Header("Configuração do Item")]
    public TipoDeItem tipo;

    private bool playerNearby = false;

    void Awake()
    {
        VerificarEDestruir();
    }

    void Start()
    {
        VerificarEDestruir();

        Invoke("VerificacaoAtrasada", 0.05f);
    }

    void Update()
    {
        if (playerNearby && Keyboard.current.eKey.wasPressedThisFrame)
        {
            Coletar();
        }
    }

    void Coletar()
    {
        if (GameManager.Instance == null) return;

        if (tipo == TipoDeItem.Peixe)
        {
            GameManager.Instance.temPeixe = true;
            Debug.Log("<color=cyan>[Inventário]</color> Você pegou o PEIXE!");
        }
        else if (tipo == TipoDeItem.ChaveDoCarro)
        {
            GameManager.Instance.temChavesDoCarro = true;
            Debug.Log("<color=yellow>[Inventário]</color> Você pegou as CHAVES DO CARRO!");
        }

        Destroy(gameObject);
    }

    void VerificacaoAtrasada()
    {
        VerificarEDestruir();
    }

    void VerificarEDestruir()
    {
        if (GameManager.Instance != null)
        {
            if (tipo == TipoDeItem.Peixe && GameManager.Instance.temPeixe)
            {
                gameObject.SetActive(false);
                Destroy(gameObject);        
            }
            else if (tipo == TipoDeItem.ChaveDoCarro && GameManager.Instance.temChavesDoCarro)
            {
                gameObject.SetActive(false); 
                Destroy(gameObject);      
            }
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
