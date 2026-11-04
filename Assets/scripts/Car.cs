using UnityEngine;

public class Car : MonoBehaviour
{
    public int damage = 3;                  // Health lost when the car hits the player
    public float knockbackX = 10f;          // How hard the player is thrown sideways
    public float knockbackY = 6f;           // How hard the player is popped upward

    public float moveSpeed = 5f;            // How fast the car drives (0 = parked)
    public float driveDistance = 8f;        // How far it drives each way from where it starts
    public bool spriteFacesLeft = false;    // Tick this if your car art faces LEFT

    float startX;
    int direction = 1;                      // 1 = right, -1 = left
    SpriteRenderer sprite;

    void Start()
    {
        startX = transform.position.x;
        sprite = GetComponentInChildren<SpriteRenderer>();
        FaceDirection();
    }

    void Update()
    {
        if (moveSpeed <= 0f) return;

        // Drive back and forth between startX - driveDistance and startX + driveDistance
        transform.Translate(Vector2.right * direction * moveSpeed * Time.deltaTime);

        float offset = transform.position.x - startX;
        if (offset > driveDistance && direction == 1)
        {
            direction = -1;
            FaceDirection();
        }
        else if (offset < -driveDistance && direction == -1)
        {
            direction = 1;
            FaceDirection();
        }
    }

    void FaceDirection()
    {
        if (sprite == null) return;

        bool movingLeft = direction < 0;
        sprite.flipX = spriteFacesLeft ? !movingLeft : movingLeft;
    }

    // Works when the car's collider is solid
    void OnCollisionEnter2D(Collision2D collision) { Hit(collision.gameObject); }
    void OnCollisionStay2D(Collision2D collision) { Hit(collision.gameObject); }

    // Works when the car's collider is set to Is Trigger
    void OnTriggerEnter2D(Collider2D other) { Hit(other.gameObject); }
    void OnTriggerStay2D(Collider2D other) { Hit(other.gameObject); }

    void Hit(GameObject other)
    {
        PlayerController controller = other.GetComponent<PlayerController>();
        if (controller == null) return;

        // PlayerController ignores damage while the player is invincible, so only throw the player if health actually dropped
        int healthBefore = controller.health;
        controller.ChangeHealth(-damage);
        if (controller.health >= healthBefore) return;

        Rigidbody2D body = other.GetComponent<Rigidbody2D>();
        if (body != null)
        {
            float away = Mathf.Sign(other.transform.position.x - transform.position.x);
            if (away == 0f) away = 1f;
            body.linearVelocity = new Vector2(away * knockbackX, knockbackY);
        }
    }
}
