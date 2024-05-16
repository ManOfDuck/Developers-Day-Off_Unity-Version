using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Version of patrol with only one patrol point, just to make things easier to develop ya hear
public class PatrolSimple : TraversePath
{
    protected override string DefaultVisualComponentName => "Patrol";

    private IEnumerator moveCoroutine;
    private Vector2 initPos;

    [SerializeField] private List<Vector2> _patrolPoints = new();
    public List<Vector2> PatrolPoints
    {
        get
        {
            // Return the patrolPoints field
            return _patrolPoints;
        }
        set
        {
            // If we've changed the amount of points or location of the next point, we need to update the coroutine accordingly
            if (value.Count == 0 || value.Count != _patrolPoints.Count || value[currentPoint] != _patrolPoints[currentPoint])
            {
                // Update the patrolPoints field
                _patrolPoints = value;

                // If the coroutine hasn't started, we're good
                if (moveCoroutine != null)
                {
                    // Stop the existing coroutine
                    StopCoroutine(moveCoroutine);

                    // We should stay at the same progress through the point list if possible
                    int startingPoint = value.Count == 0 ? 0 : currentPoint % value.Count;

                    // Start the coroutine at the adjusted index
                    StartMove(startingPoint);
                }
            }
        }
    }


    override protected void Start()
    {
        base.Start();
        StartCoroutine(DoSetup());
    }

    private IEnumerator DoSetup()
    {
        while (!ValidateReferences(Body)) yield return null;

        initPos = Body.position;
        if (moveCoroutine == null)
        {
            StartMove(0);
        }
    }

    private void StartMove(int startingPoint)
    {
        List<Vector2> adjustedPoints = new(PatrolPoints);
        if (adjustedPoints[^1] != Vector2.zero)
        {
            adjustedPoints.Add(Vector2.zero);
        }

        for (int i = 0; i < adjustedPoints.Count; i++)
        {
            adjustedPoints[i] += initPos;
        }
        moveCoroutine = Move(adjustedPoints, startingPoint, true);
        StartCoroutine(moveCoroutine);
    }

    override public SimulatedComponent Copy(ComponentHolder destination)
    {
        Patrol copy = base.Copy(destination) as Patrol;

        copy.PatrolPoints = new List<Vector2>(this.PatrolPoints); // Create a deep copy

        return copy;
    }
}

