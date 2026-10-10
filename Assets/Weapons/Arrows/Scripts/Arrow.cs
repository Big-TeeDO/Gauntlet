using UnityEngine;

public class Arrow : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator anim;
    private bool hasHit = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        // Optional: Destroy arrow after 5 seconds if it hits nothing to save memory
        Destroy(gameObject, 5f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasHit) return;

        if (collision.CompareTag("Enemy"))
        {
            hasHit = true;

            // 1. Lower the Health
            HealthSystem enemyHealth = collision.GetComponent<HealthSystem>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(HealthSystem.DamageType.Arrow);
            }

            // 2. Play the Skeleton's 'TakeDamage' animation
            // This finds the function you have in EnemyAI2D
            EnemyAI2D ai = collision.GetComponent<EnemyAI2D>();
            if (ai != null)
            {
                ai.TakeDamage();
            }

            HitTarget();
        }
        else if (collision.CompareTag("Environment"))
        {
            hasHit = true;
            HitTarget();
        }
    }

    void HitTarget()
    {
        Debug.Log("Arrow hit");
        hasHit = true;

        // 1. Stop the physics
        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Static;

        // 2. DISABLE THE SPRITE RENDERER
        // This removes the arrow image immediately so only the animation shows
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null) sr.enabled = false;

        // 3. Play the disappearing animation
        anim.SetTrigger("isHit");

        // 4. Destroy after the animation finishes
        Destroy(gameObject, 0.5f);
    }
}