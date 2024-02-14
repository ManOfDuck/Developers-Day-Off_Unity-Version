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

    private Vector2 direction;

    void Start()
    {
        direction = startingDirection == Direction.right ? Vector2.right : Vector2.left;
    }

    // Fixed update is called 50 times a second, regardless of frame rate
    void FixedUpdate()
    {
        Light(12,Color.blue);
        // Check if there's a wall or cliff in the direction we're heading
        if (CheckForWall() || (CheckForCliff() && color == KoopaColor.red))
        {
            Light(14, Color.green);
            // If there is, turn around
            ChangeDirection();
            Light(17);
        }
        else Light(14, Color.red);

        // Move in our current direction
        Move(direction);
        Light(21,Color.blue);
    }

    private void Move(Vector2 moveDir)
    {
        Light(21, Color.blue);
        // Find the difference between our current speed and our target speed
        float speedDifference = (moveDir * speed).x - body.velocity.x;
        Light(25);
        // Calculate the force required to reach our target speed
        float forceRequired = speedDifference * body.mass;
        Light(28);
        // Apply the force
        body.AddForce(Vector2.right * forceRequired, ForceMode2D.Impulse);
        Light(31);
    }

    private void ChangeDirection()
    {
        Light(34, Color.blue);
        // Flip the sign of our direction vector
        direction *= -1;
        Light(36);
        // Tell the sprite renderer to flip/unflip the sprite
        koopaRenderer.flipX = !koopaRenderer.flipX;
        Light(39);
    }

    private bool CheckForWall()
    {
        Light(42, Color.blue);
        // If we're at zero speed, we've hit something
        Light(44,Color.blue);
        return body.velocity.magnitude == 0;  
    }

    private bool CheckForCliff()
    {
        Light(47, Color.blue);
        // To tell if there's a cliff in front of us, we'll use a box cast
        // For the offset, we'll add cliffCheckDistance to the front of the enemy
        float boxCastOffset = koopaCollider.bounds.size.x + cliffCheckDistance;
        Light(50);
        Vector2 boxCastOrigin = (Vector2) koopaCollider.bounds.center + (boxCastOffset * direction);
        Light(51);

        // For the size, we'll just use the enemy's size
        Vector2 boxCastSize = koopaCollider.bounds.size;
        Light(54);

        // We want the box to not be straight, and to extend one unit down
        float boxCastAngle = 0f;
        Light(57);
        Vector2 boxCastDirection = Vector2.down;
        Light(59);
        float boxCastDistance = 1f;
        Light(60);

        // We'll output any ground it hits into boxCastHit
        RaycastHit2D boxCastHit = Physics2D.BoxCast(boxCastOrigin, boxCastSize, boxCastAngle, boxCastDirection, boxCastDistance, groundLayer);
        Light(62);

        // If no ground was hit, we're at a cliff
        Light(65, Color.blue);
        return boxCastHit.collider == null;
    }
}
