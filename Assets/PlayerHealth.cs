using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHealth : MonoBehaviour
{
    [Header("Configurações de Vida")]
    public float maxHealth = 100f;
    public float currentHealth;
    private float displayHealth; // Vida visual para o efeito suave

    [Header("Velocidade da Barra")]
    public float transitionSpeed = 5f; // Quão rápido a barra corre atrás da vida real

    [Header("Referências de UI")]
    public Slider healthSlider;
    public TextMeshProUGUI healthText;
    public Image fillImage; // Opcional: Para mudar a cor da barra do player também

    void Start()
    {
        currentHealth = maxHealth;
        displayHealth = maxHealth;

        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = maxHealth;
        }

        UpdateHealthUI();
    }

    void Update()
    {
        // O Lerp roda constantemente para fazer a transição suave
        displayHealth = Mathf.Lerp(displayHealth, currentHealth, transitionSpeed * Time.deltaTime);

        // Se a diferença for minúscula, força a igualdade para poupar processamento
        if (Mathf.Abs(displayHealth - currentHealth) < 0.01f)
        {
            displayHealth = currentHealth;
        }

        // Atualiza o Slider com o valor visual (suave)
        if (healthSlider != null)
        {
            healthSlider.value = displayHealth;
        }

        // Atualiza a cor da barra com base no valor visual
        if (fillImage != null)
        {
            float healthPercentage = displayHealth / maxHealth;
            fillImage.color = Color.Lerp(Color.red, Color.green, healthPercentage);
        }

    }


    public void TakeDamage(float amount)
    {
        currentHealth -= amount;

        if (currentHealth < 0)
        {
            currentHealth = 0;
        }

        UpdateHealthUI();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(float amount)
    {
        currentHealth += amount;

        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }

        UpdateHealthUI();
    }

    void UpdateHealthUI()
    {
        // Atualiza o texto na tela (ex: arredondando para não ficar com casas decimais quebradas)
        if (healthText != null)
        {
            healthText.text = $"{Mathf.Ceil(currentHealth)} / {maxHealth}";
        }
    }

    void Die()
    {
        Debug.Log("O personagem morreu!");
        // Aqui você pode adicionar lógica de Game Over ou reiniciar a cena
    }
}