using System.Collections;
using System.Linq;
using System.Numerics;
using UnityEngine;
using UnityEngine.InputSystem;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class PlayerController : SimulatedScript
{
    [Header("Health")]
    [SerializeField] private int maxHealth;
    [SerializeField] private float playerDamageCooldown;
    [SerializeField] private UnityEngine.Rendering.Volume damageVolume;

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
    [SerializeField] public LayerMask groundLayer;
    [SerializeField] private Rigidbody2D playerBody;
    [SerializeField] private Collider2D playerCollider;
    [SerializeField] private SpriteRenderer playerRenderer;
    [SerializeField] private Animator spriteAnimator;

    private Vector2 moveInput;
    private Vector2 slopeDir;
    private Rigidbody2D groundObject;
    private bool jumpingThisFrame = false;
    private int walljumpsRemaining;

    int health;
    bool damagable = true;

    GameManager gameManager;

    public IEnumerator DamageCoroutineObject { get; private set; }

    // Update is called once per frame
    new void Start()
    {
        gameManager = GameManager.Instance;

        gameManager.OnGamePaused.AddListener(OnGamePaused);
        gameManager.OnGameQuit.AddListener(OnGamePaused);

        gameManager.OnGameResumed.AddListener(OnGameResumed);
        gameManager.OnGameStart.AddListener(OnGameResumed);

        gameManager.OnGameWin.AddListener(OnGameEnd);
        gameManager.OnGameLoss.AddListener(OnGameEnd);
        health = maxHealth;
    }

    void FixedUpdate()
    {
        if (!jumpingThisFrame)
            UpdateGroundObject();
        else
            jumpingThisFrame = false;
        Move(moveInput);
        Fall(groundObject != null);
    }

    public void GoRMode(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            gameManager.ResetScene();
        }
    }

    #region Running
    //Listen for movement input
    public void SetMoveInput(InputAction.CallbackContext context)
    {
        Light(32, Color.blue);
        moveInput = context.ReadValue<Vector2>() * Vector2.right;
        Light(34);
    }

    private void Move(Vector2 inputDirection)
    {
        bool inputActive = inputDirection.magnitude > 0.01f;
        bool playerStopped = Mathf.Approximately(playerBody.velocity.x, 0);
        bool holdingOppositeDirection = !playerStopped && (Mathf.Sign(inputDirection.x) != Mathf.Sign(playerBody.velocity.x));

        // Running
        if (inputActive)
        {
            // Update sprite
            playerRenderer.flipX = !(Mathf.Sign(inputDirection.x) == -1);

            if (groundObject)
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
            if (groundObject)
            {
                ApplyFriction(friction);
            }
            else
            {
                ApplyFriction(airFriction);
            }
        }

        InheritVelocity(groundObject);

        // Update Animator
        UpdateAnimator(playerStopped);
    }

    private void ApplyRunForce(float amount, Vector2 direction)
    {
        float amountBelowCap = horizontalSpeedCap - Mathf.Abs(playerBody.velocity.x);
        float velocityToAdd = Mathf.Min(amount, amountBelowCap);

        playerBody.velocity += Time.fixedDeltaTime * velocityToAdd * direction;
    }

    private void ApplyFriction(float amount)
    {
        float velocityToRemove = Mathf.Min(amount * Time.fixedDeltaTime, Mathf.Abs(playerBody.velocity.x));
        float velocitySign = -Mathf.Sign(playerBody.velocity.x);
        playerBody.velocity += velocitySign * velocityToRemove * Vector2.right;
    }

    private void InheritVelocity(Rigidbody2D other)
    {
        if (other == null) return;

        Vector2 velocity = new Vector2(other.velocity.x, other.velocity.y);
        playerBody.position += velocity * Time.fixedDeltaTime;
    }

    private void UpdateAnimator(bool isStopped)
    {
        if (isStopped)
        {
            spriteAnimator.SetFloat("Horizontal Speed", 0);
        }
        else
        {
            Vector2 standingOnVelocity;
            if (groundObject is null)
            {
                standingOnVelocity = Vector2.zero;
            }
            else
            {
                standingOnVelocity = groundObject.velocity;
            }
            spriteAnimator.SetFloat("Horizontal Speed", Mathf.Abs(playerBody.velocity.x - standingOnVelocity.x));
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
            if (groundObject != null)
            {
                groundObject = null;
                jumpingThisFrame = true;

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


    public void Heal(int healthToAdd)
    {
        maxHealth += healthToAdd;
    }

    public void Hurt(int damage)
    {
        if (damagable)
        {
            DamageCoroutineObject = DoDamageCooldown();
            StartCoroutine(DamageCoroutineObject);

            maxHealth -= damage;

            if (maxHealth <= 0)
            {
                Die();
            }
        }
    }

    protected virtual IEnumerator DoDamageCooldown()
    {
        damagable = false;

        float i = 0;
        while (i < playerDamageCooldown)
        {
            i += Time.deltaTime;
            //This is a nice juice thing, if we want a volume to appear on damage (like red at the sides of the screen)

            if (damageVolume != null)
                damageVolume.weight = (playerDamageCooldown - i) / playerDamageCooldown;
            yield return null;
        }

        damagable = true;
    }

    public void Die()
    {
        PlayerSpawn.Respawn();
        health = maxHealth;
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
    private void UpdateGroundObject()
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

        // Check bottom-middle
        raycastOrigin = playerCollider.bounds.min + new Vector3(playerCollider.bounds.size.x * 0.5f, 0, 0);
        RaycastHit2D[] middlehits = Physics2D.RaycastAll(raycastOrigin, raycastDirection, raycastDistance, groundLayer);

        // Check bottom-right
        raycastOrigin = playerCollider.bounds.min + new Vector3(playerCollider.bounds.size.x, 0, 0);
        RaycastHit2D[] rightHits = Physics2D.RaycastAll(raycastOrigin, raycastDirection, raycastDistance, groundLayer);

        RaycastHit2D[] totalHits = leftHits.Concat(middlehits).Concat(rightHits).ToArray();
        bool isGrounded = totalHits.Length > 0;

        spriteAnimator.SetBool("IsGrounded", isGrounded);

        if (isGrounded)
        {
            walljumpsRemaining = walljumpCount;
            playerBody.gravityScale = 0;
        }
        else
        {
            playerBody.gravityScale = 1;
        }
        groundObject = isGrounded ? totalHits[0].rigidbody : null;
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!doCollisionEvents)
            return;

        UpdateGroundObject();

        //Prevent bouncing
        if (groundObject != null)
        {
            playerBody.velocity *= Vector2.right;
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (!doCollisionEvents)
            return;

        // Slope-handling stuff, ignore for now
        if (groundObject !=  null)
        {
            slopeDir = Vector2.Perpendicular(collision.contacts[0].normal).normalized * -1;
            if (slopeDir == Vector2.up || slopeDir == Vector2.down)
            {
                slopeDir = Vector2.right;
                return;
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (!doCollisionEvents)
            return;

        // Slope-handling stuff, ignore for now
        if (groundObject == null)
        {
            slopeDir = Vector2.right;
        }
    }
}
