using UnityEngine;

public class Bobbing : MonoBehaviour
{
    public float bobHeight = 0.25f; // How far up and down it moves
    public float bobSpeed = 2f;     // How fast it bobs

    Vector3 startPosition;
    float timeOffset;

    void Start()
    {
        startPosition = transform.position;
        timeOffset = Random.Range(0f, Mathf.PI * 2f); // so multiple collectibles don't all bob in sync
    }

    void Update()
    {
        float newY = startPosition.y + Mathf.Sin(Time.time * bobSpeed + timeOffset) * bobHeight;
        transform.position = new Vector3(startPosition.x, newY, startPosition.z);
    }
}
