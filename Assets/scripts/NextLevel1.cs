using UnityEngine;
using UnityEngine.SceneManagement;

public class NextLevel : MonoBehaviour
{
    public float delay = 1f; // Seconds to wait after the player touches this before the next level loads

    bool triggered;

    // Works when this object's collider is set to Is Trigger
    void OnTriggerEnter2D(Collider2D other)
    {
        Touched(other.gameObject);
    }

    // Works when this object's collider is solid
    void OnCollisionEnter2D(Collision2D collision)
    {
        Touched(collision.gameObject);
    }

    void Touched(GameObject other)
    {
        if (triggered) return;
        if (other.GetComponent<Player>() == null) return;

        triggered = true;
        Invoke(nameof(LoadNext), delay);
    }

    void LoadNext()
    {
        // Next scene in the Build Settings list; after the last level, go back to the first scene (Splash Screen)
        int next = SceneManager.GetActiveScene().buildIndex + 1;
        if (next >= SceneManager.sceneCountInBuildSettings)
        {
            next = 0;
        }
        SceneManager.LoadScene(next);
    }
}
