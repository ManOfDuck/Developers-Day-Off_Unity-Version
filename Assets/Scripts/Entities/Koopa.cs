using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Koopa : CharacterController
{
    enum KoopaColor { red, green }
    enum Direction { left, right }

    [Tooltip("Red for turning at edges, green for walking off edges")]
    [SerializeField] private KoopaColor _color;
    private KoopaColor Color { get => _color; set => _color = value; }

    [SerializeField] private Direction _startingDirection;
    private Direction StartingDirection { get => _startingDirection; set => _startingDirection = value; }

    [SerializeField] private float _cliffCheckDistance;
    public float CliffCheckDistance { get => _cliffCheckDistance; set => _cliffCheckDistance = value; }

    private Vector2 direction;


    override protected void Start()
    {
        base.Start();
        direction = StartingDirection == Direction.right ? Vector2.right : Vector2.left;
    }

    // Fixed update is called 50 times a second, regardless of frame rate
    override protected void FixedUpdate()
    {
        base.FixedUpdate();

        if (CheckForWall(direction) || (CheckForCliff() && Color == KoopaColor.red))
        {
            ChangeDirection();
        }

        // Move in our current direction
        Move(direction);
        Light(21, UnityEngine.Color.blue);
    }

    protected override void Move(Vector2 movementDirection)
    {
        CharacterBody.velocity = HorizontalSpeedCap * direction + CharacterBody.velocity * Vector2.up;
        InheritVelocity(groundObject);
    }

    private void ChangeDirection()
    {
        Light(34, UnityEngine.Color.blue);
        // Flip the sign of our direction vector
        direction *= -1;
        Light(36);
        // Tell the sprite renderer to flip/unflip the sprite
        CharacterRenderer.flipX = !CharacterRenderer.flipX;
        Light(39);
    }

    private bool CheckForCliff()
    {
        // Dont turn around if we're already falling
        if (groundObject == null) return false;

        Light(47, UnityEngine.Color.blue);
        // To tell if there's a cliff in front of us, we'll use a box cast
        // For the offset, we'll add cliffCheckDistance to the front of the enemy
        float boxCastOffset = CharacterCollider.bounds.size.x + CliffCheckDistance;
        Light(50);
        Vector2 boxCastOrigin = (Vector2) CharacterCollider.bounds.center + (boxCastOffset * direction);
        Light(51);

        // For the size, we'll just use the enemy's size
        Vector2 boxCastSize = CharacterCollider.bounds.size;
        Light(54);

        // We want the box to not be straight, and to extend one unit down
        float boxCastAngle = 0f;
        Light(57);
        Vector2 boxCastDirection = Vector2.down;
        Light(59);
        float boxCastDistance = 1f;
        Light(60);

        // We'll output any ground it hits into boxCastHit
        RaycastHit2D[] boxCastHit = Physics2D.BoxCastAll(boxCastOrigin, boxCastSize, boxCastAngle, boxCastDirection, boxCastDistance, GroundLayer);
        Light(62);

        // If no ground was hit (besides ourselves), we're at a cliff
        Light(65, UnityEngine.Color.blue);
        foreach (RaycastHit2D hit in boxCastHit)
        {
            if (hit.rigidbody != CharacterBody)
            {
                return false;
            }
        }
        return true;
    }
}
