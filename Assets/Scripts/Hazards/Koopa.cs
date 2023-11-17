using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Koopa : SimulatedScript
{
    enum KoopaColor { red, green }
    enum Direction { left, right }

    [Tooltip("Red for turning at edges, green for walking off edges")]
    [SerializeField] private KoopaColor color;
    [SerializeField] private float speed;
    [SerializeField] private Direction startingDirection;
    [SerializeField] private float cliffCheckDistance;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Rigidbody2D body;
    [SerializeField] private BoxCollider2D koopaCollider;
    [SerializeField] private Transform spriteTransform;
    [SerializeField] private SpriteRenderer koopaRenderer;
    [SerializeField] private float rotationSpeed;

    private Vector2 direction;

    void Start()
    {
        direction = startingDirection == Direction.right ? Vector2.right : Vector2.left;
    }

    // Fixed update is called 50 times a second, regardless of frame rate
    void FixedUpdate()
    {
        // Check if there's a wall or cliff in the direction we're heading
        if (CheckForWall() || (CheckForCliff() && color == KoopaColor.red))
        {
            // If there is, turn around
            ChangeDirection();
        }

        // Move in our current direction
        Move(direction);
    }

    private void Move(Vector2 moveDir)
    {
        // Find the difference between our current speed and our target speed
        float speedDifference = (moveDir * speed).x - body.velocity.x;

        // Calculate the force required to reach our target speed
        float forceRequired = speedDifference * body.mass;

        // Apply the force
        body.AddForce(Vector2.right * forceRequired, ForceMode2D.Impulse);
    }

    private void ChangeDirection()
    {
        // Flip the sign of our direction vector
        direction *= -1;

        // Tell the sprite renderer to flip/unflip the sprite
        koopaRenderer.flipX = !koopaRenderer.flipX;
    }

    private bool CheckForWall()
    {
        // If we're at zero speed, we've hit something
        return body.velocity.magnitude == 0;
    }

    private bool CheckForCliff()
    {
        // To tell if there's a cliff in front of us, we'll use a box cast
        // For the offset, we'll add cliffCheckDistance to the front of the enemy
        float boxCastOffset = koopaCollider.bounds.size.x + cliffCheckDistance;
        Vector2 boxCastOrigin = (Vector2) koopaCollider.bounds.center + (boxCastOffset * direction);

        // For the size, we'll just use the enemy's size
        Vector2 boxCastSize = koopaCollider.bounds.size;

        // We want the box to not be straight, and to extend one unit down
        float boxCastAngle = 0f;
        Vector2 boxCastDirection = Vector2.down;
        float boxCastDistance = 1f;

        // We'll output any ground it hits into boxCastHit
        RaycastHit2D boxCastHit = Physics2D.BoxCast(boxCastOrigin, boxCastSize, boxCastAngle, boxCastDirection, boxCastDistance, groundLayer);

        // If no ground was hit, we're at a cliff
        return boxCastHit.collider == null;
    }
}
