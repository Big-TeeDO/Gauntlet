using UnityEngine;
using UnityEngine.UI;
using System.Collections; // Required for IEnumerator
using TMPro;

public class HealthSystem : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 10;
    public int currentHealth;
    public Slider healthSlider;
    public TextMeshProUGUI healthText;

    public enum DamageType { Sword, Arrow, Magic }

    [Header("Hit Reaction Type")]
    [Tooltip("Check this for the Player. Uncheck this for Skeletons/Enemies.")]
    public bool useBlinkEffect = true;
    public string animationTriggerName = "isHit";

    [Header("Blink Settings (Player Only)")]
    public float blinkDuration = 1.0f;
    public float blinkInterval = 0.1f;

    private SpriteRenderer spriteRenderer;
    private Animator anim;
    private Rigidbody2D rb;
    private bool isInvulnerable = false;

    void Start()
    {
        currentHealth = maxHealth;

        // Grab components from children or the object itself
        anim = GetComponentInChildren<Animator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();

        UpdateUI();
    }

    public void TakeDamage(DamageType type)
    {
        // If player is currently blinking, they are invulnerable
        if (isInvulnerable) return;

        int damageAmount = 0;
        switch (type)
        {
            case DamageType.Sword: damageAmount = 3; break;
            case DamageType.Arrow: damageAmount = 1; break;
            case DamageType.Magic: damageAmount = 5; break;
        }

        currentHealth -= damageAmount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        UpdateUI();

        if (currentHealth > 0)
        {
            // --- BRANCHING HIT REACTION ---
            if (useBlinkEffect)
            {
                StartCoroutine(BlinkRoutine());
            }
            else if (anim != null)
            {
                anim.SetTrigger(animationTriggerName);
            }
        }
        else
        {
            Die();
        }
    }

    private IEnumerator BlinkRoutine()
    {
        isInvulnerable = true;
        float elapsed = 0f;

        while (elapsed < blinkDuration)
        {
            if (spriteRenderer != null)
                spriteRenderer.enabled = !spriteRenderer.enabled;

            yield return new WaitForSeconds(blinkInterval);
            elapsed += blinkInterval;
        }

        // Ensure sprite is visible and vulnerability returns
        if (spriteRenderer != null) spriteRenderer.enabled = true;
        isInvulnerable = false;
    }

    void UpdateUI()
    {
        if (healthSlider != null)
        {
            healthSlider.value = currentHealth;
        }

        if (healthText != null)
        {
            healthText.text = currentHealth.ToString() + " / " + maxHealth.ToString();
        }
    }

    void Die()
    {
        Debug.Log(gameObject.name + " has died.");

        // 1. Trigger the death animation
        if (anim != null) anim.SetTrigger("isDead");

        // 2. Stop horizontal movement ONLY (Let gravity pull them down)
        if (rb != null)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            // We do NOT set Static yet, so they can fall to the floor
        }

        // 3. Disable AI or Player movement scripts
        var enemyAI = GetComponent<EnemyAI2D>();
        if (enemyAI != null) enemyAI.enabled = false;

        var playerMove = GetComponent<IceElfMovement>();
        if (playerMove != null) playerMove.enabled = false;

        // 4. Hide the Health Bar
        if (healthSlider != null) healthSlider.gameObject.SetActive(false);

        // 5. Start the cleanup process (Wait for them to land before freezing)
        StartCoroutine(HandleDeathPhysics());
    }

    private IEnumerator HandleDeathPhysics()
    {
        // Wait a brief moment for the character to actually hit the ground
        yield return new WaitForSeconds(0.5f);

        // Now that they are likely on the ground, freeze them and disable collision
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Static;
        }

        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;
    }
}