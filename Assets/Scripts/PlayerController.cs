using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : SimulatedScript
{
    [SerializeField]
    [Range(0f, 2f)]
    private float giz_rad;

    [SerializeField]
    private Vector2 giz_offset;

    [SerializeField] private int playerHealth = 3;

    [Header("Running")]
    [SerializeField] private float runForce;
    [SerializeField] private float airRunForce;
    [SerializeField] private float friction;
    [SerializeField] private float airFriction;
    [SerializeField] private float horizontalSpeedCap;

    [Header("Jumping")]
    [SerializeField] private float jumpForce;
    [SerializeField] private float bonusGravity;
    [SerializeField] private float verticalSpeedCap;
    [SerializeField] private float groundCheckDistance;

    [Header("Walljumping")]
    [SerializeField] private float horizontalWalljumpForce;
    [SerializeField] private float verticalWalljumpForce;
    [SerializeField] private int walljumpCount;
    [SerializeField] private float wallCheckDistance;

    [Header("Other")]
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Rigidbody2D playerBody;
    [SerializeField] private Collider2D playerCollider;
    [SerializeField] private SpriteRenderer playerRenderer;
    [SerializeField] private Animator spriteAnimator;

    private Vector2 moveInput;
    private Vector2 slopeDir;
    private int walljumpsRemaining;

    GameManager gameManager;

    // Update is called once per frame
    private void Start()
    {
        gameManager = GameManager.Instance;

        gameManager.OnGamePaused.AddListener(OnGamePaused);
        gameManager.OnGameQuit.AddListener(OnGamePaused);

        gameManager.OnGameResumed.AddListener(OnGameResumed);
        gameManager.OnGameStart.AddListener(OnGameResumed);

        gameManager.OnGameWin.AddListener(OnGameEnd);
        gameManager.OnGameLoss.AddListener(OnGameEnd);

        heal(playerHealth);
    }

    void FixedUpdate()
    {
        Move(moveInput);
        Fall(CheckIsGrounded());
    }

    #region Running
    //Listen for movement input
    public void SetMoveInput(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>() * Vector2.right;
    }

    private void Move(Vector2 inputDirection)
    {
        //Get the player's speed
        float playerSpeed = Mathf.Abs(playerBody.velocity.x);

        if (inputDirection.magnitude > 0.01f)
        {
            float force = getMoveForce(inputDirection, playerSpeed);
            playerBody.AddForce(inputDirection * force, ForceMode2D.Force);

            playerRenderer.flipX = !(Mathf.Sign(inputDirection.x) == -1);
        }
        //If there is not input but the player is moving, apply friction
        //If the player is grounded, apply regular friction
        else if (CheckIsGrounded() && Mathf.Abs(playerBody.velocity.x) > 0.1f)
        {
            float amount = Mathf.Min(playerSpeed, friction);
            amount *= -Mathf.Sign(playerBody.velocity.x);
            playerBody.AddForce(Vector2.right * amount, ForceMode2D.Impulse);
        }
        //Otherwise, apply air friction
        else
        {
            float amount = Mathf.Min(Mathf.Abs(playerBody.velocity.x), Mathf.Abs(airFriction));
            amount *= Mathf.Sign(playerBody.velocity.x);
            playerBody.AddForce(slopeDir * -amount, ForceMode2D.Impulse);
        }

        //Update the animator with the current running state and horizontal speed
        //spriteAnimator.SetBool("Run?!?!?!?!", isPlayerRunning);
        
        if (Mathf.Approximately(playerBody.velocity.x, 0))
        {
            spriteAnimator.SetFloat("Horizontal Speed", 0);
        }
        else
        {
            spriteAnimator.SetFloat("Horizontal Speed", Mathf.Abs(playerBody.velocity.x));
        }
    }

    private float getMoveForce(Vector2 inputDirection, float playerSpeed)
    {
        //Calculate the gap between the max and current speed
        float speedDifference = horizontalSpeedCap - playerSpeed;

        if (!Mathf.Approximately(speedDifference, 0f))
        {
            float force;
            if (CheckIsGrounded())
            {
                force = runForce;
            }
            else
            {
                force = airRunForce;
            }
            if (speedDifference > 0)
            {
                float forceCap = speedDifference / Time.fixedDeltaTime * playerBody.mass;
                float cappedForce = Mathf.Min(forceCap, force);

                return cappedForce;
            }
            else
            {
                playerBody.AddForce(speedDifference * Mathf.Sign(playerBody.velocity.x) * playerBody.mass * Vector2.right, ForceMode2D.Impulse);
                return 0;
            }
        }
        else
        {
            return 0;
        }
    }
    #endregion

    #region Jumping
    public void Jump(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            //Jumping
            if (CheckIsGrounded())
            {
                playerBody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            }
        }
        else if (context.canceled)
        {
            float downwardsForce = playerBody.velocity.y * 0.5f;
            playerBody.AddForce(Vector2.down * downwardsForce, ForceMode2D.Impulse);
        }
    }

    void Fall(bool isGrounded)
    {
        if (!isGrounded)
        {
            playerBody.AddForce(new Vector2(0, bonusGravity * -1));
        }
        //Cap vertical speed
        CheckVerticalSpeedCap();
        //Update animator
        spriteAnimator.SetBool("IsFalling", Mathf.Sign(playerBody.velocity.y) == -1);
    }
    #endregion


    public void heal(int health)
    {
        for (int i = 0; i < health; i++)
        {
            Debug.Log("healing");
            gameManager.HealPlayer();
        }
    }

    public void hurt(int damage)
    {
        for (int i = 0; i < damage; i++)
        {
            gameManager.HurtPlayer();
        }
    }

    public void Kill()
    {
        gameManager.LoseGame();
    }

    public void Win()
    {
        gameManager.WinGame();
    }

    public void TogglePause(InputAction.CallbackContext context)
    {
        if (context.performed)
            gameManager.TogglePause();
    }

    public void OnGamePaused()
    {
        SwitchCurrentActionMap("UI");
    }

    public void OnGameResumed()
    {
        SwitchCurrentActionMap("Player");
    }

    public void OnGameEnd()
    {
        SwitchCurrentActionMap("Endscreen");
    }

    public void QuitGame(InputAction.CallbackContext context)
    {
        gameManager.QuitGame();
    }

    #region Helper Functions
    //Check if the vertical speed is over the speed cap
    void CheckVerticalSpeedCap()
    {
        float yVelocity = playerBody.velocity.y;
        if (Mathf.Abs(yVelocity) > verticalSpeedCap)
        {
            playerBody.velocity = new Vector2(playerBody.velocity.x, verticalSpeedCap * Mathf.Sign(yVelocity));
        }
    }

    //Check if the player is grounded
    private bool CheckIsGrounded()
    {
        Vector2 raycastOrigin = playerCollider.bounds.min;
        Vector2 raycastDirection = Vector2.down;
        float raycastDistance = groundCheckDistance;
        RaycastHit2D[] hits = Physics2D.RaycastAll(raycastOrigin, raycastDirection, raycastDistance, groundLayer);

        bool isGrounded = (hits.Length > 0);
        spriteAnimator.SetBool("IsGrounded", isGrounded);
        if (isGrounded)
        {
            walljumpsRemaining = walljumpCount;
        }
        return isGrounded;
    }

    //Check for a wall on the player's left
    private bool CheckIsWalledLeft()
    {
        RaycastHit2D boxCastHit = Physics2D.BoxCast(playerCollider.bounds.center, playerCollider.bounds.size, 0f, Vector2.left, wallCheckDistance, groundLayer);
        return boxCastHit.collider != null;
    }

    //Check for a wall on the player's right
    private bool CheckIsWalledRight()
    {
        RaycastHit2D boxCastHit = Physics2D.BoxCast(playerCollider.bounds.center, playerCollider.bounds.size, 0f, Vector2.right, wallCheckDistance, groundLayer);
        return boxCastHit.collider != null;
    }

    private void SwitchCurrentActionMap(string mapName)
    {
        playerInput.currentActionMap.Disable();
        playerInput.SwitchCurrentActionMap(mapName);

        /*
        switch (mapName)
        {
            case "UI":
                Cursor.lockState = CursorLockMode.Confined;
                Cursor.visible = true;
                break;
            case "Endscreen":
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                break;
            default:
            case "Player":
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                break;
        }
        */
    }
    #endregion

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere((Vector2)transform.position + giz_offset, giz_rad);
        Gizmos.DrawLine(playerBody.transform.position, (Vector2)playerBody.transform.position + slopeDir);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (CheckIsGrounded())
        {
            slopeDir = Vector2.Perpendicular(collision.contacts[0].normal).normalized * -1;
            if (slopeDir == Vector2.up || slopeDir == Vector2.down)
            {
                slopeDir = Vector2.right;
                return;
            }
            playerBody.gravityScale = 0;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        slopeDir = Vector2.right;
        playerBody.gravityScale = 1;
    }
}
