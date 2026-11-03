using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float speed = 3.0f;
    Rigidbody2D rb;
    public bool vert;
    public float changeTime = 3.0f;
    float timeLeft;
    int direction = 1;
    public int damage = 1;
    public bool spriteFacesLeft = false; // Tick this if your enemy art faces LEFT
    Animator animator;
    SpriteRenderer spriteRenderer;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        timeLeft = changeTime;
        FaceDirection();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector2 position = rb.position;
        timeLeft -= Time.fixedDeltaTime;

        if(vert)
        {
            animator.SetFloat("MoveX", 0);
            animator.SetFloat("MoveY", direction);
            position.y += speed * Time.fixedDeltaTime * direction;
        }
        else
        {
            animator.SetFloat("MoveX", direction);
            animator.SetFloat("MoveY", 0);
            position.x += speed * Time.fixedDeltaTime * direction;
        }
        rb.MovePosition(position);

        // Example movement logic - replace with actual enemy behavior
        if(timeLeft <= 0)
        {
            direction = -direction;
            timeLeft = changeTime;
            FaceDirection();
        }
    }

    // Flips the sprite so the enemy faces the way it's walking (left/right enemies only)
    void FaceDirection()
    {
        if (vert || spriteRenderer == null) return;

        bool movingLeft = direction < 0;
        spriteRenderer.flipX = spriteFacesLeft ? !movingLeft : movingLeft;
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        PlayerController player = other.gameObject.GetComponent<PlayerController>();
        if (player != null)
        {
            player.ChangeHealth(-damage);
        }
    }
}
