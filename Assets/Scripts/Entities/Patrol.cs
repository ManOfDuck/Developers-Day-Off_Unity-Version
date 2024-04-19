using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Patrol : SimulatedScript
{
    private IEnumerator moveCoroutine;
    private int currentPoint;
    private Vector2 initPos;

    [SerializeField] private List<Vector2> patrolPoints;
    public List<Vector2> PatrolPoints
    {
        get
        {
            return this.patrolPoints;
        }
        set
        {
            // If we've changed the count or next point, we need to update the coroutine
            if (value.Count != patrolPoints.Count || value[currentPoint] != patrolPoints[currentPoint])
            {
                StopCoroutine(moveCoroutine);

                int startingPoint = currentPoint % value.Count;

                moveCoroutine = Move(startingPoint);
                StartCoroutine(moveCoroutine);
            }

            patrolPoints = value;       
        }
    }

    [SerializeField] private float speed;
    public float Speed { get; set; }

    [SerializeField] private float waitTime;
    public float WaitTime { get; set; }

    private Rigidbody2D body;
    public Rigidbody2D Body => ParentObject.TryAssignReference(ref body);


    void Awake()
    {
        initPos = Body.position;
        moveCoroutine = Move(0);
        StartCoroutine(moveCoroutine);
    }

    protected virtual IEnumerator Move(int startingIndex = 0)
    {
        while (true)
        {
            Light(18);
            for (currentPoint = startingIndex; currentPoint < patrolPoints.Count + 1; currentPoint++)
            {
                // Wait for references
                while (!ValidateReferences(body)) yield return null;

                // Return to (0, 0) before looping
                Vector2 point = currentPoint < patrolPoints.Count ? patrolPoints[currentPoint] : Vector2.zero;

                Light(20);
                Vector2 initial = Body.position;
                Vector2 target = point + initPos;
                Vector2 path = target - initial;
                Vector2 direction = path.normalized;
                Vector2 traveled = Vector2.zero;

                while (traveled.magnitude < path.magnitude)
                {
                    // Wait for references
                    while (!ValidateReferences(body)) yield return null;

                    // Pause if object is disabled
                    while (!doCoroutines)
                    {
                        Body.velocity = Vector2.zero;
                        yield return null;
                    }

                    float distanceRemaining = path.magnitude - traveled.magnitude;
                    float speedToAdd = Mathf.Min(speed, (distanceRemaining) / Time.fixedDeltaTime);

                    Body.velocity = speedToAdd * direction;
                    traveled = Body.position - initial;

                    yield return null;
                }
                // Stop and wait
                Body.velocity = Vector2.zero;
                yield return new WaitForSeconds(waitTime);
                Light(31, Color.blue);

                // Start from 0 next time
                startingIndex = 0;
            }
        }
    }
}

