using System;
using Unity.Mathematics;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f; 
    public float jumpForce = 10f; 

    public Transform groundCheck;    
    
    [Header("Configurações do Sensor (Caixa)")]
    public Vector2 boxSize = new Vector2(0.4f, 0.1f); 
    public LayerMask whatIsGround;   

    [Header("Coyote time e jump buffer")]
    [SerializeField] private float coyoteTime = 0.15f;
    [SerializeField] private float jumpBufferTime = 0.15f;
    private float coyoteTimeCounter;
    private float jumpBufferCounter;

    [Header("Configurações de Ataque Físico (Z)")]
    public Transform attackPoint;      
    public float attackRange = 0.5f;    
    public LayerMask enemyLayers;      
    public int attackDamage = 25;       

    [Header("Ataque à Distância (Pedra - X)")]
    public GameObject stonePrefab;     
    public Transform throwPoint;      
    public float throwCooldown = 0.5f; // Garanta que está com esse valor padrão
    private float throwCooldownCounter;


    private Rigidbody2D rb; 
    private Animator animator; 
    private SpriteRenderer spriteRenderer; 
    public bool isGrounded; 

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        isGrounded = Physics2D.OverlapBox(groundCheck.position, boxSize, 0f, whatIsGround);

        if (isGrounded)
        {
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            jumpBufferCounter = jumpBufferTime;
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime;
        }

        UpdateAnimator();
        Movement();
        Jump();
        Attack();
    }

    private void UpdateAnimator()
    {
        float moveInput = Input.GetAxisRaw("Horizontal");
        float inputAtivo = Mathf.Abs(moveInput);

        animator.SetFloat("Speed", inputAtivo);
        animator.SetBool("IsJumping", !isGrounded);
    }

    private void Attack()
    {
        if (throwCooldownCounter > 0f)
        {
            throwCooldownCounter -= Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            animator.SetTrigger("Attack");

            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);
            foreach (Collider2D enemy in hitEnemies)
            {
                // CORRIGIDO: Procura o script no objeto atingido, no pai ou nos filhos
                EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();
                if (enemyHealth == null) enemyHealth = enemy.GetComponentInParent<EnemyHealth>();
                if (enemyHealth == null) enemyHealth = enemy.GetComponentInChildren<EnemyHealth>();
                
                if (enemyHealth != null)
                {
                    enemyHealth.TakeDamage(attackDamage);
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.X) && throwCooldownCounter <= 0f)
        {
            animator.SetTrigger("Throw");
            throwCooldownCounter = throwCooldown; 
        }
    }



    public void ThrowStone()
    {
        if (stonePrefab == null || throwPoint == null) return;

        GameObject newStone = Instantiate(stonePrefab, throwPoint.position, Quaternion.identity);
        StoneProjectile projectile = newStone.GetComponent<StoneProjectile>();

        if (projectile != null)
        {
            Vector2 shootDirection = spriteRenderer.flipX ? Vector2.left : Vector2.right;
            projectile.Launch(shootDirection);
        }
    }


    private void Jump()
    {
        // Só permite o pulo se o Buffer e o Coyote Time forem maiores que zero
        if (jumpBufferCounter > 0f && coyoteTimeCounter > 0f)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);

            // TRAVA ANTI-PULO DUPLO: Zeramos IMEDIATAMENTE os contadores
            // Isso impede que o Unity leia o comando de pulo duas vezes seguidas no ar
            jumpBufferCounter = 0f;
            coyoteTimeCounter = 0f;
        }
    }


    private void Movement()
    {
        float moveInput = Input.GetAxisRaw("Horizontal"); 

        rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);
        MirrorSprite(moveInput);
    }

    private void MirrorSprite(float moveInput)
    {
        if (moveInput < 0) spriteRenderer.flipX = true;
        else if (moveInput > 0) spriteRenderer.flipX = false;
        MirrorChildren();
    }

    private void MirrorChildren()
    {
        foreach (var child in transform.GetComponentsInChildren<Transform>())
        {
            if (child == transform) continue;
            Quaternion newRotation = Quaternion.identity;
            if (spriteRenderer.flipX) newRotation = Quaternion.Euler(0f, 180f, 0f);
            child.localRotation = newRotation;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(groundCheck.position, boxSize);
        }

        if (attackPoint != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(attackPoint.position, attackRange);
        }
    }
}
