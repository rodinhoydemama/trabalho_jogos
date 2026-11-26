using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuInicial : MonoBehaviour
{
    [Header("Painéis da UI")]
    [SerializeField] private GameObject painelMenuPrincipal;
    [SerializeField] private GameObject painelCreditos;

    [Header("Configuração da Cena")]
    [SerializeField] private string nomeCenaDoJogo = "Jogo";

    private void Start()
    {
        MostrarMenuPrincipal();
    }

    public void Jogar()
    {
        SceneManager.LoadScene(nomeCenaDoJogo);
    }

    public void AbrirCreditos()
    {
        if (painelMenuPrincipal != null) painelMenuPrincipal.SetActive(false);
        if (painelCreditos != null) painelCreditos.SetActive(true);
    }

    public void MostrarMenuPrincipal()
    {
        if (painelCreditos != null) painelCreditos.SetActive(false);
        if (painelMenuPrincipal != null) painelMenuPrincipal.SetActive(true);
    }

    public void SairDoJogo()
    {
        Application.Quit();
        Debug.Log("Sair do jogo (funciona no build final).");
    }
}