using UnityEngine;
using UnityEngine.SceneManagement;

public class TouchToSplash : MonoBehaviour
{
    public string sceneName = "SplashScreen"; // Scene to go to (must be in Build Settings)
    public float delay = 0f;                  // Seconds to wait after the touch before switching (0 = right away)

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
        Invoke(nameof(GoToSplash), delay);
    }

    void GoToSplash()
    {
        SceneManager.LoadScene(sceneName);
    }
}
