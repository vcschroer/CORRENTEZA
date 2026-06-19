using UnityEngine;
using UnityEngine.Audio;

[RequireComponent(typeof(AudioSource))]
public class ProximityAudio : MonoBehaviour
{
    [Header("Configurações de Áudio")]
    public AudioSource audioSource;

    [Tooltip("Arraste aqui o Audio Mixer Group de SFX")]
    public AudioMixerGroup sfxGroup;

    [Header("Distâncias")]
    public float activationRange = 25f;
    public float fullVolumeDistance = 8f;

    [Tooltip("Velocidade do fade de volume")]
    [Range(0.1f, 10f)]
    public float volumeSmoothSpeed = 3f;

    [Header("Configurações Gerais")]
    public bool loop = true;
    [Range(0f, 1f)]
    public float maxVolume = 1f;

    private Transform player;
    private float targetVolume = 0f;
    private float currentVolume = 0f;

    void Awake()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        // === CONFIGURAÇÃO IMPORTANTE ===
        if (sfxGroup != null)
            audioSource.outputAudioMixerGroup = sfxGroup;

        audioSource.spatialBlend = 1f;
        audioSource.loop = loop;
        audioSource.playOnAwake = false;
        audioSource.volume = 0f;

        // Busca o Player
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;
        }
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= activationRange)
        {
            if (!audioSource.isPlaying)
                audioSource.Play();

            if (distance <= fullVolumeDistance)
                targetVolume = maxVolume;
            else
                targetVolume = maxVolume * (1f - (distance - fullVolumeDistance) / (activationRange - fullVolumeDistance));
        }
        else
        {
            targetVolume = 0f;

            if (audioSource.isPlaying && currentVolume < 0.01f)
                audioSource.Stop();
        }

        // Suaviza o volume
        currentVolume = Mathf.Lerp(currentVolume, targetVolume, volumeSmoothSpeed * Time.deltaTime);
        audioSource.volume = currentVolume;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, activationRange);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, fullVolumeDistance);
    }
}