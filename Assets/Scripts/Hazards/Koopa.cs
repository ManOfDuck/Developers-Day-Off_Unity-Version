using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Koopa : SimulatedScript
{
    enum KoopaColor { red, green }
    enum Direction { left, right }

    [Tooltip("Red for turning at edges, green for walking of edges")]
    [SerializeField] private KoopaColor color;
    [SerializeField] private float speed;
    [SerializeField] private Direction startingDirection;
    [SerializeField] private float wallCheckDistance;
    [SerializeField] private float cliffCheckDistance;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Rigidbody2D body;
    [SerializeField] private BoxCollider2D koopaCollider;
    [SerializeField] private Transform spriteTransform;
    [SerializeField] private SpriteRenderer koopaRenderer;
    [SerializeField] private float rotationSpeed;


    Vector2 slopeDir;
    private Direction direction;

    void Start()
    {
        direction = startingDirection;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector2 moveDir = direction == Direction.right ? Vector2.right : Vector2.left;
        if (CheckForWall(moveDir) || (CheckForCliff(moveDir) && color == KoopaColor.red))
        {
            ChangeDirection();
            moveDir = direction == Direction.right ? Vector2.right : Vector2.left;
        }
        Move(moveDir);
    }

    private void Move(Vector2 moveDir)
    {
        float targetSpeed = (moveDir * speed).x - body.velocity.x;


        if (!Mathf.Approximately(targetSpeed, 0f))
        {
            body.AddForce(new Vector2((Vector2.right * targetSpeed * body.mass).x, slopeDir.y), ForceMode2D.Impulse);
        }
    }

    private void ChangeDirection()
    {
        if (direction == Direction.right)
        {
            direction = Direction.left;
        }
        else if (direction == Direction.left)
        {
            direction = Direction.right;
        }
        else
        {
            Debug.Log("Bro I dont even KNOW how you fucked up this bad");
        }

        koopaRenderer.flipX = !koopaRenderer.flipX;
    }

    private bool CheckForWall(Vector2 moveDir)
    {
        return body.velocity.magnitude == 0;
    }

    private bool CheckForCliff(Vector2 moveDir)
    {
        RaycastHit2D boxCastHit = Physics2D.BoxCast(koopaCollider.bounds.center + new Vector3((koopaCollider.bounds.size.x + cliffCheckDistance)
            * moveDir.x, 0, 0), koopaCollider.bounds.size, 0f, Vector2.down, 1f, groundLayer);
        return boxCastHit.collider == null;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
            slopeDir = Vector2.Perpendicular(collision.contacts[0].normal).normalized * -1;
            if (slopeDir == Vector2.up || slopeDir == Vector2.down)
            {
                slopeDir = Vector2.right;
                return;
            }
            body.gravityScale = 0;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        slopeDir = Vector2.right;
        body.gravityScale = 1;
    }
}
