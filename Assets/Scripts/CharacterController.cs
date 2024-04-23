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
    [SerializeField] private Rigidbody2D _characterBody;
    [SerializeField] private BoxCollider2D _characterCollider;
    [SerializeField] private SpriteRenderer _characterRenderer;
    [SerializeField] private Animator _spriteAnimator;
    public LayerMask GroundLayer { get => _groundLayer; set => _groundLayer = value; }

    protected Rigidbody2D CharacterBody => AssignMandatoryReference(ref _characterBody, typeof(Rigidbody2DWrapper));
    protected BoxCollider2D CharacterCollider => TryAssignReference(ref _characterCollider);
    protected SpriteRenderer CharacterRenderer => TryAssignReference(ref _characterRenderer);
    protected Animator SpriteAnimator => TryAssignReference(ref _spriteAnimator);


    protected GameManager gameManager;

    protected Vector2 slopeDir;
    protected Rigidbody2D groundObject;
    protected bool jumpingThisFrame = false;
    protected int walljumpsRemaining;
    protected float timeSinceGrounded = 0;
    protected bool coyoteJumpConsumed = false;

    public override SimulatedComponent Copy(SimulatedObject destination)
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

        copy.GroundLayer = this.GroundLayer;

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
        UpdateGroundObject();

        if (!ValidateReferences(CharacterBody)) return;

        if (groundObject == null)
        {
            DoGravity(CharacterBody.velocity.y < 0);
            // Update timeSinceGrounded, for coyote time
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
        bool playerStopped = Mathf.Approximately(CharacterBody.velocity.x, 0);
        bool holdingOppositeDirection = !playerStopped && (Mathf.Sign(movementDirection.x) != Mathf.Sign(CharacterBody.velocity.x));

        // Running
        if (inputActive)
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

        InheritVelocity(groundObject);

        // Update Animator
        UpdateAnimator(playerStopped);
    }

    private void ApplyFriction(float amount)
    {
        float velocityToRemove = Mathf.Min(amount * Time.fixedDeltaTime, Mathf.Abs(CharacterBody.velocity.x));
        float velocitySign = -Mathf.Sign(CharacterBody.velocity.x);
        CharacterBody.velocity += velocitySign * velocityToRemove * Vector2.right;
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
        CharacterBody.velocity *= Vector2.right;
        ApplyImpulse(Vector2.up, JumpForce, VerticalSpeedCap);
    }

    public void CancelJump()
    {
        if (!ValidateReferences(CharacterBody)) return;

        float downwardsForce = CharacterBody.velocity.y * CancelledJumpImpulseRatio;
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
        float amountBelowCap = cap - Mathf.Abs((CharacterBody.velocity * direction).magnitude);
        float velocityToAdd = Mathf.Min(amount, amountBelowCap);

        CharacterBody.velocity += velocityToAdd * Time.fixedDeltaTime * direction;
    }

    protected void ApplyImpulse(Vector2 direction, float amount, float cap)
    {
        if (!ValidateReferences(CharacterBody)) return;

        direction.Normalize();
        float amountBelowCap = cap - Mathf.Abs((CharacterBody.velocity * direction).magnitude);
        float velocityToAdd = Mathf.Min(amount, amountBelowCap);

        CharacterBody.velocity += velocityToAdd * direction;
    }

    protected void InheritVelocity(Rigidbody2D other)
    {
        if (!ValidateReferences(CharacterBody)) return;
        if (other == null) return;

        Vector2 velocity = new Vector2(other.velocity.x, other.velocity.y);
        CharacterBody.position += velocity * Time.deltaTime;
    }
    #endregion

    #region Helper Functions
    //Check if the player is grounded
    virtual protected void UpdateGroundObject()
    {
        // While there is backup code here for not having this reference, it causes the object to either float or fall through the floor, depending on if queries hit triggers.
        // Both feel very awkward for the player but falling through the floor makes more sense for the code and is accomplished by just never running this function (the only difference being you cant "swim" without it)
        if (!ValidateReferences(CharacterCollider)) return; 

        Vector2 raycastOrigin;
        if (ValidateReferences(CharacterCollider))
        {
            raycastOrigin = CharacterCollider.bounds.min + new Vector3(0, -0.05f, 0);
        }
        else
        {
            raycastOrigin = transform.position;
        }
        Vector2 raycastDirection = Vector2.down;
        float raycastDistance = GroundCheckDistance;

        List<RaycastHit2D> totalHits = new();

        // Check bottom-left
        RaycastHit2D[] leftHits = Physics2D.RaycastAll(raycastOrigin, raycastDirection, raycastDistance, GroundLayer);
        totalHits.AddRange(leftHits);
        Debug.DrawRay(raycastOrigin, raycastDirection * raycastDistance, Color.red);

        if (ValidateReferences(CharacterCollider))
        {
            // Check bottom-middle
            raycastOrigin += new Vector2(CharacterCollider.bounds.size.x * 0.5f, 0);
            RaycastHit2D[] middlehits = Physics2D.RaycastAll(raycastOrigin, raycastDirection, raycastDistance, GroundLayer);
            Debug.DrawRay(raycastOrigin, raycastDirection * raycastDistance, Color.red);

            // Check bottom-right
            raycastOrigin += new Vector2(CharacterCollider.bounds.size.x * 0.5f, 0);
            RaycastHit2D[] rightHits = Physics2D.RaycastAll(raycastOrigin, raycastDirection, raycastDistance, GroundLayer);
            Debug.DrawRay(raycastOrigin, raycastDirection * raycastDistance, Color.red);
        }

        bool isGrounded = totalHits.Count > 0;

        if (ValidateReferences(SpriteAnimator))
            SpriteAnimator.SetBool("IsGrounded", isGrounded);

        groundObject = isGrounded ? totalHits[0].rigidbody : null;
    }

    virtual protected void UpdateGroundObjectNoCollider()
    {

    }

    private void OnDrawGizmos()
    {

    }

    //Check for a wall on the player's left
    protected bool CheckForWall(Vector2 direction)
    {
        Vector2 origin = CharacterCollider.bounds.center;
        Vector2 size = CharacterCollider.bounds.size;
        float angle = 0f;

        RaycastHit2D[] boxCastHit = Physics2D.BoxCastAll(origin, size, angle, direction, WallCheckDistance, GroundLayer);
        foreach (RaycastHit2D hit in boxCastHit)
        {
            if (hit.rigidbody != CharacterBody)
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
            CharacterBody.velocity *= Vector2.right;
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
        if (!ValidateReferences(SpriteAnimator)) return;
        if (isStopped)
        {
            SpriteAnimator.SetFloat("Horizontal Speed", 0);
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
            SpriteAnimator.SetFloat("Horizontal Speed", Mathf.Abs(CharacterBody.velocity.x - standingOnVelocity.x));
        }
    }
}
