using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Patrol : SimulatedScript
{
    [Tooltip("Disable to make platform wait for player")]
    [SerializeField] private bool waitForPlayer = false;
    [SerializeField] private List<Vector2> patrolPoints;
    [SerializeField] private float speed;
    [SerializeField] private float waitTime;
    [SerializeField] private Rigidbody2D body;

    Vector2 initPos;
    bool touchingPlayer = false;
    bool moving = false;

    // Start is called before the first frame update
    void Awake()
    {
        patrolPoints.Add(new Vector2(0, 0));
        initPos = body.position;
        if (!waitForPlayer)
            StartCoroutine(Move());
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!waitForPlayer || !doCollisionEvents)
            return;

        if (collision.gameObject.CompareTag("Player") && collision.collider.bounds.min.y >= collision.otherCollider.bounds.max.y)
        {
            touchingPlayer = true;
            if (!moving)
                StartCoroutine(Move());
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (!waitForPlayer || !doCollisionEvents)
            return;

        if (collision.gameObject.CompareTag("Player"))
        {
            touchingPlayer = false;
        }
    }


    protected virtual IEnumerator Move()
    {
        moving = true;
        while (moving)
        {
            Light(18);
            foreach (Vector2 point in patrolPoints)
            {
                Light(20);
                Vector2 initial = body.position;
                Vector2 target = point + initPos;
                Vector2 path = target - initial;
                Vector2 direction = path.normalized;
                Vector2 traveled = Vector2.zero;

                while (traveled.magnitude < path.magnitude)
                {
                    // Pause if object is disabled
                    while (!doCoroutines)
                    {
                        body.velocity = Vector2.zero;
                        yield return null;
                    }

                    float distanceRemaining = path.magnitude - traveled.magnitude;
                    float speedToAdd = Mathf.Min(speed, (distanceRemaining) / Time.fixedDeltaTime);

                    body.velocity = speedToAdd * direction;
                    traveled = body.position - initial;

                    yield return null;
                }
                // Stop and wait
                body.velocity = Vector2.zero;
                yield return new WaitForSeconds(waitTime);
                Light(31, Color.blue);
            }

            if (waitForPlayer && !touchingPlayer)
                moving = false;
        }
    }
}

