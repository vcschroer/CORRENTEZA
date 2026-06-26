using UnityEngine;
using UnityEngine.Audio;

public class CarAlarmOnTouch : MonoBehaviour
{
    [Header("Configurações do Alarme")]
    public AudioClip carAlarmSound;

    [Range(0f, 1f)]
    [Tooltip("Chance de tocar o alarme ao encostar (20% = 0.2)")]
    public float chanceToTrigger = 0.2f;

    [Tooltip("Tempo mínimo entre alarmes (evita spam)")]
    public float cooldownTime = 8f;

    [Header("Áudio")]
    public AudioMixerGroup sfxMixerGroup;
    [Range(0f, 1f)]
    public float volume = 1f;
    public float spatialBlend = 1f; // 1 = som 3D

    private AudioSource audioSource;
    private float lastTriggerTime = 0f;
    private bool canTrigger = true;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // Configurações do som
        audioSource.playOnAwake = false;
        audioSource.loop = false;
        audioSource.volume = volume;
        audioSource.spatialBlend = spatialBlend;

        if (sfxMixerGroup != null)
        {
            audioSource.outputAudioMixerGroup = sfxMixerGroup;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!canTrigger) return;

        // Verifica se colidiu com o Player
        if (collision.gameObject.CompareTag("Player"))
        {
            TryPlayAlarm();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!canTrigger) return;

        // Caso você esteja usando Trigger ao invés de Collision
        if (other.gameObject.CompareTag("Player"))
        {
            TryPlayAlarm();
        }
    }

    private void TryPlayAlarm()
    {
        if (Time.time - lastTriggerTime < cooldownTime)
            return;

        // 20% de chance (ou o valor configurado)
        if (Random.value <= chanceToTrigger)
        {
            if (carAlarmSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(carAlarmSound);
                Debug.Log(" Alarme de carro ativado!");
            }
        }

        lastTriggerTime = Time.time;
    }

    // Método público caso queira forçar o alarme
    public void ForceAlarm()
    {
        if (carAlarmSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(carAlarmSound);
        }
    }
}