using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Configurações de Vida")]
    public int maxHealth = 100;
    private int currentHealth;

    private Animator animator;

    void Start()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
    }

    // Esta é a função que o Player e a Pedra vão chamar
    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;
        Debug.Log($"Dummy recebeu {damageAmount} de dano! Vida restante: {currentHealth}");

        // Ativa a animação de dano se você criar o trigger chamado "Hurt" no Animator do Dummy
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
        Debug.Log("Dummy foi destruído!");
        
        // Ativa a animação de morte se você criar o trigger chamado "Die" no Animator do Dummy
        if (animator != null)
        {
            animator.SetTrigger("Die");
        }

        // Destrói o Dummy após meio segundo para dar tempo de rodar a animação
        Destroy(gameObject, 0.5f);
    }
}
