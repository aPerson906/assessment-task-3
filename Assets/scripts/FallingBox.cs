using UnityEngine;

public class FallingBox : MonoBehaviour
{
    public int damage = 1;                  // Health the player loses if the box hits them
    public float disappearAfterLanding = 2f; // Seconds the box stays around after it lands
    public float maxLifetime = 10f;         // Safety: the box is removed after this long no matter what

    bool landed;

    void Start()
    {
        Destroy(gameObject, maxLifetime);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        PlayerController player = collision.gameObject.GetComponent<PlayerController>();

        // Only hurts while still falling (before it has landed on anything)
        if (player != null && !landed)
        {
            player.ChangeHealth(-damage);
            Destroy(gameObject);
            return;
        }

        // Hit the ground or something else: stop being dangerous and clean up soon
        if (!landed)
        {
            landed = true;
            Destroy(gameObject, disappearAfterLanding);
        }
    }
}
