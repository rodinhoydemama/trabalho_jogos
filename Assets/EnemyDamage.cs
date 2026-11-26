using UnityEngine;

public class EnemyDamage : MonoBehaviour
{
    [Header("Configurações de Dano")]
    public float damageAmount = 10f;       // Quanto de dano o inimigo causa
    public float damageCooldown = 1f;      // Tempo de espera entre cada dano
    private float lastDamageTime;

    // Usamos OnCollisionStay2D para verificar se ele continua encostando no player
    private void OnCollisionStay2D(Collision2D collision)
    {
        // Verifica se o objeto que ele encostou tem a Tag "Player"
        if (collision.gameObject.CompareTag("Player"))
        {
            // Verifica se já passou o tempo do cooldown
            if (Time.time >= lastDamageTime + damageCooldown)
            {
                // Pega o script PlayerHealth do jogador
                PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();

                if (playerHealth != null)
                {
                    // Aplica o dano usando o método que você já tem no PlayerHealth
                    playerHealth.TakeDamage(damageAmount);

                    // Atualiza o tempo do último dano causado
                    lastDamageTime = Time.time;
                }
            }
        }
    }
}