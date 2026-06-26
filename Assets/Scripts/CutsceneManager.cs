using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CutsceneManager : MonoBehaviour
{
    [Header("Configurações da Cutscene")]
    public Image cutsceneImage;
    public Sprite[] frames;

    [Tooltip("Tempo que cada frame fica na tela")]
    public float timePerFrame = 0.8f;

    [Tooltip("Tempo de fade entre frames")]
    public float fadeDuration = 0.3f;

    public bool autoStart = true;

    private CanvasGroup canvasGroup;

    void Awake()
    {
        if (cutsceneImage == null)
            cutsceneImage = GetComponentInChildren<Image>();

        canvasGroup = cutsceneImage.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = cutsceneImage.gameObject.AddComponent<CanvasGroup>();
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
        canvasGroup.alpha = 0f;

        for (int i = 0; i < frames.Length; i++)
        {
            cutsceneImage.sprite = frames[i];

            yield return StartCoroutine(Fade(0f, 1f, fadeDuration));

            yield return new WaitForSeconds(timePerFrame);

            if (i < frames.Length - 1)
            {
                yield return StartCoroutine(Fade(1f, 0f, fadeDuration));
            }
        }

        yield return new WaitForSeconds(2f);

        OnCutsceneEnd();
    }

    private IEnumerator Fade(float from, float to, float duration)
    {
        float startTime = Time.time;
        while (Time.time < startTime + duration)
        {
            float t = (Time.time - startTime) / duration;
            canvasGroup.alpha = Mathf.Lerp(from, to, t);
            yield return null;
        }
        canvasGroup.alpha = to;
    }

    private void OnCutsceneEnd()
    {
        Debug.Log("Cutscene finalizada!");
        SceneManager.LoadScene("CreditsScene");
    }

    // Chame este método para iniciar a cutscene
    public void StartCutscene()
    {
        StartCoroutine(PlayCutscene());
    }
}