using UnityEngine;

public class StoneProjectile : MonoBehaviour
{
    [Header("Configurações da Pedra")]
    public float speed = 10f;
    public int damage = 15;
    public float lifetime = 3f; // Tempo para a pedra sumir se não acertar nada

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        
        // Destrói a pedra automaticamente após alguns segundos para não pesar o jogo
        Destroy(gameObject, lifetime);
    }

    // Função que o Player vai chamar para dar o impulso inicial na pedra
    public void Launch(Vector2 direction)
    {
        // Certifica de pegar o Rigidbody caso o Start ainda não tenha rodado
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        
        // Aplica velocidade na direção correta
        rb.velocity = direction * speed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 1. IGNORA TOTALMENTE SE ENCOSTAR NO PLAYER (Evita que a pedra se autodestrua na mão)
        if (collision.CompareTag("Player") || collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            return; // Sai da função e não faz nada
        }

        // 2. Verifica se colidiu com um inimigo
        EnemyHealth enemy = collision.GetComponent<EnemyHealth>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);
            Destroy(gameObject); // Some ao acertar o Dummy
            return;
        }

        // 3. Se colidir com o cenário (chão/paredes/Tilemap), ela se destrói
        // Certifique-se de que seu cenário NÃO está na layer Default se der erro aqui
        if (!collision.isTrigger)
        {
            Destroy(gameObject);
        }
    }

}
