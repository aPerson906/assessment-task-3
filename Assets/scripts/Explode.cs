using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
 
public class Explode : MonoBehaviour
{
    public float maxFallHeight = 8f; // Falling farther than this (in Unity units) makes the player explode
 
    private float highestY; // Highest point reached since the player last touched the ground
    private Rigidbody2D body;
    private bool exploded;
 
    // Attaches this script to the Player automatically in every scene, so you don't have to add it by hand
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void AutoAttach()
    {
        AttachToPlayer();
        SceneManager.sceneLoaded += (scene, mode) => AttachToPlayer();
    }
 
    static void AttachToPlayer()
    {
        Player player = Object.FindFirstObjectByType<Player>();
        if (player != null && player.GetComponent<Explode>() == null)
        {
            player.gameObject.AddComponent<Explode>();
        }
    }
 
    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        highestY = transform.position.y;
    }
 
    void Update()
    {
        // Climbing a ladder turns gravity off, so that isn't a fall: keep resetting the starting height
        if (body != null && body.gravityScale == 0f)
        {
            highestY = transform.position.y;
            return;
        }
 
        // Remember the highest point of the current fall/jump
        if (transform.position.y > highestY)
        {
            highestY = transform.position.y;
        }
    }
 
    // This method is called when another Collider2D enters the trigger zone of this GameObject.
    void OnTriggerEnter2D(Collider2D target)
    {
        // Check if the entering GameObject has the "Deadly" tag
        if (target.gameObject.CompareTag("Deadly")){
            Die();
        }
    }
 
    // This method is called when a collision with another Collider2D occurs.
    void OnCollisionEnter2D(Collision2D target)
    {
        // Check if the colliding GameObject has the "Deadly" tag
        if (target.gameObject.CompareTag("Deadly")){
            Die();
            return;
        }
 
        CheckLanding(target);
    }
 
    // Keeps checking while standing on something, so walking down slopes never builds up a fall
    void OnCollisionStay2D(Collision2D target)
    {
        CheckLanding(target);
    }
 
    // If we touched a surface facing upward (ground), work out how far we fell
    void CheckLanding(Collision2D target)
    {
        foreach (ContactPoint2D contact in target.contacts)
        {
            if (contact.normal.y > 0.5f)
            {
                float fallDistance = highestY - transform.position.y;
 
                if (fallDistance > maxFallHeight)
                {
                    Die();
                }
 
                // Start measuring again from where we landed
                highestY = transform.position.y;
                return;
            }
        }
    }
 
    // The explosion: sends the player back to the Splash Screen
    void Die()
    {
        if (exploded) return;
        exploded = true;
        SceneManager.LoadScene("SplashScreen");
    }
}
 