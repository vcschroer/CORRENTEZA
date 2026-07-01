using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

[System.Serializable]
public class CutsceneFrame
{
    public Sprite sprite;

    [Header("Transform")]
    public Vector2 scale = Vector2.one;
    public Vector2 position = Vector2.zero;

    [Header("Áudio por Frame")]
    public AudioClip audioClip;
    [Tooltip("Se marcado, o áudio fica em loop enquanto o frame estiver visível")]
    public bool loopAudio = false;

    [Tooltip("Tempo que este frame fica na tela")]
    public float duration = 0.8f;
}

public class CutsceneManager : MonoBehaviour
{
    [Header("Configurações da Cutscene - Imagens")]
    public UnityEngine.UI.Image cutsceneImage;
    public CutsceneFrame[] frames;

    public float fadeDuration = 0.3f;

    [Header("Configurações da Cutscene - Textos")]
    public TextMeshProUGUI cutsceneText;
    [TextArea(3, 10)]
    public string[] texts;
    public float timePerText = 3.5f;
    public float textFadeDuration = 0.6f;

    public bool autoStart = true;

    private CanvasGroup imageCanvasGroup;
    private CanvasGroup textCanvasGroup;
    private AudioSource cutsceneAudioSource;
    private Vector2 originalPosition;

    void Awake()
    {
        if (cutsceneImage == null)
            cutsceneImage = GetComponentInChildren<UnityEngine.UI.Image>();

        imageCanvasGroup = cutsceneImage.GetComponent<CanvasGroup>();
        if (imageCanvasGroup == null)
            imageCanvasGroup = cutsceneImage.gameObject.AddComponent<CanvasGroup>();

        // Áudio
        cutsceneAudioSource = GetComponent<AudioSource>();
        if (cutsceneAudioSource == null)
            cutsceneAudioSource = gameObject.AddComponent<AudioSource>();

        cutsceneAudioSource.playOnAwake = false;

        if (cutsceneImage != null)
            originalPosition = cutsceneImage.rectTransform.anchoredPosition;

        if (cutsceneText != null)
        {
            textCanvasGroup = cutsceneText.GetComponent<CanvasGroup>();
            if (textCanvasGroup == null)
                textCanvasGroup = cutsceneText.gameObject.AddComponent<CanvasGroup>();
        }
    }

    void Start()
    {
        if (autoStart && frames.Length > 0)
            StartCoroutine(PlayCutscene());
    }

    public IEnumerator PlayCutscene()
    {
        cutsceneImage.gameObject.SetActive(true);
        imageCanvasGroup.alpha = 0f;

        for (int i = 0; i < frames.Length; i++)
        {
            var frame = frames[i];

            // Aplica visual
            cutsceneImage.sprite = frame.sprite;
            cutsceneImage.rectTransform.localScale = frame.scale;
            cutsceneImage.rectTransform.anchoredPosition = frame.position;

            // Áudio do frame
            PlayFrameAudio(frame);

            yield return StartCoroutine(Fade(imageCanvasGroup, 0f, 1f, fadeDuration));
            yield return new WaitForSeconds(frame.duration);

            if (i < frames.Length - 1)
                yield return StartCoroutine(Fade(imageCanvasGroup, 1f, 0f, fadeDuration));
        }

        // Final das imagens
        cutsceneImage.rectTransform.anchoredPosition = originalPosition;
        yield return StartCoroutine(Fade(imageCanvasGroup, 1f, 0f, fadeDuration));

        // === TEXTOS ===
        if (cutsceneText != null && texts.Length > 0)
        {
            cutsceneText.gameObject.SetActive(true);
            textCanvasGroup.alpha = 0f;

            for (int i = 0; i < texts.Length; i++)
            {
                cutsceneText.text = texts[i];
                yield return StartCoroutine(Fade(textCanvasGroup, 0f, 1f, textFadeDuration));
                yield return new WaitForSeconds(timePerText);

                if (i < texts.Length - 1)
                    yield return StartCoroutine(Fade(textCanvasGroup, 1f, 0f, textFadeDuration));
            }

            yield return StartCoroutine(Fade(textCanvasGroup, 1f, 0f, textFadeDuration));
        }

        yield return new WaitForSeconds(1.5f);
        OnCutsceneEnd();
    }

    private void PlayFrameAudio(CutsceneFrame frame)
    {
        if (frame.audioClip == null || cutsceneAudioSource == null) return;

        cutsceneAudioSource.Stop();
        cutsceneAudioSource.clip = frame.audioClip;
        cutsceneAudioSource.loop = frame.loopAudio;
        cutsceneAudioSource.Play();
    }

    private IEnumerator Fade(CanvasGroup group, float from, float to, float duration)
    {
        float startTime = Time.time;
        while (Time.time < startTime + duration)
        {
            float t = (Time.time - startTime) / duration;
            group.alpha = Mathf.Lerp(from, to, t);
            yield return null;
        }
        group.alpha = to;
    }

    private void OnCutsceneEnd()
    {
        Debug.Log("Cutscene finalizada!");
        SceneManager.LoadScene("CreditsScene");
    }

    public void StartCutscene()
    {
        StartCoroutine(PlayCutscene());
    }
}