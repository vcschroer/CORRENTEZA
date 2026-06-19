using UnityEngine;

public class TextoSubindo : MonoBehaviour
{
    public Vector2 posicaoInicial;
    public Vector2 posicaoFinal;
    public float duracao = 2f;

    private RectTransform rectTransform;
    private float tempo;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        rectTransform.anchoredPosition = posicaoInicial;
    }

    void Update()
    {
        if (tempo < duracao)
        {
            tempo += Time.deltaTime;
            float t = tempo / duracao;

            rectTransform.anchoredPosition =
                Vector2.Lerp(posicaoInicial, posicaoFinal, t);
        }
    }
}