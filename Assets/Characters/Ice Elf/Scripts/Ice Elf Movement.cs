using UnityEngine;
using UnityEngine.InputSystem;

public class IceElfMovement : MonoBehaviour
{
    private Animator anim;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer; // Added for flipping

    [Header("Movement Settings")]
    [SerializeField] public float moveSpeed = 5f;
    [SerializeField] public float jumpForce = 10f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheck;

    [Header("Archery Settings")]
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private float arrowForce = 15f;
    [SerializeField] private float fireRate = 0.5f;
    private float nextFireTime = 0f;

    private bool isFacingRight = true;
    private bool isGrounded;

    void Start()
    {
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        if (anim == null) Debug.LogError("Animator not found on Ice Elf or its children!");
    }

    void Update()
    {
        // 1. Ground Check
        // We do this at the very beginning so all logic below uses the same 'isGrounded' value.
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);

        // 2. Inputs
        float moveInput = 0;
        if (Keyboard.current.aKey.isPressed) moveInput = -1;
        else if (Keyboard.current.dKey.isPressed) moveInput = 1;

        bool jumpPressed = Keyboard.current.spaceKey.wasPressedThisFrame;
        bool isShootingInput = Mouse.current.leftButton.wasPressedThisFrame;

        // 3. Movement
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);

        // 4. Jumping Logic
        if (jumpPressed && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);

            // 1. Trigger the animation
            anim.SetTrigger("JumpTrigger");

            // 2. FORCE the boolean to false so HandleAnimations doesn't kill it
            isGrounded = false;
            anim.SetBool("isGrounded", false);

            // 3. Skip HandleAnimations for just this frame
            return;
        }

        // 5. Direction Tracking & Flipping
        if (moveInput > 0 && !isFacingRight)
        {
            Flip();
        }
        else if (moveInput < 0 && isFacingRight)
        {
            Flip();
        }

        // 6. Shooting Logic
        if (isShootingInput && Time.time >= nextFireTime)
        {
            anim.SetTrigger("Shoot");
            nextFireTime = Time.time + fireRate;
        }

        // 7. Update Animator States
        HandleAnimations(moveInput, rb.linearVelocity.y);
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        // Method A: Flip via SpriteRenderer (Good for simple sprites)
        spriteRenderer.flipX = !isFacingRight;

        /* Method B: Flip via LocalScale (Better if you have attached child objects 
        like the spawnPoint that also need to flip)
        
        Vector3 scaler = transform.localScale;
        scaler.x *= -1;
        transform.localScale = scaler;
        */
    }

    public void FireArrow()
    {
        if (arrowPrefab == null || spawnPoint == null) return;

        GameObject arrow = Instantiate(arrowPrefab, spawnPoint.position, spawnPoint.rotation);
        Vector2 shootDirection = isFacingRight ? Vector2.right : Vector2.left;

        // Note: If you use Method B (localScale) in Flip(), 
        // the spawnPoint flips automatically and you don't need to flip the arrow here.
        if (!isFacingRight)
        {
            Vector3 scaler = arrow.transform.localScale;
            scaler.x *= -1;
            arrow.transform.localScale = scaler;
        }

        Rigidbody2D arrowRb = arrow.GetComponent<Rigidbody2D>();
        if (arrowRb != null)
        {
            arrowRb.AddForce(shootDirection * arrowForce, ForceMode2D.Impulse);
        }
    }

    void HandleAnimations(float move, float yVel)
    {
        // REMOVE the Physics2D.OverlapCircle line from here!
        // It is already calculated at the top of Update().

        anim.SetBool("isRunning", move != 0);
        anim.SetBool("isGrounded", isGrounded);
        anim.SetFloat("yVelocity", yVel);
    }
}