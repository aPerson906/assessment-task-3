
using UnityEngine;
 
public class Ladder : MonoBehaviour
{
    public float climbSpeed = 4f;    // How fast the player climbs up and down
    public float jumpOffSpeed = 6f;  // Small hop when you press Space to leave the ladder
 
    // Shared by every Ladder so only one ladder can hold the player at a time
    static Ladder activeLadder;
    static float regrabUntil;
 
    Rigidbody2D playerBody;
    bool playerInside;
    float climbInput;
    float originalGravity;
    RigidbodyConstraints2D originalConstraints;
 
    bool isClimbing { get { return activeLadder == this; } }
 
    void OnDisable()
    {
        // Safety: always give the player their gravity back if this ladder goes away
        StopClimbing();
    }
 
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<Player>() != null)
        {
            playerBody = other.attachedRigidbody;
            playerInside = true;
        }
    }
 
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.GetComponent<Player>() != null)
        {
            playerInside = false;
            StopClimbing();
        }
    }
 
    void Update()
    {
        if (!playerInside) return;
 
        // Old Input Manager: W/Up climbs up, S/Down climbs down
        climbInput = 0f;
        if (Input.GetKey("w") || Input.GetKey("up")) climbInput += 1f;
        if (Input.GetKey("s") || Input.GetKey("down")) climbInput -= 1f;
 
        // Grab the ladder when pressing up or down (only if no other ladder has the player)
        if (activeLadder == null && climbInput != 0f && Time.time >= regrabUntil)
        {
            StartClimbing();
        }
 
        // Space to hop off the ladder
        if (isClimbing && Input.GetKeyDown("space"))
        {
            Rigidbody2D body = playerBody;
            StopClimbing();
            body.linearVelocity = new Vector2(body.linearVelocity.x, jumpOffSpeed);
            regrabUntil = Time.time + 0.3f; // stops it from instantly grabbing the ladder again
        }
    }
 
    void FixedUpdate()
    {
        if (isClimbing)
        {
            playerBody.linearVelocity = new Vector2(0f, climbInput * climbSpeed);
        }
    }
 
    void StartClimbing()
    {
        activeLadder = this;
        originalGravity = playerBody.gravityScale;
        originalConstraints = playerBody.constraints;
 
        playerBody.gravityScale = 0f;
        playerBody.linearVelocity = Vector2.zero;
        playerBody.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
 
        // Snap the player to the middle of the ladder
        playerBody.position = new Vector2(transform.position.x, playerBody.position.y);
    }
 
    void StopClimbing()
    {
        if (!isClimbing) return;
        activeLadder = null;
        if (playerBody != null)
        {
            playerBody.gravityScale = originalGravity;
            playerBody.constraints = originalConstraints;
        }
    }
}