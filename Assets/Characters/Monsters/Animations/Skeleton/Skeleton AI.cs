using UnityEngine;

public class EnemyAI2D : MonoBehaviour
{
    public Transform player;
    public float moveSpeed = 3f;
    public float attackRange = 2f;
    public float chaseRange = 10f;
    public float attackCooldown = 1.5f;

    private float lastAttackTime;
    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer spriteRenderer;

    // 1. ADD THIS VARIABLE
    private bool isAggroed = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        // This looks through all children to find the renderer and animator
        anim = GetComponentInChildren<Animator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player").transform;

        // Sanity check: if these are null, the game will log an error instead of crashing silently
        if (spriteRenderer == null) Debug.LogError("Check: No SpriteRenderer found in children!");
        if (anim == null) Debug.LogError("Check: No Animator found in children!");
    }

    void Update()
    {
        float distance = Vector2.Distance(player.position, transform.position);

        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Take Hit"))
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Attack1"))
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            return;
        }

        if (distance <= attackRange)
        {
            if (Time.time - lastAttackTime >= attackCooldown)
            {
                AttackPlayer();
            }
            else
            {
                StayIdle();
            }
        }
        // 2. MODIFY THIS LINE to include isAggroed
        else if (distance <= chaseRange || isAggroed)
        {
            ChasePlayer();
        }
        else
        {
            StayIdle();
        }
    }

    void AttackPlayer()
    {
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        spriteRenderer.flipX = (player.position.x < transform.position.x);
        anim.SetTrigger("attack");
        anim.SetBool("isWalking", false);
        lastAttackTime = Time.time;
    }

    void StayIdle()
    {
        rb.linearVelocity = Vector2.zero;
        anim.SetBool("isWalking", false);
    }

    void ChasePlayer()
    {
        

        // If Player X (10) > Enemy X (5), direction is 1 (Right). 
        // If Player X (2) < Enemy X (5), direction is -1 (Left).
        float direction = player.position.x > transform.position.x ? 1 : -1;

        // Apply movement
        rb.linearVelocity = new Vector2(direction * moveSpeed, rb.linearVelocity.y);

        // Flip the sprite based on direction
        // If moving right (direction 1), flipX should be false (assuming sprite faces right)
        // If moving left (direction -1), flipX should be true
        spriteRenderer.flipX = (direction < 0);

        anim.SetBool("isWalking", true);

    }

    public void TakeDamage()
    {
        // 3. SET AGGRO TO TRUE
        isAggroed = true;

        anim.SetBool("isWalking", false);
        anim.SetTrigger("isHit");
        Debug.Log("Enemy hit by arrow! Now chasing.");
    }

    public void ProcDamage()
    {
        // Check distance one last time to make sure player didn't dash away
        float distance = Vector2.Distance(player.position, transform.position);

        if (distance <= attackRange)
        {
            HealthSystem playerHealth = player.GetComponent<HealthSystem>();
            if (playerHealth != null)
            {
                // You can choose which DamageType the enemy deals here
                playerHealth.TakeDamage(HealthSystem.DamageType.Sword);
                Debug.Log("Enemy dealt damage to player!");
            }
        }
    }
}