using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

[System.Serializable]
public class CutsceneFrame
{
    public Sprite sprite;

    [Header("Transform do Frame")]
    [Tooltip("Escala do sprite (1,1 = tamanho original)")]
    public Vector2 scale = Vector2.one;

    [Tooltip("Posição ancorada (0,0 = centro da tela)")]
    public Vector2 position = Vector2.zero;

    [Tooltip("Tempo que este frame fica na tela")]
    public float duration = 0.8f;
}

public class CutsceneManager : MonoBehaviour
{
    [Header("Configurações da Cutscene - Imagens")]
    public UnityEngine.UI.Image cutsceneImage;
    public CutsceneFrame[] frames;

    public float fadeDuration = 0.3f;

    [Header("Configurações da Cutscene - Textos (TextMeshPro)")]
    public TextMeshProUGUI cutsceneText;
    [TextArea(3, 10)]
    public string[] texts;
    public float timePerText = 3.5f;
    public float textFadeDuration = 0.6f;

    public bool autoStart = true;

    private CanvasGroup imageCanvasGroup;
    private CanvasGroup textCanvasGroup;
    private Vector2 originalAnchoredPosition; // Para restaurar posição

    void Awake()
    {
        if (cutsceneImage == null)
            cutsceneImage = GetComponentInChildren<UnityEngine.UI.Image>();

        imageCanvasGroup = cutsceneImage.GetComponent<CanvasGroup>();
        if (imageCanvasGroup == null)
            imageCanvasGroup = cutsceneImage.gameObject.AddComponent<CanvasGroup>();

        // Salva posição original
        if (cutsceneImage != null)
            originalAnchoredPosition = cutsceneImage.rectTransform.anchoredPosition;

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
        {
            StartCoroutine(PlayCutscene());
        }
    }

    public IEnumerator PlayCutscene()
    {
        cutsceneImage.gameObject.SetActive(true);
        imageCanvasGroup.alpha = 0f;

        for (int i = 0; i < frames.Length; i++)
        {
            var frame = frames[i];

            // Aplica Sprite, Escala e Posição
            cutsceneImage.sprite = frame.sprite;
            cutsceneImage.rectTransform.localScale = frame.scale;
            cutsceneImage.rectTransform.anchoredPosition = frame.position;

            yield return StartCoroutine(Fade(imageCanvasGroup, 0f, 1f, fadeDuration));

            yield return new WaitForSeconds(frame.duration);

            if (i < frames.Length - 1)
                yield return StartCoroutine(Fade(imageCanvasGroup, 1f, 0f, fadeDuration));
        }

        // Restaura posição original e fade out final
        cutsceneImage.rectTransform.anchoredPosition = originalAnchoredPosition;
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