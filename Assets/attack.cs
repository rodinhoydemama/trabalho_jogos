using UnityEngine;

public class SimpleAttackAnimation : MonoBehaviour
{
    public Animator animator;

    void Update()
    {
        // Se a tecla Z for pressionada, aciona a animação de ataque
        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (animator != null)
            {
                animator.SetTrigger("Attack");
            }
        }
    }
}