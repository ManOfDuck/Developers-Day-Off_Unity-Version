using System.Linq;
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
    void Start()
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
        Light(26, Color.blue);
        Move(moveInput);
        Light(28, Color.blue);
        Fall(CheckIsGrounded());
    }

    #region Running
    //Listen for movement input
    public void SetMoveInput(InputAction.CallbackContext context)
    {
        Light(32, Color.blue);
        moveInput = context.ReadValue<Vector2>() * Vector2.right;
        Light(34);
    }

    public void GoRMode(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            gameManager.ResetScene();
        }
    }

    private void Move(Vector2 inputDirection)
    {
        bool inputActive = inputDirection.magnitude > 0.01f;
        bool holdingOppositeDirection = inputActive && (Mathf.Sign(inputDirection.x) != Mathf.Sign(playerBody.velocity.x));

        // Running
        if (inputActive)
        {
            // Update sprite
            playerRenderer.flipX = !(Mathf.Sign(inputDirection.x) == -1);

            if (CheckIsGrounded())
            {
                ApplyRunForce(runForce, inputDirection);
            }
            else
            {
                ApplyRunForce(airRunForce, inputDirection);
            }
        }

        // Friction
        if (!inputActive || holdingOppositeDirection) {
            if (CheckIsGrounded())
            {
                ApplyFriction(friction);
            }
            else
            {
                ApplyFriction(airFriction);
            }
        }

        // Update Animator
        UpdateAnimatorSpeed(Mathf.Abs(playerBody.velocity.x));
    }

    private void ApplyRunForce(float amount, Vector2 direction)
    {
        float amountBelowCap = horizontalSpeedCap - Mathf.Abs(playerBody.velocity.x);
        float velocityToAdd = Mathf.Min(amount, amountBelowCap);

        playerBody.velocity += direction * velocityToAdd * Time.fixedDeltaTime;
    }

    private void ApplyFriction(float amount)
    {
        float velocityToRemove = Mathf.Min(amount * Time.fixedDeltaTime, Mathf.Abs(playerBody.velocity.x));
        float velocitySign = -Mathf.Sign(playerBody.velocity.x);
        playerBody.velocity += Vector2.right * velocitySign * velocityToRemove;
    }

    private void ClampSpeedToZero()
    {
        if (Mathf.Approximately(playerBody.velocity.x, 0)){
            playerBody.velocity = new Vector2(0, playerBody.velocity.y);
        }
    }

    private void UpdateAnimatorSpeed(float velocity)
    {
        if (Mathf.Approximately(velocity, 0))
        {
            spriteAnimator.SetFloat("Horizontal Speed", 0);
        }
        else
        {
            spriteAnimator.SetFloat("Horizontal Speed", velocity);
        }
    }

    /*
    private void Move(Vector2 inputDirection)
    {
        Light(38, Color.blue);
        //Get the player's speed
        float playerSpeed = Mathf.Abs(playerBody.velocity.x);
        Light(40);

        if (inputDirection.magnitude > 0.01f)
        {
            Light(43, Color.green);
            float force = getMoveForce(inputDirection, playerSpeed);
            Light(45);
            playerBody.AddForce(inputDirection * force, ForceMode2D.Force);
            Light(47);

            playerRenderer.flipX = !(Mathf.Sign(inputDirection.x) == -1);
            Light(49);
        }
        //If there is not input but the player is moving, apply friction
        //If the player is grounded, apply regular friction
        else if (CheckIsGrounded() && Mathf.Abs(playerBody.velocity.x) > 0.1f)
        {
            Light(43, Color.red);
            Light(54, Color.green);
            float amount = Mathf.Min(playerSpeed, friction);
            Light(57);
            amount *= -Mathf.Sign(playerBody.velocity.x);
            Light(60);
            playerBody.AddForce(Vector2.right * amount, ForceMode2D.Impulse);
            Light(63);
        }
        //Otherwise, apply air friction
        else if (Mathf.Abs(playerBody.velocity.x) > 0.1f)
        {
            Light(43, Color.red);
            Light(54, Color.green);
            float amount = Mathf.Min(Mathf.Abs(playerBody.velocity.x), Mathf.Abs(airFriction));
            Light(57);
            amount *= Mathf.Sign(playerBody.velocity.x);
            Light(60);
            playerBody.AddForce(slopeDir * -amount, ForceMode2D.Impulse);
            Light(63);
        }
        else
        {
            Light(43, Color.red);
            Light(54, Color.red);
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
        Light(68);
    }
    */

    private float getMoveForce(Vector2 inputDirection, float playerSpeed)
    {
        Light(93, Color.blue);
        //Calculate the gap between the max and current speed
        float speedDifference = horizontalSpeedCap - playerSpeed;
        Light(96);

        if (!Mathf.Approximately(speedDifference, 0f))
        {
            Light(99, Color.green);

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
                Light(104);
                float cappedForce = Mathf.Min(forceCap, force);
                Light(105);

                Light(107, Color.blue);
                return cappedForce;
            }
            else
            {
                Light(99, Color.red); Light(110, Color.blue);
                Light(110, Color.blue);
                playerBody.AddForce(speedDifference * Mathf.Sign(playerBody.velocity.x) * playerBody.mass * Vector2.right, ForceMode2D.Impulse);
                return 0;
            }
        }
        else
        {
            Light(99, Color.red);
            Light(110, Color.green);
            Light(111, Color.blue);
            return 0;
        }
    }
    #endregion

    #region Jumping
    public void Jump(InputAction.CallbackContext context)
    {
        Light(72, Color.blue);
        if (context.started)
        {
            Light(74, Color.green);
            //Jumping
            if (CheckIsGrounded())
            {
                Light(76, Color.green);
                playerBody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                Light(78);
            }
            else Light(76, Color.red);
        }
        else if (context.canceled)
        {
            Light(74, Color.red);
            Light(84, Color.green);
            float downwardsForce = playerBody.velocity.y * 0.5f;
            Light(86);
            playerBody.AddForce(Vector2.down * downwardsForce, ForceMode2D.Impulse);
            Light(89);
        }
        else {
            Light(74, Color.red);
            Light(84, Color.red);
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
        Light(115, Color.blue);
        Vector2 raycastOrigin = playerCollider.bounds.min; 
        Light(118);
        Vector2 raycastDirection = Vector2.down;
        Light(119);
        float raycastDistance = groundCheckDistance;
        Light(120);

        // Check bottom-left
        RaycastHit2D[] leftHits = Physics2D.RaycastAll(raycastOrigin, raycastDirection, raycastDistance, groundLayer);
        Light(121);

        // Check bottom-right
        raycastOrigin = playerCollider.bounds.min + new Vector3(playerCollider.bounds.size.x, 0, 0);
        RaycastHit2D[] rightHits = Physics2D.RaycastAll(raycastOrigin, raycastDirection, raycastDistance, groundLayer);

        RaycastHit2D[] totalHits = leftHits.Concat(rightHits).ToArray();
        bool isGrounded = totalHits.Length > 0;

        if (isGrounded)
        {
            //transform.SetParent(totalHits[0].transform);

            Light(124, Color.green);
            Light(127, Color.green);
            Light(129, Color.green);
        }
        else
        {
           // transform.SetParent(null);

            Light(124, Color.red);
            Light(127, Color.red);
            Light(129, Color.red);
        }
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
