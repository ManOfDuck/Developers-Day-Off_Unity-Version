using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TraversePath : SimulatedScript
{
    protected int currentPoint = 0;

    [SerializeField] private float _speed;
    public float Speed { get => _speed; set => _speed = value; }

    [SerializeField] private float _waitTime;
    public float WaitTime { get => _waitTime; set => _waitTime = value; }

    protected bool Moving { get;  set; }

    [SerializeField] private Rigidbody2D _body;
    // When we read the value of this field, ask the parent object (if any) to search for a valid reference and assign it to the body field
    public Rigidbody2D Body => AssignMandatoryReference(ref _body, typeof(Rigidbody2DWrapper));


    protected virtual IEnumerator Move(List<Vector2> path, int startingIndex = 0, bool loop = false)
    {
        Moving = true;
        do
        {
            for (currentPoint = startingIndex; currentPoint < path.Count; currentPoint++)
            {
                // Wait for body
                while (!ValidateReferences(Body)) yield return null;
                while (!Holder.LockBody(this)) yield return null;


                Vector2 target = path[currentPoint];

                Light(20);
                Vector2 initial = Body.position;
                Vector2 direction = target - initial;
                Vector2 directionNormalized = direction.normalized;
                Vector2 traveled = Vector2.zero;


                while (direction.magnitude - traveled.magnitude > 0.01f)
                {
                    // Wait for body (in case reference broke mid-routine)
                    while (!ValidateReferences(Body)) yield return null;
                    while (!Holder.LockBody(this)) yield return null;


                    // Ensure body is correct type
                    Body.bodyType = RigidbodyType2D.Kinematic;

                    // Pause if object is disabled
                    while (!DoCoroutines)
                    {
                        Body.velocity = Vector2.zero;
                        yield return null;
                    }

                    float distanceRemaining = direction.magnitude - traveled.magnitude;
                    float speedToAdd = Mathf.Min(Speed, (distanceRemaining) / Time.fixedDeltaTime);

                    Body.velocity = speedToAdd * directionNormalized;
                    traveled = Body.position - initial;

                    yield return null;
                }
                Body.position = target; // Lets just scoooooh over a bit don't tell the physics engine

                // Stop and wait
                Body.velocity = Vector2.zero;
                yield return new WaitForSeconds(WaitTime);
                Light(31, Color.blue);

                // Start from 0 next time
                startingIndex = 0;
            }
        }
        while (loop);
        Moving = false;
        Debug.Log("releasing!");
        Holder.ReleaseBody(this);
    }

    private void OnDisable()
    {
        Holder.ReleaseBody(this);
    }

    override public SimulatedComponent Copy(ComponentHolder destination)
    {
        TraversePath copy = destination.gameObject.AddComponent(this.GetType()) as TraversePath;

        copy.Speed = this.Speed;
        copy.WaitTime = this.WaitTime;

        return copy;
    }
}
