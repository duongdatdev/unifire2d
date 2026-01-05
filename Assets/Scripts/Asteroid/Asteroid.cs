using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Asteroid : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float baseSpeed = 2f;
    [SerializeField] private float randomSpeedRange = 1f;

    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 3;
    private int _currentHealth;

    [Header("Visual & Effects")]
    [SerializeField] private GameObject explosionEffect;
    [SerializeField] private Canvas healthCanvas;
    [SerializeField] private Image healthBarFill;

    private Rigidbody2D _rb;
    private Vector2 _moveDirection;
    private bool _isDead;
    
    private Coroutine hideHealthCoroutine;


    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        if (healthCanvas != null)
            healthCanvas.enabled = false;

        _currentHealth = maxHealth;
        InitializeMovement();
    }

    private void FixedUpdate()
    {
        Move();
        // UpdateHealthBarRotation();
    }

    // Initialize asteroid direction and randomized speed
    private void InitializeMovement()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            Vector2 target = player.transform.position;
            _moveDirection = (target - (Vector2)transform.position).normalized;
            _moveDirection = Quaternion.Euler(0, 0, Random.Range(-10f, 10f)) * _moveDirection;
        }
        else
        {
            _moveDirection = (Vector2.zero - (Vector2)transform.position).normalized;
        }

        float finalSpeed = baseSpeed + Random.Range(-randomSpeedRange, randomSpeedRange);
        _rb.linearVelocity = _moveDirection * finalSpeed;
    }

    // Move asteroid using Rigidbody velocity
    private void Move()
    {
        if (_rb == null || _isDead) return;

        // Optional spin for realism
        _rb.MoveRotation(_rb.rotation + 30f * Time.fixedDeltaTime);
    }

    // Keep health bar facing up (not rotating with asteroid)
    private void UpdateHealthBarRotation()
    {
        if (healthCanvas != null)
        {
            healthCanvas.transform.rotation = Quaternion.identity;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_isDead) return;

        if (other.CompareTag("Bullet"))
        {
            Destroy(other.gameObject);
            TakeDamage(1);
        }
        else if (other.CompareTag("Player"))
        {
            Explode();
            Destroy(gameObject);
        }
    }

    // Apply damage and update health bar
    private void TakeDamage(int damage)
    {
        if (healthCanvas != null)
        {
            healthCanvas.enabled = true;

            // Reset hide health coroutine if already running
            if (hideHealthCoroutine != null)
                StopCoroutine(hideHealthCoroutine);

            // Hide health bar after delay
            hideHealthCoroutine = StartCoroutine(HideHealthAfterDelay(2f));
        }
        
        _currentHealth -= damage;

        if (healthBarFill != null)
            healthBarFill.fillAmount = (float)_currentHealth / maxHealth;

        if (_currentHealth <= 0)
        {
            _isDead = true;
            Explode();

            if (GameManager.Instance != null)
                GameManager.Instance.AddScore(10);

            Destroy(gameObject);
        }
    }
    
    private IEnumerator HideHealthAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (healthCanvas != null && !_isDead)
            healthCanvas.enabled = false;
    }


    // Spawn explosion effect and play sound
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

    private void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}
