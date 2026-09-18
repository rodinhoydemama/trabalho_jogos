using UnityEngine;

public class ColetarMoeda : MonoBehaviour
{
    private bool jaFoiColetada = false;

    private void OnTriggerEnter2D(Collider2D outro)
    {
        if (jaFoiColetada) return;

        if (outro.CompareTag("Player"))
        {
            jaFoiColetada = true;

            // 1. Desativa o Collider para nenhuma outra colisão física acontecer
            if (TryGetComponent<Collider2D>(out Collider2D meuCollider))
            {
                meuCollider.enabled = false;
            }

            // 2. Desativa a imagem para ela sumir visualmente na hora
            if (TryGetComponent<SpriteRenderer>(out SpriteRenderer meuSprite))
            {
                meuSprite.enabled = false;
            }

            // 3. Avisa o GameManager e destrói o objeto
            GameManager.instancia.SomarMoeda();
            Destroy(gameObject);
        }
    }
}
