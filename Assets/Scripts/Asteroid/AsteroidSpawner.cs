using UnityEngine;

public class AsteroidSpawner : MonoBehaviour
{
    [Header("Asteroid Settings")] public GameObject asteroidPrefab;
    public float spawnRate = 1.5f;
    public int maxAsteroids = 10;

    [Header("Random size & stats")] [Tooltip("Min and Max scale for spawned asteroids")] [SerializeField]
    private Vector2 scaleRange = new Vector2(1f, 3f);

    [Tooltip("Speed multiplier based on size (smaller = faster)")] [SerializeField]
    private float sizeSpeedFactor = 0.8f;

    [Tooltip("Health multiplier (larger = more health)")] [SerializeField]
    private float sizeHealthFactor = 1.3f;

    [Header("Spawn Offset Settings")]
    [Tooltip("Offset ratio from camera boundary (0.1 = very close, 1.0 = far)")]
    [Range(0.1f, 1.5f)]
    // Ratio of how far outside screen to spawn
    public float offsetRatio = 0.3f;

    private float nextSpawnTime;
    private Camera mainCam;

    void Start()
    {
        mainCam = Camera.main;
    }

    void Update()
    {
        if (Time.time >= nextSpawnTime)
        {
            SpawnAsteroid();
            nextSpawnTime = Time.time + spawnRate;
        }
    }

    void SpawnAsteroid()
    {
        // Skip if too many asteroids
        if (GameObject.FindGameObjectsWithTag("Asteroid").Length >= maxAsteroids)
            return;

        // Get camera boundaries in world units
        float camHeight = mainCam.orthographicSize;
        float camWidth = camHeight * mainCam.aspect;

        // Dynamic spawn offset: scales with camera size
        float offset = Mathf.Lerp(0.3f, 1.5f, offsetRatio) * (camHeight / 5f);

        // Random side
        int side = Random.Range(0, 4);
        Vector2 spawnPos = Vector2.zero;

        switch (side)
        {
            // Top
            case 0:
                spawnPos = new Vector2(Random.Range(-camWidth, camWidth), camHeight + offset);
                break;
            // Bottom
            case 1:
                spawnPos = new Vector2(Random.Range(-camWidth, camWidth), -camHeight - offset);
                break;
            // Left
            case 2:
                spawnPos = new Vector2(-camWidth - offset, Random.Range(-camHeight, camHeight));
                break;
            // Right
            case 3:
                spawnPos = new Vector2(camWidth + offset, Random.Range(-camHeight, camHeight));
                break;
        }

        // Instantiate asteroid
        GameObject asteroid = Instantiate(asteroidPrefab, spawnPos, Quaternion.identity);

        // Randomize scale
        float randomScale = Random.Range(scaleRange.x, scaleRange.y);
        asteroid.transform.localScale = Vector3.one * randomScale;

        // Adjust mass by size (for physics balance)
        Rigidbody2D rb = asteroid.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.mass = randomScale; // Bigger = heavier
        }

        // Adjust health and speed according to size
        Asteroid asteroidScript = asteroid.GetComponent<Asteroid>();
        if (asteroidScript != null)
        {
            // Smaller asteroids move faster
            float sizeFactor = Mathf.Lerp(1.2f, 0.6f, (randomScale - scaleRange.x) / (scaleRange.y - scaleRange.x));
            asteroidScript.GetType()
                .GetField("baseSpeed", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(asteroidScript, (float)(asteroidScript.GetType()
                    .GetField("baseSpeed", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                    ?.GetValue(asteroidScript) ?? 2f) * sizeFactor * sizeSpeedFactor);

            // Larger asteroids have more health
            asteroidScript.GetType()
                .GetField("maxHealth", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public)
                ?.SetValue(asteroidScript, Mathf.RoundToInt(
                    (int)(asteroidScript.GetType()
                        .GetField("maxHealth", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public)
                        ?.GetValue(asteroidScript) ?? 3) * randomScale * sizeHealthFactor));
        }
    }
}