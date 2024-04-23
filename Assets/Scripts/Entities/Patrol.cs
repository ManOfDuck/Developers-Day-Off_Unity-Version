using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Patrol : SimulatedScript
{
    protected override string DefaultVisualComponentName => "Patrol";

    private IEnumerator moveCoroutine;
    private int currentPoint = 0;
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
                // Stop the existing coroutine
                if (moveCoroutine != null)
                {
                    StopCoroutine(moveCoroutine);
                }

                // We should stay at the same progress through the point list if possible
                int startingPoint = value.Count == 0 ? 0 : currentPoint % value.Count;

                // Start the coroutine at the adjusted index
                moveCoroutine = Move(startingPoint);
                StartCoroutine(moveCoroutine);
            }

            // Update the patrolPoints field
            _patrolPoints = value;       
        }
    }

    [SerializeField] private float _speed;
    public float Speed { get => _speed; set => _speed = value; }

    [SerializeField] private float _waitTime;
    public float WaitTime { get => _waitTime; set => _waitTime = value; }

    private Rigidbody2D _body;
    // When we read the value of this field, ask the parent object (if any) to search for a valid reference and assign it to the body field
    public Rigidbody2D Body => ParentObject == null ? null : ParentObject.AssignMandatoryReference(ref _body, typeof(Rigidbody2DWrapper));


    override protected void Start()
    {
        base.Start();
        StartCoroutine(DoSetup());
    }

    private IEnumerator DoSetup()
    {
        while (!ValidateReferences(Body)) yield return null;

        initPos = transform.position;
        if (moveCoroutine == null)
        {
            moveCoroutine = Move(0);
            StartCoroutine(moveCoroutine);
        }
    }

    protected virtual IEnumerator Move(int startingIndex = 0)
    {
        while (true)
        {
            Light(18);
            for (currentPoint = startingIndex; currentPoint < PatrolPoints.Count + 1; currentPoint++)
            {
                // Wait for references
                while (!ValidateReferences(Body)) yield return null;

                // Return to (0, 0) before looping
                Vector2 point = currentPoint < PatrolPoints.Count ? PatrolPoints[currentPoint] : Vector2.zero;

                Light(20);
                Vector2 initial = Body.position;
                Vector2 target = point + initPos;
                Vector2 path = target - initial;
                Vector2 direction = path.normalized;
                Vector2 traveled = Vector2.zero;

                while (traveled.magnitude < path.magnitude && !Mathf.Approximately(traveled.magnitude, path.magnitude))
                {
                    // Wait for references
                    while (!ValidateReferences(Body)) yield return null;

                    // Pause if object is disabled
                    while (!DoCoroutines)
                    {
                        Body.velocity = Vector2.zero;
                        yield return null;
                    }

                    float distanceRemaining = path.magnitude - traveled.magnitude;
                    float speedToAdd = Mathf.Min(Speed, (distanceRemaining) / Time.fixedDeltaTime);

                    Debug.Log("velocity");
                    Body.velocity = speedToAdd * direction;
                    traveled = Body.position - initial;

                    yield return null;
                }
                // Stop and wait
                Debug.Log("wait" + WaitTime);
                Body.velocity = Vector2.zero;
                yield return new WaitForSeconds(WaitTime);
                Debug.Log("done");
                Light(31, Color.blue);

                // Start from 0 next time
                startingIndex = 0;
            }
        }
    }

    override public SimulatedComponent Copy(SimulatedObject destination)
    {
        Patrol copy = destination.gameObject.AddComponent<Patrol>();

        copy.PatrolPoints = new List<Vector2>(this.PatrolPoints); // Create a deep copy
        copy.Speed = this.Speed;
        copy.WaitTime = this.WaitTime;

        return copy;
    }
}

