using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterController : SimulatedScript
{
    protected override string DefaultVisualComponentName => "CharacterController";

    [Header("Lateral Movement")]
    [SerializeField] protected float runForce;
    [SerializeField] protected float airRunForce;
    [SerializeField] protected float friction;
    [SerializeField] protected float airFriction;
    [SerializeField] protected float horizontalSpeedCap;
    [SerializeField] protected float wallCheckDistance = 0.1f;

    [Header("Vertical Movement")]
    [SerializeField] protected float coyoteTime;
    [SerializeField] protected float jumpForce;
    [Tooltip("The portion of the character's Y velocity to remove after a cancelled jump")]
    [SerializeField] protected float cancelledJumpImpulseRatio = 0.5f;
    [SerializeField] protected float regularGravity = 20.8f;
    [SerializeField] protected float fallingGravity = 29.8f;
    [SerializeField] protected float releaseImpulse;
    [SerializeField] protected float verticalSpeedCap = 1000f;
    [SerializeField] protected float groundCheckDistance = 0.1f;

    [Header("Other")]
    [SerializeField] public LayerMask groundLayer;
    [SerializeField] protected Rigidbody2D characterBody;
    [SerializeField] protected Collider2D characterCollider;
    [SerializeField] protected SpriteRenderer characterRenderer;
    [SerializeField] protected Animator spriteAnimator;

    protected GameManager gameManager;

    protected Vector2 slopeDir;
    protected Rigidbody2D groundObject;
    protected bool jumpingThisFrame = false;
    protected int walljumpsRemaining;
    protected float timeSinceGrounded = 0;
    protected bool coyoteJumpConsumed = false;

    // Start is called before the first frame update
    virtual protected new void Start()
    {
        gameManager = GameManager.Instance;
    }

    // Update is called once per frame
    virtual protected void FixedUpdate()
    {
        UpdateGroundObject();

        // Update timeSinceGrounded, for coyote time
        if (groundObject == null)
        {
            DoGravity(characterBody.velocity.y < 0);
            timeSinceGrounded += Time.deltaTime;
        }
        else
        {
            timeSinceGrounded = 0;
            coyoteJumpConsumed = false;
        }
    }

    #region Lateral Movement
    virtual protected void Move(Vector2 movementDirection)
    {
        bool inputActive = movementDirection.magnitude > 0.01f;
        bool playerStopped = Mathf.Approximately(characterBody.velocity.x, 0);
        bool holdingOppositeDirection = !playerStopped && (Mathf.Sign(movementDirection.x) != Mathf.Sign(characterBody.velocity.x));

        // Running
        if (inputActive)
        {
            // Update sprite
            characterRenderer.flipX = !(Mathf.Sign(movementDirection.x) == -1);

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
        float velocityToRemove = Mathf.Min(amount * Time.fixedDeltaTime, Mathf.Abs(characterBody.velocity.x));
        float velocitySign = -Mathf.Sign(characterBody.velocity.x);
        characterBody.velocity += velocitySign * velocityToRemove * Vector2.right;
    }
    #endregion

    #region Vertical Movement
    public bool TryJump()
    {
        if (groundObject != null || (timeSinceGrounded <= coyoteTime && !coyoteJumpConsumed))
        {
            Jump();
            coyoteJumpConsumed = true;
            return true;
        }
        else return false;
    }

    private void Jump()
    {
        groundObject = null;
        // Cancel out existing vertical velocity, for coyote gamers
        characterBody.velocity *= Vector2.right;
        ApplyImpulse(Vector2.up, jumpForce, verticalSpeedCap);
    }

    public void CancelJump()
    {
        float downwardsForce = characterBody.velocity.y * cancelledJumpImpulseRatio;
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

    #region Forces
    protected void ApplyForce(Vector2 direction, float amount, float cap)
    {
        direction.Normalize();
        float amountBelowCap = cap - Mathf.Abs((characterBody.velocity * direction).magnitude);
        float velocityToAdd = Mathf.Min(amount, amountBelowCap);

        characterBody.velocity += velocityToAdd * Time.fixedDeltaTime * direction;
    }

    protected void ApplyImpulse(Vector2 direction, float amount, float cap)
    {
        direction.Normalize();
        float amountBelowCap = cap - Mathf.Abs((characterBody.velocity * direction).magnitude);
        float velocityToAdd = Mathf.Min(amount, amountBelowCap);

        characterBody.velocity += velocityToAdd * direction;
    }

    protected void InheritVelocity(Rigidbody2D other)
    {
        if (other == null) return;

        Vector2 velocity = new Vector2(other.velocity.x, other.velocity.y);
        characterBody.position += velocity * Time.deltaTime;
    }
    #endregion

    #region Helper Functions
    //Check if the player is grounded
    virtual protected void UpdateGroundObject()
    {
        Vector2 raycastOrigin = characterCollider.bounds.min;
        Vector2 raycastDirection = Vector2.down;
        float raycastDistance = groundCheckDistance;

        // Check bottom-left
        RaycastHit2D[] leftHits = Physics2D.RaycastAll(raycastOrigin, raycastDirection, raycastDistance, groundLayer);

        // Check bottom-middle
        raycastOrigin = characterCollider.bounds.min + new Vector3(characterCollider.bounds.size.x * 0.5f, 0, 0);
        RaycastHit2D[] middlehits = Physics2D.RaycastAll(raycastOrigin, raycastDirection, raycastDistance, groundLayer);

        // Check bottom-right
        raycastOrigin = characterCollider.bounds.min + new Vector3(characterCollider.bounds.size.x, 0, 0);
        RaycastHit2D[] rightHits = Physics2D.RaycastAll(raycastOrigin, raycastDirection, raycastDistance, groundLayer);

        RaycastHit2D[] totalHits = leftHits.Concat(middlehits).Concat(rightHits).ToArray();
        bool isGrounded = totalHits.Length > 0;

        spriteAnimator.SetBool("IsGrounded", isGrounded);

        groundObject = isGrounded ? totalHits[0].rigidbody : null;
    }

    //Check for a wall on the player's left
    protected bool CheckForWall(Vector2 direction)
    {
        Vector2 origin = characterCollider.bounds.center;
        Vector2 size = characterCollider.bounds.size;
        float angle = 0f;

        RaycastHit2D[] boxCastHit = Physics2D.BoxCastAll(origin, size, angle, direction, wallCheckDistance, groundLayer);
        foreach (RaycastHit2D hit in boxCastHit)
        {
            if (hit.rigidbody != characterBody)
            {
                return true;
            }
        }
        return false;
    }
    #endregion

    #region Collisions
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!doCollisionEvents)
            return;

        UpdateGroundObject();

        //Prevent bouncing
        if (groundObject != null)
        {
            characterBody.velocity *= Vector2.right;
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
    #endregion

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
            spriteAnimator.SetFloat("Horizontal Speed", Mathf.Abs(characterBody.velocity.x - standingOnVelocity.x));
        }
    }

    public override SimulatedComponent Copy(SimulatedObject destination)
    {
        throw new System.NotImplementedException();
    }
}
