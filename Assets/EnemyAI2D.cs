using UnityEngine;

public class EnemyAI2D : MonoBehaviour
{
    private Transform player; // Referência para a posição do jogador

    [Header("Configurações do Inimigo")]
    public float moveSpeed = 3f;           // Velocidade de movimento do inimigo
    public float detectionRange = 7f;      // Distância em que ele te detecta

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
        if (player != null)
        {
            // Calcula a distância 2D entre o inimigo e o jogador
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);

            // Se o jogador estiver dentro do raio de detecção
            if (distanceToPlayer <= detectionRange)
            {
                // Move suavemente a posição do inimigo em direção ao jogador
                transform.position = Vector2.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime);

                // Opcional: Fazer o inimigo olhar para a direção do jogador (inverter o sprite no eixo X)
                if (player.position.x > transform.position.x)
                {
                    transform.localScale = new Vector3(1, 1, 1); // Olhando para a direita
                }
                else
                {
                    transform.localScale = new Vector3(-1, 1, 1); // Olhando para a esquerda
                }
            }
        }
    }

    // Desenha um círculo vermelho no editor para visualizar o alcance de visão
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}