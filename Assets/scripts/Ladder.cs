
using UnityEngine;
 
public class Ladder : MonoBehaviour
{
    public float climbSpeed = 4f;    // How fast the player climbs up and down
    public float jumpOffSpeed = 6f;  // Small hop when you press Space to leave the ladder
    public bool showDebug = true;    // Prints messages in the Console explaining why a ladder isn't working (turn off when it's fixed)
 
    // Shared by every Ladder so only one ladder can hold the player at a time
    static Ladder activeLadder;
    static float regrabUntil;
 
    Collider2D ladderCol;
    Collider2D playerCol;
    Rigidbody2D playerBody;
    bool playerInside;
    float climbInput;
    float nextLogTime;
    float originalGravity;
    RigidbodyConstraints2D originalConstraints;
 
    bool isClimbing { get { return activeLadder == this; } }
 
    // Clears the shared state every time Play starts
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void ResetStatics()
    {
        activeLadder = null;
        regrabUntil = 0f;
    }
 
    void Start()
    {
        // Use this object's collider, or one on a child if this object doesn't have its own
        ladderCol = GetComponent<Collider2D>();
        if (ladderCol == null)
        {
            ladderCol = GetComponentInChildren<Collider2D>();
        }
        if (ladderCol == null)
        {
            Debug.LogWarning("Ladder '" + name + "' has no Collider2D, so it can't be climbed. Add a Box Collider 2D with Is Trigger ticked.", this);
        }
    }
 
    void OnDisable()
    {
        // Safety: always give the player their gravity back if this ladder goes away
        StopClimbing();
    }
 
    void DebugLog(string message)
    {
        if (!showDebug || Time.time < nextLogTime) return;
        nextLogTime = Time.time + 1f;
        Debug.Log("[Ladder " + name + "] " + message, this);
    }
 
    // Checks if the player is overlapping this ladder directly, so it never depends on trigger events
    bool IsPlayerOverlapping()
    {
        if (ladderCol == null || playerCol == null) return false;
 
        // Compare only X and Y (ignores depth), so a ladder at a different Z position still works
        Bounds a = ladderCol.bounds;
        Bounds b = playerCol.bounds;
        return a.min.x < b.max.x && a.max.x > b.min.x
            && a.min.y < b.max.y && a.max.y > b.min.y;
    }
 
    void Update()
    {
        if (ladderCol == null) return;
 
        // Find the player (again if the scene was reloaded)
        if (playerCol == null || playerBody == null)
        {
            Player p = Object.FindAnyObjectByType<Player>();
            if (p == null)
            {
                DebugLog("PROBLEM: no object with the Player script exists in this scene.");
                return;
            }
            playerCol = p.GetComponent<Collider2D>();
            playerBody = p.GetComponent<Rigidbody2D>();
            if (playerCol == null || playerBody == null)
            {
                DebugLog("PROBLEM: the Player object is missing a Collider2D or Rigidbody2D.");
                return;
            }
        }
 
        // Safety: if another ladder is holding the player but the player isn't on it anymore, let go
        if (activeLadder != null && activeLadder != this && !activeLadder.IsPlayerOverlapping())
        {
            activeLadder.StopClimbing();
        }
 
        playerInside = IsPlayerOverlapping();
        if (!playerInside)
        {
            if (showDebug && Vector2.Distance(transform.position, playerCol.bounds.center) < 4f)
            {
                DebugLog("Player is near but NOT touching this ladder's collider. Ladder collider covers x "
                    + ladderCol.bounds.min.x.ToString("F1") + " to " + ladderCol.bounds.max.x.ToString("F1")
                    + ", y " + ladderCol.bounds.min.y.ToString("F1") + " to " + ladderCol.bounds.max.y.ToString("F1")
                    + ". Player covers x " + playerCol.bounds.min.x.ToString("F1") + " to " + playerCol.bounds.max.x.ToString("F1")
                    + ", y " + playerCol.bounds.min.y.ToString("F1") + " to " + playerCol.bounds.max.y.ToString("F1") + ".");
            }
            StopClimbing();
            return;
        }
 
        // Old Input Manager: W/Up climbs up, S/Down climbs down
        climbInput = 0f;
        if (Input.GetKey("w") || Input.GetKey("up")) climbInput += 1f;
        if (Input.GetKey("s") || Input.GetKey("down")) climbInput -= 1f;
 
        // Grab the ladder when pressing up or down (only if no other ladder has the player)
        if (activeLadder == null && climbInput != 0f && Time.time >= regrabUntil)
        {
            StartClimbing();
        }
 
        if (climbInput != 0f && !isClimbing)
        {
            if (activeLadder != null) DebugLog("Not grabbing: another ladder (" + activeLadder.name + ") is holding the player.");
            else if (Time.time < regrabUntil) DebugLog("Not grabbing: short cooldown after hopping off.");
        }
 
        // Space to hop off the ladder
        if (isClimbing && Input.GetKeyDown("space"))
        {
            Rigidbody2D body = playerBody;
            StopClimbing();
            body.linearVelocity = new Vector2(body.linearVelocity.x, jumpOffSpeed);
            regrabUntil = Time.time + 0.3f; // stops it from instantly grabbing a ladder again
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
 