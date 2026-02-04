using UnityEngine;

public class SwordProjectile : MonoBehaviour
{
    [Header("Settings")]
    public float speed = 10f;
    public float damage = 10f;
    public float lifeTime = 3f;

    [Header("Visuals")]
    public GameObject impactEffect; // Optional

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null) rb = gameObject.AddComponent<Rigidbody2D>();
        
        rb.gravityScale = 0f; // Fly straight
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        
        // Fly forward based on rotation
        rb.linearVelocity = transform.right * speed;

        Destroy(gameObject, lifeTime);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // Hit Player?
        if (collision.CompareTag("Player") || collision.gameObject.name == "Zenitsu")
        {
            // Apply Damage
            var health = collision.GetComponent<PlayerHealth>();
            if (health != null)
            {
                health.TakeDamage(damage);
            }
            
            // Effect
            if (impactEffect) Instantiate(impactEffect, transform.position, Quaternion.identity);

            Destroy(gameObject);
        }
        // Hit Ground?
        // Hit Ground?
        // else if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        // {
        //     // Removed wall collision as per request
        // }
    }
}
