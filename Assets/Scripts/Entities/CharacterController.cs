using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterController : SimulatedScript
{
    protected override string DefaultVisualComponentName => "CharacterController";

    [Header("Lateral Movement")]
    [SerializeField] private float _runForce;
    [SerializeField] private float _airRunForce;
    [SerializeField] private float _friction;
    [SerializeField] private float _airFriction;
    [SerializeField] private float _horizontalSpeedCap;
    [SerializeField] private float _wallCheckDistance = 0.1f;
    protected float RunForce { get => _runForce; set => _runForce = value; }
    protected float AirRunForce { get => _airRunForce; set => _airRunForce = value; }
    protected float Friction { get => _friction; set => _friction = value; }
    protected float AirFriction { get => _airFriction; set => _airFriction = value; }
    protected float HorizontalSpeedCap { get => _horizontalSpeedCap; set => _horizontalSpeedCap = value; }
    protected float WallCheckDistance { get => _wallCheckDistance; set => _wallCheckDistance = value; }
    protected float CoyoteTime { get => _coyoteTime; set => _coyoteTime = value; }

    [Header("Vertical Movement")]
    [SerializeField] private float _coyoteTime;
    [SerializeField] private float _jumpForce;
    [Tooltip("The portion of the character's Y velocity to remove after a cancelled jump")]
    [SerializeField] private float _cancelledJumpImpulseRatio = 0.5f;
    [SerializeField] private float _regularGravity = 20.8f;
    [SerializeField] private float _fallingGravity = 29.8f;
    [SerializeField] private float _releaseImpulse;
    [SerializeField] private float _verticalSpeedCap = 1000f;
    [SerializeField] private float _groundCheckDistance = 0.1f;
    protected float JumpForce { get => _jumpForce; set => _jumpForce = value; }
    protected float CancelledJumpImpulseRatio { get => _cancelledJumpImpulseRatio; set => _cancelledJumpImpulseRatio = value; }
    protected float RegularGravity { get => _regularGravity; set => _regularGravity = value; }
    protected float FallingGravity { get => _fallingGravity; set => _fallingGravity = value; }
    protected float ReleaseImpulse { get => _releaseImpulse; set => _releaseImpulse = value; }
    protected float VerticalSpeedCap { get => _verticalSpeedCap; set => _verticalSpeedCap = value; }
    protected float GroundCheckDistance { get => _groundCheckDistance; set => _groundCheckDistance = value; }

    [Header("Other")]
    [SerializeField] private LayerMask _groundLayer;
    private Rigidbody2D _characterBody;
    private BoxCollider2D _characterCollider;
    private SpriteRenderer _characterRenderer;
    private Animator _spriteAnimator;

    protected Rigidbody2D CharacterBody => AssignMandatoryReference(ref _characterBody, typeof(Rigidbody2DWrapper));
    protected BoxCollider2D CharacterCollider => TryAssignReference(ref _characterCollider);
    protected SpriteRenderer CharacterRenderer => TryAssignReference(ref _characterRenderer);
    protected Animator SpriteAnimator => TryAssignReference(ref _spriteAnimator);


    protected GameManager gameManager;

    protected Vector2 slopeDir;
    protected Rigidbody2D groundObject;
    protected Rigidbody2D wallObject;
    protected Rigidbody2D ceilingObject;
    protected bool jumpingThisFrame = false;
    protected int walljumpsRemaining;
    protected float timeSinceGrounded = 0;
    protected bool coyoteJumpConsumed = false;

    protected Vector2 localVelocity = Vector2.zero;

    [SerializeField] AudioSource landingSound;
    public override SimulatedComponent Copy(ComponentHolder destination)

    {
        CharacterController copy = destination.gameObject.AddComponent(this.GetType()) as CharacterController;

        copy.RunForce = this.RunForce;
        copy.AirRunForce = this.AirRunForce;
        copy.Friction = this.Friction;
        copy.AirFriction = this.AirFriction;
        copy.HorizontalSpeedCap = this.HorizontalSpeedCap;
        copy.WallCheckDistance = this.WallCheckDistance;

        copy.CoyoteTime = this.CoyoteTime;
        copy.JumpForce = this.JumpForce;
        copy.CancelledJumpImpulseRatio = this.CancelledJumpImpulseRatio;
        copy.RegularGravity = this.RegularGravity;
        copy.FallingGravity = this.FallingGravity;
        copy.ReleaseImpulse = this.ReleaseImpulse;
        copy.VerticalSpeedCap = this.VerticalSpeedCap;
        copy.GroundCheckDistance = this.GroundCheckDistance;

        return copy;
    }


    // Start is called before the first frame update
    override protected void Start()
    {
        base.Start();
        gameManager = GameManager.Instance;
    }

    // Update is called once per frame
    virtual protected void FixedUpdate()
    {
        CheckForGround();

        if (!ValidateReferences(CharacterBody) || CharacterBody.bodyType == RigidbodyType2D.Kinematic) return;

        if (groundObject == null)
        {
            DoGravity(localVelocity.magnitude < 0);
            // Update timeSinceGrounded, for coyote time
            timeSinceGrounded += Time.deltaTime;
        }
        else
        {
            Vector2 downwardsVelocity = (groundObject.velocity * Vector2.up).y < 0 ? groundObject.velocity * Vector2.up : Vector2.zero;
            localVelocity = localVelocity * Vector2.right + downwardsVelocity;
            timeSinceGrounded = 0;
            coyoteJumpConsumed = false;
        }

        if(Mathf.Approximately(CharacterBody.velocity.x, 0) || CheckForWall(CharacterBody.velocity * Vector2.right))
        {
            localVelocity *= Vector2.up;
        }
        if (CheckForCeiling())
        {
            localVelocity.y = Mathf.Min(localVelocity.y, 0);
        }
    }

    #region Lateral Movement
    virtual protected void Move(Vector2 movementDirection)
    {
        bool inputActive = movementDirection.magnitude > 0.01f;
        bool playerStopped = Mathf.Approximately(localVelocity.x, 0);
        bool holdingOppositeDirection = !playerStopped && (Mathf.Sign(movementDirection.x) != Mathf.Sign(localVelocity.x));

        // Running
        if (inputActive && !CheckForWall(movementDirection))
        {
            // Update sprite
            if (ValidateReferences(CharacterRenderer))
                CharacterRenderer.flipX = !(Mathf.Sign(movementDirection.x) == -1);

            if (groundObject)
            {
                ApplyForce(movementDirection, RunForce, HorizontalSpeedCap);
            }
            else
            {
                ApplyForce(movementDirection, AirRunForce, HorizontalSpeedCap);
            }
        }

        // Friction
        if (!inputActive || holdingOppositeDirection)
        {
            if (groundObject)
            {
                ApplyFriction(Friction);
            }
            else
            {
                ApplyFriction(AirFriction);
            }
        }

        InheritMovement(groundObject);

        // Update Animator
        UpdateAnimator(playerStopped);
    }

    private void ApplyFriction(float amount)
    {
        float velocityToRemove = Mathf.Min(amount * Time.fixedDeltaTime, Mathf.Abs(localVelocity.x));
        float velocitySign = -Mathf.Sign(localVelocity.x);
        localVelocity += velocitySign * velocityToRemove * Vector2.right;
    }
    #endregion

    #region Vertical Movement
    public bool TryJump()
    {
        if (groundObject != null || (timeSinceGrounded <= CoyoteTime && !coyoteJumpConsumed))
        {
            Jump();
            coyoteJumpConsumed = true;
            return true;
        }
        else return false;
    }

    private void Jump()
    {
        if (!ValidateReferences(CharacterBody)) return;

        groundObject = null;
        // Cancel out existing vertical velocity, for coyote gamers
        localVelocity *= Vector2.right;
        ApplyImpulse(Vector2.up, JumpForce, VerticalSpeedCap);
    }

    public void CancelJump()
    {
        if (!ValidateReferences(CharacterBody)) return;

        float downwardsForce = localVelocity.magnitude * CancelledJumpImpulseRatio;
        ApplyImpulse(Vector2.down, downwardsForce, VerticalSpeedCap);
    }

    protected void DoGravity(bool falling)
    {
        if (falling)
        {
            ApplyForce(Vector2.down, FallingGravity, VerticalSpeedCap);
        }
        else
        {
            ApplyForce(Vector2.down, RegularGravity, VerticalSpeedCap);
        }
    }
    #endregion

    #region Forces
    protected void ApplyForce(Vector2 direction, float amount, float cap)
    {
        if (!ValidateReferences(CharacterBody)) return;

        direction.Normalize();
        float amountBelowCap = cap - Mathf.Abs((localVelocity * direction).magnitude);
        float velocityToAdd = Mathf.Min(amount, amountBelowCap);

        localVelocity += velocityToAdd * Time.fixedDeltaTime * direction;
        InheritMovement(groundObject);
    }

    protected void ApplyImpulse(Vector2 direction, float amount, float cap)
    {
        if (!ValidateReferences(CharacterBody)) return;

        direction.Normalize();
        float amountBelowCap = cap - Mathf.Abs((localVelocity * direction).magnitude);
        float velocityToAdd = Mathf.Min(amount, amountBelowCap);

        localVelocity += velocityToAdd * direction;
        InheritMovement(groundObject);
    }

    protected void InheritMovement(Rigidbody2D other)
    {
        if (!ValidateReferences(CharacterBody)) return;
        if (other == null)
        {
            CharacterBody.velocity = localVelocity;
            return;
        }

        Vector2 otherVelocity = new(other.velocity.x, other.velocity.y);
        CharacterBody.velocity = localVelocity + otherVelocity;
    }
    #endregion

    #region Helper Functions
    virtual protected bool CheckForObject(Vector2 direction, float distance, ref Rigidbody2D foundObject)
    {
        if (!ValidateReferences(CharacterCollider)) return false;

        ContactFilter2D filter = new()
        {
            useTriggers = false
        };

        RaycastHit2D[] hits = new RaycastHit2D[2];
        Vector2 raycastOrigin = (Vector2)transform.position + CharacterCollider.offset;
        Vector2 raycastDirection = direction;
        float raycastDistance = distance;
        int numHits = Physics2D.BoxCast(raycastOrigin, CharacterCollider.bounds.size, 0, raycastDirection, filter, hits, raycastDistance);
        Debug.Log(numHits);

        bool objectFound = numHits > 1; // We'll always hit ourselves
        foundObject = objectFound ? hits[1].rigidbody : null;

        return objectFound;
    }

    virtual protected void CheckForGround()
    {
        bool isGrounded = CheckForObject(Vector2.down, GroundCheckDistance, ref groundObject);
        if (ValidateReferences(SpriteAnimator))
            SpriteAnimator.SetBool("IsGrounded", isGrounded);

        if (isGrounded && groundObject == null)
        {
            landingSound?.Play();
        }
    }

    virtual protected bool CheckForWall(Vector2 direction)
    {
        return CheckForObject(direction.normalized, WallCheckDistance, ref wallObject); 
    }

    virtual protected bool CheckForCeiling()
    {
        return CheckForObject(Vector2.up, 0.1f, ref ceilingObject);
    }
    #endregion

    #region Collisions
    virtual protected void OnCollisionEnter2D(Collision2D collision)
    {
        if (!DoCollisionEvents)
            return;

        CheckForGround();

        //Prevent bouncing
        if (groundObject != null)
        {
            localVelocity *= Vector2.right;
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (!DoCollisionEvents)
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
        if (!DoCollisionEvents)
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
        if (!ValidateReferences(SpriteAnimator)) return;
        if (isStopped)
        {
            SpriteAnimator.SetFloat("Horizontal Speed", 0);
        }
        else
        {
            SpriteAnimator.SetFloat("Horizontal Speed", Mathf.Abs(localVelocity.x));
        }
    }
}
