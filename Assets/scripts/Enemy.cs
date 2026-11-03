using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int damage = 1;              // How much health the player loses per hit
    public float knockbackX = 10f;      // How hard the player is pushed away sideways
    public float knockbackY = 6f;       // How hard the player is popped upward

    // Works when the enemy has a normal (solid) collider
    void OnCollisionEnter2D(Collision2D collision) { Hit(collision.gameObject); }
    void OnCollisionStay2D(Collision2D collision) { Hit(collision.gameObject); }

    // Works when the enemy's collider is set to Is Trigger
    void OnTriggerEnter2D(Collider2D other) { Hit(other.gameObject); }
    void OnTriggerStay2D(Collider2D other) { Hit(other.gameObject); }

    void Hit(GameObject other)
    {
        PlayerController controller = other.GetComponent<PlayerController>();
        if (controller == null) return;

        // PlayerController ignores damage while the player is invincible, so only bounce if health actually dropped
        int healthBefore = controller.health;
        controller.ChangeHealth(-damage);
        if (controller.health >= healthBefore) return;

        Rigidbody2D body = other.GetComponent<Rigidbody2D>();
        if (body != null)
        {
            // Push the player away from the enemy and up a little
            float direction = Mathf.Sign(other.transform.position.x - transform.position.x);
            if (direction == 0f) direction = 1f;
            body.linearVelocity = new Vector2(direction * knockbackX, knockbackY);
        }
    }
}
