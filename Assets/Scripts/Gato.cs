using UnityEngine;
using System.Collections;

public class Gato : MonoBehaviour
{
    public enum EstadoGato { IDLE, ATACANDO, COMENDO }
    [Header("Configurações de Estado")]
    public EstadoGato estadoAtual = EstadoGato.IDLE;

    [Header("Força do Susto")]
    [Tooltip("Força com que o cachorro será empurrado para trás ao ser assustado")]
    public float forcaRepulsao = 8f;
    [Tooltip("Tempo que o player perderá o controle ao se assustar")]
    public float tempoBloqueioControle = 0.5f;

    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }

        if (animator == null)
        {
            Debug.LogError("O Gato não encontrou nenhum componente Animator! Verifique se ele está no objeto ou no modelo/filho.");
        }

        if (GameManager.Instance != null && GameManager.Instance.gatoDistraido)
        {
            MudarEstado(EstadoGato.COMENDO);
        }
        else
        {
            MudarEstado(EstadoGato.IDLE);
        }
    }

    void MudarEstado(EstadoGato novoEstado)
    {
        estadoAtual = novoEstado;
        if (animator == null) return;

        switch (estadoAtual)
        {
            case EstadoGato.IDLE:
                animator.Play("IDLE");
                break;
            case EstadoGato.ATACANDO:
                animator.Play("ATACANDO");
                break;
            case EstadoGato.COMENDO:
                animator.Play("COMENDO");
                break;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (GameManager.Instance != null && GameManager.Instance.gatoDistraido) return;

            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                ReagirAoPlayer(player);
            }
        }
    }

    void ReagirAoPlayer(PlayerController player)
    {
        if (GameManager.Instance != null && GameManager.Instance.temPeixe)
        {
            GameManager.Instance.gatoDistraido = true;
            MudarEstado(EstadoGato.COMENDO);
            Debug.Log("<color=green>[Progresso]</color> O Gato aceitou o peixe e está comendo!");
        }
        else
        {
            MudarEstado(EstadoGato.ATACANDO);

            Vector3 direcaoEmpurrão = (player.transform.position - transform.position);
            direcaoEmpurrão.y = 0;
            direcaoEmpurrão.z = 0; 

            if (direcaoEmpurrão.x == 0) direcaoEmpurrão.x = -1f;

            direcaoEmpurrão = direcaoEmpurrão.normalized;

            player.LevarSusto(direcaoEmpurrão * forcaRepulsao, tempoBloqueioControle);

            StartCoroutine(VoltarParaIdleDepoisDoAtaque());
        }
    }

    IEnumerator VoltarParaIdleDepoisDoAtaque()
    {
        yield return new WaitForSeconds(1f);
        if (estadoAtual == EstadoGato.ATACANDO && (!GameManager.Instance || !GameManager.Instance.gatoDistraido))
        {
            MudarEstado(EstadoGato.IDLE);
        }
    }
}