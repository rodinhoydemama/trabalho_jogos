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

    [Header("Wall Jump e Wall Slide")]
    public Transform wallCheck;
    public Vector2 wallBoxSize = new Vector2(0.15f, 0.6f);
    public LayerMask whatIsWall;
    public float wallSlideSpeed = 2f;
    public Vector2 wallJumpForce = new Vector2(8f, 11f);
    public float wallJumpLockTime = 0.15f;
    private bool isTouchingWall;
    private bool isWallSliding;
    private int wallDirection; // -1 = parede à esquerda, 1 = parede à direita
    private float wallJumpLockCounter;

    [Header("Dash")]
    public float dashSpeed = 16f;
    public float dashDuration = 0.16f;
    public float dashCooldown = 0.8f;
    public bool canAirDash = true;
    private bool isDashing;
    private bool hasAirDash;
    private float dashTimeCounter;
    private float dashCooldownCounter;
    private Vector2 dashDirection;
    private float originalGravityScale;

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
        originalGravityScale = rb.gravityScale;
    }

    void Update()
    {
        isGrounded = Physics2D.OverlapBox(groundCheck.position, boxSize, 0f, whatIsGround);
        CheckWall();

        if (isGrounded)
        {
            coyoteTimeCounter = coyoteTime;
            hasAirDash = true;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        if (wallJumpLockCounter > 0f)
        {
            wallJumpLockCounter -= Time.deltaTime;
        }

        if (dashCooldownCounter > 0f)
        {
            dashCooldownCounter -= Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            jumpBufferCounter = jumpBufferTime;
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime;
        }

        HandleDashInput();
        UpdateWallSlide();

        if (!isDashing)
        {
            UpdateAnimator();
            Movement();
            Jump();
            Attack();
        }
        else
        {
            DashStep();
        }
    }

    private void CheckWall()
    {
        if (wallCheck == null)
        {
            isTouchingWall = false;
            return;
        }

        isTouchingWall = Physics2D.OverlapBox(wallCheck.position, wallBoxSize, 0f, whatIsWall);
        if (isTouchingWall)
        {
            // Descobre de qual lado a parede está em relação ao centro do player
            float diffX = wallCheck.position.x - transform.position.x;
            if (Mathf.Abs(diffX) > 0.01f)
            {
                wallDirection = diffX > 0f ? 1 : -1;
            }
            else
            {
                wallDirection = spriteRenderer.flipX ? -1 : 1;
            }
        }
    }

    private void UpdateWallSlide()
    {
        float moveInput = Input.GetAxisRaw("Horizontal");
        bool pushingIntoWall = (moveInput > 0 && wallDirection == 1) || (moveInput < 0 && wallDirection == -1);

        if (!isGrounded && isTouchingWall && rb.velocity.y < 0f && pushingIntoWall && !isDashing)
        {
            isWallSliding = true;
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Max(rb.velocity.y, -wallSlideSpeed));
        }
        else
        {
            isWallSliding = false;
        }
    }

    private void HandleDashInput()
    {
        if (isDashing || dashCooldownCounter > 0f) return;

        bool podeDashar = isGrounded || (canAirDash && hasAirDash);
        if (Input.GetKeyDown(KeyCode.LeftShift) && podeDashar)
        {
            float moveInput = Input.GetAxisRaw("Horizontal");
            float dirX = moveInput != 0f ? Mathf.Sign(moveInput) : (spriteRenderer.flipX ? -1f : 1f);

            dashDirection = new Vector2(dirX, 0f);
            isDashing = true;
            dashTimeCounter = dashDuration;
            dashCooldownCounter = dashCooldown;

            if (!isGrounded)
            {
                hasAirDash = false;
            }

            MirrorSprite(dirX);
            rb.gravityScale = 0f;
            rb.velocity = dashDirection * dashSpeed;
            animator.SetTrigger("Dash");
        }
    }

    private void DashStep()
    {
        dashTimeCounter -= Time.deltaTime;
        rb.velocity = dashDirection * dashSpeed;

        if (dashTimeCounter <= 0f)
        {
            isDashing = false;
            rb.gravityScale = originalGravityScale;
            rb.velocity = new Vector2(0f, rb.velocity.y);
        }
    }

    private void UpdateAnimator()
    {
        float moveInput = Input.GetAxisRaw("Horizontal");
        float inputAtivo = Mathf.Abs(moveInput);

        animator.SetFloat("Speed", inputAtivo);
        animator.SetBool("IsJumping", !isGrounded);
        animator.SetBool("IsWallSliding", isWallSliding);
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
        if (jumpBufferCounter <= 0f) return;

        // 1. Pulo normal / Coyote Time
        if (coyoteTimeCounter > 0f)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            jumpBufferCounter = 0f;
            coyoteTimeCounter = 0f;
        }
        // 2. Wall Jump (quando encosta na parede ou desliza nela)
        else if (isTouchingWall || isWallSliding)
        {
            int awayDirection = -wallDirection;
            rb.velocity = new Vector2(awayDirection * wallJumpForce.x, wallJumpForce.y);
            wallJumpLockCounter = wallJumpLockTime;
            jumpBufferCounter = 0f;
            isWallSliding = false;

            MirrorSprite(awayDirection);
        }
    }


    private void Movement()
    {
        if (wallJumpLockCounter > 0f) return;

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

        if (wallCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(wallCheck.position, wallBoxSize);
        }

        if (attackPoint != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(attackPoint.position, attackRange);
        }
    }
}