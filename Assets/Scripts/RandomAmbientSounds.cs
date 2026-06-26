using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class RandomAmbientSounds : MonoBehaviour
{
    [Header("Configurações Gerais")]
    public bool enableAmbientSounds = true;

    [Header("Sons")]
    public AudioClip ambulanciaSound;
    public AudioClip helicopteroSound;

    [Header("Intervalos de Tempo")]
    [Tooltip("Tempo mínimo entre os sons (em segundos)")]
    public float minInterval = 45f;

    [Tooltip("Tempo máximo entre os sons (em segundos)")]
    public float maxInterval = 120f;

    [Header("Probabilidades")]
    [Range(0f, 1f)]
    [Tooltip("Chance de tocar a ambulância")]
    public float chanceAmbulancia = 0.5f;

    [Range(0f, 1f)]
    [Tooltip("Chance de tocar o helicóptero")]
    public float chanceHelicoptero = 0.5f;

    [Header("Configurações de Áudio")]
    public AudioMixerGroup ambientMixerGroup;
    [Range(0f, 1f)]
    public float volume = 0.7f;

    [Tooltip("Quanto tempo o som pode ser ouvido (distância 3D)")]
    public float spatialBlend = 1f;

    private AudioSource audioSource;
    private Coroutine currentCoroutine;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.playOnAwake = false;
        audioSource.loop = false;
        audioSource.volume = volume;
        audioSource.spatialBlend = spatialBlend;

        if (ambientMixerGroup != null)
        {
            audioSource.outputAudioMixerGroup = ambientMixerGroup;
        }
    }

    void Start()
    {
        if (enableAmbientSounds)
        {
            currentCoroutine = StartCoroutine(PlayRandomAmbientSounds());
        }
    }

    private IEnumerator PlayRandomAmbientSounds()
    {
        while (enableAmbientSounds)
        {
            float waitTime = Random.Range(minInterval, maxInterval);
            yield return new WaitForSeconds(waitTime);

            if (!enableAmbientSounds) break;

            float randomValue = Random.value;
            AudioClip chosenClip = null;

            if (randomValue < chanceAmbulancia)
            {
                chosenClip = ambulanciaSound;
            }
            else if (randomValue < chanceAmbulancia + chanceHelicoptero)
            {
                chosenClip = helicopteroSound;
            }

            if (chosenClip != null && audioSource != null)
            {
                audioSource.PlayOneShot(chosenClip);
                Debug.Log($"[Ambient Sound] Tocando: {(chosenClip == ambulanciaSound ? "Ambulância" : "Helicóptero")}");
            }
        }
    }

    public void EnableSounds(bool enable)
    {
        enableAmbientSounds = enable;

        if (enable && currentCoroutine == null)
        {
            currentCoroutine = StartCoroutine(PlayRandomAmbientSounds());
        }
        else if (!enable && currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
            currentCoroutine = null;
        }
    }

    public void ChangeVolume(float newVolume)
    {
        volume = Mathf.Clamp01(newVolume);
        if (audioSource != null) audioSource.volume = volume;
    }

    public void StopAll()
    {
        if (audioSource != null)
            audioSource.Stop();
    }
}