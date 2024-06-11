using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{

    private Rigidbody2D rb;
    private Animator anim;
    private float inputH;
    private float inputV;
    private bool wallJumping = false;
    private HealthSystem healthSystem;
    private AudioManager audioManager;
    [SerializeField] private GameObject gameOverMessage;
    [SerializeField] private bool showHitBox = true;
    [SerializeField] private Transform startposition;
    [SerializeField] private Slider healthbar;
    [Header("Movement system")]
    [SerializeField] private float speed = 10f;
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundDetectionLength = 0.1f;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private float wallDetectionLength = 0.1f;
    [SerializeField] private Vector2 wallJumpForce;
    [SerializeField] private float wallJumpDuration = 1f;
    [Header("Knockback system")]
    [SerializeField] private float hitKnockBack = 5f;
    [SerializeField] private float enemyKnockBack = 10f;
    [Header("Attack system")]
    [SerializeField] private Transform attackHitBoxTransform;
    [SerializeField] private float attackHitBoxRadius = 0.5f;
    [SerializeField] private LayerMask affectedLayer;
    [SerializeField] private float damage = 10f;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        healthSystem = GetComponent<HealthSystem>();
        audioManager = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>();
    }

    private void FixedUpdate()
    {
        if(wallJumping)
        {
            //transform.eulerAngles = new Vector3(0, transform.eulerAngles.y == 0 ? 180 : 0, 0); 
            float playerDirection = transform.eulerAngles.y == 0 ? 1 : -1;
            rb.velocity = new Vector2(-playerDirection * wallJumpForce.x, wallJumpForce.y);

            // flip player
            anim.SetTrigger("jump");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            // Pushback the player when hit the enemy
            rb.AddForce(Vector2.left * hitKnockBack, ForceMode2D.Impulse);
        }
        AttackAnimation();
        Movement();
    }

    public void OnTakeDamage()
    {
        // update health bar
        float health = healthSystem.GetHealth();
        healthbar.value = health * 0.01f;

       if (healthSystem.GetHealth() <= 0)
        {
            // Game Over
            GameOver();
        }
    }

    void Movement()
    {
        inputH = Input.GetAxisRaw("Horizontal");
        inputV = Input.GetAxisRaw("Vertical");
        // Detect if is falling
        anim.SetBool("falling", rb.velocity.y < -2);
        rb.velocity = new Vector2(inputH * speed, rb.velocity.y);
        // Set Running Animation only if the player is not crawling
        if (!isWalled())
        {
            anim.SetBool("running", inputH != 0);
        }

        // Flip player and wallCheck
        transform.eulerAngles = inputH == 1 ? Vector3.zero : inputH == -1 ? new Vector3(0, 180, 0) : transform.eulerAngles;
        wallCheck.eulerAngles = inputH == 1 ? Vector3.zero : inputH == -1 ? new Vector3(0, 180, 0) : wallCheck.eulerAngles;


        if (isWalled())
        {
            // Set Wall Crawl Animation
            // Wall Crawl
            anim.SetBool("isWallCrawling", inputV == 0);
            if (!wallJumping)
            {
                rb.velocity = new Vector2(rb.velocity.x, inputV * speed);
                anim.SetBool("isWallRunning", inputV != 0);
            }
        }
        else
        {
            anim.SetBool("isWallRunning", false);
        }
        anim.SetBool("isWallCrawling", isWalled());

        // Jump
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (IsGrounded() && !isWalled())
            {
                rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                anim.SetTrigger("jump");
            }
            else if (isWalled())
            {
                // Debug.Log to check if wall jump conditions are met
                Debug.Log("Wall jump condition met");
                wallJumping = true;
                Invoke(nameof(StopWallJumping), wallJumpDuration);
            }
        }
    }


    void AttackAnimation()
    {
        if(Input.GetKeyDown(KeyCode.F) || Input.GetKeyDown(KeyCode.K) || Input.GetMouseButtonDown(0))
       anim.SetTrigger("attack");
    }
    // Call this function from the animation event
    void AttackHitbox()
    {
        Collider2D[] collisions = Physics2D.OverlapCircleAll(attackHitBoxTransform.position, attackHitBoxRadius, affectedLayer);
        foreach (Collider2D other in collisions)
        {
            if(!other.CompareTag("Player"))
            {
                // Damage the enemy
                HealthSystem healthSystem = other.GetComponent<HealthSystem>();
                if (healthSystem != null)
                {
                    healthSystem.TakeDamage(damage);
                }

                // Pushback the enemy and player when hit the enemy
                Rigidbody2D otherRb = other.GetComponent<Rigidbody2D>();
                if (otherRb != null)
                {
                    otherRb.AddForce(Vector2.right * enemyKnockBack, ForceMode2D.Impulse);
                }
                // Pushback the player when hit the enemy
                rb.velocity = new Vector2(rb.velocity.x, 0);
                rb.AddForce(Vector2.left * hitKnockBack, ForceMode2D.Impulse);
            }
           
        }
    }

    bool IsGrounded()
    {
        return  Physics2D.Raycast(groundCheck.position, Vector2.down, groundDetectionLength, groundLayer);
    }

    void CheckCanWallCrawl()
    {
        if (isWalled())
        {
            // Set Wall Crawl Animation
            // Wall Crawl
            float inputV = Input.GetAxisRaw("Vertical");
            anim.SetBool("isWallCrawling",inputV == 0);
            if(!wallJumping)
            {
            rb.velocity = new Vector2(rb.velocity.x, inputV * speed);
            anim.SetBool("isWallRunning", inputV != 0);
            }
        } else
        {
            anim.SetBool("isWallRunning", false);
        }
        anim.SetBool("isWallCrawling", isWalled());

    }
    bool isWalled()
    {
        // Check if the player is walled
        return Physics2D.Raycast(wallCheck.position, transform.right, wallDetectionLength, groundLayer);
    }

    void StopWallJumping()
    {
        wallJumping = false;
    }

    void GameOver()
    {
        audioManager.StopBackground();
        gameOverMessage.SetActive(true);
        Time.timeScale = 0;
    }

    private void OnDrawGizmos()
    {
        // draw attack hitbox
        if (showHitBox)
        {
        Gizmos.DrawSphere(attackHitBoxTransform.position, attackHitBoxRadius);
        }
        // draw ground check
        Gizmos.DrawLine(groundCheck.position, new Vector3(groundCheck.position.x, groundCheck.position.y - groundDetectionLength, groundCheck.position.z));
        // draw wall check
        Gizmos.DrawLine(wallCheck.position, new Vector3(wallCheck.position.x + wallDetectionLength, wallCheck.position.y, wallCheck.position.z));
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Killzone"))
        {
            // Respawning the player
            transform.position = startposition.position;
        }
    }
}
