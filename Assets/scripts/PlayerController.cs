using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // ---- Health (from first script) ----
    public int health { get { return currentHealth; }}
    public int maxHealth = 5;
    int currentHealth;
    public float timeInvincible = 2f;
    bool isInvincible;
    float damageCooldown;
    Animator animator;

    // ---- Movement (from second script) ----
    public Vector2 moving = new Vector2(); // Public variable to store player's movement direction

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        // Invincibility timer
        if (isInvincible)
        {
            damageCooldown -= Time.deltaTime;
            if (damageCooldown < 0)
            {
                isInvincible = false;
            }
        }

        // Reset the movement vector to (0, 0) at the start of each frame
        moving.x = moving.y = 0;

        // Check if the right arrow key or 'D' key is pressed
        if (Input.GetKey("right") || Input.GetKey("d")){
            // Set the x component of the movement vector to 1 (right direction)
            moving.x = 1;
        }

        // Check if the left arrow key or 'A' key is pressed
        else if (Input.GetKey("left") || Input.GetKey("a")){
            // Set the x component of the movement vector to -1 (left direction)
            moving.x = -1;
        }

        // Check if the spacebar is pressed (jump)
        if (Input.GetKey("space")){
            // Set the y component of the movement vector to 1 (upward direction)
            moving.y = 1;
        }

        // Check if the down arrow key or 'S' key is pressed
        else if (Input.GetKey("down") || Input.GetKey("s")){
            // Set the y component of the movement vector to -1 (downward direction)
            moving.y = -1;
        }
    }

    public void ChangeHealth(int amount)
    {
        if (amount < 0 || amount > 0)
        {
            if (isInvincible)
            {
                return;
            }
            isInvincible = true;
            damageCooldown = timeInvincible;
            if (amount < 0)
            {
                animator.SetTrigger("Hit");
            }
        }
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        UIHandler.instance.SetHealthValue(currentHealth / (float)maxHealth);
    }
}
