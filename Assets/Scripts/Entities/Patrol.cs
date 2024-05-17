using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class Patrol : TraversePath
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
            // Update the patrolPoints field
            _patrolPoints = value;

            // If the coroutine hasn't started, we're good
            if (moveCoroutine != null)
            {
                // Stop the existing coroutine
                StopCoroutine(moveCoroutine);

                // Start the coroutine at the adjusted index
                StartMove();
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
            StartMove();
        }
    }

    private void StartMove()
    {
        List<Vector2> adjustedPoints = new(PatrolPoints);
        if (adjustedPoints[^1] != Vector2.zero)
        {
            adjustedPoints.Add(Vector2.zero);
        }

        for(int i = 0; i < adjustedPoints.Count; i++)
        {
            adjustedPoints[i] += initPos;
        }

        int startingPoint = adjustedPoints.Count == 0 ? 0 : currentPoint % adjustedPoints.Count;
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

