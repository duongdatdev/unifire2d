using UnityEngine;

public class AsteroidSpawner : MonoBehaviour
{
    [Header("Asteroid Settings")]
    public GameObject asteroidPrefab;
    public float spawnRate = 1.5f;
    public int maxAsteroids = 10;

    [Header("Spawn Offset Settings")]
    [Tooltip("Offset ratio from camera boundary (0.1 = very close, 1.0 = far)")]
    [Range(0.1f, 1.5f)]
    public float offsetRatio = 0.3f;   // Ratio of how far outside screen to spawn

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
            case 0: // Top
                spawnPos = new Vector2(Random.Range(-camWidth, camWidth), camHeight + offset);
                break;
            case 1: // Bottom
                spawnPos = new Vector2(Random.Range(-camWidth, camWidth), -camHeight - offset);
                break;
            case 2: // Left
                spawnPos = new Vector2(-camWidth - offset, Random.Range(-camHeight, camHeight));
                break;
            case 3: // Right
                spawnPos = new Vector2(camWidth + offset, Random.Range(-camHeight, camHeight));
                break;
        }

        Instantiate(asteroidPrefab, spawnPos, Quaternion.identity);
    }
}
