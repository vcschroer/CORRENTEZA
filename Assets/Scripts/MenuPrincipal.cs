using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuPrincipal : MonoBehaviour
{
    public string nomeDaPrimeiraCena = "RuaPrincipal";
    [SerializeField] private GameObject painelCreditos;
    [SerializeField] private GameObject painelOpcoes;

    public void Jogar()
    {
        SceneManager.LoadScene(nomeDaPrimeiraCena);
    }

    public void AbrirCreditos()
    {
        painelCreditos.SetActive(true);
    }

    public void FecharCreditos()
    {
        painelCreditos.SetActive(false);
    }

    public void AbrirOpcoes()
    {
        painelOpcoes.SetActive(true);
    }

    public void FecharOpcoes()
    {
        painelOpcoes.SetActive(false);
    }

    public void Sair()
    {
        Application.Quit();
        Debug.Log("Saiu do Jogo");
    }
}