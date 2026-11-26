using UnityEngine;

public class StoneProjectile : MonoBehaviour
{
    [Header("Configurações da Pedra")]
    public float speed = 10f;
    public int damage = 15;
    public float lifetime = 3f; // Tempo para a pedra sumir se não acertar nada

    private Rigidbody2D rb;
    private bool thrownByEnemy = false; // Define se foi o inimigo que jogou

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Destrói a pedra automaticamente após alguns segundos para não pesar o jogo
        Destroy(gameObject, lifetime);
    }

    // Função que o Player (ou Inimigo) vai chamar para dar o impulso inicial na pedra
    public void Launch(Vector2 direction, bool fromEnemy = false)
    {
        thrownByEnemy = fromEnemy;

        // Certifica de pegar o Rigidbody caso o Start ainda não tenha rodado
        if (rb == null) rb = GetComponent<Rigidbody2D>();

        // Aplica velocidade na direção correta
        rb.velocity = direction * speed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (thrownByEnemy)
        {
            // 1. IGNORA SE ENCOSTAR EM INIMIGOS (Evita que a pedra se autodestrua no inimigo que lançou)
            if (collision.CompareTag("Enemy") || collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            {
                return;
            }

            // 2. Verifica se colidiu com o Player e aplica o dano
            if (collision.CompareTag("Player") || collision.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                // Troque "TakeDamage" pelo nome exato da função de dano no seu script do Player
                collision.SendMessage("TakeDamage", damage, SendMessageOptions.DontRequireReceiver);
                Destroy(gameObject);
                return;
            }
        }
        else
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
        }

        // 3. Se colidir com o cenário (chão/paredes/Tilemap), ela se destrói
        // Certifique-se de que seu cenário NÃO está na layer Default se der erro aqui
        if (!collision.isTrigger)
        {
            Destroy(gameObject);
        }
    }
}