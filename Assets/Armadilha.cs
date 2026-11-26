using UnityEngine;

public class Armadilha : MonoBehaviour
{
    [SerializeField] float dano = 50;

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
            playerHealth.TakeDamage(dano);
        }
    }
}