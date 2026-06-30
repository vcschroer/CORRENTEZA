using UnityEngine;
using UnityEngine.UI;

public class ScreenFade : MonoBehaviour
{
    public static ScreenFade Instance;

    public Image fadeImage;

    void Awake()
    {
        Instance = this;

        if (fadeImage == null)
            fadeImage = GetComponentInChildren<Image>();

        var canvasGroup = fadeImage.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = fadeImage.gameObject.AddComponent<CanvasGroup>();

        fadeImage.gameObject.SetActive(true);
        fadeImage.color = Color.black;
    }

    public void FadeOut(float duration = 1.2f)
    {
        var canvasGroup = fadeImage.GetComponent<CanvasGroup>();
        StartCoroutine(Fade(canvasGroup, 1f, 0f, duration));
    }

    private System.Collections.IEnumerator Fade(CanvasGroup group, float from, float to, float duration)
    {
        float start = Time.time;
        while (Time.time < start + duration)
        {
            float t = (Time.time - start) / duration;
            group.alpha = Mathf.Lerp(from, to, t);
            yield return null;
        }
        group.alpha = to;
    }
}