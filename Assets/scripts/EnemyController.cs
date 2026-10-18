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
    Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        timeLeft = changeTime;

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
        }
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
