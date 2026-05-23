using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private string nomeDaPrimeiraCenaMenu;
    [SerializeField] private GameObject painelPause;
    [SerializeField] private GameObject botaoPause;

    private bool jogoPausado = false;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (jogoPausado)
                Retomar();
            else
                Pause();
        }
    }

    public void Pause()
    {
        jogoPausado = true;
        painelPause.SetActive(true);
        if (botaoPause != null) botaoPause.SetActive(false);

        Time.timeScale = 0f;
    }

    public void Retomar()
    {
        jogoPausado = false;
        painelPause.SetActive(false);
        if (botaoPause != null) botaoPause.SetActive(true);

        Time.timeScale = 1f;
    }

    public void VoltarMenu()
    {
        Time.timeScale = 1f;
        Destroy(gameObject);
        SceneManager.LoadScene(nomeDaPrimeiraCenaMenu);
    }

    public void Sair()
    {
        Time.timeScale = 1f;
        Application.Quit();
        Debug.Log("Saiu do Jogo");
    }
}