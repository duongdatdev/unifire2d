using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")] public float moveSpeed = 5f;
    public float stopDistance = 0.3f;

    [Header("Shooting Settings")] public GameObject bulletPrefab;
    public Transform firePoint;
    public float fireRate = 0.2f;

    [Header("Particle Effects")]
    public ParticleSystem engineSmoke;
    
    private Animator _animator;

    private Camera mainCamera;
    private float nextFireTime = 0f;
    private float currentSpeed = 0f;

    void Start()
    {
        mainCamera = Camera.main;
        _animator = GetComponentInChildren<Animator>();

        if (Mouse.current == null)
        {
            Debug.LogError("No mouse connected. Please connect a mouse to use this controller.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        MoveTowardsMouse();
        AutoFire();
        ClampToScreen();

        UpdateAnimation();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Asteroid"))
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.LoseLife();
            }
        }
    }

    void MoveTowardsMouse()
    {
        // Get mouse position in world space
        Vector3 mousePos = Mouse.current.position.ReadValue();
        mousePos = mainCamera.ScreenToWorldPoint(mousePos);
        mousePos.z = 0f;

        // Calculate direction towards mouse
        Vector2 direction = (mousePos - transform.position).normalized;

        // Rotate player to face mouse
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);

        // Move player towards mouse position
        float distance = Vector2.Distance(transform.position, mousePos);

        if (distance > stopDistance)
        {
            transform.position += (Vector3)(direction * moveSpeed * Time.deltaTime);
            currentSpeed = moveSpeed;
        }
        else
        {
            currentSpeed = 0f;
        }
        
        if (distance > stopDistance)
        {
            transform.position += (Vector3)(direction * moveSpeed * Time.deltaTime);
            currentSpeed = moveSpeed;

            // Play smoke if moving
            if (engineSmoke && !engineSmoke.isPlaying)
                engineSmoke.Play();
        }
        else
        {
            currentSpeed = 0f;

            // Stop smoke when idle
            if (engineSmoke && engineSmoke.isPlaying)
                engineSmoke.Stop();
        }
    }

    void Fire()
    {
        if (bulletPrefab && firePoint)
        {
            Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

            if (AudioManager.instance != null)
            {
                AudioManager.instance.PlayShootSound();
            }
        }
    }

    void AutoFire()
    {
        if (Time.time >= nextFireTime)
        {
            Fire();
            nextFireTime = Time.time + fireRate;
        }
    }

    void UpdateAnimation()
    {
        if (_animator != null)
        {
            _animator.SetFloat("Speed", currentSpeed);
        }
    }

    void ClampToScreen()
    {
        // Get player's position in viewport coordinates
        Vector3 pos = transform.position;
        Vector3 viewPos = mainCamera.WorldToViewportPoint(pos);

        // Clamp to viewport bounds
        viewPos.x = Mathf.Clamp01(viewPos.x);
        viewPos.y = Mathf.Clamp01(viewPos.y);

        // Convert back to world coordinates
        transform.position = mainCamera.ViewportToWorldPoint(viewPos);
    }
}