using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterController : SimulatedScript
{
    [Header("Lateral Movement")]
    [SerializeField] protected float runForce;
    [SerializeField] protected float airRunForce;
    [SerializeField] protected float friction;
    [SerializeField] protected float airFriction;
    [SerializeField] protected float horizontalSpeedCap;

    [Header("Vertical Movement")]
    [SerializeField] protected float jumpForce;
    [Tooltip("The portion of the character's Y velocity to remove after a cancelled jump")]
    [SerializeField] protected float cancelledJumpImpulseRatio = 0.5f;
    [SerializeField] protected float regularGravity;
    [SerializeField] protected float fallingGravity;
    [SerializeField] protected float releaseImpulse;
    [SerializeField] protected float verticalSpeedCap;
    [SerializeField] protected float groundCheckDistance;

    [Header("Other")]
    [SerializeField] public LayerMask groundLayer;
    [SerializeField] protected Rigidbody2D playerBody;
    [SerializeField] protected Collider2D playerCollider;
    [SerializeField] protected SpriteRenderer playerRenderer;
    [SerializeField] protected Animator spriteAnimator;

    protected Vector2 slopeDir;
    protected Rigidbody2D groundObject;
    protected bool jumpingThisFrame = false;
    protected int walljumpsRemaining;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    #region Lateral Movement
    virtual protected void Move(Vector2 movementDirection)
    {
        bool inputActive = movementDirection.magnitude > 0.01f;
        bool playerStopped = Mathf.Approximately(playerBody.velocity.x, 0);
        bool holdingOppositeDirection = !playerStopped && (Mathf.Sign(movementDirection.x) != Mathf.Sign(playerBody.velocity.x));

        // Running
        if (inputActive)
        {
            // Update sprite
            playerRenderer.flipX = !(Mathf.Sign(movementDirection.x) == -1);

            if (groundObject)
            {
                ApplyForce(movementDirection, runForce, horizontalSpeedCap);
            }
            else
            {
                ApplyForce(movementDirection, airRunForce, horizontalSpeedCap);
            }
        }

        // Friction
        if (!inputActive || holdingOppositeDirection)
        {
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

    private void ApplyFriction(float amount)
    {
        float velocityToRemove = Mathf.Min(amount * Time.fixedDeltaTime, Mathf.Abs(playerBody.velocity.x));
        float velocitySign = -Mathf.Sign(playerBody.velocity.x);
        playerBody.velocity += velocitySign * velocityToRemove * Vector2.right;
    }
    #endregion


    #region Vertical Movement
    public bool TryJump()
    {
        if (groundObject != null)
        {
            groundObject = null;
            ApplyImpulse(Vector2.up, jumpForce, verticalSpeedCap);

            return true;
        }

        return false;
    }

    public void CancelJump()
    {
        float downwardsForce = playerBody.velocity.y * cancelledJumpImpulseRatio;
        ApplyImpulse(Vector2.down, downwardsForce, verticalSpeedCap);
    }

    protected void DoGravity(bool falling)
    {
        if (falling)
        {
            ApplyForce(Vector2.down, fallingGravity, verticalSpeedCap);
        }
        else
        {
            ApplyForce(Vector2.down, regularGravity, verticalSpeedCap);
        }
    }
    #endregion


    protected void ApplyForce(Vector2 direction, float amount, float cap)
    {
        direction.Normalize();
        float amountBelowCap = cap - Mathf.Abs((playerBody.velocity * direction).magnitude);
        float velocityToAdd = Mathf.Min(amount, amountBelowCap);

        playerBody.velocity += velocityToAdd * Time.fixedDeltaTime * direction;
    }

    protected void ApplyImpulse(Vector2 direction, float amount, float cap)
    {
        direction.Normalize();
        float amountBelowCap = cap - Mathf.Abs((playerBody.velocity * direction).magnitude);
        float velocityToAdd = Mathf.Min(amount, amountBelowCap);

        playerBody.velocity += velocityToAdd * direction;
    }

    private void InheritVelocity(Rigidbody2D other)
    {
        if (other == null) return;

        Vector2 velocity = new Vector2(other.velocity.x, other.velocity.y);
        playerBody.position += velocity * Time.deltaTime;
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
    virtual protected void UpdateGroundObject()
    {
        Vector2 raycastOrigin = playerCollider.bounds.min;
        Vector2 raycastDirection = Vector2.down;
        float raycastDistance = groundCheckDistance;

        // Check bottom-left
        RaycastHit2D[] leftHits = Physics2D.RaycastAll(raycastOrigin, raycastDirection, raycastDistance, groundLayer);

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
            playerBody.gravityScale = 0;
        }
        else
        {
            playerBody.gravityScale = 1;
        }
        groundObject = isGrounded ? totalHits[0].rigidbody : null;
    }

    /*
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
    */
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
        if (groundObject != null)
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
