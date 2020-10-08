using System.Collections;
using System.Security.Cryptography;
//using UnityEditor.ShaderGraph.Internal;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Diagnostics;
//using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;

public class playerController : MonoBehaviour
{
    public float speed;
    public float jump;
    public bool isGrounded;
    public bool isWalled;
    public bool hasDoubleJump = false;
    public bool hasInvulnerablePowerup;
    public float jumpNum;
    public float dashSpeed;
    public float slideSpeed = 10f;
    public float movementWait;
    public Tilemap breakableTilemap;
    public LifeUIController lifeUIController;
    static bool setHealth;
    static bool gameOver;
    static GameObject audio1;
    static AudioManager audioScript;
    static int totalHealth = 10;
    [SerializeField] GameObject gameOverObj;

    // For Animations
    public Animator animator;
    private bool facingRight = true;
    private SpriteRenderer sr;

    private Rigidbody2D rb;
    public float direction;
    public bool hasDash;
    public bool hasWallJump;
    private bool wallJumped;
    private float wait;
    public float dashingDuration;
    public bool isDashing;
    private float dashEndTime;
    static HealthSystem healthSystem;
    private bool isInvulnerable;
    



    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        wait = movementWait;
        //isInvulnerable = false;
        //hasInvulnerablePowerup = false;
        dashEndTime = 0f;
        if(setHealth == false)
        {
            healthSystem = new HealthSystem(totalHealth);
            setHealth = true;
            
        }
       /*if(gameOver == true)
        {
            Debug.Log("Player position set");
            gameObject.transform.position = new Vector3(-50, -4, 0);//sets player position to start
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            
            gameOver = false;
        }*/
        if(SceneManager.GetActiveScene().name != "CY_MenuScreen")
        {
            lifeUIController.SetAmountHearts(healthSystem.getAmountLives());
        }
        
    }

    public bool getGrounded()
    {
        return isGrounded;
    }

    public float getJump()
    {
        return jumpNum;
    }

    // Update is called once per frame
    void Update()
    {
        direction = Input.GetAxisRaw("Horizontal");
        animator.SetFloat("Speed", Mathf.Abs(direction));

        //Game Over
        if (healthSystem.getAmountLives() <= 0)
        {
            Debug.Log("Health zero!");
            startGameOver(3);
            //gameOver = true;
            //SceneManager.LoadScene("GameOverScreen");
            audio1 = GameObject.Find("MusicManager");

            audioScript = audio1.GetComponent<AudioManager>();

            audioScript.stopMusic();
            setHealth = false;
            //gameObject.transform.localPosition = new Vector3 (-50, -4, 0);//sets player position to start*/
        }

        //Flip sprite
        if (direction < 0)
            sr.flipX = true;
        else if (direction > 0)
            sr.flipX = false;

        if (wallJumped && wait > 0)
        {
            wait--;
        }
        else
        {
            rb.velocity = new Vector2(speed * direction, rb.velocity.y);
            wallJumped = false;
            if (wait != movementWait)
                wait = movementWait;
        }

        //General Jump
        if ((Input.GetButtonDown("Jump")) && (isGrounded || jumpNum > 0))
        {
            rb.velocity = Vector2.up * jump;
            if (isGrounded)
                jumpNum--;
            else
                jumpNum = 0;
        }

        //Wall Jump
        if ((Input.GetButtonDown("Jump")) && (hasWallJump))
        {
            if (isWalled && hasWallJump)
            {
                rb.velocity = new Vector2(-1f * direction * speed/4, jump);
                wallJumped = true;
            }
        }

        // Wall slide
        if (direction != 0 && isWalled && !wallJumped && hasWallJump)
        {
            rb.velocity = new Vector2(rb.velocity.x, slideSpeed);
        }
        
        // Dash mechanic
        if (Input.GetKeyDown(KeyCode.LeftShift) && hasDash && !isDashing)
        {
            //dash left
            isDashing = true;
            rb.velocity = new Vector2(-1 * dashSpeed, 0);
            if (sr.flipX)
            {
                rb.velocity = new Vector2(-1 * dashSpeed, 0);
            }
            else
            {
                rb.velocity = new Vector2(dashSpeed, 0); 
            }
            dashEndTime = Time.time + dashingDuration;
        }

        if (Time.time >= dashEndTime)
        {
            isDashing = false;
        }

        //Punch animation
        if (Input.GetKeyDown(KeyCode.F))
        {
            //UnityEngine.Debug.Log("Punch Hit!");
            animator.SetBool("isPunch", true);
        } else if (Input.GetKeyUp(KeyCode.F))
        {
            animator.SetBool("isPunch", false);
        }

    }
   
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Dash")
        {
            this.hasDash = true;
            collision.GetComponentInChildren<PowerupToolTipSpawner>().SpawnTooltip();
            Destroy(collision.gameObject);
        }

        if (collision.tag == "wallJump")
        {
            this.hasWallJump = true;
            collision.GetComponentInChildren<PowerupToolTipSpawner>().SpawnTooltip();
            Destroy(collision.gameObject);
        }

        if (collision.tag == "invulnerability")
        {
            hasInvulnerablePowerup = true;
            //isInvulnerable = true;
            collision.GetComponentInChildren<PowerupToolTipSpawner>().SpawnTooltip();
            Destroy(collision.gameObject);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        Damager damager = collision.GetComponent<Damager>();
        // Only objects with the damager component will hurt the player
        // OR do not hurt the player if they have invulnerability powerup and touch lava and spikes
        if (damager != null && !isInvulnerable &&
            !(hasInvulnerablePowerup && damager.isDamagerSpikesOrLava()))
        {
            if (!healthSystem.Damage(damager.getDamageAmount()))
            {
                // Only do this flash if the player is not dead
                StartCoroutine(PlayerHurtFlash(0.2f, 7));
                lifeUIController.SetAmountHearts(healthSystem.getAmountLives());
                
            }
            else
            {
                lifeUIController.SetAmountHearts(healthSystem.getAmountLives());


                // TODO: Handle player death
            }
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        Damager damager = collision.gameObject.GetComponent<Damager>();
        // Only objects with the damager component will hurt the player
        // OR do not hurt the player if they have invulnerability powerup and touch lava and spikes
        if (damager != null && !isInvulnerable &&
            !(hasInvulnerablePowerup && damager.isDamagerSpikesOrLava()))
        {
            if (!healthSystem.Damage(damager.getDamageAmount()))
            {
                // Only do this flash if the player is not dead
                StartCoroutine(PlayerHurtFlash(0.2f, 7));
                lifeUIController.SetAmountHearts(healthSystem.getAmountLives());
               
            }
            else
            {
                lifeUIController.SetAmountHearts(healthSystem.getAmountLives());


                // TODO: Handle player death
            }
        }
    }

    // Flashes the player sprite when taking damage with a 'seconds' cycle and 'amountTimesFlash' times.
    private IEnumerator PlayerHurtFlash(float seconds, int amountTimesFlash)
    {
        isInvulnerable = true;
        for (int i = 0; i < amountTimesFlash; i++)
        {
            Color c = sr.material.color;
            c.a = 0.1f;
            sr.material.color = c;
            yield return new WaitForSeconds(seconds / 2.0f);
            c = sr.material.color;
            c.a = 1f;
            sr.material.color = c;
            yield return new WaitForSeconds(seconds / 2.0f);
        }
        isInvulnerable = false;
    }

    public void startGameOver(float seconds)
    {
        StartCoroutine(GameOver(3));
    }
    private IEnumerator GameOver(float seconds)
    {
        Debug.Log("Game Over!");
        gameOverObj.SetActive(true);
        wait = seconds;
        yield return new WaitForSeconds(seconds);
        resetPlayer();
        SceneManager.LoadScene("CY_MainMenu");
    }

    private void resetPlayer()
    {
        Debug.Log("Health Reset!");
        setHealth = false;
        totalHealth = 10;
        gameObject.transform.position = new Vector3(-50, -4, 0);
        hasDash = false;
        hasWallJump = false;
        hasInvulnerablePowerup = false;
    }
}
