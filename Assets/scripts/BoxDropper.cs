using UnityEngine;

public class BoxDropper : MonoBehaviour
{
    public GameObject boxPrefab;            // The box that gets dropped (drag your box prefab here)
    public float dropInterval = 2f;         // Seconds between drops
    public Vector2 spawnOffset = new Vector2(0f, -1f); // Where the box appears, relative to the enemy

    Collider2D myCollider;
    float timer;

    void Start()
    {
        myCollider = GetComponent<Collider2D>();
        timer = dropInterval;
    }

    void Update()
    {
        // Count down, then drop a box and start over
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            DropBox();
            timer = dropInterval;
        }
    }

    void DropBox()
    {
        if (boxPrefab == null) return;

        Vector3 spawnPos = transform.position + (Vector3)spawnOffset;
        GameObject box = Instantiate(boxPrefab, spawnPos, Quaternion.identity);

        // Stop the box from bumping into the enemy that dropped it
        Collider2D boxCollider = box.GetComponent<Collider2D>();
        if (boxCollider != null && myCollider != null)
        {
            Physics2D.IgnoreCollision(boxCollider, myCollider);
        }
    }
}
