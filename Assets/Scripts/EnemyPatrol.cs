using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPatrol : MonoBehaviour
{
    [Header("Enemy Movement")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float patrolTime = 15f;

    private enum Direction
    {
        Right, Left
    }
    private Direction moveDirection;
    [SerializeField] private bool enemyPatrolling = true;

    [Header("Enemy Animation")]
    [SerializeField] private Rigidbody2D enemyRB;
    [SerializeField] private BoxCollider2D enemyBC;
    [SerializeField] private SpriteRenderer enemySR;

    private void Move()
    {
        Vector2 targetPosition = transform.position;

        switch (moveDirection)
        {
            default:
            case Direction.Right:
                targetPosition += Vector2.right * moveSpeed * Time.fixedDeltaTime;
                break;
            case Direction Left:
                targetPosition -= Vector2.right * moveSpeed * Time.fixedDeltaTime;
                break;
        }
        enemyRB.MovePosition(targetPosition);
    }

    private void FlipMoveDirection()
    {
        if (moveDirection == Direction.Right)
        {
            moveDirection = Direction.Left;
            enemySR.flipX = false;
        }
        else
        {
            moveDirection = Direction.Right;
            enemySR.flipX = true;
        }
    }

    private IEnumerator StartEnemyPatrol()
    {
        while (enemyPatrolling)
        {
            float counter = 0f;

            while (counter < patrolTime)
            {
                counter += Time.fixedDeltaTime;

                Move();

                yield return new WaitForFixedUpdate();
            }



            FlipMoveDirection();

            //might want to move
            yield return new WaitForSeconds(1.5f);
        }
    }
    void Start()
    {
        StartCoroutine(StartEnemyPatrol());
        enemySR.flipX = true;
    }
}
