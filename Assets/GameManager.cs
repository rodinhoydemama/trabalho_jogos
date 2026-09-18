using UnityEngine;
using TMPro; // Importante para controlar o TextMeshPro

public class GameManager : MonoBehaviour
{
    // Singleton para que outros scripts acessem o GameManager facilmente
    public static GameManager instancia;

    public TextMeshProUGUI textoMoedas; // Arraste o seu texto da UI para cá no Inspector
    private int quantidadeMoedas = 0;

    void Awake()
    {
        // Garante que só exista um GameManager no jogo
        if (instancia == null) instancia = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        AtualizarInterface();
    }

    // Função que será chamada pelo script da moeda
    public void SomarMoeda()
    {
        quantidadeMoedas++;
        AtualizarInterface();
    }

    // Atualiza o texto visual na tela
    void AtualizarInterface()
    {
        textoMoedas.text = quantidadeMoedas.ToString();
    }
}
