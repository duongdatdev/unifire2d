using UnityEngine;

public class AutoDestroy : MonoBehaviour
{
    void Start()
    {
        // destroy after animation ends
        Destroy(gameObject, 0.7f); 
    }
}