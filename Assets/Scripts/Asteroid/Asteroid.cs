using UnityEngine;
using UnityEngine.UI;

public class Asteroid : MonoBehaviour
{
    // Movement speed
    public float speed = 2f;   
    // Fixed direction toward the player
    private Vector2 moveDirection;      
    // Explosion effect prefab
    [SerializeField]
    private GameObject explosionEffect;
    
    [Header("Health Settings")]
    public int maxHealth = 3;
    private int _currentHealth;
    
    [Header("Health Bar")]
    public Image healthBarFill;
    public Canvas healthCanvas;

    void Start()
    {
        _currentHealth = maxHealth;
        // Find player (make sure player has the tag "Player")
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            // Get direction from asteroid to player
            Vector2 target = player.transform.position;
            moveDirection = (target - (Vector2)transform.position).normalized;

            // Slight random rotation for natural motion
            moveDirection = Quaternion.Euler(0, 0, Random.Range(-10f, 10f)) * moveDirection;
        }
        else
        {
            // Default direction to center if player not found
            moveDirection = (Vector2.zero - (Vector2)transform.position).normalized;
        }

        // Randomize speed slightly
        speed = Random.Range(1.5f, 3.5f);
    }

    void Update()
    {
        // Move asteroid along the fixed direction
        transform.Translate(moveDirection * speed * Time.deltaTime, Space.World);

        // Optional: rotate asteroid for visual effect
        transform.Rotate(0, 0, 30 * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Bullet"))
        {
            Destroy(other.gameObject);
            TakeDamage(1);
        }
        else if (other.CompareTag("Player"))
        {
            if (explosionEffect != null)
            {
                // Instantiate explosion effect at asteroid position
                GameObject explosion = Instantiate(explosionEffect, transform.position, transform.rotation);
                explosion.transform.localScale = transform.localScale;
            }
            Destroy(gameObject);
        }
    }
    
    // Reduce asteroid health
    private void TakeDamage(int damage)
    {
        _currentHealth -= damage;

        // Update health bar fill
        if (healthBarFill != null)
            healthBarFill.fillAmount = (float)_currentHealth / maxHealth;

        // Destroy asteroid if health is 0
        if (_currentHealth <= 0)
        {
            Explode();
            if (GameManager.Instance != null)
                GameManager.Instance.AddScore(10);
            Destroy(gameObject);
        }
    }
    
    private void Explode()
    {
        if (explosionEffect != null)
        {
            GameObject explosion = Instantiate(explosionEffect, transform.position, transform.rotation);
            explosion.transform.localScale = transform.localScale;
        }

        if (AudioManager.instance != null)
            AudioManager.instance.PlayExplosionSound();
    }

    void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}