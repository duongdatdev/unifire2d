using UnityEngine;

public class StarfieldController : MonoBehaviour
{
    private ParticleSystem ps;
    private ParticleSystem.VelocityOverLifetimeModule velocityModule;

    public Transform player;
    public float speed = 1f; // speed of starfield movement

    void Start()
    {
        ps = GetComponent<ParticleSystem>();
        velocityModule = ps.velocityOverLifetime;
    }

    void Update()
    {
        if (player == null) return;

        // move starfield in opposite direction of player movement
        Vector2 moveDir = player.up; 
        velocityModule.x = -moveDir.x * speed;
        velocityModule.z = -moveDir.y * speed;
    }
}