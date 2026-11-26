using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    [Header("Configurações de Vida")]
    public int maxHealth = 100;
    private float currentHealth; // Mudado para float para o cálculo suave funcionar perfeitamente
    private float displayHealth; // Vida visual da barra

    [Header("Referências de UI")]
    public Slider enemyHealthSlider;
    public Image fillImage;

    [Header("Velocidade da Barra")]
    public float transitionSpeed = 5f; // Quão rápido a barra corre atrás da vida real

    private Animator animator;

    void Start()
    {
        currentHealth = maxHealth;
        displayHealth = maxHealth;
        animator = GetComponent<Animator>();

        if (enemyHealthSlider != null)
        {
            enemyHealthSlider.maxValue = maxHealth;
            enemyHealthSlider.value = maxHealth;
        }
    }

    void Update()
    {
        // Faz a barra visual ir deslizando suavemente até alcançar a vida real
        if (displayHealth != currentHealth)
        {
            displayHealth = Mathf.Lerp(displayHealth, currentHealth, transitionSpeed * Time.deltaTime);

            if (enemyHealthSlider != null)
            {
                enemyHealthSlider.value = displayHealth;
            }

            // Atualiza a cor com base na porcentagem visual
            if (fillImage != null)
            {
                float healthPercentage = displayHealth / maxHealth;
                fillImage.color = Color.Lerp(Color.red, Color.green, healthPercentage);
            }
        }
    }

    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;

        if (currentHealth < 0)
        {
            currentHealth = 0;
        }

        Debug.Log($"Inimigo recebeu {damageAmount} de dano! Vida restante: {currentHealth}");

        if (animator != null)
        {
            animator.SetTrigger("Hurt");
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Inimigo foi destruído!");

        if (animator != null)
        {
            animator.SetTrigger("Die");
        }

        Destroy(gameObject, 0.5f);
    }
}