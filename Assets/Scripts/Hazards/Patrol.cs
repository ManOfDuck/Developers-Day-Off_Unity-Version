using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Patrol : SimulatedScript
{
    [SerializeField] private List<Vector2> patrolPoints;
    [SerializeField] private float speed;
    [SerializeField] private float waitTime;
    [SerializeField] private Rigidbody2D body;

    Vector2 initPos;

    // Start is called before the first frame update
    void Awake()
    {
        patrolPoints.Add(new Vector2(0, 0));
        initPos = body.position;
        StartCoroutine(Move());
    }

    protected virtual IEnumerator Move()
    {
        float step = 0f;
        Vector2 target = Vector2.zero;
        while (true)
        {
            Light(18);
            foreach (Vector2 point in patrolPoints)
            {
                Light(20);
                target = point + initPos;
                while (body.position != target)
                {
                    Light(22);
                    step = speed * Time.deltaTime;
                    body.position = Vector2.MoveTowards(body.position, target, step);
                    Light(25);
                    yield return null;
                    Light(28, Color.blue);
                }
                yield return new WaitForSeconds(waitTime);
                Light(31, Color.blue);
            }
        }
    }
}

