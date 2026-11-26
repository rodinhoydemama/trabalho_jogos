using UnityEngine;

public class EnemyIATrhow : MonoBehaviour
{
    private Transform player; // Referência para a posição do jogador

    [Header("Configurações do Inimigo")]
    public float detectionRange = 7f;      // Distância em que ele te detecta
    public float throwCooldown = 2f;       // Tempo entre um arremesso e outro
    public float stoneSpeed = 8f;          // Velocidade da pedra
    public int stoneDamage = 1;            // Dano causado pela pedra

    [Header("Configurações de Vigilância (Vasculhar)")]
    public float lookAroundInterval = 2.5f; // Tempo para virar de lado enquanto procura

    [Header("Referências do Arremesso")]
    public GameObject stonePrefab;         // Prefab da pedra
    public Transform throwPoint;           // Ponto de origem da pedra (opcional)

    private float nextThrowTime;
    private float nextLookTime;
    private int facingDirection = 1;       // 1 = direita, -1 = esquerda

    void Start()
    {
        // Procura automaticamente pelo objeto que tem a Tag "Player"
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
    }

    void Update()
    {
        if (player == null) return;

        // Calcula a distância 2D entre o inimigo e o jogador
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // Se o jogador estiver dentro do raio de detecção
        if (distanceToPlayer <= detectionRange)
        {
            // Olha para a direção do jogador
            if (player.position.x > transform.position.x)
            {
                facingDirection = 1;
            }
            else
            {
                facingDirection = -1;
            }

            ApplyFacingDirection();

            // Joga a pedra quando o cooldown terminar
            if (Time.time >= nextThrowTime)
            {
                ThrowStone();
                nextThrowTime = Time.time + throwCooldown;
            }
        }
        else
        {
            // Se o jogador estiver fora do alcance, vasculha a área olhando para os lados
            PatrolLookAround();
        }
    }

    void PatrolLookAround()
    {
        if (Time.time >= nextLookTime)
        {
            facingDirection *= -1; // Inverte entre 1 e -1
            ApplyFacingDirection();
            nextLookTime = Time.time + lookAroundInterval;
        }
    }

    void ApplyFacingDirection()
    {
        transform.localScale = new Vector3(facingDirection, 1, 1);
    }

    void ThrowStone()
    {
        if (stonePrefab == null) return;

        Vector3 spawnPos = throwPoint != null ? throwPoint.position : transform.position;
        GameObject stone = Instantiate(stonePrefab, spawnPos, Quaternion.identity);

        Vector2 direction = (player.position - spawnPos).normalized;

        if (stone.TryGetComponent<StoneProjectile>(out StoneProjectile projectile))
        {
            projectile.damage = stoneDamage;
            projectile.speed = stoneSpeed;
            projectile.Launch(direction, true);
        }
    }

    // Desenha um círculo vermelho no editor para visualizar o alcance de visão
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}