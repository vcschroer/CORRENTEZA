using UnityEngine;
using System.Collections;

public class Gato : MonoBehaviour
{
    public enum EstadoGato { IDLE, ATACANDO, COMENDO }

    [Header("Configurações de Estado")]
    public EstadoGato estadoAtual = EstadoGato.IDLE;

    [Header("Força do Susto")]
    public float forcaRepulsao = 8f;
    public float tempoBloqueioControle = 0.5f;

    [Header("Sons do Gato")]
    public AudioClip idleSound;
    public AudioClip[] ataqueSounds;
    public AudioClip comendoSound;

    [Header("Configurações de Áudio")]
    public AudioSource audioSource;

    [Tooltip("Delay entre cada reprodução do som Idle")]
    public float idleDelay = 1.5f;

    private Animator animator;
    private Coroutine idleCoroutine;

    void Start()
    {
        animator = GetComponent<Animator>();
        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.playOnAwake = false;
        audioSource.loop = false;

        Invoke(nameof(IniciarEstadoInicial), 0.1f);
    }

    private void IniciarEstadoInicial()
    {
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
        if (audioSource.isPlaying)
            audioSource.Stop();

        if (idleCoroutine != null)
        {
            StopCoroutine(idleCoroutine);
            idleCoroutine = null;
        }

        estadoAtual = novoEstado;

        if (animator != null)
        {
            switch (estadoAtual)
            {
                case EstadoGato.IDLE:
                    animator.Play("IDLE");
                    idleCoroutine = StartCoroutine(IdleSoundLoop());
                    break;

                case EstadoGato.ATACANDO:
                    animator.Play("ATACANDO");
                    TocarSomAtaque();
                    break;

                case EstadoGato.COMENDO:
                    animator.Play("COMENDO");
                    TocarSomComendoLoop();
                    break;
            }
        }
    }

    // ====================== SONS ======================

    private IEnumerator IdleSoundLoop()
    {
        while (estadoAtual == EstadoGato.IDLE)
        {
            if (idleSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(idleSound);
            }

            float waitTime = (idleSound != null ? idleSound.length : 0f) + idleDelay;
            yield return new WaitForSeconds(waitTime);
        }
    }

    private void TocarSomComendoLoop()
    {
        if (comendoSound != null && audioSource != null)
        {
            audioSource.clip = comendoSound;
            audioSource.loop = true;
            audioSource.Play();
        }
    }

    private void TocarSomAtaque()
    {
        if (ataqueSounds == null || ataqueSounds.Length < 3 || audioSource == null)
            return;

        AudioClip somEscolhido = Random.value <= 0.01f ? ataqueSounds[2] : ataqueSounds[Random.Range(0, 2)];

        audioSource.loop = false;
        audioSource.PlayOneShot(somEscolhido);
    }

    // ====================== INTERAÇÃO ======================

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