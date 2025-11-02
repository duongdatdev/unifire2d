using UnityEngine;

public class Asteroid : MonoBehaviour
{
    // Movement speed
    public float speed = 2f;   
    // Fixed direction toward the player
    private Vector2 moveDirection;      
    // Explosion effect prefab
    [SerializeField]
    private GameObject explosionEffect;

    void Start()
    {
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
            if (explosionEffect != null)
            {
                // Khởi tạo hiệu ứng tại đúng vị trí, hướng và tỉ lệ của asteroid
                GameObject explosion = Instantiate(explosionEffect, transform.position, transform.rotation);
                explosion.transform.localScale = transform.localScale;
            }
            
            Destroy(other.gameObject);
            Destroy(gameObject);
            
            // Increase score via GameManager
            if (GameManager.Instance != null)
            {
                GameManager.Instance.AddScore(10);
            }
        }
    }

    void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}