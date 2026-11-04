using UnityEngine;

public class Bird : MonoBehaviour
{
    public GameObject eggPrefab;            // The egg that gets dropped (drag your egg PREFAB from the Project window)
    public float moveSpeed = 3f;            // How fast the bird flies
    public float flyDistance = 5f;          // How far it flies each way from where it starts
    public float minDropTime = 1f;          // Shortest wait between eggs (seconds)
    public float maxDropTime = 4f;          // Longest wait between eggs (seconds)
    public Vector2 spawnOffset = new Vector2(0f, -0.5f); // Where the egg appears, relative to the bird
    public bool spriteFacesLeft = false;    // Tick this if your bird art faces LEFT

    float startX;
    int direction = 1;                      // 1 = right, -1 = left
    float dropTimer;
    SpriteRenderer sprite;
    Collider2D myCollider;

    void Start()
    {
        startX = transform.position.x;
        sprite = GetComponentInChildren<SpriteRenderer>();
        myCollider = GetComponent<Collider2D>();
        dropTimer = Random.Range(minDropTime, maxDropTime);
        FaceDirection();
    }

    void Update()
    {
        // Fly back and forth between startX - flyDistance and startX + flyDistance
        transform.Translate(Vector2.right * direction * moveSpeed * Time.deltaTime);

        float offset = transform.position.x - startX;
        if (offset > flyDistance && direction == 1)
        {
            direction = -1;
            FaceDirection();
        }
        else if (offset < -flyDistance && direction == -1)
        {
            direction = 1;
            FaceDirection();
        }

        // Drop an egg at a random time
        dropTimer -= Time.deltaTime;
        if (dropTimer <= 0f)
        {
            DropEgg();
            dropTimer = Random.Range(minDropTime, maxDropTime);
        }
    }

    void FaceDirection()
    {
        if (sprite == null) return;

        bool movingLeft = direction < 0;
        sprite.flipX = spriteFacesLeft ? !movingLeft : movingLeft;
    }

    void DropEgg()
    {
        if (eggPrefab == null)
        {
            Debug.LogWarning("Bird: Egg Prefab is empty. Drag the egg PREFAB from the Project window into the slot.");
            return;
        }

        Vector3 spawnPos = transform.position + (Vector3)spawnOffset;
        GameObject egg = Instantiate(eggPrefab, spawnPos, Quaternion.identity);

        // Stop the egg from bumping into the bird that dropped it
        Collider2D eggCollider = egg.GetComponent<Collider2D>();
        if (eggCollider != null && myCollider != null)
        {
            Physics2D.IgnoreCollision(eggCollider, myCollider);
        }
    }
}
