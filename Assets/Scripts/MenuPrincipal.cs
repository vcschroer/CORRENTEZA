using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuPrincipal : MonoBehaviour
{
    public string nomeDaPrimeiraCena = "RuaPrincipal";

    public void Jogar()
    {
        SceneManager.LoadScene(nomeDaPrimeiraCena);
    }

    public void Sair()
    {
        Application.Quit();
        Debug.Log("Saiu do Jogo");
    }
}