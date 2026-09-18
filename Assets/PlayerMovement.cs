using System;
using Unity.Mathematics;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f; 
    public float jumpForce = 10f; 

    [Header("Configurações do Sensor de Chão")]
    public Transform groundCheck;    // Arraste o objeto dos pés aqui
    public float checkRadius = 0.2f;  // Tamanho do sensor
    public LayerMask whatIsGround;   // Selecione a Layer do chão no Inspector

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
        // Esta linha substitui o OnCollisionEnter e NUNCA falha no Tilemap
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, whatIsGround);

        UpdateAnimator();
        Movement();
        Jump();
        Attack();
    }

    private void UpdateAnimator()
    {
        float moveInput = Input.GetAxisRaw("Horizontal");

        // Se moveInput for diferente de 0, significa que alguma tecla está sendo pressionada
        // Usamos Mathf.Abs para transformar -1 (esquerda) em 1, garantindo que o valor seja sempre positivo
        float inputAtivo = Mathf.Abs(moveInput);

        // Envia 1 se estiver clicando e 0 se não estiver clicando para o parâmetro "Speed"
        animator.SetFloat("Speed", inputAtivo);
        
        animator.SetBool("IsJumping", !isGrounded);
    }

    private void Attack()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            animator.SetTrigger("Attack");
        }
    }

    private void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
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
            child.rotation = newRotation;
        }
    }

    // Desenha uma esfera vermelha na janela Scene para você ver o sensor nos pés
    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, checkRadius);
        }
    }
}
