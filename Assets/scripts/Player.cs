using UnityEngine;
using UnityEngine.SceneManagement; //Required if we want to work with Scenes

public class Player : MonoBehaviour
{
    // Animation state values. These must match the conditions on your AnyState transitions.
    private const int STATE_IDLE = 0;
    private const int STATE_RUN = 1;
    private const int STATE_JUMP = 2;
    private const int STATE_FALL = 3;

    // Must match the Animator parameter name EXACTLY (case-sensitive).
    private const string ANIM_PARAM = "aniState";

    public float speed = 150f; //Stores player speed as a float
    public Vector2 maxVelocity = new Vector2(60, 100); //Stores player velocity (60x, 100y)
    public float jumpVelocity = 12f; //Upward speed of a jump. Raise for a higher jump, lower for a lower one
    public bool standing; //True while the player is touching ground (watch this tick in Play mode)
    public float airSpeedMultiplier = .3f; //Multiplies the air speed of the player
    public float groundCheckDistance = 0.1f; //How far below the feet to look for ground
    public bool spriteFacesLeft = false; //Tick this if your sprite art faces LEFT (fixes "walking backwards")
    public CapsuleCollider2D alive; //Creates a variable to check if the player is alive

    private Rigidbody2D body2D; //Creates a variable for the RigidBody2D component
    private SpriteRenderer renderer2D; //Creates a variable to render the sprite
    private PlayerController controller; //Creates a variable for the controller (created in a separate script)
    private Animator animator; //Creates a variable to work with the Player animations
    private AudioSource audioData;
    private Collider2D col;

    private bool isJumping = false;
    private float jumpLockTimer = 0f; //Short lock after a jump so it can't count as "landed" immediately

    private ContactFilter2D groundFilter;
    private readonly RaycastHit2D[] hitBuffer = new RaycastHit2D[8];

    void Start()
    {
        body2D = GetComponent<Rigidbody2D>();
        renderer2D = GetComponent<SpriteRenderer>();
        controller = GetComponent<PlayerController>();
        animator = GetComponent<Animator>();
        audioData = GetComponent<AudioSource>();
        col = GetComponent<Collider2D>();

        // Ground check ignores trigger colliders and respects the physics layer matrix
        groundFilter = new ContactFilter2D();
        groundFilter.useTriggers = false;
        groundFilter.SetLayerMask(Physics2D.GetLayerCollisionMask(gameObject.layer));
        groundFilter.useLayerMask = true;
    }

    // Looks just below the player's collider for a surface that faces upward.
    // Works even when the Rigidbody is asleep, and needs no tags.
    private bool CheckGrounded()
    {
        int count = col.Cast(Vector2.down, groundFilter, hitBuffer, groundCheckDistance);
        for (int i = 0; i < count; i++)
        {
            if (hitBuffer[i].normal.y > 0.5f)
            {
                return true;
            }
        }
        return false;
    }

    void Update()
    {
        var absVelX = Mathf.Abs(body2D.linearVelocity.x); //Horizontal movement velocity (x)
        var velY = body2D.linearVelocity.y;               //Vertical velocity (signed)

        // Fell off the level: back to the Splash Screen
        if (transform.position.y < -150)
        {
            SceneManager.LoadScene("SplashScreen");
            return;
        }

        bool grounded = CheckGrounded();
        standing = grounded;

        if (jumpLockTimer > 0f) jumpLockTimer -= Time.deltaTime;

        // Landed: touching ground, not rising, and the short jump lock has expired
        if (grounded && jumpLockTimer <= 0f && velY <= 0.1f)
        {
            isJumping = false;
        }

        // Jump: only from the ground, once per jump
        if (grounded && !isJumping && controller.moving.y > 0)
        {
            //audioData.Play(0);
            isJumping = true;
            jumpLockTimer = 0.15f;
            // Launch straight up by setting vertical speed directly (not affected by mass or frame rate)
            body2D.linearVelocity = new Vector2(body2D.linearVelocity.x, jumpVelocity);
        }

        var forceX = 0f;

        if (controller.moving.x != 0) //Check if the player is moving horizontally
        {
            if (absVelX < maxVelocity.x) //Only push while under the speed limit
            {
                var newSpeed = speed * controller.moving.x;
                forceX = grounded ? newSpeed : (newSpeed * airSpeedMultiplier);
            }

            bool movingLeft = controller.moving.x < 0;
            renderer2D.flipX = spriteFacesLeft ? !movingLeft : movingLeft;
        }

        body2D.AddForce(new Vector2(forceX, 0f)); //Apply the horizontal force to the player's Rigidbody2D

        // ---- Animation: pick ONE state per frame and set it once ----
        int state;
        if (!grounded || isJumping)
        {
            state = velY > 0f ? STATE_JUMP : STATE_FALL; //Going up = jump, going down = fall
        }
        else if (controller.moving.x != 0)
        {
            state = STATE_RUN;
        }
        else
        {
            state = STATE_IDLE;
        }

        animator.SetInteger(ANIM_PARAM, state);
    }
}
