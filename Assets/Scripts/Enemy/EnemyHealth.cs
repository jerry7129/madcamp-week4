using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Settings")]
    public int maxHealth = 100;
    public int currentHealth;
    public bool isFinalBoss = false; // Add this flag

    [Header("Visuals")]
    public SpriteRenderer spriteRenderer;
    public Color hitColor = Color.red;
    private Color originalColor;

    void Awake()
    {
        currentHealth = maxHealth;
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null) originalColor = spriteRenderer.color;
    }

    // Events for UI
    public System.Action<float> OnHealthChanged;

    // Triggered when Player's Attack Collider (Trigger) touches this Enemy (Trigger usually)
    void OnTriggerEnter2D(Collider2D other)
    {
        // Removed passive check. Damage is now handled by DamageDealer.cs
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log($"Enemy Hit! remaining HP: {currentHealth}");

        // Update UI if anyone is listening
        if (OnHealthChanged != null)
        {
            float ratio = (float)currentHealth / maxHealth;
            OnHealthChanged.Invoke(ratio);
        }

        // Flash Red
        if (spriteRenderer != null) StartCoroutine(FlashRoutine());

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    System.Collections.IEnumerator FlashRoutine()
    {
        if (spriteRenderer) spriteRenderer.color = hitColor;
        yield return new WaitForSeconds(0.1f);
        if (spriteRenderer) spriteRenderer.color = originalColor;
    }

    void Die()
    {
        Debug.Log("Enemy Died!");

        if (isFinalBoss)
        {
            // Trigger Game Clear
            if (GameUIManager.Instance != null)
            {
                GameUIManager.Instance.ShowGameClear();
            }
            else
            {
                Debug.LogError("GameUIManager Instance not found!");
            }
            
            // Optionally disable instead of destroy to keep scripts running if needed? 
            // Or just destroy. If destroyed, this script stops running, but UI manager is separate.
            // Let's hide the boss visually or destroy it.
            gameObject.SetActive(false); 
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
