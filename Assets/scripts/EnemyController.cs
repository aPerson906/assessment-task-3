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
        Vector2 position = rb != null ? rb.position : (Vector2)transform.position;
        timeLeft -= Time.fixedDeltaTime;
 
        if(vert)
        {
            SetAnim("MoveX", 0);
            SetAnim("MoveY", direction);
            position.y += speed * Time.fixedDeltaTime * direction;
        }
        else
        {
            SetAnim("MoveX", direction);
            SetAnim("MoveY", 0);
            position.x += speed * Time.fixedDeltaTime * direction;
        }
        if (rb != null)
        {
            rb.MovePosition(position);
        }
        else
        {
            transform.position = position;
        }
 
        // Example movement logic - replace with actual enemy behavior
        if(timeLeft <= 0)
        {
            direction = -direction;
            timeLeft = changeTime;
            FaceDirection();
        }
    }
 
    // Only sends values to the Animator if the enemy has one, so no Animator is needed
    void SetAnim(string parameter, float value)
    {
        if (animator != null)
        {
            animator.SetFloat(parameter, value);
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
 