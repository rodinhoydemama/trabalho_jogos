using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    public GameObject posicaoDeRetorno;
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.position = posicaoDeRetorno.transform.position;
        }
    }
}